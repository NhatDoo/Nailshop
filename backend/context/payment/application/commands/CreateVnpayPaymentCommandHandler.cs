using System.Threading.Tasks;
using backend.context.payment.application.services;
using backend.context.payment.domain.entity;
using backend.context.payment.domain.repo;

namespace backend.context.payment.application.commands;

public class CreateVnpayPaymentCommandHandler
{
    private readonly IPaymentRepository _repository;
    private readonly IVnpayService _vnpayService;

    public CreateVnpayPaymentCommandHandler(IPaymentRepository repository, IVnpayService vnpayService)
    {
        _repository = repository;
        _vnpayService = vnpayService;
    }

    public async Task<CreateVnpayPaymentResult> HandleAsync(CreateVnpayPaymentCommand command)
    {
        var payment = VnpayPayment.Create(
            command.BookingId,
            command.Amount,
            command.OrderInfo,
            command.ReturnUrl,
            command.IpAddress,
            bankCode: command.BankCode
        );

        var paymentUrl = _vnpayService.CreatePaymentUrl(payment);
        
        // Extract SecureHash to save it
        var uri = new System.Uri(paymentUrl);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var secureHash = query["vnp_SecureHash"];

        payment.SetPaymentUrl(paymentUrl, secureHash!);

        await _repository.AddAsync(payment);

        return new CreateVnpayPaymentResult(payment.Id, paymentUrl);
    }
}
