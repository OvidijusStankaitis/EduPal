namespace PSI_Project.Repositories.For_tests;

public class FileOperations : IFileOperations
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public void Delete(string path)
    {
        File.Delete(path);
    }
}