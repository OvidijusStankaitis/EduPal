using PSI_Project.Models;

namespace PSI_Project.Responses;

public record CreationResponseDTO<T>(string Message, T? Entity) where T : BaseEntity;