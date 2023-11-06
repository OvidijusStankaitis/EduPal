using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PSI_Project.Models;
using PSI_Project.Exceptions;
using System.Text;

namespace PSI_Project.Services;

public class NoteService
{
    public async Task<MemoryStream> CreatePdfAsync(Note note)
    {
        if (string.IsNullOrEmpty(note.Content))
        {
            throw new EntityCreationException("Note content cannot be null or empty.");
        }

        var pdf = new PdfDocument();
        var page = pdf.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
            
        var font = new XFont("Arial", 20, XFontStyle.Bold);

        var format = new XStringFormat();
        format.Alignment = XStringAlignment.Near;
        format.LineAlignment = XLineAlignment.Near;

        var wrappedText = InsertLineBreaks(note.Content, 80); // Insert line breaks after 80 characters
        var formatter = new XTextFormatter(gfx);
        formatter.DrawString(wrappedText, font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), format);

        var stream = new MemoryStream();
        pdf.Save(stream, false);
        stream.Position = 0;

        return await Task.FromResult(stream);
    }

    private string InsertLineBreaks(string text, int maxChars)
    {
        if (text.Contains(" ")) return text; // If text contains spaces, return as is

        var sb = new StringBuilder();
        for (int i = 0; i < text.Length; i += maxChars)
        {
            if (i + maxChars < text.Length)
                sb.AppendLine(text.Substring(i, maxChars));
            else
                sb.Append(text.Substring(i));
        }
        return sb.ToString();
    }
}