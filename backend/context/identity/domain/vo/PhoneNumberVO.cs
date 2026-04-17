using System;
using System.Text.RegularExpressions;

namespace backend.context.identity.domain.vo;

public record PhoneNumberVO
{
    // Regex cho số điện thoại (Ví dụ: bắt đầu bằng 0 hoặc +, độ dài từ 9-12 ký số)
    private static readonly Regex PhoneRegex = new(@"^(\+84|0)(3|5|7|8|9|1[2689])([0-9]{8})$", RegexOptions.Compiled);

    public string Value { get; }

    public PhoneNumberVO(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Số điện thoại không được để trống.");
        }

        // Loại bỏ khoảng trắng hoặc ký tự đặc biệt nếu có trước khi check (Tùy chọn)
        var sanitizedValue = value.Replace(" ", "").Replace("-", "");

        if (!PhoneRegex.IsMatch(sanitizedValue))
        {
            throw new ArgumentException("Số điện thoại không hợp lệ.");
        }

        Value = sanitizedValue;
    }

    public static implicit operator string(PhoneNumberVO phone) => phone.Value;
    public override string ToString() => Value;
}
