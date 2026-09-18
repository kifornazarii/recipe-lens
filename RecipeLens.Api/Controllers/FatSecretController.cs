using Microsoft.AspNetCore.Mvc;

namespace RecipeLens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FatSecretController : ControllerBase
{
    private readonly FatSecretService _fatSecret;

    public FatSecretController(FatSecretService fatSecret)
    {
        _fatSecret = fatSecret;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchFoods(string query)
    {
        var result = await _fatSecret.SearchFoodsAsync(query);

        return Content(result, "application/json");
    }
}