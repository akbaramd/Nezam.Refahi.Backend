using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Value object representing a hashed secret token
/// </summary>
public class HashedSecret : ValueObject
{
    public string Hash { get; }
    public string Algorithm { get; }
    public string Salt { get; }

    public HashedSecret(string hash, string algorithm, string salt)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash cannot be empty", nameof(hash));
            
        if (string.IsNullOrWhiteSpace(algorithm))
            throw new ArgumentException("Algorithm cannot be empty", nameof(algorithm));
            
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));

        Hash = hash;
        Algorithm = algorithm;
        Salt = salt;
    }

    /// <summary>
    /// Creates a hashed secret with default algorithm (SHA256)
    /// </summary>
    public static HashedSecret Create(string hash, string salt)
    {
        return new HashedSecret(hash, "SHA256", salt);
    }

    /// <summary>
    /// Creates a hashed secret with SHA512 algorithm
    /// </summary>
    public static HashedSecret CreateSha512(string hash, string salt)
    {
        return new HashedSecret(hash, "SHA512", salt);
    }

    /// <summary>
    /// Creates a hashed secret with Argon2 algorithm
    /// </summary>
    public static HashedSecret CreateArgon2(string hash, string salt)
    {
        return new HashedSecret(hash, "Argon2", salt);
    }

    public override string ToString() => $"{Algorithm}:{Hash}";

    protected override System.Collections.Generic.IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
        yield return Algorithm;
        yield return Salt;
    }
}
