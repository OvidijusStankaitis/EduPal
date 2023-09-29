using PSI_Project;

public abstract class BaseHandler<T> where T : BaseEntity
{
    public List<T> Items { get; private set; } = new List<T>();

    // This abstract property ensures that derived classes provide the correct file path.
    protected abstract string DbFilePath { get; }

    protected void WriteItemToDB(T item)
    {
        using (var streamWriter = new StreamWriter(DbFilePath, true))
        {
            streamWriter.WriteLine(ItemToDbString(item));
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

    public virtual T CreateItem(T item)
    {
        Items.Add(item);
        WriteItemToDB(item);
        AfterOperation();
        return item;
    }

    public virtual bool RemoveItem(T item)
    {
        bool removed = Items.Remove(item);
        if (removed) AfterOperation();
        return removed;
    }

    public virtual T ModifyItem(T oldItem, T newItem)
    {
        int index = Items.IndexOf(oldItem);
        if (index != -1)
        {
            Items[index] = newItem;
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