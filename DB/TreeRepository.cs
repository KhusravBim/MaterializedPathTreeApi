using MaterializedPathTreeAPI.DB.Models;
using MaterializedPathTreeAPI.Models.Info;
using Microsoft.EntityFrameworkCore;

namespace MaterializedPathTreeAPI.DB
{
    public class TreeRepository
    {
        private readonly TreeContext _context;

        public TreeRepository(TreeContext context)
        {
            _context = context;
        }

        public async Task<Tree> CreateTree(Guid treeKey, string sortType, string name)
        {
            var tree = new Tree
            {
                Id = treeKey,
                Name = name,
                Sorting = sortType,
            };

            await _context.Trees.AddAsync(tree);

            await _context.SaveChangesAsync();

            return tree;
        }

        public async Task<TreeItem> CreateTreeItem(Guid treeKey, TreeItemInfo treeItemInfo)
        {
            var treeItem = new TreeItem
            {
                EntityId = treeItemInfo.Id,
                EntityValue = treeItemInfo.Value,
                ParentEntityId = treeItemInfo.ParentId,
                TreeKey = treeKey,
            };

            //получаем родителя элемента для извлечения MaterializedPath
            var parentTreeItem = await _context.TreeItems
                                                 .Where(x => x.EntityId == treeItem.ParentEntityId)
                                                 .FirstOrDefaultAsync();

            //Обновляем MaterializedPath в соотвествии наличия родителя элемента
            treeItem.MaterializedPath =
                new LTree(parentTreeItem == null ?
                    GetPath(treeItem.EntityValue) :
                    ConcatPath(parentTreeItem.MaterializedPath, treeItem.EntityValue));

            await _context.TreeItems.AddAsync(treeItem);
            await _context.SaveChangesAsync();

            return treeItem;
        }

        public async Task<Tree?> GetTree(Guid treekey)
        {
            return await _context.Trees.FindAsync(treekey);
        }

        public async Task<TreeItem?> GetTreeItem(Guid treeItemKey) =>
            await _context.TreeItems
                          .Where(s => s.EntityId == treeItemKey)
                          .Select(x =>
                             new TreeItem
                             {
                                 EntityId = x.EntityId,
                                 EntityValue = x.EntityValue,
                                 Id = x.Id,
                                 Level = x.MaterializedPath.NLevel,
                                 MaterializedPath = x.MaterializedPath,
                                 ParentEntityId = x.ParentEntityId,
                                 TreeKey = x.TreeKey
                             })
                          .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<TreeItem>> GetTreeItemsTree(
            Guid treeKey,
            int fromIndex,
            int toIndex,
            IReadOnlyCollection<Guid> expandedItemIds)
        {
            //получаем список корневых элементов + список тех элементов которые надо раскрыть
            var query = _context.TreeItems
                                .Where(t => t.TreeKey == treeKey &&
                                            (!t.ParentEntityId.HasValue ||
                                             t.ParentEntityId.HasValue &&
                                             expandedItemIds.Contains(t.ParentEntityId.Value)));

            //если список пустой то вовзращаем пустой список
            if (await query.CountAsync() < 1)
                return Array.Empty<TreeItem>();

            // получаем результирующий список элементов
            return await query
                           .OrderBy(t => t.MaterializedPath)
                           .Skip(fromIndex)
                           .Take(toIndex - fromIndex + 1)
                           .Select(x =>
                               new TreeItem
                               {
                                   EntityId = x.EntityId,
                                   EntityValue = x.EntityValue,
                                   Id = x.Id,
                                   Level = x.MaterializedPath.NLevel,
                                   MaterializedPath = x.MaterializedPath,
                                   ParentEntityId = x.ParentEntityId,
                                   TreeKey = x.TreeKey
                               })
                           .ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<TreeItem>> GetParentTreeItems(Guid treeItemId)
        {
            var treeItem = await GetTreeItem(treeItemId);

            //MaterializedPath.IsAncestorOf соотвествует в postgresSql данному оператору @>
            return await _context.TreeItems
                                 .Where(x =>
                                        x.MaterializedPath.IsAncestorOf(treeItem.MaterializedPath) &&
                                        x.EntityId != treeItemId)
                                 .Select(x =>
                                    new TreeItem
                                    {
                                        EntityId = x.EntityId,
                                        EntityValue = x.EntityValue,
                                        Id = x.Id,
                                        Level = x.MaterializedPath.NLevel,
                                        MaterializedPath = x.MaterializedPath,
                                        ParentEntityId = x.ParentEntityId,
                                        TreeKey = x.TreeKey
                                    })
                                 .ToListAsync();
        }

        public async Task<IReadOnlyCollection<TreeItem>> GetChildTreeItems(Guid treeItemId)
        {
            var treeItem = await GetTreeItem(treeItemId);

            //MaterializedPath.IsDescendantOf соотвествует в postgresSql данному оператору <@
            return await _context.TreeItems
                                 .Where(x =>
                                        x.MaterializedPath.IsDescendantOf(treeItem.MaterializedPath) &&
                                        x.EntityId != treeItemId)
                                 .Select(x =>
                                    new TreeItem
                                    {
                                        EntityId = x.EntityId,
                                        EntityValue = x.EntityValue,
                                        Id = x.Id,
                                        Level = x.MaterializedPath.NLevel,
                                        MaterializedPath = x.MaterializedPath,
                                        ParentEntityId = x.ParentEntityId,
                                        TreeKey = x.TreeKey
                                    })
                                 .ToListAsync();
        }

        private string GetPath(string value)
        {
            //метод для создания MaterializedPath
            //Тут можно задать логику по пребразованию пути например в base64
            return value;
        }

        private string ConcatPath(string prevPath, string value)
        {
            //метод для создания MaterializedPath
            //Тут можно задать логику по преброзованию пути например в base64
            return $"{prevPath}.{value}";
        }
    }
}
