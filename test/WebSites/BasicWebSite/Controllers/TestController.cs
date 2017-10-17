using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{
    public IActionResult Index() => View();

    public IActionResult IndexInvalidAssetKey() => View();
}