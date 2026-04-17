using System;
using System.Text.RegularExpressions;

namespace backend.context.identity.domain.vo;

public record EmailVO
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public EmailVO(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email không được để trống.");
        }

        if (value.Length > 255 || value.Contains("..") || !EmailRegex.IsMatch(value))
        {
            throw new ArgumentException("Định dạng Email không hợp lệ hoặc quá dài.");
        }

        // Kiểm tra ký tự đặc biệt ngoài @ và các ký tự email hợp lệ (tùy chỉnh nếu cần khắc khe hơn)
        // Regex trên đã khá an toàn, nhưng nếu muốn cấm cả các ký tự lạ trong phần name:
        if (Regex.IsMatch(value, @"[#$%\^&*]")) 
        {
             // Ví dụ chặn một số ký tự đặc biệt người dùng có thể coi là 'lạ'
             // throw new ArgumentException("Email contains forbidden special characters.");
        }

        Value = value.ToLowerInvariant();
    }

    public static implicit operator string(EmailVO email) => email.Value;
    public override string ToString() => Value;
}
