namespace PSI_Project.Repositories.For_tests;

public interface IFileOperations
{
    bool Exists(string path);
    void Delete(string path);
}