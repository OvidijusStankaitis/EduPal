using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public abstract class BaseRepository<TEntity> where TEntity : BaseEntity
{
    protected abstract string DbFilePath { get; }
    protected List<TEntity> Items { get; private set; }
    protected abstract string ItemToDbString(TEntity item);  
    protected abstract TEntity StringToItem(string dbString);
    
    protected BaseRepository()  
    {
        Items = ReadAllItemsFromDB();
        AfterOperation();
    }

    public TEntity? GetItemById(string itemId)  
    {
        return Items.FirstOrDefault(item => (item as BaseEntity).Id.Equals(itemId));
    }

    protected List<TEntity> ReadAllItemsFromDB() 
    {
        List<TEntity> items = new List<TEntity>();

        using (var sr = new StreamReader(DbFilePath))
        {
            string? itemInfo = sr.ReadLine();
            while (itemInfo != null)
            {
                items.Add(StringToItem(itemInfo));
                itemInfo = sr.ReadLine();
            }
        }

        return items;
    }
    
    public bool InsertItem(TEntity item) 
    {
        Items.Add(item);
        bool isInserted = InsertItemToDB(item);
        
        AfterOperation();
        
        return isInserted;
    }
    
    protected bool InsertItemToDB(TEntity item) 
    {
        try
        {
            using (StreamWriter sw = File.AppendText(DbFilePath))
            {
                sw.WriteLine(ItemToDbString(item));
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    
    protected bool UpdateDB()  
    {
        bool updated = false;
        try
        {
            using (StreamWriter sw = new StreamWriter(DbFilePath))
            {
                foreach (var item in Items)
                {
                    sw.WriteLine(ItemToDbString(item));
                }
            }
            updated = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return updated;
    }

    public virtual bool RemoveItemById(string itemId)  
    {
        bool removed = RemoveItemFromDB(itemId);
        if (removed)
        {
            Items.RemoveAll(i => (i as BaseEntity).Id == itemId);
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
                    TEntity item = StringToItem(line);
                    if ((item as BaseEntity).Id.Equals(itemId))
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
