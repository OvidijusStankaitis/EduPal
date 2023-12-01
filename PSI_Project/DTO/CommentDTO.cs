namespace PSI_Project.DTO;

public record CommentDTO(string Id, string Content, DateTime TimeStamp, bool IsFromCurrentUser);