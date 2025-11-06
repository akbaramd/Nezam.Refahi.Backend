# Recreation Module - DTO Changes & Client Migration Guide

## Overview

This document describes the changes made to Tour and Reservation DTOs in the Recreation module. These changes ensure that all status fields are consistently calculated using domain behaviors and properly exposed to clients.

## Changes Summary

### 1. Capacity DTOs - New Fields Added

#### `CapacityDetailDto` - New Fields
The following boolean status fields have been added to help clients understand capacity availability:

- **`IsRegistrationOpen`** (bool): Indicates whether registration is currently open for this capacity
  - `true`: Registration is open (current date is between `RegistrationStart` and `RegistrationEnd`, and capacity is active)
  - `false`: Registration is closed or capacity is inactive

- **`IsFullyBooked`** (bool): Indicates whether this capacity is completely full
  - `true`: No remaining spots available (`RemainingParticipants <= 0`)
  - `false`: There are still spots available

- **`IsNearlyFull`** (bool): Indicates whether this capacity is nearly full (≥80% utilized)
  - `true`: Capacity is at least 80% utilized but not fully booked
  - `false`: Capacity has less than 80% utilization or is fully booked

#### `CapacitySummaryDto` - New Fields
Same fields as `CapacityDetailDto`:
- `IsRegistrationOpen` (bool)
- `IsFullyBooked` (bool)
- `IsNearlyFull` (bool)

### 2. Tour DTOs - No Breaking Changes

The following fields in `TourDto` remain unchanged but are now calculated using domain behaviors for consistency:
- `IsActive` (bool)
- `IsRegistrationOpen` (bool)
- `IsFullyBooked` (bool)
- `IsNearlyFull` (bool)
- `CapacityState` (string)

## Client Adaptation Guide

### For Tour Listings

#### Before (Manual Calculation - Not Recommended)
```typescript
// ❌ Don't calculate manually on client side
const isRegistrationOpen = tour.registrationStart <= now && 
                           tour.registrationEnd >= now && 
                           tour.isActive;

const isFullyBooked = tour.remainingCapacity <= 0;
const isNearlyFull = tour.utilizationPct >= 80 && !isFullyBooked;
```

#### After (Use DTO Fields - Recommended)
```typescript
// ✅ Use the provided fields directly
const isRegistrationOpen = tour.isRegistrationOpen;
const isFullyBooked = tour.isFullyBooked;
const isNearlyFull = tour.isNearlyFull;

// Display UI based on status
if (tour.isFullyBooked) {
  showMessage("Tour is fully booked");
} else if (tour.isNearlyFull) {
  showWarning("Tour is nearly full - book soon!");
} else if (tour.isRegistrationOpen) {
  showBookingButton();
}
```

### For Capacity Selection

#### Displaying Capacity Status
```typescript
interface CapacityDetailDto {
  id: Guid;
  registrationStart: DateTime;
  registrationEnd: DateTime;
  maxParticipants: number;
  remainingParticipants: number;
  isActive: boolean;
  isSpecial: boolean;
  capacityState: string;
  // NEW FIELDS
  isRegistrationOpen: boolean;
  isFullyBooked: boolean;
  isNearlyFull: boolean;
  description?: string;
}

// Example: Filter and display capacities
function renderCapacityCard(capacity: CapacityDetailDto) {
  // Check registration status
  if (!capacity.isRegistrationOpen) {
    return <div className="capacity-closed">Registration Closed</div>;
  }
  
  // Check availability
  if (capacity.isFullyBooked) {
    return <div className="capacity-full">Fully Booked</div>;
  }
  
  // Show warning if nearly full
  if (capacity.isNearlyFull) {
    return (
      <div className="capacity-warning">
        <span>Nearly Full - Only {capacity.remainingParticipants} spots left</span>
        <button>Book Now</button>
      </div>
    );
  }
  
  // Normal availability
  return (
    <div className="capacity-available">
      <span>{capacity.remainingParticipants} spots available</span>
      <button>Book Now</button>
    </div>
  );
}
```

### For Reservation Details

When displaying reservation details that include capacity information:

```typescript
interface ReservationDetailDto {
  // ... other fields
  capacity?: CapacitySummaryDto;
}

// Example: Show capacity status in reservation
function showReservationCapacity(reservation: ReservationDetailDto) {
  if (!reservation.capacity) {
    return <div>No capacity information</div>;
  }
  
  const cap = reservation.capacity;
  
  // Registration status
  if (!cap.isRegistrationOpen) {
    return <div>Registration period has ended</div>;
  }
  
  // Availability status
  const status = cap.isFullyBooked 
    ? "Fully Booked" 
    : cap.isNearlyFull 
    ? "Nearly Full" 
    : "Available";
  
  return (
    <div>
      <h3>Capacity Status: {status}</h3>
      <p>Max Participants: {cap.maxParticipants}</p>
      <p>Registration: {cap.registrationStart} to {cap.registrationEnd}</p>
    </div>
  );
}
```

## Field Semantics & Business Rules

### `IsRegistrationOpen`
- **Calculation**: `IsActive && currentDate >= RegistrationStart && currentDate <= RegistrationEnd`
- **Use Case**: Determines if users can currently register for this capacity/tour
- **Update Frequency**: Changes based on current date/time

### `IsFullyBooked`
- **Calculation**: `RemainingParticipants <= 0`
- **Use Case**: Prevents booking attempts when no spots are available
- **Update Frequency**: Changes when reservations are created/cancelled

### `IsNearlyFull`
- **Calculation**: `UtilizationPercentage >= 80 && !IsFullyBooked`
- **Use Case**: Shows urgency indicators to encourage faster booking
- **Update Frequency**: Changes when reservations are created/cancelled

### `CapacityState`
- **Values**: `"HasSpare"`, `"Tight"`, `"Full"`
- **Mapping**:
  - `HasSpare`: ≥50% remaining capacity
  - `Tight`: 10-50% remaining capacity
  - `Full`: <10% remaining capacity or fully booked

## UI/UX Recommendations

### 1. Tour List Display
```typescript
function TourCard({ tour }: { tour: TourDto }) {
  const getStatusBadge = () => {
    if (!tour.isActive) return <Badge color="gray">Inactive</Badge>;
    if (tour.isFullyBooked) return <Badge color="red">Fully Booked</Badge>;
    if (tour.isNearlyFull) return <Badge color="orange">Nearly Full</Badge>;
    if (tour.isRegistrationOpen) return <Badge color="green">Open</Badge>;
    return <Badge color="gray">Closed</Badge>;
  };
  
  return (
    <Card>
      <h3>{tour.title}</h3>
      {getStatusBadge()}
      <p>Capacity: {tour.remainingCapacity} / {tour.maxCapacity}</p>
      {tour.isRegistrationOpen && !tour.isFullyBooked && (
        <Button>Book Now</Button>
      )}
    </Card>
  );
}
```

### 2. Capacity Selection UI
```typescript
function CapacitySelector({ capacities }: { capacities: CapacityDetailDto[] }) {
  const availableCapacities = capacities.filter(
    cap => cap.isRegistrationOpen && !cap.isFullyBooked
  );
  
  const nearlyFullCapacities = availableCapacities.filter(
    cap => cap.isNearlyFull
  );
  
  return (
    <div>
      {nearlyFullCapacities.length > 0 && (
        <Alert type="warning">
          {nearlyFullCapacities.length} capacity/capacities are nearly full!
        </Alert>
      )}
      
      {availableCapacities.map(capacity => (
        <CapacityCard 
          key={capacity.id}
          capacity={capacity}
          highlight={capacity.isNearlyFull}
        />
      ))}
    </div>
  );
}
```

### 3. Real-time Updates
```typescript
// Poll or use WebSocket to update status fields
function useTourStatus(tourId: string) {
  const [tour, setTour] = useState<TourDto | null>(null);
  
  useEffect(() => {
    const interval = setInterval(async () => {
      const updated = await fetchTour(tourId);
      setTour(updated);
    }, 30000); // Update every 30 seconds
    
    return () => clearInterval(interval);
  }, [tourId]);
  
  return tour;
}
```

## Migration Checklist

- [ ] **Update TypeScript/Type Definitions**
  - Add new fields to `CapacityDetailDto` interface
  - Add new fields to `CapacitySummaryDto` interface
  - Verify `TourDto` fields are correctly typed

- [ ] **Remove Manual Calculations**
  - Remove client-side calculations for `IsRegistrationOpen`
  - Remove client-side calculations for `IsFullyBooked`
  - Remove client-side calculations for `IsNearlyFull`
  - Use DTO fields directly instead

- [ ] **Update UI Components**
  - Update capacity display components to use new fields
  - Update tour listing components to use new fields
  - Add visual indicators for `IsNearlyFull` status

- [ ] **Update Business Logic**
  - Replace manual availability checks with DTO field checks
  - Update filtering logic to use `IsRegistrationOpen`
  - Update booking eligibility checks

- [ ] **Test Scenarios**
  - Test with fully booked tours/capacities
  - Test with nearly full tours/capacities (>80%)
  - Test with closed registration periods
  - Test with active registration periods

## Backward Compatibility

✅ **No Breaking Changes**: All existing fields remain unchanged. New fields are additive only.

- Existing code will continue to work
- New fields provide additional information without removing functionality
- Old manual calculations can be gradually replaced

## API Response Examples

### TourDto Response
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Mountain Hiking Tour",
  "isActive": true,
  "isRegistrationOpen": true,
  "isFullyBooked": false,
  "isNearlyFull": true,
  "capacityState": "Tight",
  "maxCapacity": 50,
  "remainingCapacity": 8,
  "reservedCapacity": 42,
  "utilizationPct": 84.0,
  "registrationStart": "2024-01-01T00:00:00Z",
  "registrationEnd": "2024-06-30T23:59:59Z"
}
```

### CapacityDetailDto Response
```json
{
  "id": "456e7890-e89b-12d3-a456-426614174001",
  "registrationStart": "2024-01-01T00:00:00Z",
  "registrationEnd": "2024-06-30T23:59:59Z",
  "maxParticipants": 30,
  "remainingParticipants": 5,
  "allocatedParticipants": 25,
  "isActive": true,
  "isSpecial": false,
  "capacityState": "Tight",
  "isRegistrationOpen": true,
  "isFullyBooked": false,
  "isNearlyFull": true
}
```

## Support & Questions

For questions or issues regarding these changes:
1. Check this documentation first
2. Review the API response examples
3. Verify your TypeScript types are up to date
4. Contact the backend team if inconsistencies are found

---

**Last Updated**: [Current Date]
**Version**: 1.0.0
**Module**: Recreation

