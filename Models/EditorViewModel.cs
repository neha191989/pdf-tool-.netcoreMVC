namespace PdfTool.Mvc.Models;

public class EditorViewModel
{
    public string Theme { get; set; } = "light";
    public bool EnableAiSummary { get; set; }
    public string[] SupportedLanguages { get; set; } = Array.Empty<string>();
}
