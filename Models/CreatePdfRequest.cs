namespace PdfTool.Mvc.Models;

public class CreatePdfRequest
{
    public string Title { get; set; } = "Untitled";
    public List<PageElement> Elements { get; set; } = new();
}

public class PageElement
{
    public string Type { get; set; } = "text";
    public double X { get; set; }
    public double Y { get; set; }
    public string Value { get; set; } = string.Empty;
    public int FontSize { get; set; } = 12;
}

public class PdfBinaryResult
{
    public string FileName { get; init; } = "document.pdf";
    public byte[] Bytes { get; init; } = Array.Empty<byte>();
}

public class UploadedPdfResponse
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public long Size { get; init; }
}
