using PSI_Project.DAL;

namespace PSI_Project.HelperFunctions;

public abstract class BaseHandler<T, S> 
    where T : BaseEntity, IStorable
    where S : EntityDbOperations<T>
{
    public abstract S DbOperations { get; set; }

    public List<T> ItemList { get; set; } = new List<T>();
    
    public virtual T? GetItemById(string itemId)
    {
        return DbOperations.GetById(itemId);
    }

    public virtual T InsertItem(T item)
    {
        ItemList.Add(item);
        DbOperations.InsertItem(item);
        AfterOperation();
        return item;
    }

    public virtual bool RemoveItem(string itemId)
    {
        DbOperations.RemoveItem(itemId);
        bool removed = false;
        foreach (T item in ItemList)
        {
            if (item.Id == itemId)
            {
                removed = ItemList.Remove(item);
                break;
            }
        } 
        
        if (removed) 
            AfterOperation();
        
        return removed;
    }
    
    public virtual T? CheckItemInList (string itemName)
    {
        foreach (var item in ItemList)
        {
            if (item.Name.Equals(itemName))
            {
                return item;
            }
        }
        return default;
    }

    // This method is executed after any operation. By default, it does nothing.
    protected virtual void AfterOperation()
    {
    }
}