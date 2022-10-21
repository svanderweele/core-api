using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace Core.Authentication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IAmazonS3 _s3Client;

    public AuthenticationController(ILogger<AuthenticationController> logger, IAmazonS3 s3Client)
    {
        _logger = logger;
        _s3Client = s3Client;
    }

    [HttpGet("list")]
    public async Task<List<S3Bucket>> Add(int x, int y)
    {
        var response = await _s3Client.ListBucketsAsync();
        _logger.LogInformation($"Response for S3 {response}");
        return response.Buckets;
    }
    
}