using System;
using System.Collections.Generic;
using System.IO;
using backend.context.payment.domain.entity;
using backend.context.payment.infrastructure.services;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Nailshop.Backend.Tests
{
    /// <summary>
    /// UNIT test cho VnpayService — không gọi mạng thật.
    /// 
    /// FIX (Medium):
    /// 1. Đường dẫn tuyệt đối (d:\...) → dùng BaseDirectory + đi ngược lên project root.
    /// 2. Gọi HTTP thật ra sandbox VNPAY → đã bỏ hoàn toàn; chỉ kiểm tra URL được tạo ra
    ///    có đúng format và chứa các tham số bắt buộc.
    /// 3. Nếu cần integration test thực sự, hãy dùng [Trait("Category","Integration")]
    ///    và bỏ khỏi CI pipeline.
    /// </summary>
    public class VnpayServiceUnitTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IConfiguration _configuration;

        public VnpayServiceUnitTests(ITestOutputHelper output)
        {
            _output = output;

            // Tìm appsettings.json từ thư mục build, đi lên project root
            var binDir = AppContext.BaseDirectory;  // e.g. .../backend.tests/bin/Debug/net8.0/
            var projectRoot = Path.GetFullPath(Path.Combine(binDir, "..", "..", "..", "..", "backend"));
            var settingsPath = Path.Combine(projectRoot, "appsettings.json");

            _configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsPath, optional: false, reloadOnChange: false)
                .AddEnvironmentVariables() // CI có thể override qua env vars
                .Build();
        }

        [Fact]
        public void CreatePaymentUrl_ShouldReturnValidUrl_WithRequiredParams()
        {
            // Arrange
            var vnpayService = new VnpayService(_configuration);

            var payment = VnpayPayment.Create(
                bookingId: Guid.NewGuid(),
                amount: 100000,
                orderInfo: "Thanh toan don hang test",
                returnUrl: "http://localhost:5017/api/payment/vnpay-return",
                ipAddress: "127.0.0.1",
                locale: "vn",
                orderType: "other"
            );

            // Act
            var url = vnpayService.CreatePaymentUrl(payment);
            _output.WriteLine($"Generated VNPAY URL: {url}");

            // Assert — kiểm tra URL hợp lệ, không gọi mạng
            Assert.NotNull(url);
            Assert.StartsWith("https://", url);
            Assert.Contains("vnp_Amount",      url);
            Assert.Contains("vnp_TxnRef",      url);
            Assert.Contains("vnp_SecureHash",  url);
            Assert.Contains("vnp_ReturnUrl",   url);
        }

        [Fact]
        public void CreatePaymentUrl_WithZeroAmount_ShouldThrow()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                VnpayPayment.Create(
                    bookingId: Guid.NewGuid(),
                    amount: 0,
                    orderInfo: "Test",
                    returnUrl: "http://localhost/return",
                    ipAddress: "127.0.0.1"
                )
            );
        }
    }
}
