namespace atmglobalapi.Model.User
{
    public class A02Role
    {
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }

        // 1=Active, 0=Inactive, 2=Archived
        public int? Status { get; set; }

        // Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? IsSystem { get; set; }

        public int Type { get; set; }
    }
}