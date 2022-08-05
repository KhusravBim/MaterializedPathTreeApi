namespace MaterializedPathTreeAPI.Models.Info
{
    public class GetTreeItemsByTreeInfo
    {
        public Guid TreeKey { get; set; }
        public IReadOnlyCollection<Guid>? ExpandedTreeItemIds { get; set; }
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
    }
}