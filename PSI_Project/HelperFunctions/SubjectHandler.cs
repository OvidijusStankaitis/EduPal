using PSI_Project.DAL;

namespace PSI_Project.HelperFunctions;

public class SubjectHandler : BaseHandler<Subject, SubjectDbOperations>
{
    public override SubjectDbOperations DbOperations { get; set; } = new SubjectDbOperations();

    public SubjectHandler()
    {
        Items = DbOperations.ReadAllItemsFromDB();
    }

    public List<Subject> GetSubjectList()
    {
        return DbOperations.GetSubjectList();
    }
    
    protected override void AfterOperation()
    {
        Items.Sort();
    }
}