// -----------------------------------------------------------------------------
// Features/Auth/Commands/Logout/LogoutCommandHandler.cs
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Commands.Logout;

public sealed class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, ApplicationResult>
{
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IIdentityUnitOfWork _uow;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IUserRepository userRepository,
            IUserTokenRepository userTokenRepository,
            ITokenService tokenService,
            IIdentityUnitOfWork uow,
            ILogger<LogoutCommandHandler> logger)
        {
            _userRepository     = userRepository     ?? throw new ArgumentNullException(nameof(userRepository));
            _userTokenRepository = userTokenRepository ?? throw new ArgumentNullException(nameof(userTokenRepository));
            _tokenService       = tokenService       ?? throw new ArgumentNullException(nameof(tokenService));
            _uow                = uow                ?? throw new ArgumentNullException(nameof(uow));
            _logger             = logger            ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApplicationResult> Handle(LogoutCommand cmd, CancellationToken ct)
        {
            await _uow.BeginAsync(ct);

            try
            {
                //-----------------------------------------------------------------
                // 1) کاربر باید وجود داشته باشد
                //-----------------------------------------------------------------
                var user = await _userRepository.GetByIdAsync(cmd.UserId);
                if (user is null)
                {
                    await _uow.RollbackAsync(ct);
                    return ApplicationResult.Failure("User could not be found");
                }

                //-----------------------------------------------------------------
                // 2) ابطال Access-Token (jti)
                //-----------------------------------------------------------------
                string? jwtId = null;
                Guid? userId = null;
                if (_tokenService.ValidateAccessToken(cmd.AccessToken, out jwtId , out userId) &&
                    !string.IsNullOrWhiteSpace(jwtId))
                {
                    // فقط همان توکن جاری ابطال می‌شود؛ توکن‌های دیگر دست‌نخورده می‌مانند
                    var jwtToken = await _userTokenRepository.GetByTokenValueAsync(cmd.AccessToken, "AccessToken");
                    if (jwtToken != null && jwtToken.UserId == cmd.UserId) // Ensure token belongs to user
                    {
                        jwtToken.Revoke();
                        await _userTokenRepository.UpdateAsync(jwtToken, ct);
                    }
                }

                //-----------------------------------------------------------------
                // 3) ابطال Refresh-Token (اختیاری)
                //-----------------------------------------------------------------
                if (!string.IsNullOrWhiteSpace(cmd.RefreshToken))
                {
                    var refreshToken = await _userTokenRepository.GetByTokenValueAsync(cmd.RefreshToken, "RefreshToken");
                    if (refreshToken != null)
                    {
                        refreshToken.Revoke();
                        await _userTokenRepository.UpdateAsync(refreshToken);
                    }
                }

                //-----------------------------------------------------------------
                // 4) Commit
                //-----------------------------------------------------------------
                await _uow.SaveAsync(ct);
                await _uow.CommitAsync(ct);

                _logger.LogInformation("User {UserId} logged out. jti revoked: {Jti}",
                                       user.Id, jwtId ?? "<none>");

                return  ApplicationResult.Success();
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync(ct);
                _logger.LogError(ex, "Logout failed for User {UserId}", cmd.UserId);

                return ApplicationResult.Failure("Logout failed");
            }
        }
    }
