using System;
using System.Threading.Tasks;
using backend.context.booking.domain.repo;
using backend.context.payment.application.services;
using backend.context.payment.domain.entity;
using backend.context.payment.domain.repo;

namespace backend.context.payment.application.commands;

public class CreateVnpayPaymentCommandHandler
{
    private readonly IPaymentRepository _repository;
    private readonly IVnpayService _vnpayService;
    private readonly IBookingRepository _bookingRepository;

    public CreateVnpayPaymentCommandHandler(
        IPaymentRepository repository,
        IVnpayService vnpayService,
        IBookingRepository bookingRepository)
    {
        _repository = repository;
        _vnpayService = vnpayService;
        _bookingRepository = bookingRepository;
    }

    public async Task<CreateVnpayPaymentResult> HandleAsync(CreateVnpayPaymentCommand command)
    {
        // 1. Tra Booking để lấy giá thực — không tin client
        var booking = await _bookingRepository.GetByIdAsync(command.BookingId)
            ?? throw new ArgumentException($"Booking không tồn tại: {command.BookingId}");

        // 2. Chuyển sang đơn vị tiền VND cho VNPAY (Price là VND, VNPAY nhân 100 trong service)
        var amount = (long)Math.Round(booking.Price);
        if (amount <= 0)
            throw new ArgumentException("Booking không có giá trị hợp lệ để thanh toán.");

        // 3. Tạo VnpayPayment từ giá lấy trong DB
        var payment = VnpayPayment.Create(
            command.BookingId,
            amount,
            command.OrderInfo,
            command.ReturnUrl,
            command.IpAddress,
            bankCode: command.BankCode
        );

        var paymentUrl = _vnpayService.CreatePaymentUrl(payment);

        // 4. Extract SecureHash để lưu
        var uri = new Uri(paymentUrl);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var secureHash = query["vnp_SecureHash"];

        payment.SetPaymentUrl(paymentUrl, secureHash!);

        await _repository.AddAsync(payment);

        return new CreateVnpayPaymentResult(payment.Id, paymentUrl);
    }
}
