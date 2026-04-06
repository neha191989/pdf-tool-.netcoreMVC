namespace PdfTool.Mvc.Services;

public interface IOcrService
{
    Task<string> ExtractTextFromScannedPdfAsync(IFormFile file, CancellationToken ct);
}
