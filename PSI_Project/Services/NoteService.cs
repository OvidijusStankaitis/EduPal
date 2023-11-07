using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PSI_Project.Models;
using PSI_Project.Exceptions;
using System.Text;

namespace PSI_Project.Services;

public class NoteService
{
    public async Task<MemoryStream> CreatePdfAsync(Note note)
    {
        Console.WriteLine($"Received note: {note.Content}");
        if (string.IsNullOrEmpty(note.Content))
        {
            throw new NoteCreationException("Note content cannot be null or empty.");
        }

        var pdf = new PdfDocument();
        var page = pdf.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 15, XFontStyle.Regular);

        const double leftMargin = 50;
        const double topMargin = 50;
        const double bottomMargin = 50;

        double currentYPosition = topMargin;

        var paragraphs = note.Content.Replace("\r\n", "\n").Split('\n');

        foreach (var paragraph in paragraphs)
        {
            var words = paragraph.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var line = new StringBuilder();

            foreach (var word in words)
            {
                var testLine = line + (line.Length == 0 ? "" : " ") + word;
                var testLineWidth = gfx.MeasureString(testLine, font).Width;

                if (testLineWidth > page.Width - leftMargin * 2)
                {
                    if (line.Length > 0)
                    {
                        gfx.DrawString(line.ToString(), font, XBrushes.Black, leftMargin, currentYPosition);
                        line.Clear();
                    }

                    currentYPosition += font.Height;

                    if (currentYPosition > page.Height - bottomMargin - font.Height)
                    {
                        page = pdf.AddPage();
                        gfx.Dispose();
                        gfx = XGraphics.FromPdfPage(page);
                        currentYPosition = topMargin;
                    }

                    line.Append(word);
                }
                else
                {
                    if (line.Length > 0)
                    {
                        line.Append(" ");
                    }
                    line.Append(word);
                }
            }

            if (line.Length > 0)
            {
                gfx.DrawString(line.ToString(), font, XBrushes.Black, leftMargin, currentYPosition);
                currentYPosition += font.Height; 
            }

            currentYPosition += font.Height;
        }

        if (currentYPosition > topMargin)
        {
            currentYPosition -= font.Height;
        }

        var stream = new MemoryStream();
        pdf.Save(stream, false);
        stream.Position = 0;

        gfx.Dispose();

        return await Task.FromResult(stream);
    }
}

