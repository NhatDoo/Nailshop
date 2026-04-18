using System;
using System.Threading.Tasks;
using backend.context.payment.domain.entity;
using backend.context.payment.domain.repo;
using Microsoft.EntityFrameworkCore;

namespace backend.context.payment.infrastructure.persistence;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments.FindAsync(id);
    }

    public async Task<VnpayPayment?> GetByTxnRefAsync(string txnRef)
    {
        return await _context.Payments
            .OfType<VnpayPayment>()
            .FirstOrDefaultAsync(p => p.TxnRef == txnRef);
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }
}
