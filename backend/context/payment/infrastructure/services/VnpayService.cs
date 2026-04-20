using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using backend.context.payment.application.services;
using backend.context.payment.domain.entity;
using Microsoft.Extensions.Configuration;

namespace backend.context.payment.infrastructure.services;

public class VnpayService : IVnpayService
{
    private readonly IConfiguration _config;

    public VnpayService(IConfiguration config)
    {
        _config = config;
    }

    public string CreatePaymentUrl(VnpayPayment payment)
    {
        var vnp_Url = _config["Vnpay:BaseUrl"];
        var vnp_TmnCode = _config["Vnpay:TmnCode"];
        var vnp_HashSecret = _config["Vnpay:HashSecret"];

        var requestData = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode! },
            { "vnp_Amount", (payment.Amount * 100).ToString() },
            { "vnp_CreateDate", payment.CreatedAt.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", payment.Currency },
            { "vnp_IpAddr", payment.IpAddress },
            { "vnp_Locale", payment.Locale },
            { "vnp_OrderInfo", payment.OrderInfo },
            { "vnp_OrderType", payment.OrderType },
            { "vnp_ReturnUrl", payment.ReturnUrl },
            { "vnp_TxnRef", payment.TxnRef }
        };

        if (!string.IsNullOrEmpty(payment.BankCode))
        {
            requestData.Add("vnp_BankCode", payment.BankCode);
        }

        var queryString = new StringBuilder();
        var hashData = new StringBuilder();

        foreach (var kv in requestData)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                var encodedKey = HttpUtility.UrlEncode(kv.Key);
                var encodedValue = HttpUtility.UrlEncode(kv.Value);

                queryString.Append($"{encodedKey}={encodedValue}&");
                hashData.Append($"{encodedKey}={encodedValue}&");
            }
        }

        // remove last '&'
        if (queryString.Length > 0) queryString.Length--;
        if (hashData.Length > 0) hashData.Length--;

        var secureHash = HmacSha512(vnp_HashSecret!, hashData.ToString());

        queryString.Append($"&vnp_SecureHash={secureHash}");

        return $"{vnp_Url}?{queryString}";
    }

    public bool ValidateSignature(IDictionary<string, string> responseData, string inputHash)
    {
        var vnp_HashSecret = _config["Vnpay:HashSecret"];

        var sortedParams = new SortedDictionary<string, string>(StringComparer.Ordinal);

        foreach (var kv in responseData)
        {
            if (!string.IsNullOrEmpty(kv.Key)
                && kv.Key.StartsWith("vnp_")
                && kv.Key != "vnp_SecureHash"
                && kv.Key != "vnp_SecureHashType")
            {
                sortedParams.Add(kv.Key, kv.Value);
            }
        }

        var hashData = new StringBuilder();

        foreach (var kv in sortedParams)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                var encodedKey = HttpUtility.UrlEncode(kv.Key);
                var encodedValue = HttpUtility.UrlEncode(kv.Value);

                hashData.Append($"{encodedKey}={encodedValue}&");
            }
        }

        if (hashData.Length > 0) hashData.Length--;

        var expectedHash = HmacSha512(vnp_HashSecret!, hashData.ToString());

        return expectedHash.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
    }

    private string HmacSha512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}