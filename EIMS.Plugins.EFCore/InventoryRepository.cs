using EIMS.CoreBusiness;
using EIMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIMS.Plugins.EFCore
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly EIMSContext _db;

        public InventoryRepository(EIMSContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByName(string name)
        {
            //return await _db.Inventories.Where(x => x.InventoryName.Contains(name, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(name)).ToListAsync();
            return await _db.Inventories.Where(x => x.InventoryName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            // to prevent different inventories from having the same name
            if (_db.Inventories.Any(x =>  x.InventoryName.ToLower() == inventory.InventoryName.ToLower())) { return; }

            _db.Inventories.Add(inventory);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            // to prevent different inventories from having the same name
            //if (_db.Inventories.Any(x => x.InventoryId != inventory.InventoryId && x.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase))) { return; }
            if (_db.Inventories.Any(x => x.InventoryId != inventory.InventoryId && x.InventoryName.ToLower() == inventory.InventoryName.ToLower())) { return; }

            var inv = await _db.Inventories.FindAsync(inventory.InventoryId);

            if (inv != null)
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Price = inventory.Price;
                inv.Quantity = inventory.Quantity;

                await _db.SaveChangesAsync();
            }
        }
        
        public async Task<Inventory?> GetInventoryByIdAsync(int inventoryId)
        {
            return await _db.Inventories.FindAsync(inventoryId);
        }
    }
}
