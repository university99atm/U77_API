namespace atmglobalapi.Model.Master
{
    public class M02State
    {
        public int? Id { get; set; }
        public int? CountryId { get; set; }
        public string? StateName { get; set; }
        public string? StateCode { get; set; }

        // 1=Active,0=Inactive,2=Deleted
        public int? Status { get; set; }

        // Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? System { get; set; }
        public string? IPAddress { get; set; }

        public int Type { get; set; }
    }
}
