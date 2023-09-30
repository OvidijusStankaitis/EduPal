namespace PSI_Project.DAL;

public abstract class EntityDbOperations<T> where T : BaseEntity
{
    // This abstract property ensures that derived classes provide the correct file path.
    protected abstract string DbFilePath { get; }
    public List<T> Items { get; private set; } = new List<T>();
    
    public void WriteItemToDB(T item)
    {
        using (StreamWriter sw = File.AppendText(DbFilePath))
        {
            sw.WriteLine(ItemToDbString(item));
        }
    }
    
    // Derived classes must implement how an item is converted to a string for the database.
    protected abstract string ItemToDbString(T item);

    public void ReadAllItemsFromDB()
    {
        using (var streamReader = new StreamReader(DbFilePath))
        {
            streamReader.ReadLine();
            string? itemInfo = streamReader.ReadLine();
            while (itemInfo != null)
            {
                Items.Add(StringToItem(itemInfo));
                itemInfo = streamReader.ReadLine();
            }
        }
    }

    // Derived classes must implement how a string from the database is converted to an item.
    protected abstract T StringToItem(string dbString);
}