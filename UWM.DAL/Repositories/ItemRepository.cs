﻿using Microsoft.EntityFrameworkCore;
using System.Data;
using UWM.DAL.Data;
using UWM.DAL.Interfaces.Items;
using UWM.Domain.Entity;

namespace UWM.DAL.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDBContext _db;

        public ItemRepository(AppDBContext db) 
        {
            _db = db;
        }

        public async Task<int> Create(Item item)
        {
            await _db.Item.AddAsync(item);
            
            try
            {
                await _db.SaveChangesAsync();
                return item.Id;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task Delete(int id)
        {
            var item = await _db.Item.FindAsync(id);
            if (item == null)
                throw new Exception();
            
            _db.Item.Remove(item);
            
            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<Item> Get(int id)
        {
            return await _db.Item
                .Include(p => p.Provider)
                .Include(w => w.Warehouse)
                .Include(s => s.SubCategory)
                .Where(i => i.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetAll()
        {
            return await _db.Item
                .Include(p => p.Provider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetBySubCategory(int subCategoryId)
        {
            return await _db.Item
                .Include(p => p.Provider)
                .Include(s => s.SubCategory)
                .Where(f => f.SubCategory.Id == subCategoryId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task Update(Item item)
        {
            var result = _db.Entry<Item>(item);
            result.State = EntityState.Modified;
            
            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
