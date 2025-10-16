using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.Events;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Services;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents a user in the system with OTP authentication capabilities
/// </summary>
public class User : FullAggregateRoot<Guid>
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public NationalId? NationalId { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Username { get; private set; }
    public Guid? ExternalUserId { get; private set; }
    // Role is now managed through UserRole entity (many-to-many relationship)
    public DateTime? LastLoginAt { get; private set; }

    // OTP Authentication fields
    public bool IsPhoneVerified { get; private set; }
    public DateTime? PhoneVerifiedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastAuthenticatedAt { get; private set; }
    public int FailedAttempts { get; private set; }
    public DateTime? LockedAt { get; private set; }
    public string? LockReason { get; private set; }
    public DateTime? UnlockAt { get; private set; }
    public DeviceFingerprint? LastDeviceFingerprint { get; private set; }
    public string? LastIpAddress { get; private set; }
    public string? LastUserAgent { get; private set; }

    // External source metadata for seeding/audit
    public string? SourceSystem { get; private set; }
    public string? SourceVersion { get; private set; }
    public string? SourceChecksum { get; private set; }
    public string? ProfileSnapshot { get; private set; }

    private readonly List<UserToken> _tokens = new();
    public IReadOnlyCollection<UserToken> Tokens => _tokens.AsReadOnly();

    private readonly List<UserPreference> _preferences = new();
    public IReadOnlyCollection<UserPreference> Preferences => _preferences.AsReadOnly();

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<UserClaim> _userClaims = new();
    public IReadOnlyCollection<UserClaim> UserClaims => _userClaims.AsReadOnly();

    // Private constructor for EF Core
    private User() : base() { }

    /// <summary>
    /// Creates a new user with full details
    /// </summary>
    public User(string firstName, string lastName, string nationalId, string? phoneNumber = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException("National ID cannot be empty", nameof(nationalId));

        FirstName = firstName;
        LastName = lastName;
        NationalId = new NationalId(nationalId);
        PhoneNumber = new PhoneNumber(phoneNumber ?? string.Empty);
        Email = null;
        Username = null;
        ExternalUserId = null;
        // Role will be assigned through UserRole entity
        
        // Initialize OTP authentication fields
        IsPhoneVerified = false;
        IsActive = true;
        FailedAttempts = 0;
        
        // Note: Default preferences will be initialized after the user is saved to database
        // and has a valid ID via EnsureDefaultPreferences() method
        
        // Raise domain event
        AddDomainEvent(new UserCreatedEvent(Id, PhoneNumber.Value, nationalId));
    } 

    /// <summary>
    /// Creates a new user with extended fields used in seeding scenarios
    /// </summary>
    public User(
        string firstName,
        string lastName,
        string nationalId,
        string? phoneNumber,
        string? email,
        string? username,
        Guid? externalUserId)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException("National ID cannot be empty", nameof(nationalId));

        FirstName = firstName;
        LastName = lastName;
        NationalId = new NationalId(nationalId);
        PhoneNumber = new PhoneNumber(phoneNumber ?? string.Empty);
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Username = string.IsNullOrWhiteSpace(username) ? null : username.Trim();
        ExternalUserId = externalUserId;

        // Initialize OTP authentication fields
        IsPhoneVerified = false;
        IsActive = true;
        FailedAttempts = 0;

        AddDomainEvent(new UserCreatedEvent(Id, PhoneNumber.Value, nationalId));
    }

    /// <summary>
    /// Creates a new user with just a phone number (for OTP authentication)
    /// </summary>
    public User(string phoneNumber)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        PhoneNumber = new PhoneNumber(phoneNumber);
        // Role will be assigned through UserRole entity
        
        // Initialize OTP authentication fields
        IsPhoneVerified = false;
        IsActive = true;
        FailedAttempts = 0;
        
        // Note: Default preferences will be initialized after the user is saved to database
        // and has a valid ID via EnsureDefaultPreferences() method
        
        // Raise domain event
        AddDomainEvent(new UserCreatedEvent(Id, phoneNumber));
    }

    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        if (phoneNumber == PhoneNumber)
            return; // No change

        var oldPhoneNumber = PhoneNumber.Value;
        PhoneNumber = new PhoneNumber(phoneNumber);
        IsPhoneVerified = false;
        PhoneVerifiedAt = null;
        
        // Reset authentication state for new phone number
        FailedAttempts = 0;
        LockedAt = null;
        LockReason = null;
        UnlockAt = null;
        
        // Raise domain event
        AddDomainEvent(new UserPhoneUpdatedEvent(Id, oldPhoneNumber, phoneNumber));
    }

    /// <summary>
    /// Assigns the specified role to the user
    /// </summary>
    /// <param name="role">The role to assign</param>
    public void AssignRole(Role role)
    {
        _userRoles.Add(new UserRole(Id, role));
    }

    /// <summary>
    /// Adds the specified role to the user's existing role
    /// </summary>
    /// <param name="role">The role to add</param>
    public void AddRole(Role role)
    {
        _userRoles.Add(new UserRole(Id, role.Id));
    }

    /// <summary>
    /// Removes the specified role from the user
    /// </summary>
    /// <param name="role">The role to remove</param>
    public void RemoveRole(Role role)
    {
        _userRoles.Remove(new UserRole(Id, role.Id));
    }

    /// <summary>
    /// Checks if the user has the specified role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>True if the user has the role, false otherwise</returns>
    public bool HasRole(Role role)
    {
        return _userRoles.Any(ur => ur.RoleId == role.Id && ur.IsActive);
    }

    /// <summary>
    /// Updates the user's profile information
    /// </summary>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="nationalId">User's national ID</param>
    public void UpdateProfile(string firstName, string lastName, NationalId nationalId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (nationalId == null)
            throw new ArgumentException("National ID cannot be null", nameof(nationalId));

        // Update basic information
        FirstName = firstName;
        LastName = lastName;
        NationalId = nationalId;
        
        // Raise domain event
        AddDomainEvent(new UserProfileUpdatedEvent(Id, firstName, lastName, nationalId.Value));
    }

    /// <summary>
    /// Sets external source metadata for audit and reconciliation
    /// </summary>
    public void SetSourceMetadata(string sourceSystem, string? sourceVersion, string? sourceChecksum)
    {
        if (string.IsNullOrWhiteSpace(sourceSystem))
            throw new ArgumentException("Source system cannot be empty", nameof(sourceSystem));

        SourceSystem = sourceSystem.Trim();
        SourceVersion = string.IsNullOrWhiteSpace(sourceVersion) ? null : sourceVersion.Trim();
        SourceChecksum = string.IsNullOrWhiteSpace(sourceChecksum) ? null : sourceChecksum.Trim();
    }

    /// <summary>
    /// Sets profile snapshot payload for audit purposes
    /// </summary>
    public void SetProfileSnapshot(string profileSnapshot)
    {
        ProfileSnapshot = string.IsNullOrWhiteSpace(profileSnapshot) ? null : profileSnapshot;
    }

    /// <summary>
    /// Adds a direct claim to the user
    /// </summary>
    public void AddClaim(string type, string value, DateTime? expiresAt = null, string? assignedBy = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Claim type and value cannot be empty");

        var claim = new Shared.Domain.ValueObjects.Claim(type.Trim(), value.Trim());
        var userClaim = new UserClaim(Id, claim, expiresAt, assignedBy, notes);
        _userClaims.Add(userClaim);
    }

    /// <summary>
    /// Gets the user's roles as a list
    /// </summary>
    /// <returns>List of roles the user has</returns>
    public List<Role> GetRoles()
    {
        var roles = new List<Role>();
        foreach (Role role in _userRoles.Select(ur => ur.Role))
        {
            if (HasRole(role))
            {
                roles.Add(role);
            }
        }
        return roles;
    }

    public void LoggedIn()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    // OTP Authentication Methods

    /// <summary>
    /// Marks the phone number as verified
    /// </summary>
    public void VerifyPhone()
    {
        if (IsPhoneVerified)
            throw new InvalidOperationException("Phone number is already verified");

        IsPhoneVerified = true;
        PhoneVerifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a successful authentication
    /// </summary>
    /// <param name="deviceFingerprint">Optional device fingerprint</param>
    /// <param name="ipAddress">IP address used</param>
    /// <param name="userAgent">User agent used</param>
    public void RecordSuccessfulAuthentication(
        DeviceFingerprint? deviceFingerprint = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot authenticate with inactive account");

        if (IsLocked())
            throw new InvalidOperationException("Cannot authenticate with locked account");

        LastAuthenticatedAt = DateTime.UtcNow;
        FailedAttempts = 0; // Reset failed attempts on success
        LastLoginAt = DateTime.UtcNow; // Also update last login

        if (deviceFingerprint != null)
            LastDeviceFingerprint = deviceFingerprint;

        if (!string.IsNullOrWhiteSpace(ipAddress))
            LastIpAddress = ipAddress;

        if (!string.IsNullOrWhiteSpace(userAgent))
            LastUserAgent = userAgent;
    }

    /// <summary>
    /// Records a failed authentication attempt
    /// </summary>
    /// <param name="maxAttempts">Maximum allowed attempts before locking</param>
    /// <param name="lockDurationMinutes">Duration to lock account if max attempts exceeded</param>
    public void RecordFailedAuthentication(int maxAttempts = 5, int lockDurationMinutes = 15)
    {
        if (IsLocked())
            return; // Already locked, don't increment

        FailedAttempts++;
        LastAuthenticatedAt = DateTime.UtcNow;

        if (FailedAttempts >= maxAttempts)
        {
            Lock($"Too many failed authentication attempts ({FailedAttempts})", lockDurationMinutes);
        }
    }

    /// <summary>
    /// Locks the account for security reasons
    /// </summary>
    /// <param name="reason">Reason for locking</param>
    /// <param name="lockDurationMinutes">Duration to lock account (0 for indefinite)</param>
    public void Lock(string reason, int lockDurationMinutes = 0)
    {
        if (IsLocked())
            return; // Already locked

        IsActive = false;
        LockedAt = DateTime.UtcNow;
        LockReason = reason;

        if (lockDurationMinutes > 0)
        {
            UnlockAt = DateTime.UtcNow.AddMinutes(lockDurationMinutes);
        }
    }

    /// <summary>
    /// Unlocks the account
    /// </summary>
    /// <param name="reason">Reason for unlocking</param>
    public void Unlock(string reason)
    {
        if (!IsLocked())
            return; // Not locked

        IsActive = true;
        LockedAt = null;
        LockReason = null;
        UnlockAt = null;
        FailedAttempts = 0;
    }

    /// <summary>
    /// Checks if the account is currently locked
    /// </summary>
    public bool IsLocked()
    {
        if (!IsActive || LockedAt == null)
            return false;

        // If unlock time is set and has passed, auto-unlock
        if (UnlockAt.HasValue && DateTime.UtcNow >= UnlockAt.Value)
        {
            Unlock("Auto-unlocked after lock duration expired");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the user can authenticate
    /// </summary>
    public bool CanAuthenticate => IsActive && !IsLocked() && IsPhoneVerified;

    /// <summary>
    /// Gets the remaining lock time in minutes (if locked)
    /// </summary>
    public int? RemainingLockTimeMinutes
    {
        get
        {
            if (!IsLocked() || !UnlockAt.HasValue)
                return null;

            var remaining = UnlockAt.Value - DateTime.UtcNow;
            return remaining.TotalMinutes > 0 ? (int)remaining.TotalMinutes : 0;
        }
    }

    /// <summary>
    /// Checks if the user has a verified phone number for OTP authentication
    /// </summary>
    public bool HasVerifiedPhone => !string.IsNullOrWhiteSpace(PhoneNumber) && IsPhoneVerified;

    // User Preference Management Methods

    /// <summary>
    /// Adds a preference to the user's collection
    /// </summary>
    /// <param name="preference">The preference to add</param>
    public void AddPreference(UserPreference preference)
    {
        if (preference == null)
            throw new ArgumentNullException(nameof(preference));

        if (preference.UserId != Id)
            throw new InvalidOperationException("Preference does not belong to this user");

        if (_preferences.Any(p => p.Key.Value.Equals(preference.Key.Value, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Preference '{preference.Key.Value}' already exists for this user");

        _preferences.Add(preference);
    }

    /// <summary>
    /// Removes a preference from the user's collection
    /// </summary>
    /// <param name="preference">The preference to remove</param>
    public void RemovePreference(UserPreference preference)
    {
        if (preference == null)
            throw new ArgumentNullException(nameof(preference));

        _preferences.Remove(preference);
    }

    /// <summary>
    /// Gets a preference by key
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <returns>The preference if found, null otherwise</returns>
    public UserPreference? GetPreference(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        return _preferences.FirstOrDefault(p => p.Key.Value.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets a preference value by key with fallback to default
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <param name="defaultValue">Default value if preference not found</param>
    /// <returns>The preference value or default</returns>
    public string GetPreferenceValue(string key, string defaultValue = "")
    {
        var preference = GetPreference(key);
        return preference?.Value.RawValue ?? defaultValue;
    }

    /// <summary>
    /// Gets a typed preference value by key with fallback to default
    /// </summary>
    /// <typeparam name="T">The expected type</typeparam>
    /// <param name="key">The preference key</param>
    /// <param name="defaultValue">Default value if preference not found</param>
    /// <returns>The typed preference value or default</returns>
    public T GetPreferenceValue<T>(string key, T defaultValue = default!)
    {
        var preference = GetPreference(key);
        if (preference == null)
            return defaultValue;

        try
        {
            return preference.GetTypedValue<T>();
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Checks if the user has a specific preference
    /// </summary>
    /// <param name="key">The preference key</param>
    /// <returns>True if preference exists, false otherwise</returns>
    public bool HasPreference(string key)
    {
        return GetPreference(key) != null;
    }

    /// <summary>
    /// Gets all active preferences
    /// </summary>
    /// <returns>Collection of active preferences</returns>
    public IEnumerable<UserPreference> GetActivePreferences()
    {
        return _preferences.Where(p => p.IsActive);
    }

    /// <summary>
    /// Initializes default preferences for the user
    /// This method is called during user creation
    /// </summary>
    private void InitializeDefaultPreferences()
    {
        // Skip initialization if user doesn't have an ID yet (not saved to database)
        if (Id == Guid.Empty)
            return;
            
        var defaultPreferences = UserPreferenceDefaultsService.CreateDefaultPreferences(Id);
        
        foreach (var preference in defaultPreferences)
        {
            _preferences.Add(preference);
        }
    }

    /// <summary>
    /// Ensures the user has all required default preferences
    /// This method can be called for existing users to add missing defaults
    /// </summary>
    public void EnsureDefaultPreferences()
    {
        var missingPreferences = UserPreferenceDefaultsService.GetMissingDefaultPreferences(Id, _preferences);
        
        foreach (var preference in missingPreferences)
        {
            _preferences.Add(preference);
        }
    }

    /// <summary>
    /// Checks if the user has all required default preferences
    /// </summary>
    /// <returns>True if user has all default preferences, false otherwise</returns>
    public bool HasAllDefaultPreferences()
    {
        return UserPreferenceDefaultsService.HasAllDefaultPreferences(_preferences);
    }

    /// <summary>
    /// Gets the count of missing default preferences
    /// </summary>
    /// <returns>Number of missing default preferences</returns>
    public int GetMissingDefaultPreferencesCount()
    {
        return UserPreferenceDefaultsService.GetMissingDefaultPreferencesCount(_preferences);
    }

    /// <summary>
    /// Gets preferences by category
    /// </summary>
    /// <param name="category">The preference category</param>
    /// <returns>Collection of preferences in the specified category</returns>
    public IEnumerable<UserPreference> GetPreferencesByCategory(PreferenceCategory category)
    {
        return _preferences.Where(p => p.Category == category).OrderBy(p => p.DisplayOrder);
    }

    /// <summary>
    /// Gets active preferences by category
    /// </summary>
    /// <param name="category">The preference category</param>
    /// <returns>Collection of active preferences in the specified category</returns>
    public IEnumerable<UserPreference> GetActivePreferencesByCategory(PreferenceCategory category)
    {
        return _preferences.Where(p => p.Category == category && p.IsActive).OrderBy(p => p.DisplayOrder);
    }

    /// <summary>
    /// Gets the count of preferences by category
    /// </summary>
    /// <param name="category">The preference category</param>
    /// <returns>Number of preferences in the specified category</returns>
    public int GetPreferencesCountByCategory(PreferenceCategory category)
    {
        return _preferences.Count(p => p.Category == category);
    }

    /// <summary>
    /// Gets the count of active preferences by category
    /// </summary>
    /// <param name="category">The preference category</param>
    /// <returns>Number of active preferences in the specified category</returns>
    public int GetActivePreferencesCountByCategory(PreferenceCategory category)
    {
        return _preferences.Count(p => p.Category == category && p.IsActive);
    }

    // Role Management Methods

    /// <summary>
    /// Assigns a role to the user
    /// </summary>
    /// <param name="roleId">The role ID to assign</param>
    /// <param name="expiresAt">Optional expiration date</param>
    /// <param name="assignedBy">Who assigned the role</param>
    /// <param name="notes">Optional notes</param>
    public void AssignRole(Guid roleId, DateTime? expiresAt = null, string? assignedBy = null, string? notes = null)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        // Check if user already has this role
        if (_userRoles.Any(ur => ur.RoleId == roleId && ur.IsActive))
            throw new InvalidOperationException("User already has this role");

        var userRole = new UserRole(Id, roleId, expiresAt, assignedBy, notes);
        _userRoles.Add(userRole);
    }
    
    public void AssignRole(Role role, DateTime? expiresAt = null, string? assignedBy = null, string? notes = null)
    {

      // Check if user already has this role
      if (_userRoles.Any(ur => ur.RoleId == role.Id && ur.IsActive))
        throw new InvalidOperationException("User already has this role");

      var userRole = new UserRole(Id, role, expiresAt, assignedBy, notes);
      _userRoles.Add(userRole);
    }

    /// <summary>
    /// Removes a role from the user
    /// </summary>
    /// <param name="roleId">The role ID to remove</param>
    public void RemoveRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId && ur.IsActive);
        if (userRole != null)
        {
            userRole.Deactivate();
        }
    }

    /// <summary>
    /// Checks if the user has a specific role
    /// </summary>
    /// <param name="roleId">The role ID to check</param>
    /// <returns>True if user has the role and it's active</returns>
    public bool HasRole(Guid roleId)
    {
        return _userRoles.Any(ur => ur.RoleId == roleId && ur.IsValid());
    }

    /// <summary>
    /// Checks if the user has a role by name
    /// </summary>
    /// <param name="roleName">The role name to check</param>
    /// <returns>True if user has the role and it's active</returns>
    public bool HasRole(string roleName)
    {
        return _userRoles.Any(ur => ur.Role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) && ur.IsValid());
    }

    /// <summary>
    /// Gets all active roles for the user
    /// </summary>
    /// <returns>Collection of active user roles</returns>
    public IEnumerable<UserRole> GetActiveRoles()
    {
        return _userRoles.Where(ur => ur.IsValid()).OrderBy(ur => ur.AssignedAt);
    }

    /// <summary>
    /// Gets all roles for the user (including inactive)
    /// </summary>
    /// <returns>Collection of all user roles</returns>
    public IEnumerable<UserRole> GetAllRoles()
    {
        return _userRoles.OrderBy(ur => ur.AssignedAt);
    }

    /// <summary>
    /// Gets the count of active roles
    /// </summary>
    /// <returns>Number of active roles</returns>
    public int GetActiveRoleCount()
    {
        return _userRoles.Count(ur => ur.IsValid());
    }

    /// <summary>
    /// Gets roles that will expire soon
    /// </summary>
    /// <param name="timeThreshold">Time threshold for "soon"</param>
    /// <returns>Collection of roles expiring soon</returns>
    public IEnumerable<UserRole> GetRolesExpiringSoon(TimeSpan timeThreshold)
    {
        return _userRoles.Where(ur => ur.IsActive && ur.WillExpireSoon(timeThreshold));
    }

    /// <summary>
    /// Gets expired roles
    /// </summary>
    /// <returns>Collection of expired roles</returns>
    public IEnumerable<UserRole> GetExpiredRoles()
    {
        return _userRoles.Where(ur => ur.IsExpired());
    }

    /// <summary>
    /// Checks if the user has any of the specified roles
    /// </summary>
    /// <param name="roleIds">Collection of role IDs to check</param>
    /// <returns>True if user has any of the roles</returns>
    public bool HasAnyRole(IEnumerable<Guid> roleIds)
    {
        return _userRoles.Any(ur => roleIds.Contains(ur.RoleId) && ur.IsValid());
    }

    /// <summary>
    /// Checks if the user has all of the specified roles
    /// </summary>
    /// <param name="roleIds">Collection of role IDs to check</param>
    /// <returns>True if user has all of the roles</returns>
    public bool HasAllRoles(IEnumerable<Guid> roleIds)
    {
        var userRoleIds = _userRoles.Where(ur => ur.IsValid()).Select(ur => ur.RoleId);
        return roleIds.All(roleId => userRoleIds.Contains(roleId));
    }

    public void RevokeAllUserRefreshTokens(bool isSoftDelete = true)
    {

      var tokens = _tokens
        .Where(t => t is { TokenType: "RefreshToken", IsRevoked: false });

      if (isSoftDelete)
      {
        foreach (var token in tokens)
        {
          token.Revoke();
        }
      }
      else
      {
        foreach (var token in tokens)
        {
          _tokens.Remove(token);
        }
      }


    }

    public  IEnumerable<UserToken> GetActiveTokensForUserAsync(Guid userId, string? tokenType = null)
    {
      var query = _tokens
        .Where(t => t.UserId == userId && !t.IsUsed && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

      if (!string.IsNullOrEmpty(tokenType))
      {
        query = query.Where(t => t.TokenType == tokenType);
      }

      return  query;
    }

    public void AssignToken(UserToken jwtToken)
    {
      _tokens.Add(jwtToken);
    }

    // Scope Management Methods

    /// <summary>
    /// Gets all scopes available to the user based on their active roles
    /// </summary>
    /// <returns>Collection of scope values the user has access to</returns>
    public IEnumerable<string> GetUserScopes()
    {
        var scopes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var userRole in UserRoles.Where(ur => ur.IsValid()))
        {
            var role = userRole.Role;
            if (role != null && role.IsActive)
            {
                var roleScopeClaims = role.GetScopeClaims();
                foreach (var scopeClaim in roleScopeClaims)
                {
                    scopes.Add(scopeClaim.Value);
                }
            }
        }

        return scopes;
    }

    /// <summary>
    /// Validates if the user has access to a specific scope
    /// </summary>
    /// <param name="requestedScope">The scope to validate</param>
    /// <returns>True if user has access to the scope, false otherwise</returns>
    public bool ValidateScope(string requestedScope)
    {
        if (string.IsNullOrWhiteSpace(requestedScope))
            return false;

        var userScopes = GetUserScopes();
        return userScopes.Contains(requestedScope, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates if the user has access to any of the specified scopes
    /// </summary>
    /// <param name="requestedScopes">Collection of scopes to validate</param>
    /// <returns>True if user has access to any of the scopes, false otherwise</returns>
    public bool ValidateAnyScope(IEnumerable<string> requestedScopes)
    {
        if (requestedScopes == null || !requestedScopes.Any())
            return false;

        var userScopes = GetUserScopes();
        return requestedScopes.Any(scope => userScopes.Contains(scope, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Validates if the user has access to all of the specified scopes
    /// </summary>
    /// <param name="requestedScopes">Collection of scopes to validate</param>
    /// <returns>True if user has access to all of the scopes, false otherwise</returns>
    public bool ValidateAllScopes(IEnumerable<string> requestedScopes)
    {
        if (requestedScopes == null || !requestedScopes.Any())
            return false;

        var userScopes = GetUserScopes();
        return requestedScopes.All(scope => userScopes.Contains(scope, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the scopes that the user is missing from a requested collection
    /// </summary>
    /// <param name="requestedScopes">Collection of requested scopes</param>
    /// <returns>Collection of scopes the user doesn't have access to</returns>
    public IEnumerable<string> GetMissingScopes(IEnumerable<string> requestedScopes)
    {
        if (requestedScopes == null || !requestedScopes.Any())
            return Enumerable.Empty<string>();

        var userScopes = GetUserScopes();
        return requestedScopes.Where(scope => !userScopes.Contains(scope, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets scope claims from all active user roles
    /// </summary>
    /// <returns>Collection of scope claims</returns>
    public IEnumerable<Shared.Domain.ValueObjects.Claim> GetScopeClaims()
    {
        var scopeClaims = new List<Shared.Domain.ValueObjects.Claim>();

        foreach (var userRole in UserRoles.Where(ur => ur.IsValid()))
        {
            var role = userRole.Role;
            if (role != null && role.IsActive)
            {
                scopeClaims.AddRange(role.GetScopeClaims());
            }
        }

        return scopeClaims.Distinct();
    }

    /// <summary>
    /// Checks if the user has access to a scope with specific claim type
    /// </summary>
    /// <param name="claimType">The claim type to check</param>
    /// <param name="claimValue">The claim value to check</param>
    /// <returns>True if user has the specific claim, false otherwise</returns>
    public bool HasScopeClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
            return false;

        var scopeClaims = GetScopeClaims();
        return scopeClaims.Any(claim => 
            claim.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) &&
            claim.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the count of available scopes for the user
    /// </summary>
    /// <returns>Number of scopes available to the user</returns>
    public int GetScopeCount()
    {
        return GetUserScopes().Count();
    }

    /// <summary>
    /// Checks if the user has any scopes available
    /// </summary>
    /// <returns>True if user has at least one scope, false otherwise</returns>
    public bool HasAnyScopes()
    {
        return GetUserScopes().Any();
    }
}
