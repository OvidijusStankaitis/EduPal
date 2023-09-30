using PSI_Project.DAL;

namespace PSI_Project.HelperFunctions;

public class SubjectHandler : BaseHandler<Subject>
{
    public override EntityDbOperations<Subject> EntityDbOperations { get; set; } = new SubjectDbOperations();
    
    protected override void AfterOperation()
    {
        Items.Sort();
    }
}