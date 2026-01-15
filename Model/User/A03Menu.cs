namespace atmglobalapi.Model.User
{
    public class A03Menu
    {
        public int? MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? MenuCode { get; set; }
        public int? ParentMenuId { get; set; }
        public int? DisplayOrder { get; set; }
        public string? MenuUrl { get; set; }
        public string? MenuIcon { get; set; }
        public bool? IsVisible { get; set; }

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