using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PKCE_article.Pages;

public class PkceConfidentialModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public PkceConfidentialModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? Error { get; set; }

    public async Task OnGetAsync()
    {
        string authCode = Request.Query["code"];
        if (authCode != null)
        {
            var client = _clientFactory.CreateClient();
            var requestUrl = "https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/oauth2/v2.0/token";
            var formData = new Dictionary<string, string>
            {
                { "client_id", "00000000-0000-0000-0000-000000000000" },
                { "scope", "openid offline_access" },
                { "code", authCode },
                { "redirect_uri", "http://localhost:5218/pkceconfidential" },
                { "grant_type", "authorization_code" },
                { "code_verifier", "~ThisIsThe1stArticleI_veWrittenForXmsMagazine.IHopeYouFindItInformative-" },
                { "client_secret", "bogus_secret" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Perform the POST request
            var response = await client.PostAsync(requestUrl, content);
            var resultDictionary = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

            if(resultDictionary.TryGetValue("access_token",out var accessToken))
                AccessToken = accessToken.GetString();
            if(resultDictionary.TryGetValue("id_token",out var idToken))
                IdToken = idToken.GetString();
            if(resultDictionary.TryGetValue("refresh_token",out var refreshToken))
                RefreshToken = refreshToken.GetString();
            if(resultDictionary.TryGetValue("error_description",out var error))
                Error = error.GetString();
        }
    }
}