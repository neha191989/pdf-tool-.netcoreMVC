using Microsoft.AspNetCore.Mvc;
using PdfTool.Mvc.Models;
using PdfTool.Mvc.Services;

namespace PdfTool.Mvc.Controllers;

[Route("api/pdf")]
public class PdfController : Controller
{
    private readonly IPdfDocumentService _pdfService;
    private readonly IOcrService _ocrService;

    public PdfController(IPdfDocumentService pdfService, IOcrService ocrService)
    {
        _pdfService = pdfService;
        _ocrService = ocrService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("No file provided");
        }

        var result = await _pdfService.SaveUploadedPdfAsync(file, ct);
        return Json(result);
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] CreatePdfRequest request)
    {
        var output = _pdfService.CreateDocument(request);
        return File(output.Bytes, "application/pdf", output.FileName);
    }

    [HttpPost("merge")]
    public async Task<IActionResult> Merge(List<IFormFile> files, CancellationToken ct)
    {
        if (files.Count < 2)
        {
            return BadRequest("At least two files are required for merge.");
        }

        var output = await _pdfService.MergeDocumentsAsync(files, ct);
        return File(output.Bytes, "application/pdf", output.FileName);
    }

    [HttpPost("split")]
    public async Task<IActionResult> Split(IFormFile file, int startPage, int endPage, CancellationToken ct)
    {
        var output = await _pdfService.SplitDocumentAsync(file, startPage, endPage, ct);
        return File(output.Bytes, "application/pdf", output.FileName);
    }

    [HttpPost("extract-text")]
    public async Task<IActionResult> ExtractText(IFormFile file, CancellationToken ct)
    {
        var text = await _pdfService.ExtractTextAsync(file, ct);
        return Json(new { text });
    }

    [HttpPost("ocr")]
    public async Task<IActionResult> Ocr(IFormFile file, CancellationToken ct)
    {
        var text = await _ocrService.ExtractTextFromScannedPdfAsync(file, ct);
        return Json(new { text, engine = "stub" });
    }

    [HttpPost("compress")]
    public async Task<IActionResult> Compress(IFormFile file, int quality = 75, CancellationToken ct = default)
    {
        var output = await _pdfService.CompressDocumentAsync(file, quality, ct);
        return File(output.Bytes, "application/pdf", output.FileName);
    }

    [HttpPost("protect")]
    public async Task<IActionResult> Protect(IFormFile file, [FromForm] string password, CancellationToken ct)
    {
        var output = await _pdfService.EncryptDocumentAsync(file, password, ct);
        return File(output.Bytes, "application/pdf", output.FileName);
    }
}
