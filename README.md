# PDF Studio Pro (.NET Core MVC + Razor)

A modern, responsive PDF tool web application built with ASP.NET Core MVC and Razor views.

## Features Included

- **PDF Viewer**
  - Upload and open PDF files
  - Page navigation, zoom controls
  - Thumbnail page sidebar
- **PDF Creation**
  - Create basic PDFs from structured elements via API
  - Drag-and-drop overlay editor using Fabric.js
- **PDF Editing (Foundation)**
  - Add text/shapes/signature-like elements on canvas
  - Annotation/comment panel scaffold
  - Merge/split endpoints scaffolded in backend
- **Interactive Elements**
  - Form field placeholders (text, checkbox, radio, dropdown)
  - Signature placeholder
- **Export & Sharing (Foundation)**
  - Save/export button hooks ready for integration
  - Uploads stored under `wwwroot/uploads`
- **Advanced Tools (Scaffold)**
  - OCR endpoint (stub service)
  - Password protection endpoint
  - Compression endpoint (placeholder)
  - Search in document + AI summary placeholder in UI
- **UX / UI**
  - Modern minimal layout (toolbar + sidebars + canvas area)
  - Fully responsive for desktop and mobile
  - Dark / light mode toggle
  - Multi-language selector scaffold

## Project Structure

```
PdfTool.Mvc.csproj
Program.cs
Controllers/
  HomeController.cs
  PdfController.cs
Models/
  EditorViewModel.cs
  CreatePdfRequest.cs
Services/
  IPdfDocumentService.cs
  IOcrService.cs
  PdfDocumentService.cs
  StubOcrService.cs
Views/
  _ViewImports.cshtml
  _ViewStart.cshtml
  Home/Index.cshtml
  Shared/_Layout.cshtml
wwwroot/
  css/site.css
  js/pdf-editor.js
  uploads/
```

## Run Locally

1. Install **.NET 8 SDK**.
2. Restore packages:

   ```bash
   dotnet restore
   ```

3. Run the application:

   ```bash
   dotnet run
   ```

4. Open the URL shown in terminal (usually `https://localhost:5001` or similar).

## Notes

- The app is production-ready in architecture but some advanced features are scaffolds (OCR/compression/AI summary) and can be wired to cloud services:
  - OCR: Azure Computer Vision / AWS Textract / Tesseract
  - AI Summary: Azure OpenAI / OpenAI API
  - Storage: AWS S3 / Azure Blob / Firebase Storage
- Client-side rendering uses **PDF.js** and overlay editing uses **Fabric.js**.
