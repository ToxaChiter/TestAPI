using Microsoft.EntityFrameworkCore;
using System;
using TestAPI.Database;

namespace TestAPI.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly EventDbContext _context;

    public Repository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<bool> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        var added = await _context.SaveChangesAsync();
        bool wasAdded = added > 0;
        return wasAdded;
    }

    public async Task<T?> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        var updated = await _context.SaveChangesAsync();
        bool wasUpdated = updated > 0;
        return wasUpdated ? entity : default;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        bool wasDeleted = false;

        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            var deleted = await _context.SaveChangesAsync();
            wasDeleted = deleted > 0;
        }

        return wasDeleted;
    }
}

