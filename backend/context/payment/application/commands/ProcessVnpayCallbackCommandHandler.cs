using System;
using System.Threading.Tasks;
using backend.context.payment.application.services;
using backend.context.payment.domain.repo;

namespace backend.context.payment.application.commands;

public class ProcessVnpayCallbackCommandHandler
{
    private readonly IPaymentRepository _repository;
    private readonly IVnpayService _vnpayService;

    public ProcessVnpayCallbackCommandHandler(IPaymentRepository repository, IVnpayService vnpayService)
    {
        _repository = repository;
        _vnpayService = vnpayService;
    }

    public async Task<ProcessVnpayCallbackResult> HandleAsync(ProcessVnpayCallbackCommand command)
    {
        var data = command.VnpayData;

        if (!data.TryGetValue("vnp_SecureHash", out var secureHash))
        {
            return new ProcessVnpayCallbackResult(false, "Không tìm thấy SecureHash trong dữ liệu trả về.");
        }

        var isValidSignature = _vnpayService.ValidateSignature(data, secureHash);
        if (!isValidSignature)
        {
            return new ProcessVnpayCallbackResult(false, "Chữ ký số không hợp lệ.");
        }

        if (!data.TryGetValue("vnp_TxnRef", out var txnRef))
            return new ProcessVnpayCallbackResult(false, "Không tìm thấy mã giao dịch (vnp_TxnRef).");

        var payment = await _repository.GetByTxnRefAsync(txnRef);
        if (payment == null)
            return new ProcessVnpayCallbackResult(false, "Không tìm thấy giao dịch trong hệ thống.");

        if (payment.Status != domain.vo.PaymentStatus.Pending)
            return new ProcessVnpayCallbackResult(false, "Giao dịch này đã được xử lý trước đó.");

        data.TryGetValue("vnp_ResponseCode", out var responseCode);
        data.TryGetValue("vnp_TransactionNo", out var transactionNo);

        if (responseCode == "00")
        {
            // Thành công
            payment.ConfirmFromCallback(transactionNo ?? "");
            await _repository.UpdateAsync(payment);
            return new ProcessVnpayCallbackResult(true, "Thanh toán thành công.");
        }
        else
        {
            // Thất bại hoặc Bị Hủy
            payment.MarkFailed();
            await _repository.UpdateAsync(payment);
            return new ProcessVnpayCallbackResult(false, $"Thanh toán thất bại. Mã lỗi VNPAY: {responseCode}");
        }
    }
}
