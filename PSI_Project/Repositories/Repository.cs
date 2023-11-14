using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class Repository<TEntity> where TEntity : BaseEntity // 2: generic constraint
{
    protected readonly DbContext Context;
    
    public Repository(EduPalDatabaseContext context)
    {
        Context = context;
    }
    
    public TEntity Get(string id)
    {
        TEntity? item = Context.Set<TEntity>().Find(id);
        if (item == null)
        {
            throw new ObjectNotFoundException("Couldn't get object with specified id");
        }
        return item;
    }

    public IEnumerable<TEntity> GetAll()
    {
        return Context.Set<TEntity>().ToList();
    }

    public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) // 2, 3: generic delegate
    {
        return Context.Set<TEntity>().Where(predicate);
    }

    public TEntity Add(TEntity entity)
    {
        return Context.Set<TEntity>().Add(entity).Entity;
    }

    public TEntity Remove(TEntity entity)
    {
        return Context.Set<TEntity>().Remove(entity).Entity;
    }

    public bool Exists(string id)
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