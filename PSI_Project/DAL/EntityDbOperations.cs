namespace PSI_Project.DAL;

public abstract class EntityDbOperations<T> where T : IStorable
{
    protected abstract string DbFilePath { get; }
    protected abstract string ItemToDbString(T item);
    protected abstract T StringToItem(string dbString);

    public T? GetById(string itemId)
    {
        using (StreamReader sr = File.OpenText(DbFilePath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                T item = StringToItem(line);
                if (item.Id == itemId)
                    return item;
            }
        }

        return default(T?);
    }
    
    public List<T> ReadAllItemsFromDB()
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

    public void InsertItem(T item)
    {
        using (StreamWriter sw = File.AppendText(DbFilePath))
        {
            sw.WriteLine(ItemToDbString(item));
        }
    }

    public void RemoveItem(string itemId)
    {
        try
        {
            string tempFileName = Path.GetFileNameWithoutExtension(DbFilePath) + "_temp" + ".txt";
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
                        continue;

                    sw.WriteLine(line);
                }
            }
            
            oldDbFile.Delete();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}