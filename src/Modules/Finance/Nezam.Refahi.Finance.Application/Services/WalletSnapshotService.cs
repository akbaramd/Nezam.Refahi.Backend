using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Services;

/// <summary>
/// Service for generating wallet snapshots
/// </summary>
public sealed class WalletSnapshotService : IWalletSnapshotService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletSnapshotRepository _walletSnapshotRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public WalletSnapshotService(
        IWalletRepository walletRepository,
        IWalletSnapshotRepository walletSnapshotRepository,
        IWalletTransactionRepository walletTransactionRepository,
        IFinanceUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletSnapshotRepository = walletSnapshotRepository ?? throw new ArgumentNullException(nameof(walletSnapshotRepository));
        _walletTransactionRepository = walletTransactionRepository ?? throw new ArgumentNullException(nameof(walletTransactionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Generate snapshots for all wallets for a specific date
    /// </summary>
    /// <param name="snapshotDate">Date to generate snapshots for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of snapshots created</returns>
    public async Task<int> GenerateSnapshotsForDateAsync(DateTime snapshotDate, CancellationToken cancellationToken = default)
    {
        var targetDate = snapshotDate.Date;
        var snapshotsCreated = 0;

        // Get all wallets that don't have a snapshot for this date
        var walletIdsWithoutSnapshot = await _walletSnapshotRepository.GetWalletsWithoutSnapshotForDateAsync(targetDate, cancellationToken);

        foreach (var walletId in walletIdsWithoutSnapshot)
        {
            try
            {
                await GenerateSnapshotForWalletAsync(walletId, targetDate, cancellationToken);
                snapshotsCreated++;
            }
            catch (Exception ex)
            {
                // Log error but continue with other wallets
                Console.WriteLine($"Error generating snapshot for wallet {walletId}: {ex.Message}");
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return snapshotsCreated;
    }

    /// <summary>
    /// Generate snapshot for a specific wallet and date
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="snapshotDate">Date to generate snapshot for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task GenerateSnapshotForWalletAsync(Guid walletId, DateTime snapshotDate, CancellationToken cancellationToken = default)
    {
        var targetDate = snapshotDate.Date;

        // Check if snapshot already exists
        var existingSnapshot = await _walletSnapshotRepository.GetByWalletIdAndDateAsync(walletId, targetDate, cancellationToken);
        if (existingSnapshot != null)
        {
            return; // Snapshot already exists
        }

        // Get wallet
        var wallet = await _walletRepository.GetByIdAsync(walletId, cancellationToken);
        if (wallet == null)
        {
            throw new ArgumentException($"Wallet with ID {walletId} not found");
        }

        // Calculate balance up to the snapshot date
        var balance = await _walletRepository.CalculateWalletBalanceAsync(walletId, targetDate, cancellationToken);

        // Get all transactions up to the snapshot date
        var allTransactions = await _walletTransactionRepository.GetByWalletIdAsync(walletId,1,1000,cancellationToken);
        var transactionsUpToDate = allTransactions.Where(t => t.CreatedAt.Date <= targetDate).ToList();

        // Calculate totals
        var totalDeposits = transactionsUpToDate
            .Where(t => t.IsIn())
            .Sum(t => t.Amount.AmountRials);

        var totalWithdrawals = transactionsUpToDate
            .Where(t => t.IsOut())
            .Sum(t => t.Amount.AmountRials);

        var lastTransactionAt = transactionsUpToDate
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefault()?.CreatedAt;

        // Create snapshot
        var snapshot = new WalletSnapshot(
            walletId: walletId,
            externalUserId: wallet.ExternalUserId,
            balance: balance,
            snapshotDate: targetDate,
            transactionCount: transactionsUpToDate.Count,
            totalDeposits: Money.FromRials(totalDeposits),
            totalWithdrawals: Money.FromRials(totalWithdrawals),
            lastTransactionAt: lastTransactionAt,
            metadata: new Dictionary<string, string>
            {
                ["GeneratedAt"] = DateTime.UtcNow.ToString("O"),
                ["TransactionCount"] = transactionsUpToDate.Count.ToString(),
                ["TotalDeposits"] = totalDeposits.ToString(),
                ["TotalWithdrawals"] = totalWithdrawals.ToString()
            });

        await _walletSnapshotRepository.AddAsync(snapshot, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Generate snapshots for all wallets for yesterday (for daily batch job)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of snapshots created</returns>
    public async Task<int> GenerateSnapshotsForYesterdayAsync(CancellationToken cancellationToken = default)
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        return await GenerateSnapshotsForDateAsync(yesterday, cancellationToken);
    }

    /// <summary>
    /// Generate snapshots for a specific wallet for yesterday
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task GenerateDailySnapshotForWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        await GenerateSnapshotForWalletAsync(walletId, yesterday, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Generate snapshots for a specific user for yesterday
    /// </summary>
    /// <param name="externalUserId">User external user ID</param>   
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task GenerateDailySnapshotForUserAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetByExternalUserIdAsync(externalUserId, cancellationToken);
        if (wallet == null)
        {
            throw new ArgumentException($"Wallet for user {externalUserId} not found");
        }

        await GenerateDailySnapshotForWalletAsync(wallet.Id, cancellationToken);
    }

    /// <summary>
    /// Generate snapshots for a date range
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total number of snapshots created</returns>
    public async Task<int> GenerateSnapshotsForDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var totalSnapshotsCreated = 0;
        var currentDate = fromDate.Date;

        while (currentDate <= toDate.Date)
        {
            var snapshotsCreated = await GenerateSnapshotsForDateAsync(currentDate, cancellationToken);
            totalSnapshotsCreated += snapshotsCreated;
            currentDate = currentDate.AddDays(1);
        }

        return totalSnapshotsCreated;
    }
}
