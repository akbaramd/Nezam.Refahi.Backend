# Quick Start Guide - Client Migration

## TL;DR - What Changed?

Three new boolean fields were added to capacity DTOs:
- `isRegistrationOpen` - Can users register right now?
- `isFullyBooked` - Is capacity completely full?
- `isNearlyFull` - Is capacity ≥80% full?

**Action Required**: Update your TypeScript types and use these fields instead of calculating manually.

## 1. Update Type Definitions

Copy the types from `CLIENT_TYPESCRIPT_TYPES.d.ts` or add these fields:

```typescript
interface CapacityDetailDto {
  // ... existing fields
  isRegistrationOpen: boolean;  // NEW
  isFullyBooked: boolean;        // NEW
  isNearlyFull: boolean;       // NEW
}

interface CapacitySummaryDto {
  // ... existing fields
  isRegistrationOpen: boolean;  // NEW
  isFullyBooked: boolean;       // NEW
  isNearlyFull: boolean;        // NEW
}
```

## 2. Replace Manual Calculations

### ❌ Before (Don't do this)
```typescript
// Manual calculation - remove this
const now = new Date();
const isOpen = capacity.isActive && 
               now >= new Date(capacity.registrationStart) && 
               now <= new Date(capacity.registrationEnd);
const isFull = capacity.remainingParticipants <= 0;
```

### ✅ After (Do this)
```typescript
// Use DTO fields directly
const isOpen = capacity.isRegistrationOpen;
const isFull = capacity.isFullyBooked;
const nearlyFull = capacity.isNearlyFull;
```

## 3. Update UI Components

### Example: Capacity Card Component
```typescript
function CapacityCard({ capacity }: { capacity: CapacityDetailDto }) {
  // Simple status check
  if (!capacity.isRegistrationOpen) {
    return <div>Registration Closed</div>;
  }
  
  if (capacity.isFullyBooked) {
    return <div>Fully Booked</div>;
  }
  
  if (capacity.isNearlyFull) {
    return (
      <div className="warning">
        ⚠️ Nearly Full! Only {capacity.remainingParticipants} spots left
        <button>Book Now</button>
      </div>
    );
  }
  
  return (
    <div>
      {capacity.remainingParticipants} spots available
      <button>Book Now</button>
    </div>
  );
}
```

### Example: Tour List Filter
```typescript
function TourList({ tours }: { tours: TourDto[] }) {
  // Filter available tours
  const available = tours.filter(
    tour => tour.isRegistrationOpen && !tour.isFullyBooked
  );
  
  // Show urgent tours
  const urgent = available.filter(tour => tour.isNearlyFull);
  
  return (
    <div>
      {urgent.length > 0 && (
        <Alert>⚠️ {urgent.length} tours are nearly full!</Alert>
      )}
      {available.map(tour => (
        <TourCard key={tour.id} tour={tour} />
      ))}
    </div>
  );
}
```

## 4. Common Patterns

### Check if Booking is Allowed
```typescript
function canBookTour(tour: TourDto): boolean {
  return tour.isActive && 
         tour.isRegistrationOpen && 
         !tour.isFullyBooked;
}

function canBookCapacity(capacity: CapacityDetailDto): boolean {
  return capacity.isActive && 
         capacity.isRegistrationOpen && 
         !capacity.isFullyBooked;
}
```

### Display Status Badge
```typescript
function getStatusBadge(tour: TourDto) {
  if (!tour.isActive) return { text: 'Inactive', color: 'gray' };
  if (!tour.isRegistrationOpen) return { text: 'Closed', color: 'gray' };
  if (tour.isFullyBooked) return { text: 'Full', color: 'red' };
  if (tour.isNearlyFull) return { text: 'Nearly Full', color: 'orange' };
  return { text: 'Available', color: 'green' };
}
```

### Sort by Urgency
```typescript
function sortToursByUrgency(tours: TourDto[]): TourDto[] {
  return [...tours].sort((a, b) => {
    // Fully booked last
    if (a.isFullyBooked !== b.isFullyBooked) {
      return a.isFullyBooked ? 1 : -1;
    }
    // Nearly full first
    if (a.isNearlyFull !== b.isNearlyFull) {
      return a.isNearlyFull ? -1 : 1;
    }
    // Then by remaining capacity
    return a.remainingCapacity - b.remainingCapacity;
  });
}
```

## 5. Testing Checklist

- [ ] Type definitions updated
- [ ] Manual calculations removed
- [ ] UI components use new fields
- [ ] Fully booked tours show correct status
- [ ] Nearly full tours show warning
- [ ] Closed registration periods handled
- [ ] Booking buttons disabled when appropriate

## 6. Benefits

✅ **Consistency**: All clients use the same calculation logic  
✅ **Accuracy**: Server-side calculations are always correct  
✅ **Performance**: No client-side date/time calculations needed  
✅ **Maintainability**: Business logic changes in one place  
✅ **Type Safety**: TypeScript types ensure correct usage  

## Need Help?

- See `CLIENT_DTO_CHANGES.md` for detailed documentation
- See `CLIENT_TYPESCRIPT_TYPES.d.ts` for complete type definitions
- Check API response examples in the main documentation

---

**Migration Time Estimate**: 30-60 minutes for most applications

