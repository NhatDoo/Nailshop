using System;
using System.Linq;
using System.Threading.Tasks;
using backend.context.booking.application.commands;
using backend.context.booking.application.queries;
using backend.context.booking.infrastructure.persistence;
using backend.context.booking.domain.entity; // Thêm cái này
using backend.context.identity.domain.vo;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Nailshop.Backend.Tests
{
    public class BookingApplicationTests
    {
        private readonly global::AppDbContext _context;
        private readonly BookingRepository _repository;

        public BookingApplicationTests()
        {
            var options = new DbContextOptionsBuilder<global::AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new global::AppDbContext(options);
            _repository = new BookingRepository(_context);
        }

        [Fact]
        public async Task CreateBooking_WithValidData_ShouldSaveToDatabase()
        {
            var handler = new CreateBookingCommandHandler(_repository);
            var customerId = Guid.NewGuid();
            var command = new CreateBookingCommand(
                customerId,
                "Làm móng sơn Gel",
                200000,
                DateTime.UtcNow.AddDays(2),
                "Test Note"
            );

            var bookingId = await handler.HandleAsync(command);

            var booking = await _context.Bookings.FindAsync(bookingId);
            Assert.NotNull(booking);
            Assert.Equal("Làm móng sơn Gel", booking.ServiceName);
            Assert.Equal(customerId, booking.CustomerId.Value);
        }

        [Fact]
        public async Task CreateBooking_InPast_ShouldThrowException()
        {
            var handler = new CreateBookingCommandHandler(_repository);
            var command = new CreateBookingCommand(
                Guid.NewGuid(),
                "Service",
                100,
                DateTime.UtcNow.AddDays(-1),
                null
            );

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
            Assert.Contains("tương lai", exception.Message);
        }

        [Fact]
        public async Task GetMyBookings_ShouldOnlyReturnOwnBookings()
        {
            var handler = new CreateBookingCommandHandler(_repository);
            var queryHandler = new GetMyBookingsQueryHandler(_repository);
            
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            await handler.HandleAsync(new CreateBookingCommand(user1, "S1", 100, DateTime.UtcNow.AddDays(1), null));
            await handler.HandleAsync(new CreateBookingCommand(user2, "S3", 300, DateTime.UtcNow.AddDays(3), null));

            var result = await queryHandler.HandleAsync(new GetMyBookingsQuery(user1));

            Assert.Single(result);
            Assert.Equal("S1", result.First().ServiceName);
        }

        [Fact]
        public void BookingStatus_InvalidTransitions_ShouldFail() // Đổi thành đồng bộ vì entity logic là đồng bộ
        {
            // Arrange
            var customerId = new UserIdVO(Guid.NewGuid());
            var booking = Booking.Create(customerId, "Service", 100, DateTime.UtcNow.AddDays(1), null);

            // Act
            booking.Cancel(); 

            // Assert
            var exception = Assert.Throws<Exception>(() => booking.Confirm());
            Assert.Contains("chờ duyệt", exception.Message);
        }

        [Fact]
        public async Task CreateBooking_WithOverlyLongServiceName_ShouldFail()
        {
            var handler = new CreateBookingCommandHandler(_repository);
            var longServiceName = new string('A', 1000); 

            var command = new CreateBookingCommand(
                Guid.NewGuid(),
                longServiceName,
                100,
                DateTime.UtcNow.AddDays(5),
                "Note"
            );

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
            Assert.Contains("200 ký tự", exception.Message);
        }
    }
}
