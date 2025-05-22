using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Events;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Services;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared;
using System.Reflection;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Services.Payment
{
    public class PaymentAccommodationIntegrationServiceTests
    {
        private readonly Mock<IPaymentRepository> _mockPaymentRepository;
        private readonly Mock<IReservationRepository> _mockReservationRepository;
        private readonly PaymentDomainService _paymentDomainService;
        private readonly PaymentAccommodationIntegrationService _integrationService;
        
        private readonly Guid _reservationId;
        private readonly Guid _customerId;
        private readonly Guid _hotelId;
        private readonly Guid _cityId;
        private readonly Guid _provinceId;
        private readonly Guid _paymentId;
        private readonly PaymentTransaction _paymentTransaction;
        
        /// <summary>
        /// سازنده کلاس تست که وظیفه آماده‌سازی شرایط اولیه برای اجرای تست‌ها را بر عهده دارد.
        /// در این متد، نمونه‌های موک از مخازن و سرویس‌های دامنه ایجاد می‌شوند
        /// و داده‌های پایه برای استفاده در تست‌ها تنظیم می‌شود.
        /// </summary>
        public PaymentAccommodationIntegrationServiceTests()
        {
            _mockPaymentRepository = new Mock<IPaymentRepository>(MockBehavior.Strict);
            _mockReservationRepository = new Mock<IReservationRepository>(MockBehavior.Strict);
            
            // Create a real domain service for testing
            _paymentDomainService = new PaymentDomainService(_mockPaymentRepository.Object);
            
            _integrationService = new PaymentAccommodationIntegrationService(
                _mockReservationRepository.Object,
                _mockPaymentRepository.Object,
                _paymentDomainService);
            
            _reservationId = Guid.NewGuid();
            _customerId = Guid.NewGuid();
            _hotelId = Guid.NewGuid();
            _cityId = Guid.NewGuid();
            _provinceId = Guid.NewGuid();
            _paymentId = Guid.NewGuid();
            
            _paymentTransaction = new PaymentTransaction(
                _paymentId,
                _reservationId,
                _customerId,
                new Money(100, "USD"),
                PaymentMethod.CreditCard,
                "Test Gateway",
                false,
                null
            );
            // Mark payment as completed to allow refunds
            _paymentTransaction.SetTransactionReference("TX12345");
            _paymentTransaction.MarkAsCompleted("RCPT12345");
            
            // Setup base repository operations needed for most tests
            _mockReservationRepository.Setup(r => r.GetByIdAsync(_reservationId))
                .ReturnsAsync(() => {
                    var reservation = CreateTestReservation();
                    return reservation;
                });
            
            _mockPaymentRepository.Setup(p => p.GetByIdAsync(_paymentId))
                .ReturnsAsync(_paymentTransaction);
                
            _mockPaymentRepository.Setup(p => p.AddAsync(It.IsAny<PaymentTransaction>()))
                .Returns(Task.CompletedTask);
                
            _mockPaymentRepository.Setup(p => p.UpdateAsync(It.IsAny<PaymentTransaction>()))
                .Returns(Task.CompletedTask);
                
            _mockReservationRepository.Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);
                
            // Setup for total amount calculations
            _mockPaymentRepository.Setup(p => p.GetTotalAmountByReservationIdAsync(_reservationId))
                .ReturnsAsync(100m); // Default to fully paid
                
            _mockPaymentRepository.Setup(p => p.GetByReservationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<PaymentTransaction> { _paymentTransaction });
        }
        
        /// <summary>
        /// متدی کمکی برای ایجاد یک نمونه رزرو آزمایشی با مقادیر از پیش تعریف شده.
        /// این متد، یک شیء رزرو با قیمت کل، وضعیت و مقدار پرداخت شده مشخص ایجاد می‌کند.
        /// </summary>
        private Reservation CreateTestReservation()
        {
            var hotel = CreateTestHotel();
            var guest = CreateTestGuest();
            var stayPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 
                                         DateOnly.FromDateTime(DateTime.Now.AddDays(3)));
            
            var reservation = new Reservation(hotel, guest, stayPeriod);
            
            typeof(Reservation).GetProperty("Id")?.SetValue(reservation, _reservationId);
            typeof(Reservation).GetProperty("Status")?.SetValue(reservation, ReservationStatus.PendingPayment);
            typeof(Reservation).GetProperty("TotalPrice")?.SetValue(reservation, new Money(100, "USD"));
            
            // Set up PaidAmount to reflect completed payments
            typeof(Reservation).GetProperty("PaidAmount")?.SetValue(reservation, 100m);
            
            return reservation;
        }
        
        /// <summary>
        /// متدی کمکی برای ایجاد یک هتل آزمایشی با مشخصات از پیش تعریف شده.
        /// این هتل شامل موقعیت مکانی، قیمت و ظرفیت مشخص است.
        /// </summary>
        private Hotel CreateTestHotel()
        {
            var location = new LocationReference(_cityId, _provinceId, "Test City", "Test Province","Iran - Urmia");
            var hotel = new Hotel(_hotelId, "Test Hotel", "A test hotel description", 
                                 location, new Money(100, "USD"), 4);
            return hotel;
        }
        
        /// <summary>
        /// متدی کمکی برای ایجاد یک مهمان آزمایشی با مشخصات از پیش تعریف شده.
        /// این مهمان دارای نام، نام خانوادگی، کد ملی و سن مشخص است.
        /// </summary>
        private Guest CreateTestGuest()
        {
            var guest = new Guest("John", "Doe", "0123456789", 30);
            typeof(Guest).GetProperty("Id")?.SetValue(guest, _customerId);
            return guest;
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد ProcessReservationPaymentAsync رزرو را به‌روزرسانی می‌کند
        /// و تراکنش پرداخت را به درستی برمی‌گرداند.
        /// این تست مطابق با اصول DDD، تعامل بین حوزه‌های محدود پرداخت و اقامت را بررسی می‌کند.
        /// </summary>
        [Fact]
        public async Task ProcessReservationPaymentAsync_Updates_Reservation_And_Returns_Transaction()
        {
            // Setup additional mocks for this test
            var newTransactionId = Guid.NewGuid();
            var transaction = new PaymentTransaction(
                newTransactionId,
                _reservationId,
                _customerId,
                new Money(100, "USD"),
                PaymentMethod.CreditCard,
                "Test Gateway",
                false,
                null
            );
            
            _mockPaymentRepository.Setup(p => p.AddAsync(It.IsAny<PaymentTransaction>()))
                .Callback<PaymentTransaction>(t => {
                    typeof(PaymentTransaction).GetProperty("Id")?.SetValue(t, newTransactionId);
                })
                .Returns(Task.CompletedTask);
                
            _mockPaymentRepository.Setup(p => p.GetByIdAsync(newTransactionId))
                .ReturnsAsync(transaction);
            
            // Act
            var result = await _integrationService.ProcessReservationPaymentAsync(
                _reservationId, _customerId, new Money(100, "USD"), PaymentMethod.CreditCard, "Test Gateway");
            
            // Assert
            Assert.NotNull(result);
            _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Once);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد ProcessReservationPaymentAsync در صورت عدم وجود رزرو،
        /// به درستی استثنا پرتاب می‌کند. این تست اصل غنی‌سازی دامنه در DDD را بررسی می‌کند
        /// که طبق آن خطاها باید به صورت صریح مدیریت شوند.
        /// </summary>
        [Fact]
        public async Task ProcessReservationPaymentAsync_Throws_When_Reservation_Not_Found()
        {
            var nonExistingReservationId = Guid.NewGuid();
            
            _mockReservationRepository.Setup(r => r.GetByIdAsync(nonExistingReservationId))
                .ReturnsAsync((Reservation)null);
            
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _integrationService.ProcessReservationPaymentAsync(
                    nonExistingReservationId, _customerId, new Money(100, "USD"), PaymentMethod.CreditCard, 
                    "Test Gateway"));
            
            Assert.Contains($"Reservation with ID {nonExistingReservationId} not found", ex.Message);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد ProcessReservationPaymentAsync در صورت بروز خطا در آغاز پرداخت،
        /// به درستی آن را مدیریت می‌کند. این تست نشان می‌دهد که سرویس یکپارچه‌سازی
        /// باید در برابر خطاهای سرویس‌های دامنه مقاوم باشد.
        /// </summary>
        [Fact]
        public async Task ProcessReservationPaymentAsync_Handles_Payment_Initiation_Failure()
        {
            // Setup mocks to simulate a payment failure
            _mockPaymentRepository.Setup(p => p.AddAsync(It.IsAny<PaymentTransaction>()))
                .ThrowsAsync(new InvalidOperationException("Payment gateway error"));
            
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _integrationService.ProcessReservationPaymentAsync(
                    _reservationId, _customerId, new Money(100, "USD"), PaymentMethod.CreditCard, 
                    "Test Gateway"));
            
            Assert.Contains("Payment gateway error", ex.Message);
            
            _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Never);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد ProcessReservationRefundAsync به درستی بازپرداخت را پردازش می‌کند
        /// و رزرو را به‌روزرسانی می‌نماید. این تست تعامل میان دو حوزه محدود پرداخت و اقامت را بررسی می‌کند.
        /// </summary>
        [Fact]
        public async Task ProcessReservationRefundAsync_Processes_Refund_And_Updates_Reservation()
        {
            // Arrange
            var refundAmount = new Money(50, "USD");
            var refundTransactionId = Guid.NewGuid();
            
            // Create a reservation with sufficient paid amount to allow refunds
            var reservation = CreateTestReservation();
            reservation.ProcessPartialPayment(_paymentId, new Money(100, "USD")); // Ensure proper payment state
            
            _mockReservationRepository.Setup(r => r.GetByIdAsync(_reservationId))
                .ReturnsAsync(reservation);
            
            var refundTransaction = new PaymentTransaction(
                refundTransactionId,
                _reservationId,
                _customerId,
                refundAmount,
                PaymentMethod.CreditCard,
                "Test Gateway",
                true,
                _paymentId
            );
            
            // Setup the repository to return the refund transaction
            _mockPaymentRepository.Setup(p => p.AddAsync(It.IsAny<PaymentTransaction>()))
                .Callback<PaymentTransaction>(t => {
                    typeof(PaymentTransaction).GetProperty("Id")?.SetValue(t, refundTransactionId);
                })
                .Returns(Task.CompletedTask);
                
            _mockPaymentRepository.Setup(p => p.GetByIdAsync(refundTransactionId))
                .ReturnsAsync(refundTransaction);
            
            // Act
            var result = await _integrationService.ProcessReservationRefundAsync (
                _paymentId, refundAmount, "Test Gateway");
            
            // Assert
            Assert.NotNull(result);
            _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Once);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد ProcessReservationRefundAsync در صورت عدم وجود تراکنش پرداخت،
        /// به درستی استثنا پرتاب می‌کند. این بررسی نشان می‌دهد که سرویس یکپارچه‌سازی
        /// باید وجود موجودیت‌های مرتبط را تضمین کند.
        /// </summary>
        [Fact]
        public async Task ProcessReservationRefundAsync_Throws_When_Payment_Not_Found()
        {
            // Arrange
            var refundAmount = new Money(50, "USD");
            var nonExistentPaymentId = Guid.NewGuid();
            
            _mockPaymentRepository.Setup(p => p.GetByIdAsync(nonExistentPaymentId))
                .ReturnsAsync((PaymentTransaction)null);
            
            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _integrationService.ProcessReservationRefundAsync(
                    nonExistentPaymentId, refundAmount, "Test Gateway"));
            
            Assert.Contains($"Original payment transaction with ID {nonExistentPaymentId} not found", ex.Message);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد HandlePaymentCompletedAsync رزرو را با اطلاعات پرداخت
        /// به‌روزرسانی می‌کند. این تست نشان می‌دهد که رویدادهای دامنه پرداخت
        /// باید به درستی در حوزه محدود اقامت منعکس شوند.
        /// </summary>
        [Fact]
        public async Task HandlePaymentCompletedAsync_Updates_Reservation_With_Payment_Info()
        {
            var completionDate = DateTimeOffset.UtcNow;
            var paymentEvent = new PaymentCompletedEvent(
                _paymentId,
                _reservationId,
                _customerId,
                100,
                "USD",
                PaymentMethod.CreditCard,
                completionDate,       
                "TX12345",             
                "RN12345",             
                false,                 
                null                   
            );
            
            await _integrationService.HandlePaymentCompletedAsync(_paymentId);
            
            _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Once);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد HandlePaymentCompletedAsync در صورت عدم وجود رزرو،
        /// به درستی استثنا پرتاب می‌کند. این تست اهمیت وجود اجماعی در DDD را نشان می‌دهد
        /// که عملیات روی آن‌ها انجام می‌شود.
        /// </summary>
        [Fact]
        public async Task HandlePaymentCompletedAsync_Throws_When_Reservation_Not_Found()
        {
            // Arrange
            var nonExistentReservationId = Guid.NewGuid();
            var completionDate = DateTimeOffset.UtcNow;
            
            var paymentEvent = new PaymentCompletedEvent(
                _paymentId,
                nonExistentReservationId,
                _customerId,
                100,
                "USD",
                PaymentMethod.CreditCard,
                completionDate,       
                "TX12345",             
                "RN12345",             
                false,                 
                null                   
            );
            
            // Create a special transaction with non-existent reservation ID
            var specialTransaction = new PaymentTransaction(
                _paymentId,
                nonExistentReservationId,
                _customerId,
                new Money(100, "USD"),
                PaymentMethod.CreditCard,
                "Test Gateway",
                false,
                null
            );
            specialTransaction.SetTransactionReference("TX12345");
            specialTransaction.MarkAsCompleted("RN12345");
            
            // Setup to return our special transaction with non-existent reservation ID
            _mockPaymentRepository.Setup(p => p.GetByIdAsync(_paymentId))
                .ReturnsAsync(specialTransaction);
            
            // This is crucial - mock should return null for this specific reservation ID
            _mockReservationRepository.Setup(r => r.GetByIdAsync(nonExistentReservationId))
                .ReturnsAsync((Reservation)null);
                
            // Act & Assert - the integration service should throw when reservation not found
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _integrationService.HandlePaymentCompletedAsync(paymentEvent.PaymentId));
            
            Assert.Contains($"Reservation with ID {nonExistentReservationId} not found", ex.Message);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد HandlePaymentFailedAsync اطلاعات پرداخت ناموفق را
        /// در رزرو به‌روزرسانی می‌کند. این تست نشان می‌دهد که شکست‌های پرداخت
        /// باید به درستی در حوزه محدود اقامت منعکس شوند.
        /// </summary>
        [Fact]
        public async Task HandlePaymentFailedAsync_Updates_Reservation_With_Failed_Payment_Info()
        {
            var paymentEvent = new PaymentFailedEvent(
                _paymentId,
                _reservationId,
                _customerId,
                100,
                "USD",
                PaymentMethod.CreditCard,
                "Insufficient funds",
                DateTimeOffset.UtcNow
            );
            
            await _integrationService.HandlePaymentFailedAsync(paymentEvent.PaymentId);
            
            _mockReservationRepository.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Once);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد HandlePaymentFailedAsync در صورت عدم وجود رزرو،
        /// به درستی استثنا پرتاب می‌کند. این تست اصل جامعیت دامنه در DDD را نشان می‌دهد.
        /// </summary>
        [Fact]
        public async Task HandlePaymentFailedAsync_Throws_When_Reservation_Not_Found()
        {
            // Arrange
            var nonExistentReservationId = Guid.NewGuid();
            
            var paymentEvent = new PaymentFailedEvent(
                _paymentId,
                nonExistentReservationId,
                _customerId,
                100,
                "USD",
                PaymentMethod.CreditCard,
                "Insufficient funds",
                DateTimeOffset.UtcNow
            );
            
            // Create a special transaction with non-existent reservation ID
            var specialTransaction = new PaymentTransaction(
                _paymentId,
                nonExistentReservationId,
                _customerId,
                new Money(100, "USD"),
                PaymentMethod.CreditCard,
                "Test Gateway",
                false,
                null
            );
            specialTransaction.SetTransactionReference("TX12345");
            specialTransaction.MarkAsFailed("Insufficient funds");
            
            // Setup to return our special transaction with non-existent reservation ID
            _mockPaymentRepository.Setup(p => p.GetByIdAsync(_paymentId))
                .ReturnsAsync(specialTransaction);
            
            // Ensure the reservation repository returns null for this specific ID
            _mockReservationRepository.Setup(r => r.GetByIdAsync(nonExistentReservationId))
                .ReturnsAsync((Reservation)null);
            
            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _integrationService.HandlePaymentFailedAsync(paymentEvent.PaymentId));
            
            Assert.Contains($"Reservation with ID {nonExistentReservationId} not found", ex.Message);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد IsReservationFullyPaidAsync برای رزروهای رایگان،
        /// به درستی مقدار true برمی‌گرداند. این تست قانون تجاری مهمی را در دامنه بررسی می‌کند.
        /// </summary>
        [Fact]
        public async Task IsReservationFullyPaidAsync_Returns_True_For_Free_Reservation()
        {
            var freeReservationId = Guid.NewGuid();
            
            var hotel = CreateTestHotel();
            var guest = CreateTestGuest();
            var stayPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 
                                         DateOnly.FromDateTime(DateTime.Now.AddDays(2)));
            
            typeof(Hotel).GetProperty("PricePerNight")?.SetValue(hotel, new Money(0, "USD"));
            
            var freeReservation = new Reservation(hotel, guest, stayPeriod);
            
            _mockReservationRepository.Setup(r => r.GetByIdAsync(freeReservationId))
                .ReturnsAsync(freeReservation);
                
            // Setup for free reservation - no transactions needed
            _mockPaymentRepository.Setup(p => p.GetByReservationIdAsync(freeReservationId))
                .ReturnsAsync(new List<PaymentTransaction>());
                
            _mockPaymentRepository.Setup(p => p.GetTotalAmountByReservationIdAsync(freeReservationId))
                .ReturnsAsync(0m);
            
            var result = await _integrationService.IsReservationFullyPaidAsync(freeReservationId);
            
            Assert.True(result);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد IsReservationFullyPaidAsync زمانی که مبلغ پرداخت شده برابر با قیمت کل است،
        /// به درستی مقدار true برمی‌گرداند. این تست قانون تجاری مهمی را در دامنه بررسی می‌کند.
        /// </summary>
        [Fact]
        public async Task IsReservationFullyPaidAsync_Returns_True_When_Paid_Amount_Equals_Total_Price()
        {
            // Arrange
            // Create a reservation with a specific price
            var hotel = CreateTestHotel();
            var guest = CreateTestGuest();
            var stayPeriod = new DateRange(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 
                                         DateOnly.FromDateTime(DateTime.Now.AddDays(3)));
            
            var reservation = new Reservation(hotel, guest, stayPeriod);
            typeof(Reservation).GetProperty("Id")?.SetValue(reservation, _reservationId);
            typeof(Reservation).GetProperty("TotalPrice")?.SetValue(reservation, new Money(100, "USD"));
            
            // Set up repositories
            _mockReservationRepository.Setup(r => r.GetByIdAsync(_reservationId))
                .ReturnsAsync(reservation);
                
            // Complete transaction match the total price
            var completedTransaction = new PaymentTransaction(
                Guid.NewGuid(),
                _reservationId,
                _customerId,
                new Money(100, "USD"),
                PaymentMethod.CreditCard,
                "Test Gateway",
                false,
                null
            );
            completedTransaction.SetTransactionReference("TX12345");
            completedTransaction.MarkAsCompleted("RCPT12345");
            
            _mockPaymentRepository.Setup(p => p.GetByReservationIdAsync(_reservationId))
                .ReturnsAsync(new List<PaymentTransaction> { completedTransaction });
                
            _mockPaymentRepository.Setup(p => p.GetTotalAmountByReservationIdAsync(_reservationId))
                .ReturnsAsync(100m);
            
            // Act
            var result = await _integrationService.IsReservationFullyPaidAsync(_reservationId);
            
            // Assert
            Assert.True(result);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد IsReservationFullyPaidAsync زمانی که مبلغ پرداخت شده
        /// کمتر از قیمت کل است، به درستی مقدار false برمی‌گرداند.
        /// این تست تضمین می‌کند که قوانین تجاری تعیین وضعیت پرداخت کامل، به درستی اعمال می‌شوند.
        /// </summary>
        [Fact]
        public async Task IsReservationFullyPaidAsync_Returns_False_When_Paid_Amount_Less_Than_Total_Price()
        {
            // Create a transaction with half the amount
            var halfPaymentTransaction = new PaymentTransaction(
                Guid.NewGuid(),
                _reservationId,
                _customerId,
                new Money(50, "USD"),
                PaymentMethod.CreditCard,
                "Test Gateway",
                false,
                null
            );
            
            // Set up repository to return transactions that don't add up to the total amount
            _mockPaymentRepository.Setup(p => p.GetByReservationIdAsync(_reservationId))
                .ReturnsAsync(new List<PaymentTransaction> { halfPaymentTransaction });
                
            _mockPaymentRepository.Setup(p => p.GetTotalAmountByReservationIdAsync(_reservationId))
                .ReturnsAsync(50m);
            
            var result = await _integrationService.IsReservationFullyPaidAsync(_reservationId);
            
            Assert.False(result);
        }
        
        /// <summary>
        /// تست بررسی می‌کند که آیا متد IsReservationFullyPaidAsync در صورت عدم وجود رزرو،
        /// به درستی استثنا پرتاب می‌کند. این تست تضمین می‌کند که سرویس یکپارچه‌سازی،
        /// وجود موجودیت‌های مرتبط را قبل از انجام عملیات بررسی می‌کند.
        /// </summary>
        [Fact]
        public async Task IsReservationFullyPaidAsync_Throws_When_Reservation_Not_Found()
        {
            var nonExistentReservationId = Guid.NewGuid();
            
            _mockReservationRepository.Setup(r => r.GetByIdAsync(nonExistentReservationId))
                .ReturnsAsync((Reservation)null);
            
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _integrationService.IsReservationFullyPaidAsync(nonExistentReservationId));
            
            Assert.Contains($"Reservation with ID {nonExistentReservationId} not found", ex.Message);
        }
    }
}
