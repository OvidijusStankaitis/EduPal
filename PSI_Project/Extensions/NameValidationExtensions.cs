using System.Text.RegularExpressions;

namespace PSI_Project.Models;

public static class NameValidationExtensions
{
    public static bool IsValidPersonName(this string name)
    {
        string nameValidationPattern = @"^[a-zA-Z]+$";
        return Regex.IsMatch(name, nameValidationPattern);
    }

    public static bool IsValidContainerName(this string? name)
    {
        string nameValidationPattern = @"^[\.\sa-zA-Z0-9]+$";
        return name is not null && Regex.IsMatch(name, nameValidationPattern);
    }

    public static bool IsValidFileName(this string name)
    {
        return name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
    }
}