using System;
using System.Linq;
using System.Threading.Tasks;
using backend.context.identity.domain.vo;
using backend.context.naildesign.application.commands;
using backend.context.naildesign.application.queries;
using backend.context.naildesign.domain.entity;
using backend.context.naildesign.domain.vo;
using backend.context.naildesign.infrastructure.persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Nailshop.Backend.Tests
{
    // ============================================================
    // PHẦN 1: DOMAIN UNIT TESTS (Test Entity thuần túy, không DB)
    // ============================================================
    public class NailDesignDomainTests
    {
        private static UserIdVO AdminId() => new UserIdVO(Guid.NewGuid());

        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var design = NailDesign.Create("Móng hoa", "http://img.com/1.jpg", AdminId(), "Preset", "Mẫu đẹp");

            Assert.NotEqual(Guid.Empty, design.Id);
            Assert.Equal("Móng hoa", design.Name);
            Assert.Equal(NailDesignType.Preset, design.Type);
            Assert.Equal(NailDesignStatus.Active, design.Status); // Mặc định phải là Active
        }

        [Fact]
        public void Create_WithEmptyName_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                NailDesign.Create("", "http://img.com/1.jpg", AdminId(), "Custom", null));

            Assert.Contains("trống", ex.Message);
        }

        [Fact]
        public void Create_WithNameOver100Chars_ShouldThrow()
        {
            var longName = new string('A', 101);

            var ex = Assert.Throws<ArgumentException>(() =>
                NailDesign.Create(longName, "http://img.com/1.jpg", AdminId(), "Custom", null));

            Assert.Contains("100 ký tự", ex.Message);
        }

        [Fact]
        public void Create_WithEmptyImageUrl_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                NailDesign.Create("Mẫu A", "", AdminId(), "Preset", null));

            Assert.Contains("URL", ex.Message);
        }

        [Fact]
        public void Create_WithInvalidType_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                NailDesign.Create("Mẫu A", "http://img.com/1.jpg", AdminId(), "InvalidType", null));

            Assert.Contains("không hợp lệ", ex.Message);
        }

        [Fact]
        public void Create_WithNumericType_ShouldThrow() // Chặn "0", "1" như RoleVO
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                NailDesign.Create("Mẫu A", "http://img.com/1.jpg", AdminId(), "0", null));

            Assert.Contains("không hợp lệ", ex.Message);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var design = NailDesign.Create("Cũ", "http://old.jpg", AdminId(), "Preset", null);

            design.Update("Mới", "http://new.jpg", "Mô tả mới");

            Assert.Equal("Mới", design.Name);
            Assert.Equal("http://new.jpg", design.ImageUrl);
            Assert.NotNull(design.UpdatedAt);
        }

        [Fact]
        public void Update_DeletedDesign_ShouldThrow()
        {
            var design = NailDesign.Create("Mẫu", "http://img.jpg", AdminId(), "Preset", null);
            design.Delete();

            var ex = Assert.Throws<InvalidOperationException>(() =>
                design.Update("New Name", "http://new.jpg", null));

            Assert.Contains("đã bị xóa", ex.Message);
        }

        [Fact]
        public void Delete_ActiveDesign_ShouldSoftDelete()
        {
            var design = NailDesign.Create("Mẫu", "http://img.jpg", AdminId(), "Custom", null);

            design.Delete();

            Assert.Equal(NailDesignStatus.Deleted, design.Status);
            Assert.NotNull(design.UpdatedAt);
        }

        [Fact]
        public void Delete_AlreadyDeletedDesign_ShouldThrow()
        {
            var design = NailDesign.Create("Mẫu", "http://img.jpg", AdminId(), "Preset", null);
            design.Delete();

            var ex = Assert.Throws<InvalidOperationException>(() => design.Delete());
            Assert.Contains("đã bị xóa", ex.Message);
        }
    }

    // =====================================================================
    // PHẦN 2: APPLICATION INTEGRATION TESTS (Test qua Command/Query + InMemory DB)
    // =====================================================================
    public class NailDesignApplicationTests
    {
        private readonly global::AppDbContext _context;
        private readonly NailDesignRepository _repository;
        private readonly string _adminId = Guid.NewGuid().ToString();

        public NailDesignApplicationTests()
        {
            var options = new DbContextOptionsBuilder<global::AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new global::AppDbContext(options);
            _repository = new NailDesignRepository(_context);
        }

        private CreateNailDesignCommand ValidCommand(string name = "Móng đính đá") =>
            new CreateNailDesignCommand(name, "http://img.com/1.jpg", _adminId, "Preset", "Mô tả");

        [Fact]
        public async Task Create_WithValidData_ShouldPersistToDatabase()
        {
            var handler = new CreateNailDesignCommandHandler(_repository);

            var id = await handler.HandleAsync(ValidCommand());

            var saved = await _context.NailDesigns.FindAsync(id);
            Assert.NotNull(saved);
            Assert.Equal("Móng đính đá", saved.Name);
            Assert.Equal(NailDesignStatus.Active, saved.Status);
        }

        [Fact]
        public async Task Create_WithInvalidType_ShouldFail()
        {
            var handler = new CreateNailDesignCommandHandler(_repository);
            var command = new CreateNailDesignCommand("Mẫu", "http://img.jpg", _adminId, "WrongType", null);

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
        }

        [Fact]
        public async Task GetAll_ShouldOnlyReturnActiveDesigns()
        {
            var createHandler = new CreateNailDesignCommandHandler(_repository);
            var queryHandler = new GetAllNailDesignsQueryHandler(_repository);
            var deleteHandler = new DeleteNailDesignCommandHandler(_repository);

            var id1 = await createHandler.HandleAsync(ValidCommand("Mẫu Active"));
            var id2 = await createHandler.HandleAsync(ValidCommand("Mẫu Bị Xóa"));

            // Xóa mềm mẫu thứ 2
            await deleteHandler.HandleAsync(new DeleteNailDesignCommand(id2));

            var results = await queryHandler.HandleAsync(new GetAllNailDesignsQuery());

            // Chỉ trả về 1 cái Active, không trả về cái đã xóa
            Assert.Single(results);
            Assert.Equal("Mẫu Active", results.First().Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectDesign()
        {
            var createHandler = new CreateNailDesignCommandHandler(_repository);
            var queryHandler = new GetNailDesignByIdQueryHandler(_repository);

            var id = await createHandler.HandleAsync(ValidCommand("Tìm theo ID"));

            var result = await queryHandler.HandleAsync(new GetNailDesignByIdQuery(id));

            Assert.NotNull(result);
            Assert.Equal("Tìm theo ID", result.Name);
            Assert.Equal("Preset", result.Type);
        }

        [Fact]
        public async Task GetById_WithNonExistentId_ShouldReturnNull()
        {
            var queryHandler = new GetNailDesignByIdQueryHandler(_repository);

            var result = await queryHandler.HandleAsync(new GetNailDesignByIdQuery(Guid.NewGuid()));

            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            var createHandler = new CreateNailDesignCommandHandler(_repository);
            var updateHandler = new UpdateNailDesignCommandHandler(_repository);

            var id = await createHandler.HandleAsync(ValidCommand("Tên Cũ"));
            await updateHandler.HandleAsync(new UpdateNailDesignCommand(id, "Tên Mới", "http://new.jpg", "Mô tả mới"));

            var updated = await _context.NailDesigns.FindAsync(id);
            Assert.Equal("Tên Mới", updated!.Name);
            Assert.NotNull(updated.UpdatedAt);
        }

        [Fact]
        public async Task Update_NonExistentDesign_ShouldThrow()
        {
            var updateHandler = new UpdateNailDesignCommandHandler(_repository);

            await Assert.ThrowsAsync<Exception>(() =>
                updateHandler.HandleAsync(new UpdateNailDesignCommand(Guid.NewGuid(), "X", "http://x.jpg", null)));
        }

        [Fact]
        public async Task Delete_ShouldSoftDeleteInDatabase()
        {
            var createHandler = new CreateNailDesignCommandHandler(_repository);
            var deleteHandler = new DeleteNailDesignCommandHandler(_repository);

            var id = await createHandler.HandleAsync(ValidCommand());
            await deleteHandler.HandleAsync(new DeleteNailDesignCommand(id));

            // Vẫn còn trong DB nhưng Status là Deleted
            var deleted = await _context.NailDesigns.FindAsync(id);
            Assert.NotNull(deleted);
            Assert.Equal(NailDesignStatus.Deleted, deleted.Status);
        }

        [Fact]
        public async Task Delete_AlreadyDeletedDesign_ShouldThrow()
        {
            var createHandler = new CreateNailDesignCommandHandler(_repository);
            var deleteHandler = new DeleteNailDesignCommandHandler(_repository);

            var id = await createHandler.HandleAsync(ValidCommand());
            await deleteHandler.HandleAsync(new DeleteNailDesignCommand(id));

            // Xóa lần 2 phải bắn lỗi
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                deleteHandler.HandleAsync(new DeleteNailDesignCommand(id)));
        }
    }
}
