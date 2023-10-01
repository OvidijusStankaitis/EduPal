using PSI_Project.DAL;

namespace PSI_Project.HelperFunctions;

public abstract class BaseHandler<T, S> 
    where T : IStorable
    where S : EntityDbOperations<T>
{
    public abstract S DbOperations { get; set; }

    public List<T> Items { get; set; } = new List<T>();
    
    public virtual T? GetItemById(string itemId)
    {
        return DbOperations.GetById(itemId);
    }

    public virtual T InsertItem(T item)
    {
        Items.Add(item);
        DbOperations.InsertItem(item);
        AfterOperation();
        return item;
    }

    public virtual bool RemoveItem(string itemId)
    {
        DbOperations.RemoveItem(itemId);
        bool removed = false;
        foreach (T item in Items)
        {
            if (item.Id == itemId)
            {
                removed = Items.Remove(item);
                break;
            }
        } 
        
        if (removed) 
            AfterOperation();
        
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
    protected virtual void AfterOperation()
    {
    }
}