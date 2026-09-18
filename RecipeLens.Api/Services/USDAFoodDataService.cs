using System.Net.Http.Headers;
using System.Text.Json;

public class USDAFoodDataService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public USDAFoodDataService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> SearchFoodsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("A food search query is required.", nameof(query));
        }

        var apiKey = _configuration["USDA:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("USDA:ApiKey is not configured.");
        }

        var url =
            "https://api.nal.usda.gov/fdc/v1/foods/search" +
            $"?api_key={Uri.EscapeDataString(apiKey)}" +
            $"&query={Uri.EscapeDataString(query.Trim())}" +
            "&pageSize=20";

        using var response = await _httpClient.GetAsync(url);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"USDA FoodData request failed " +
                $"({(int)response.StatusCode}): {responseJson}"
            );
        }

        return responseJson;
    }
}
