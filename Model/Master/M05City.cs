namespace atmglobalapi.Model.Master
{
    public class M05City
    {
        public int? Id { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public string? CityName { get; set; }
        public string? CityCode { get; set; }

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
