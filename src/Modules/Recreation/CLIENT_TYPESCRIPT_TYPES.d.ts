/**
 * TypeScript type definitions for Recreation Module DTOs
 * Updated with new status fields for capacity availability
 */

/**
 * Tour DTO - Lightweight tour information for listings
 */
export interface TourDto {
  // Identity
  id: string;
  title: string;

  // Schedule
  tourStart: string; // ISO 8601 DateTime
  tourEnd: string; // ISO 8601 DateTime

  // Status
  isActive: boolean;
  status: string;
  capacityState: string; // "HasSpare" | "Tight" | "Full"

  // Registration window
  registrationStart: string | null; // ISO 8601 DateTime
  registrationEnd: string | null; // ISO 8601 DateTime
  isRegistrationOpen: boolean; // Calculated using domain behavior

  // Capacity aggregates (public numbers exclude special capacities)
  maxCapacity: number;
  remainingCapacity: number;
  reservedCapacity: number;
  utilizationPct: number; // 0..100
  isFullyBooked: boolean; // Calculated using domain behavior
  isNearlyFull: boolean; // ≥80% utilized, calculated using domain behavior

  // Relation summaries
  agencies: AgencySummaryDto[];
  features: FeatureSummaryDto[];
  photos: PhotoSummaryDto[];

  // Pricing summaries
  lowestPriceRials: number | null;
  highestPriceRials: number | null;
  hasDiscount: boolean;
  pricing: PricingDetailDto[];
}

/**
 * Detailed capacity information for a tour
 * NEW: Includes status fields for availability checking
 */
export interface CapacityDetailDto {
  id: string;
  registrationStart: string; // ISO 8601 DateTime
  registrationEnd: string; // ISO 8601 DateTime
  maxParticipants: number;
  remainingParticipants: number;
  allocatedParticipants: number;
  minParticipantsPerReservation: number;
  maxParticipantsPerReservation: number;
  isActive: boolean;
  isSpecial: boolean;
  capacityState: string; // "HasSpare" | "Tight" | "Full"
  
  /** NEW: Indicates whether registration is currently open for this capacity */
  isRegistrationOpen: boolean;
  
  /** NEW: Indicates whether this capacity is completely full (RemainingParticipants <= 0) */
  isFullyBooked: boolean;
  
  /** NEW: Indicates whether this capacity is nearly full (≥80% utilized but not fully booked) */
  isNearlyFull: boolean;
  
  description: string | null;
}

/**
 * Minimal capacity summary used in reservation details
 * NEW: Includes status fields for availability checking
 */
export interface CapacitySummaryDto {
  id: string;
  tourId: string;
  maxParticipants: number;
  registrationStart: string; // ISO 8601 DateTime
  registrationEnd: string; // ISO 8601 DateTime
  isActive: boolean;
  capacityState: string; // "HasSpare" | "Tight" | "Full"
  
  /** NEW: Indicates whether registration is currently open for this capacity */
  isRegistrationOpen: boolean;
  
  /** NEW: Indicates whether this capacity is completely full */
  isFullyBooked: boolean;
  
  /** NEW: Indicates whether this capacity is nearly full (≥80% utilized) */
  isNearlyFull: boolean;
  
  description: string | null;
}

/**
 * Reservation DTO - Compact reservation information
 */
export interface ReservationDto {
  id: string;
  tourId: string;
  trackingCode: string;
  status: string;
  reservationDate: string; // ISO 8601 DateTime
  
  // Extended fields
  expiryDate: string | null;
  confirmationDate: string | null;
  
  // Amounts
  totalAmountRials: number | null;
  paidAmountRials: number | null;
  remainingAmountRials: number | null;
  isFullyPaid: boolean;
  
  // Counts
  participantCount: number;
  mainParticipantCount: number;
  guestParticipantCount: number;
  
  // Quick flags
  isExpired: boolean;
  isConfirmed: boolean;
  isPending: boolean;
  isDraft: boolean;
  isPaying: boolean;
  isCancelled: boolean;
  isTerminal: boolean;
  
  // Cross-links
  capacityId: string | null;
  billId: string | null;
  
  // Tour hints
  tourTitle: string | null;
  tourStart: string | null;
  tourEnd: string | null;
  tourStatus: string | null;
  tourIsActive: boolean | null;
}

/**
 * Detailed reservation view with capacity information
 */
export interface ReservationDetailDto extends ReservationDto {
  cancellationDate: string | null;
  cancellationReason: string | null;
  memberId: string | null;
  externalUserId: string;
  
  // Capacity summary (includes NEW status fields)
  capacity: CapacitySummaryDto | null;
  
  // Tour brief
  tour: TourBriefDto | null;
  
  // Participants and pricing
  participants: ParticipantDto[];
  priceSnapshots: PriceSnapshotDto[];
  
  // Notes
  notes: string | null;
  tenantId: string;
  
  // Audit
  createdAt: string;
  updatedAt: string | null;
  createdBy: string;
  updatedBy: string | null;
}

// Supporting types
export interface AgencySummaryDto {
  agencyId: string;
  name: string;
}

export interface FeatureSummaryDto {
  featureId: string;
  name: string;
}

export interface PhotoSummaryDto {
  photoId: string;
  url: string;
  displayOrder: number;
}

export interface PricingDetailDto {
  participantType: string;
  basePriceRials: number;
  effectivePriceRials: number;
  discountPercentage: number | null;
  isActive: boolean;
  validFrom: string | null;
  validTo: string | null;
}

export interface TourBriefDto {
  id: string;
  title: string;
  tourStart: string;
  tourEnd: string;
  status: string;
  isActive: boolean;
}

export interface ParticipantDto {
  id: string;
  participantType: string;
  // ... other participant fields
}

export interface PriceSnapshotDto {
  participantType: string;
  finalPriceRials: number;
}

/**
 * Helper functions for client-side usage
 */
export namespace CapacityHelpers {
  /**
   * Check if a capacity can be booked
   */
  export function canBook(capacity: CapacityDetailDto | CapacitySummaryDto): boolean {
    return capacity.isActive && 
           capacity.isRegistrationOpen && 
           !capacity.isFullyBooked;
  }

  /**
   * Get display status for a capacity
   */
  export function getCapacityStatus(capacity: CapacityDetailDto | CapacitySummaryDto): 
    'available' | 'nearly-full' | 'fully-booked' | 'closed' {
    if (!capacity.isActive || !capacity.isRegistrationOpen) {
      return 'closed';
    }
    if (capacity.isFullyBooked) {
      return 'fully-booked';
    }
    if (capacity.isNearlyFull) {
      return 'nearly-full';
    }
    return 'available';
  }

  /**
   * Get display status for a tour
   */
  export function getTourStatus(tour: TourDto): 
    'available' | 'nearly-full' | 'fully-booked' | 'closed' | 'inactive' {
    if (!tour.isActive) {
      return 'inactive';
    }
    if (!tour.isRegistrationOpen) {
      return 'closed';
    }
    if (tour.isFullyBooked) {
      return 'fully-booked';
    }
    if (tour.isNearlyFull) {
      return 'nearly-full';
    }
    return 'available';
  }
}

