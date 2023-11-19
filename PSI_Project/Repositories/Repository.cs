using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class Repository<TEntity> where TEntity : BaseEntity
{
    protected readonly DbContext Context;
    
    public Repository(EduPalDatabaseContext context)
    {
        Context = context;
    }
    
    public virtual TEntity Get(string id)
    {
        TEntity? item = Context.Set<TEntity>().Find(id);
        if (item == null)
        {
            // Create at least 1 exception type and throw it; meaningfully deal with it; 
            throw new ObjectNotFoundException("Couldn't get object with specified id");
        }
        return item;
    }

    public virtual IEnumerable<TEntity> GetAll()
    {
        return Context.Set<TEntity>().ToList();
    }

    public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) // 2, 3: generic delegate
    {
        return Context.Set<TEntity>().Where(predicate);
    }

    public virtual int Add(TEntity entity)
    {
        //return Context.Set<TEntity>().Add(entity).Entity;
        Context.Set<TEntity>().Add(entity);
        return Context.SaveChanges();
    }

    public virtual int Remove(TEntity entity)
    {
        //return Context.Set<TEntity>().Remove(entity).Entity;
        Context.Set<TEntity>().Remove(entity);
        return Context.SaveChanges();
    }

    public virtual bool Exists(string id)
    {
        try
        {
            Get(id);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
    }
}