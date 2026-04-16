using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.context.identity.domain.entity;
using backend.context.identity.domain.vo;

namespace backend.context.identity.infrastructure.persistence.configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(v => v.Value, v => new UserIdVO(v))
            .IsRequired();

        builder.Property(u => u.Ten)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.SDT)
            .HasConversion(v => v.Value, v => new PhoneNumberVO(v))
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .HasConversion(v => v.Value, v => new EmailVO(v))
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Role)
            .HasConversion(v => v.Value.ToString(), v => new RoleVO(v))
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(u => u.Email).IsUnique();

        // Cấu hình Auth như một thực thể phụ thuộc trong cùng Aggregate
        builder.HasOne(u => u.Auth)
            .WithOne()
            .HasForeignKey<Auth>(a => a.IDNguoidung)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuthConfiguration : IEntityTypeConfiguration<Auth>
{
    public void Configure(EntityTypeBuilder<Auth> builder)
    {
        builder.ToTable("Auths");

        builder.HasKey(a => a.IDNguoidung);

        builder.Property(a => a.IDNguoidung)
            .HasConversion(v => v.Value, v => new UserIdVO(v))
            .IsRequired();

        builder.Property(a => a.PasswordHash)
            .IsRequired();

        builder.Property(a => a.RefreshTokent)
            .HasMaxLength(500);

        builder.Property(a => a.Resettokent)
            .HasMaxLength(500);
    }
}
