using System.IO;
using System.Threading.Tasks;

namespace backend.context.common.application;

public interface IStorageService
{
    /// <summary>
    /// Upload file lên Object Storage và trả về URL công khai.
    /// </summary>
    /// <param name="stream">Luồng dữ liệu file</param>
    /// <param name="fileName">Tên file sẽ lưu (e.g. "nail_abc123.jpg")</param>
    /// <param name="contentType">MIME type (e.g. "image/jpeg")</param>
    /// <param name="bucketName">Tên bucket/container</param>
    /// <returns>URL công khai để truy cập file</returns>
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, string bucketName);

    /// <summary>
    /// Xóa file khỏi Object Storage.
    /// </summary>
    Task DeleteAsync(string fileName, string bucketName);
}
