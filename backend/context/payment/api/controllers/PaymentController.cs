using System;
using System.Linq;
using System.Threading.Tasks;
using backend.context.payment.api.dtos;
using backend.context.payment.application.commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.context.payment.api.controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly CreateVnpayPaymentCommandHandler _createHandler;
    private readonly ProcessVnpayCallbackCommandHandler _callbackHandler;

    public PaymentController(
        CreateVnpayPaymentCommandHandler createHandler,
        ProcessVnpayCallbackCommandHandler callbackHandler)
    {
        _createHandler = createHandler;
        _callbackHandler = callbackHandler;
    }

    /// <summary>
    /// POST /api/payment/vnpay — Yêu cầu thanh toán VNPAY.
    /// Amount KHÔNG nhận từ client; handler tra Booking để lấy giá thực tế.
    /// </summary>
    [HttpPost("vnpay")]
    [Authorize]
    public async Task<IActionResult> CreateVnpayPayment(CreatePaymentRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var orderInfo = $"Thanh toan don hang {request.BookingId}";

            // Amount bị loại khỏi command — handler tự tra DB
            var command = new CreateVnpayPaymentCommand(
                request.BookingId,
                orderInfo,
                request.ReturnUrl,
                ipAddress,
                request.BankCode
            );

            var result = await _createHandler.HandleAsync(command);
            return Ok(new { Message = "Yêu cầu thanh toán tạo thành công", PaymentUrl = result.PaymentUrl });
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex)         { return StatusCode(500, ex.Message); }
    }

    /// <summary>
    /// GET /api/payment/vnpay-return — Nhận callback từ VNPAY (ReturnUrl / IPN)
    /// </summary>
    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnpayReturn()
    {
        var queryDictionary = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
        if (queryDictionary.Count == 0) return BadRequest("Không có dữ liệu trả về từ VNPAY.");

        var command = new ProcessVnpayCallbackCommand(queryDictionary);
        var result = await _callbackHandler.HandleAsync(command);

        if (result.IsSuccess)
            return Ok(new { Status = "Success", Message = result.Message });

        return BadRequest(new { Status = "Failed", Message = result.Message });
    }
}
