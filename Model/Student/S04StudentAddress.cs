namespace atmglobalapi.Model.Student
{
    public class S04StudentAddress
    {
        public long? Id { get; set; }

        // Student Info
        public long? StudentId { get; set; }
        public int? AddressTypeId { get; set; }

        // Address Info
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }

        // Location
        public int? AreaId { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }

        public string? Pincode { get; set; }
        public bool? IsCurrent { get; set; }

        // Status
        public int? Status { get; set; }

        // Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? System { get; set; }

        // Operation Type
        public int Type { get; set; }
    }
}