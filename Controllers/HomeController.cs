using Microsoft.AspNetCore.Mvc;
using PdfTool.Mvc.Models;

namespace PdfTool.Mvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var vm = new EditorViewModel
        {
            Theme = "light",
            SupportedLanguages = new[] { "en", "es", "fr", "de" },
            EnableAiSummary = true
        };

        return View(vm);
    }
}
