namespace PSI_Project.Models
{
    public static class StringExtensions
    {
        public static bool IsValidPdfName(this string pdfName)
        {
            // Check if pdfName ends with ".pdf" and add any other validation criteria if needed.
            return pdfName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
        }
    }
}