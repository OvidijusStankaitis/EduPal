using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.DTO;

public record ConspectusFileContentDTO(string Name, FileContentResult FileContent);