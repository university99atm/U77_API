namespace atmglobalapi.Model.Master
{
    public class M07College
    {
        public int? Id { get; set; }
        public int? OrganizationId { get; set; }
        public string? CollegeName { get; set; }

        // 1 = Active, 0 = Inactive, 2 = Deleted
        public int? Status { get; set; }

        // Pagination & Search
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? System { get; set; }
        public string? IPAddress { get; set; }

        public int Type { get; set; }
    }
}
