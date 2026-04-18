using System;
using System.Threading.Tasks;
using backend.context.payment.domain.entity;

namespace backend.context.payment.domain.repo;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<Payment?> GetByIdAsync(Guid id);
    Task<VnpayPayment?> GetByTxnRefAsync(string txnRef);
    Task UpdateAsync(Payment payment);
}
