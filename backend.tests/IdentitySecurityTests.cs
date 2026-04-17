using System;
using System.Linq;
using backend.context.identity.domain.vo;
using Xunit;

namespace Nailshop.Backend.Tests
{
    public class IdentitySecurityTests
    {
        #region XSS & Injection Tests
        [Theory]
        [InlineData("<script>alert('xss')</script>")]
        [InlineData("<img src=x onerror=alert(1)>")]
        [InlineData("javascript:void(0)")]
        public void Data_WithScriptTags_ShouldBeHandled(string maliciousInput)
        {
            // Trong DDD, VO thường không sanitize (đó là việc của Application/UI)
            // Nhưng chúng ta test để xem hệ thống có crash khi nhận data lạ không.
            var emailResult = Record.Exception(() => new EmailVO(maliciousInput + "@gmail.com"));
            
            // Nếu Regex của chúng ta tốt, nó sẽ chặn script trong email
            Assert.NotNull(emailResult); 
        }

        [Theory]
        [InlineData("' OR 1=1 --")]
        [InlineData("'; DROP TABLE Users; --")]
        public void Data_WithSqlInjection_ShouldNotCrash(string sqlInjection)
        {
            // Kiểm tra xem VO có ném lỗi khi nhận ký tự đặc biệt SQL trong Email không
            Assert.Throws<ArgumentException>(() => new EmailVO(sqlInjection + "@test.com"));
        }
        #endregion

        #region Unicode & Emoji Tests
        [Fact]
        public void EmailVO_WithUnicodeAndEmojis_ShouldFailValidation()
        {
            string emojiEmail = "thanh_nail💅@gmail.com";
            Assert.Throws<ArgumentException>(() => new EmailVO(emojiEmail));
        }
        #endregion

        #region Stress Tests (Long Strings)
        [Fact]
        public void EmailVO_WithExtremelyLongString_ShouldBeHandled()
        {
            // Tạo một chuỗi 5000 ký tự
            string longEmail = new string('a', 5000) + "@gmail.com";
            
            // Một số Regex có thể bị StackOverflow với chuỗi quá dài (ReDoS attack)
            // Test này đảm bảo Regex của chúng ta an toàn
            var exception = Record.Exception(() => new EmailVO(longEmail));
            
            // Nếu không văng lỗi StackOverflow mà chỉ văng ArgumentException là thành công
            Assert.IsType<ArgumentException>(exception);
        }
        #endregion

        #region Logic Null-Byte Tests
        [Fact]
        public void EmailVO_WithNullByte_ShouldThrowException()
        {
            string nullByteEmail = "test\0@gmail.com";
            Assert.Throws<ArgumentException>(() => new EmailVO(nullByteEmail));
        }
        #endregion
    }
}
