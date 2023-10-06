namespace PSI_Project.Repositories;

public abstract class BaseRepository<T> where T : BaseEntity
{
    protected abstract string DbFilePath { get; }
    protected abstract string ItemToDbString(T item);
    protected abstract T StringToItem(string dbString);

    public List<T> Items { get; private set; } = new List<T>();

    public BaseRepository()
    {
        Items = ReadAllItemsFromDB();
    }
    
    // From BaseHandler
    public T? GetItemById(string itemId)
    {
        return Items.FirstOrDefault(item => item.Id.Equals(itemId));
    }
    
    public T? GetItemByName(string itemName)
    {
        return Items.FirstOrDefault(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
    }

    // From EntityDbOperations
    protected List<T> ReadAllItemsFromDB()
    {
        List<T> items = new List<T>();

        using (var streamReader = new StreamReader(DbFilePath))
        {
            string? itemInfo = streamReader.ReadLine();
            while (itemInfo != null)
            {
                items.Add(StringToItem(itemInfo));
                itemInfo = streamReader.ReadLine();
            }
        }

        return items;
    }
    
    public virtual T InsertItem(T item)
    {
        Items.Add(item);
        InsertItemToDB(item);
        AfterOperation();
        return item;
    }
    
    protected void InsertItemToDB(T item)
    {
        using (StreamWriter sw = File.AppendText(DbFilePath))
        {
            sw.WriteLine(ItemToDbString(item));
        }
    }

    public virtual bool RemoveItemById(string itemId)
    {
        bool removed = RemoveItemFromDB(itemId);
        if (removed)
        {
            Items.RemoveAll(i => i.Id == itemId);
            AfterOperation();
        }
        return removed;
    }

    protected bool RemoveItemFromDB(string itemId)
    {
        bool removed = false;

        try
        {
            string tempFileName = Path.GetFileNameWithoutExtension(DbFilePath) + "_temp.txt";
            string tempFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DB", tempFileName);
            FileInfo oldDbFile = new FileInfo(DbFilePath);
            oldDbFile.MoveTo(tempFilePath);

            using (StreamReader sr = File.OpenText(tempFilePath))
            using (StreamWriter sw = File.CreateText(DbFilePath))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    T item = StringToItem(line);
                    if (item.Id.Equals(itemId))
                    {
                        removed = true;
                        continue;
                    }
                    sw.WriteLine(line);
                }
            }

            oldDbFile.Delete();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return removed;
    }
    
    // This method is executed after any operation. By default, it does nothing.
    protected virtual void AfterOperation()
    {
    }
}
