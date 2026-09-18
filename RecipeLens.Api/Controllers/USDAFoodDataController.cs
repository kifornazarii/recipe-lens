using Microsoft.AspNetCore.Mvc;

namespace RecipeLens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class USDAFoodDataController : ControllerBase
{
    private readonly USDAFoodDataService _USDAFoodDataService;

    public USDAFoodDataController(USDAFoodDataService USDAService)
    {
        _USDAFoodDataService = USDAService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchFoods(string query)
    {
        var result = await _USDAFoodDataService.SearchFoodsAsync(query);

        return Content(result, "application/json");
    }
}