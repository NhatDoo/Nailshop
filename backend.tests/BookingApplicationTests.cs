using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.booking.application.commands;
using backend.context.booking.application.queries;
using backend.context.booking.infrastructure.persistence;
using backend.context.booking.domain.entity;
using backend.context.identity.domain.vo;
using backend.context.nailservice.domain.repo;
using backend.context.nailservice.domain.enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Nailshop.Backend.Tests
{
    // ─── Stub NailServiceRepository dùng trong unit test ───────────────────────
    internal class StubNailServiceRepository : INailServiceRepository
    {
        private readonly List<backend.context.nailservice.domain.entity.NailService> _data = new();

        public StubNailServiceRepository(IEnumerable<backend.context.nailservice.domain.entity.NailService> seed)
            => _data.AddRange(seed);

        public Task<backend.context.nailservice.domain.entity.NailService?> GetByIdAsync(Guid id)
            => Task.FromResult(_data.FirstOrDefault(s => s.Id == id));

        public Task<IEnumerable<backend.context.nailservice.domain.entity.NailService>> GetAllAsync()
            => Task.FromResult(_data.AsEnumerable());

        public Task AddAsync(backend.context.nailservice.domain.entity.NailService service)
        {
            _data.Add(service);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(backend.context.nailservice.domain.entity.NailService service) => Task.CompletedTask;
        public Task DeleteAsync(backend.context.nailservice.domain.entity.NailService service) => Task.CompletedTask;
    }

    // ─── Tests ──────────────────────────────────────────────────────────────────
    public class BookingApplicationTests
    {
        private readonly global::AppDbContext _context;
        private readonly BookingRepository _repository;

        // Service cố định dùng trong test
        private readonly backend.context.nailservice.domain.entity.NailService _fakeService;
        private readonly StubNailServiceRepository _nailServiceRepo;

        public BookingApplicationTests()
        {
            var options = new DbContextOptionsBuilder<global::AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context    = new global::AppDbContext(options);
            _repository = new BookingRepository(_context);

            _fakeService = backend.context.nailservice.domain.entity.NailService.Create(
                name: "Làm móng sơn Gel",
                description: null,
                price: 200_000m,
                duration: 60,
                category: ServiceCategory.MANICURE,
                image: null);

            _nailServiceRepo = new StubNailServiceRepository(new[] { _fakeService });
        }

        private CreateBookingCommandHandler BuildHandler()
            => new CreateBookingCommandHandler(_repository, _nailServiceRepo);

        // ────────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateBooking_WithValidData_ShouldSaveToDatabase()
        {
            var handler    = BuildHandler();
            var customerId = Guid.NewGuid();

            var command = new CreateBookingCommand(
                customerId,
                _fakeService.Id,
                DateTime.UtcNow.AddDays(2),
                "Test Note"
            );

            var bookingId = await handler.HandleAsync(command);

            var booking = await _context.Bookings.FindAsync(bookingId);
            Assert.NotNull(booking);
            Assert.Equal("Làm móng sơn Gel", booking.ServiceName);
            Assert.Equal(200_000m, booking.Price);          // giá đến từ DB, không từ client
            Assert.Equal(customerId, booking.CustomerId.Value);
        }

        [Fact]
        public async Task CreateBooking_InPast_ShouldThrowException()
        {
            var handler = BuildHandler();
            var command  = new CreateBookingCommand(
                Guid.NewGuid(),
                _fakeService.Id,
                DateTime.UtcNow.AddDays(-1),
                null
            );

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
            Assert.Contains("tương lai", ex.Message);
        }

        [Fact]
        public async Task GetMyBookings_ShouldOnlyReturnOwnBookings()
        {
            var handler      = BuildHandler();
            var queryHandler = new GetMyBookingsQueryHandler(_repository);

            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            await handler.HandleAsync(new CreateBookingCommand(user1, _fakeService.Id, DateTime.UtcNow.AddDays(1), null));
            await handler.HandleAsync(new CreateBookingCommand(user2, _fakeService.Id, DateTime.UtcNow.AddDays(3), null));

            var result = await queryHandler.HandleAsync(new GetMyBookingsQuery(user1));

            Assert.Single(result);
            Assert.Equal("Làm móng sơn Gel", result.First().ServiceName);
        }

        [Fact]
        public void BookingStatus_InvalidTransitions_ShouldFail()
        {
            var customerId = new UserIdVO(Guid.NewGuid());
            var booking    = Booking.Create(customerId, "Service", 100, DateTime.UtcNow.AddDays(1), null);

            booking.Cancel();

            var ex = Assert.Throws<Exception>(() => booking.Confirm());
            Assert.Contains("chờ duyệt", ex.Message);
        }

        [Fact]
        public async Task CreateBooking_WithNonExistentService_ShouldThrow()
        {
            var handler = BuildHandler();
            var command  = new CreateBookingCommand(
                Guid.NewGuid(),
                Guid.NewGuid(), // ServiceId không tồn tại
                DateTime.UtcNow.AddDays(5),
                null
            );

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
            Assert.Contains("Dịch vụ không tồn tại", ex.Message);
        }

        [Fact]
        public async Task CreateBooking_WithDuplicateSlot_ShouldThrowConflict()
        {
            var handler = BuildHandler();
            var slotTime = DateTime.UtcNow.AddDays(1).Date.AddHours(10); // 10:00 UTC ngày mai

            // Đặt lịch lần 1
            await handler.HandleAsync(new CreateBookingCommand(
                Guid.NewGuid(), _fakeService.Id, slotTime, null));

            // Đặt lịch lần 2 cùng slot → phải throw
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(new CreateBookingCommand(
                    Guid.NewGuid(), _fakeService.Id, slotTime, null)));

            Assert.Contains("đã được đặt", ex.Message);
        }
    }
}
