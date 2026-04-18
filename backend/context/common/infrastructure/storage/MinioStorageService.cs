using System;
using System.IO;
using System.Threading.Tasks;
using backend.context.common.application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace backend.context.common.infrastructure.storage;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _config;
    private readonly ILogger<MinioStorageService> _logger;

    public MinioStorageService(IMinioClient minioClient, IConfiguration config, ILogger<MinioStorageService> logger)
    {
        _minioClient = minioClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, string bucketName)
    {
        // 1. Đảm bảo bucket tồn tại, nếu chưa thì tạo
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs);

        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs);
            _logger.LogInformation("Đã tạo bucket mới: {BucketName}", bucketName);
        }

        // 2. Upload file
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs);

        // 3. Trả về URL công khai
        var endpoint = _config["MinIO:Endpoint"];
        var url = $"http://{endpoint}/{bucketName}/{fileName}";

        _logger.LogInformation("Upload thành công: {Url}", url);
        return url;
    }

    public async Task DeleteAsync(string fileName, string bucketName)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs);
        _logger.LogInformation("Đã xóa file: {BucketName}/{FileName}", bucketName, fileName);
    }
}
