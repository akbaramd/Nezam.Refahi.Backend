# Bounded Contexts in Nezam.Refahi Domain

This project follows Domain-Driven Design (DDD) principles by organizing the domain model into separate bounded contexts. Each bounded context manages its own boundary and has its own domain model, services, and repositories.

## Structure

The domain is organized into the following bounded contexts:

### Identity Bounded Context

Responsible for managing user identities and authentication.

- **Entities**: User
- **Value Objects**: NationalId
- **Repositories**: IUserRepository
- **Domain Services**: UserDomainService

### Shared Bounded Context

Contains shared domain elements that are used across multiple bounded contexts.

- **Entities**: BaseEntity

## Domain-Driven Design Implementation

This implementation follows DDD principles:

1. **Rich Domain Model**: Entities contain business logic and enforce invariants
2. **Value Objects**: Immutable objects that represent concepts with no identity (e.g., NationalId)
3. **Domain Services**: For operations that don't naturally belong to a single entity
4. **Repositories**: Abstractions for data access

## Design Decisions

- **Entity vs. Domain Service**: 
  - Logic that belongs to a single entity and operates on that entity's state is implemented as a method inside the entity
  - Logic that involves coordination across multiple entities or doesn't naturally belong to a single entity is implemented as a domain service

- **Value Objects**:
  - Used for concepts that are defined by their attributes rather than an identity
  - Immutable and validate their state on creation
  - Example: NationalId is a value object that validates the format of Iranian national IDs

## Adding New Bounded Contexts

To add a new bounded context:

1. Create a new directory under `BoundedContexts`
2. Create subdirectories for Entities, ValueObjects, Repositories, and Services
3. Implement the domain model for the bounded context
4. Define clear boundaries and context mapping between bounded contexts
