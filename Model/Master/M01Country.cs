namespace atmglobalapi.Model.Master
{
    public class M01Country
    {
        public int? Id { get; set; }
        public string? CountryName { get; set; }
        public string? Country_Code { get; set; }

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
