namespace PdfTool.Mvc.Services;

public class StubOcrService : IOcrService
{
    public Task<string> ExtractTextFromScannedPdfAsync(IFormFile file, CancellationToken ct)
    {
        const string message = "OCR placeholder: integrate Tesseract, Azure Computer Vision, or AWS Textract here.";
        return Task.FromResult(message);
    }
}
