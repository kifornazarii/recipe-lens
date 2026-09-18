using System.Net.Http.Headers;
using System.Text.Json;

public class FatSecretService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public FatSecretService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var clientId = _configuration["FatSecret:ClientId"];
        var clientSecret = _configuration["FatSecret:ClientSecret"];

        var credentials =
            Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes(
                    $"{clientId}:{clientSecret}"
                )
            );

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://oauth.fatsecret.com/connect/token"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                credentials
            );

        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["scope"] = "basic"
            }
        );

        var response = await _httpClient.SendAsync(request);

        var responseJson =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"FatSecret authentication failed " +
                $"({(int)response.StatusCode}): {responseJson}"
            );
        }

        using var document =
            JsonDocument.Parse(responseJson);

        return document
            .RootElement
            .GetProperty("access_token")
            .GetString()
            ?? throw new Exception(
                "FatSecret did not return an access token."
            );
    }

    public async Task<string> SearchFoodsAsync(string query)
    {
        var accessToken = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://platform.fatsecret.com/rest/server.api"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );

        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["method"] = "foods.search",
                ["search_expression"] = query,
                ["format"] = "json",
                ["max_results"] = "20"
            }
        );

        var response = await _httpClient.SendAsync(request);

        var responseJson =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"FatSecret request failed " +
                $"({(int)response.StatusCode}): {responseJson}"
            );
        }

        return responseJson;
    }
}