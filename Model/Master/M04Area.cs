namespace atmglobalapi.Model.Master
{
    public class M04Area
    {
        public int? Id { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public string? AreaName { get; set; }
        public string? Pincode { get; set; }

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
