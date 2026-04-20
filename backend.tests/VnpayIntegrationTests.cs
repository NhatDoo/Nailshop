using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using backend.context.payment.domain.entity;
using backend.context.payment.infrastructure.services;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Nailshop.Backend.Tests
{
    public class VnpayIntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public VnpayIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestVnpayUrlAndCheckErrorCode70()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(@"d:\Project\Nailshop\backend\appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var vnpayService = new VnpayService(configuration);

            // 2. Create mock VnpayPayment
            var payment = VnpayPayment.Create(
                bookingId: Guid.NewGuid(),
                amount: 100000,
                orderInfo: "Thanh toan don hang test",
                returnUrl: "http://localhost:5017/api/payment/vnpay-return",
                ipAddress: "127.0.0.1",
                locale: "vn",
                orderType: "other"
            );

            // 3. Generate URL
            var url = vnpayService.CreatePaymentUrl(payment);
            _output.WriteLine($"Generated VNPAY URL: {url}");

            // 4. Call URL using HttpClient to check response
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Response Status: {response.StatusCode}");
            
            // Check for error code 70
            bool hasError70 = content.Contains("errorcode=70") || url.Contains("vnp_ResponseCode=70") || content.Contains("Mã định danh không tồn tại");
            
            if (hasError70)
            {
                _output.WriteLine("DETECTED ERROR CODE 70: Merchant doesn't exist or Invalid TmnCode");
            }
            else
            {
                _output.WriteLine("NO ERROR 70 DETECTED. Form loaded successfully or other error.");
                if (content.Contains("Mã kiểm tra (checksum) không hợp lệ"))
                {
                    _output.WriteLine("BUT DETECTED ERROR 97: Invalid Checksum!");
                }
            }

            Assert.NotNull(url);
        }
    }
}
