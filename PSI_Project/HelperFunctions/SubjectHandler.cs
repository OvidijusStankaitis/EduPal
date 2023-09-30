using PSI_Project;

public class SubjectHandler : BaseHandler<Subject>
{
    public SubjectHandler()
    {
        ReadAllItemsFromDB();
    }

    protected override string DbFilePath => "..//DB//SubjectInformation.txt";

    protected override string ItemToDbString(Subject item)
    {
        return $"{item.Id};{item.Name};{item.Description}";
    }

    protected override Subject StringToItem(string dbString)
    {
        String[] subjectFields = dbString.Split(new char[] { ';' });
        return new Subject(subjectFields[0], subjectFields[1], subjectFields[2]);
    }

    protected override void AfterOperation()
    {
        Items.Sort();
    }
}