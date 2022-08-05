namespace MaterializedPathTreeAPI.Models.View
{
    public class TreeItemView
    {
        public Guid EntityId { get; set; }

        public Guid? ParentEntityId { get; set; }

        public string EntityValue { get; set; }

        public string Path { get; set; }

        public int Level { get; set; }
    }
}