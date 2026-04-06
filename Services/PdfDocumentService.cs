using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfTool.Mvc.Models;
using UglyToad.PdfPig;

namespace PdfTool.Mvc.Services;

public class PdfDocumentService : IPdfDocumentService
{
    private readonly IWebHostEnvironment _environment;

    public PdfDocumentService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<UploadedPdfResponse> SaveUploadedPdfAsync(IFormFile file, CancellationToken ct)
    {
        var uploads = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);

        var safeFile = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
        var destination = Path.Combine(uploads, safeFile);

        await using var stream = new FileStream(destination, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return new UploadedPdfResponse
        {
            FileName = file.FileName,
            Url = $"/uploads/{safeFile}",
            Size = file.Length
        };
    }

    public PdfBinaryResult CreateDocument(CreatePdfRequest request)
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
        gfx.DrawString(request.Title, titleFont, XBrushes.Black, new XRect(20, 20, page.Width, 40));

        foreach (var element in request.Elements)
        {
            var font = new XFont("Arial", element.FontSize);
            gfx.DrawString(element.Value, font, XBrushes.Black, new XRect(element.X, element.Y, page.Width, page.Height));
        }

        using var ms = new MemoryStream();
        document.Save(ms, false);

        return new PdfBinaryResult
        {
            FileName = $"{request.Title.Replace(' ', '-')}.pdf",
            Bytes = ms.ToArray()
        };
    }

    public async Task<PdfBinaryResult> MergeDocumentsAsync(List<IFormFile> files, CancellationToken ct)
    {
        await Task.CompletedTask;
        using var output = new PdfDocument();

        foreach (var file in files)
        {
            await using var inStream = file.OpenReadStream();
            var import = PdfSharpCore.Pdf.IO.PdfReader.Open(inStream, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import);
            for (var i = 0; i < import.PageCount; i++)
            {
                output.AddPage(import.Pages[i]);
            }
        }

        using var ms = new MemoryStream();
        output.Save(ms, false);

        return new PdfBinaryResult { FileName = "merged.pdf", Bytes = ms.ToArray() };
    }

    public async Task<PdfBinaryResult> SplitDocumentAsync(IFormFile file, int startPage, int endPage, CancellationToken ct)
    {
        await Task.CompletedTask;
        await using var inStream = file.OpenReadStream();
        var input = PdfSharpCore.Pdf.IO.PdfReader.Open(inStream, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import);

        var output = new PdfDocument();
        startPage = Math.Max(1, startPage);
        endPage = Math.Min(input.PageCount, endPage);

        for (var i = startPage - 1; i < endPage; i++)
        {
            output.AddPage(input.Pages[i]);
        }

        using var ms = new MemoryStream();
        output.Save(ms, false);

        return new PdfBinaryResult { FileName = "split.pdf", Bytes = ms.ToArray() };
    }

    public async Task<string> ExtractTextAsync(IFormFile file, CancellationToken ct)
    {
        await Task.CompletedTask;
        await using var stream = file.OpenReadStream();
        using var pdf = PdfDocument.Open(stream);

        var text = string.Join(Environment.NewLine, pdf.GetPages().Select(p => p.Text));
        return text;
    }

    public async Task<PdfBinaryResult> CompressDocumentAsync(IFormFile file, int quality, CancellationToken ct)
    {
        await Task.CompletedTask;
        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);

        return new PdfBinaryResult
        {
            FileName = "compressed.pdf",
            Bytes = ms.ToArray()
        };
    }

    public async Task<PdfBinaryResult> EncryptDocumentAsync(IFormFile file, string password, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var input = PdfSharpCore.Pdf.IO.PdfReader.Open(stream, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Modify);

        input.SecuritySettings.UserPassword = password;
        input.SecuritySettings.OwnerPassword = password;
        input.SecuritySettings.PermitAccessibilityExtractContent = false;
        input.SecuritySettings.PermitExtractContent = false;

        await Task.CompletedTask;
        using var ms = new MemoryStream();
        input.Save(ms, false);

        return new PdfBinaryResult
        {
            FileName = "protected.pdf",
            Bytes = ms.ToArray()
        };
    }
}
