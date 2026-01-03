namespace atmglobalapi.Model.Master
{
    public class M05City
    {
        public int? Id { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public string? CityName { get; set; }

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
