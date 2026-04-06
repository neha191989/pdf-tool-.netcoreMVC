using PdfTool.Mvc.Models;

namespace PdfTool.Mvc.Services;

public interface IPdfDocumentService
{
    Task<UploadedPdfResponse> SaveUploadedPdfAsync(IFormFile file, CancellationToken ct);
    PdfBinaryResult CreateDocument(CreatePdfRequest request);
    Task<PdfBinaryResult> MergeDocumentsAsync(List<IFormFile> files, CancellationToken ct);
    Task<PdfBinaryResult> SplitDocumentAsync(IFormFile file, int startPage, int endPage, CancellationToken ct);
    Task<string> ExtractTextAsync(IFormFile file, CancellationToken ct);
    Task<PdfBinaryResult> CompressDocumentAsync(IFormFile file, int quality, CancellationToken ct);
    Task<PdfBinaryResult> EncryptDocumentAsync(IFormFile file, string password, CancellationToken ct);
}
