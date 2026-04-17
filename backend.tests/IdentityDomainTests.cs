using System;
using backend.context.identity.domain.vo;
using Xunit;

namespace Nailshop.Backend.Tests
{
    public class IdentityDomainTests
    {
        #region EmailVO Tests
        [Theory]
        [InlineData("email_khong_at.com")]
        [InlineData("email@domain")]
        [InlineData("email@domain..com")]
        [InlineData("test#email@gmail.com")]
        public void EmailVO_WithInvalidFormat_ShouldThrowException(string invalidEmail)
        {
            var exception = Assert.Throws<ArgumentException>(() => new EmailVO(invalidEmail));
            Assert.Contains("Định dạng Email không hợp lệ", exception.Message);
        }

        [Fact]
        public void EmailVO_Empty_ShouldThrowException()
        {
            var exception = Assert.Throws<ArgumentException>(() => new EmailVO(""));
            Assert.Contains("không được để trống", exception.Message);
        }
        #endregion

        #region PasswordVO Tests
        [Theory]
        [InlineData("1234567")]
        [InlineData("admin")]
        public void PasswordVO_TooShort_ShouldThrowException(string shortPassword)
        {
            var exception = Assert.Throws<ArgumentException>(() => new PasswordVO(shortPassword));
            Assert.Contains("từ 8 đến 100 ký tự", exception.Message);
        }
        #endregion

        #region RoleVO Tests
        [Theory]
        [InlineData("Manager")]
        [InlineData("Staff")]
        [InlineData("123")]
        public void RoleVO_InvalidRole_ShouldThrowException(string invalidRole)
        {
            var exception = Assert.Throws<ArgumentException>(() => new RoleVO(invalidRole));
            Assert.Contains("Customer hoặc Admin", exception.Message);
        }
        #endregion

        #region PhoneNumberVO Tests
        [Theory]
        [InlineData("0912abc789")]
        [InlineData("1234567")]
        [InlineData("012345678901234")]
        public void PhoneNumberVO_InvalidFormat_ShouldThrowException(string invalidPhone)
        {
            var exception = Assert.Throws<ArgumentException>(() => new PhoneNumberVO(invalidPhone));
            Assert.Contains("Số điện thoại không hợp lệ", exception.Message);
        }
        #endregion
    }
}
