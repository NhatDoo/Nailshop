using backend.context.payment.domain.entity;
using backend.context.payment.domain.vo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.context.payment.infrastructure.persistence.configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Sử dụng TPH (Table-per-hierarchy)
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Currency).HasMaxLength(10).IsRequired();
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
            
        builder.Property(p => p.Method)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Cấu hình Discriminator
        builder.HasDiscriminator(p => p.Method)
            .HasValue<VnpayPayment>(PaymentMethod.VNPay);
    }
}

public class VnpayPaymentConfiguration : IEntityTypeConfiguration<VnpayPayment>
{
    public void Configure(EntityTypeBuilder<VnpayPayment> builder)
    {
        builder.Property(p => p.TxnRef).HasMaxLength(100);
        builder.Property(p => p.OrderInfo).HasMaxLength(255);
        builder.Property(p => p.OrderType).HasMaxLength(100);
        builder.Property(p => p.ReturnUrl).HasMaxLength(500);
        builder.Property(p => p.IpAddress).HasMaxLength(50);
        builder.Property(p => p.Locale).HasMaxLength(10);
        builder.Property(p => p.BankCode).HasMaxLength(20);
        builder.Property(p => p.PaymentUrl).HasColumnType("text");
        builder.Property(p => p.SecureHash).HasMaxLength(256);
        builder.Property(p => p.TransactionNo).HasMaxLength(100);

        // Đảm bảo TxnRef là duy nhất
        builder.HasIndex(p => p.TxnRef).IsUnique();
    }
}
