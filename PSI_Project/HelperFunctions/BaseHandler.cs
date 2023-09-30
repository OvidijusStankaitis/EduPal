using PSI_Project;
using PSI_Project.DAL;

public abstract class BaseHandler<T> where T : BaseEntity
{
    public abstract EntityDbOperations<T> EntityDbOperations { get; set; }

    public List<T> Items { get; private set; } = new List<T>();

    public virtual T CreateItem(T item)
    {
        Items.Add(item);
        EntityDbOperations.WriteItemToDB(item);
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