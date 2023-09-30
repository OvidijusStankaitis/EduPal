using PSI_Project;

public abstract class BaseHandler<T> where T : BaseEntity
{
    public List<T> ItemList { get; private set; } = new List<T>();

    // This abstract property ensures that derived classes provide the correct file path.
    protected abstract string DbFilePath { get; }
    protected abstract string TempDbFilePath { get; }
    
    // Derived classes must implement how an item is converted to a string for the database/ how a string is converted to an item.
    protected abstract string ItemToDbString(T item);
    protected abstract T StringToItem(string dbString);
    protected void WriteItemToDB(T item)
    {
        using (var streamWriter = new StreamWriter(DbFilePath, true))
        {
            streamWriter.WriteLine(ItemToDbString(item));
        }
    }
    public void ReadAllItemsFromDB()
    {
        using (var streamReader = new StreamReader(DbFilePath))
        {
            streamReader.ReadLine();
            string? itemInfo = streamReader.ReadLine();
            while (itemInfo != null)
            {
                ItemList.Add(StringToItem(itemInfo));
                itemInfo = streamReader.ReadLine();
            }
        }
    }
    public virtual T CreateItem(T item)
    {
        ItemList.Add(item);
        WriteItemToDB(item);
        AfterOperation();
        return item;
    }

    public virtual bool RemoveItem(T item)
    {
        try
        {
            FileInfo tempFile = new FileInfo(DbFilePath);
            tempFile.MoveTo(TempDbFilePath);
            
            StreamReader sr = File.OpenText(TempDbFilePath);
            StreamWriter sw = File.CreateText(DbFilePath);
            
            string? line;
            while((line = sr.ReadLine()) != null)
            {
                string dbId = line.Split(" ")[0]; // TO DO: TO CHOOSE ID FOR SUBJECT AND HANDLER
                if (dbId == item.Name)
                    continue;
                sw.WriteLine(line);
            }
            
            sw.Close();
            sr.Close();
            tempFile.Delete();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        bool removed = ItemList.Remove(item);
        if (removed) AfterOperation();
        return removed;
    }

    public virtual T ModifyItem(T oldItem, T newItem)
    {
        int index = ItemList.IndexOf(oldItem);
        if (index != -1)
        {
            ItemList[index] = newItem;
            AfterOperation();
        }
        return newItem;
    }

    // This method is executed after any operation. By default, it does nothing.
    // Derived classes can override it to add specific behavior, like sorting. 
    // Only overriden in SubjectHandler class, not TopicHandler class.
    protected virtual void AfterOperation()
    {
    }
}