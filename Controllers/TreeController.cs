using MaterializedPathTreeAPI.Models.Info;
using MaterializedPathTreeAPI.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace MaterializedPathTreeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TreeController : Controller
    {
        private readonly TreeService _treeService;

        public TreeController(TreeService treeService)
        {
            _treeService = treeService;
        }

        [HttpPost]
        [Route("{treekey}/treeitem")]
        public async Task<ActionResult<TreeItemView>> CreateTreeItem(Guid treekey, [FromBody] TreeItemInfo info)
        {
            var result = await _treeService.CreateTreeItem(treekey, info);
            return Ok(result);
        }

        [HttpPost]
        [Route("tree")]
        public async Task<ActionResult> CreateTree([FromBody] CreateTreeInfo info)
        {
            var result = await _treeService.CreateTree(info.TreeKey, info.SortType, info.Name);
            return Ok(result);
        }

        [HttpGet]
        [Route("treeitem/{treeitemid}")]
        public async Task<ActionResult<TreeItemView>> GetTreeItem(Guid treeitemid)
        {
            var result = await _treeService.GetTreeItem(treeitemid);

            return Ok(result);
        }

        [HttpGet]
        [Route("treeitem/{treeitemid}/children")]
        public async Task<ActionResult<TreeItemView>> GetChildTreeItems(Guid treeitemid)
        {
            var result = await _treeService.GetChildTreeItems(treeitemid);

            return Ok(result);
        }

        [HttpGet]
        [Route("treeitem/{treeitemid}/parents")]
        public async Task<ActionResult<TreeItemView>> GetParentTreeItems(Guid treeitemid)
        {
            var result = await _treeService.GetParentTreeItems(treeitemid);

            return Ok(result);
        }

        [HttpPost]
        [Route("FlatTree")]
        public async Task<ActionResult<TreeItemView>> GetTreeItemsFlatTree([FromBody] GetTreeItemsByTreeInfo info)
        {
            var result = await _treeService.GetTreeItemsTree(info);

            return Ok(result);
        }
    }
}