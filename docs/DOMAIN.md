# Nezam Refahi Domain Architecture

## Overview

This document analyzes the domain layer architecture of Nezam Refahi Backend, focusing exclusively on the domain projects and their business logic. The system implements a modular monolith with four distinct bounded contexts following Domain-Driven Design principles.

## Domain Projects Structure

```
src/
├── Modules/
│   ├── Identity/Nezam.Refahi.Identity.Domain/
│   ├── Membership/Nezam.Refahi.Membership.Domain/
│   └── Settings/Nezam.Refahi.Settings.Domain/
└── Nezam.Refahi.Shared.Domain/
```

## 1. Identity Domain

**Purpose**: User authentication, authorization, and account management with OTP-based security.

### Core Entities

#### User (Aggregate Root)
```csharp
public class User : AggregateRoot<Guid>
```

**Business Responsibilities:**
- OTP-based authentication lifecycle
- Account security (locking, failed attempts, device tracking)
- Profile management (name, phone, national ID)
- User preference management with categories
- Role and claim assignment

**Key Domain Logic:**
- Phone number verification workflow
- Account locking based on failed attempts
- Authentication state tracking (last login, IP, device fingerprint)
- User preference validation and type safety

#### Role (Aggregate Root)
```csharp
public class Role : AggregateRoot<Guid>
```

**Business Responsibilities:**
- Hierarchical permission management through claims
- System role protection (cannot be modified/deleted)
- User assignment control with business rules
- Role lifecycle management

#### OtpChallenge (Entity)
```csharp
public class OtpChallenge : Entity<Guid>
```

**Business Responsibilities:**
- OTP verification with attempt limiting
- Security monitoring (IP, device fingerprint tracking)
- Rate limiting and suspicious activity detection
- Challenge state management (pending, verified, expired, locked)

### Value Objects
- **`OtpPolicy`**: Configures OTP behavior (length, TTL, attempt limits)
- **`DeviceFingerprint`**: Device identification for security tracking
- **`HashedSecret`**: Secure storage for sensitive data
- **`PreferenceKey/PreferenceValue`**: Type-safe user preferences

### Domain Services
- **`RoleDomainService`**: Cross-entity role operations and validation
- **`UserPreferenceDefaultsService`**: Default preference initialization

### Business Rules
- **`RoleBusinessRules`**: Role creation, modification, deletion rules
- **`UserPreferenceBusinessRules`**: Preference validation and constraints
- **`OtpChallengePolicy`**: Security policies for OTP challenges

## 2. Membership Domain

**Purpose**: Professional membership management with service type associations and claims.

### Core Entities

#### Member (Aggregate Root)
```csharp
public class Member : AggregateRoot<Guid>
```

**Business Responsibilities:**
- Professional membership lifecycle management
- Service type and field associations with expiration tracking
- Member-specific claims with temporal validity
- Integration with external systems via `ExternalUserId`

**Key Domain Logic:**
- Membership date validation and status calculation
- Service associations with expiration dates
- Claims management with activity states
- Personal information validation (email format, required fields)

#### ServiceType & ServiceField (Entities)
```csharp
public class ServiceType : Entity<Guid>
public class ServiceField : Entity<Guid>
```

**Business Responsibilities:**
- Professional service categorization
- Member service associations with expiration tracking
- Persian title management for localization

### Junction Entities
- **`MemberServiceType`**: Links members to service types with expiration
- **`MemberServiceField`**: Links members to service fields with expiration
- **`MemberClaim`**: Member-specific claims with temporal validity

### Key Business Logic
- Active service calculation based on expiration dates
- Claim expiration and automatic deactivation
- Service association lifecycle management
- Persian language support for service titles


## 3. Settings Domain

**Purpose**: Hierarchical system configuration with audit trails and type-safe value storage.

### Core Entities

#### Setting (Aggregate Root)
```csharp
public class Setting : AggregateRoot<Guid>
```

**Business Responsibilities:**
- Type-safe configuration value management
- Change audit trail generation
- Read-only setting protection
- Hierarchical categorization support

#### SettingsSection & SettingsCategory (Entities)
```csharp
public class SettingsSection : Entity<Guid>
public class SettingsCategory : Entity<Guid>
```

**Business Responsibilities:**
- Hierarchical organization (Section → Category → Setting)
- Display ordering and grouping logic
- Active status management

#### SettingChangeEvent (Entity)
```csharp
public class SettingChangeEvent : Entity<Guid>
```

**Business Responsibilities:**
- Complete change history tracking
- Before/after value comparison
- User attribution and timestamping

### Value Objects
- **`SettingKey`**: Validated setting identifiers with format rules
- **`SettingValue`**: Type-safe value storage (string, int, bool, decimal, DateTime, JSON)
- **`SettingType`**: Enumeration of supported value types
- **`SettingStatus`**: Configuration state management

### Domain Services
- **`SettingsDomainService`**: Cross-entity validation and coordination
- **`SettingsValidationService`**: Comprehensive validation logic

## 4. Shared Domain

**Purpose**: Common value objects and entities used across all bounded contexts.

### Value Objects

#### Iranian Localization
- **`NationalId`**: Iranian national ID with checksum validation
- **`PhoneNumber`**: Iranian phone number normalization (+98 format)

#### Security Primitives
- **`IpAddress`**: IP address validation and representation
- **`ClientId`**: Multi-tenant client identification
- **`Claim`**: Authorization claims for permissions and scopes

### Geographic Entities
- **`Province`**: Iranian provinces with predefined data
- **`City`**: Cities within provinces with hierarchical relationships

### Constants and Static Data
- **`IranProvinces`**: Complete list of Iranian provinces
- **`IranCities`**: City data with province associations
- **`IranGeographyConstants`**: Geographic constants and mappings

## Domain Relationships and Integration

### Inter-Domain Dependencies
```
Identity Domain
├── Uses: NationalId, PhoneNumber, Claim (from Shared)
├── Publishes: UserCreatedEvent, UserPhoneVerifiedEvent
└── Integrates with: Membership (via ExternalUserId)

Membership Domain  
├── Uses: NationalId, PhoneNumber (from Shared)
├── Consumes: UserCreatedEvent (from Identity)
└── Manages: Service associations with Persian titles

Settings Domain
├── Uses: Base domain patterns (from Shared)
└── Provides: System-wide configuration

Shared Domain
└── Provides: Common value objects and Iranian localization
```

### Key Integration Patterns
- **User-Member Linking**: `Member.ExternalUserId` → `User.Id`
- **Event-Driven Integration**: Domain events for cross-context communication
- **Shared Value Objects**: Common validation and business rules
- **Persian Language Support**: Localized titles and validation rules

## Business Rules and Validation

### Identity Domain Rules
- OTP challenges have rate limiting per phone/IP/device
- Users can be locked after failed attempts with automatic unlocking
- System roles cannot be modified or deleted
- Phone numbers must be verified before full account activation

### Membership Domain Rules
- Members must have valid membership dates for active status
- Service associations can expire and become inactive
- Claims have temporal validity and automatic expiration
- Persian service titles are stored directly (no English conversion)

### Settings Domain Rules
- Read-only settings cannot be modified
- Setting keys must be unique within their category
- All changes generate audit events with user attribution
- Hierarchical structure must be maintained (Section → Category → Setting)

### Shared Domain Rules
- Iranian national IDs must pass checksum validation
- Phone numbers are normalized to international format
- Geographic data is immutable and predefined
- Claims must have valid type and value combinations

## Domain Events

### Identity Events
- `UserCreatedEvent`: New user registration
- `UserPhoneVerifiedEvent`: Phone verification completed
- `UserProfileUpdatedEvent`: Profile information changes
- `UserAuthenticatedEvent`: Successful authentication
- `UserRoleAssignedEvent`: Role assignment changes

### Membership Events
- Member creation and updates (consumed from Identity events)
- Service association changes
- Claim expiration and updates

### Settings Events
- Configuration changes with audit trails
- Bulk setting updates
- System configuration validation

## Architecture Patterns

### Domain-Driven Design
- **Aggregate Roots**: Clear transactional boundaries
- **Value Objects**: Immutable domain primitives with validation
- **Domain Services**: Cross-entity business logic
- **Domain Events**: Loose coupling between bounded contexts

### Security & Audit
- **OTP Security**: Complete challenge lifecycle with rate limiting
- **Audit Trails**: Change tracking with user attribution
- **Role-Based Access**: Claims-based authorization
- **Security Monitoring**: IP and device tracking

### Iranian Localization
- **National ID Validation**: Checksum algorithm implementation
- **Phone Number Normalization**: Iranian format standardization
- **Persian Language Support**: Direct storage without conversion
- **Geographic Data**: Complete Iranian provinces and cities

This domain architecture supports the Iranian Engineering Council's requirements while maintaining clean separation of concerns, robust business rule enforcement, and strong security considerations.