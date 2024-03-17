using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace PKCE_article.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _clientFactory;
    public string CodeChallenge { get; set; }
    public string State { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public static string CreateCodeVerifier()
    {
        const int size = 32; // Size recommended by RFC 7636 for code verifier
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[size];
        rng.GetBytes(bytes);
        // Using URL-safe base64 encoding without padding
        return Convert.ToBase64String(bytes)
            .TrimEnd('=') // Remove any base64 padding
            .Replace('+', '-') // 62nd char of encoding
            .Replace('/', '_'); // 63rd char of encoding
    }

    public static string CreateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        // Using URL-safe base64 encoding without padding
        return Convert.ToBase64String(challengeBytes)
            .TrimEnd('=') // Remove any base64 padding
            .Replace('+', '-') // 62nd char of encoding
            .Replace('/', '_'); // 63rd char of encoding
    }

    public Task OnGetAsync()
    {
        State = CreateCodeVerifier();
        CodeChallenge = CreateCodeChallenge(State);
        return Task.CompletedTask;
    }
}