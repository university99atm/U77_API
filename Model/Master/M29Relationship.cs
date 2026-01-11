namespace atmglobalapi.Model.Master
{
    public class M29Relationship
    {
        public int? Id { get; set; }
        public string? RelationshipName { get; set; }
        public bool? IsForParental { get; set; }
        public string? RelationType { get; set; }

        // 1=Active, 0=Inactive, 2=Archived
        public int? Status { get; set; }

        // Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? System { get; set; }

        public int Type { get; set; }
    }
}