namespace atmglobalapi.Model.Lead
{
    public class L01Lead
    {
        public long? Id { get; set; }

        // Lead Basic Info
        public int? TitleId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        // Contact Info
        public string? MobileNo1 { get; set; }
        public string? MobileNo2 { get; set; }
        public string? EmailId { get; set; }

        // Personal Info
        public int? GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Address Info
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public int? AreaId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Pincode { get; set; }

        // Lead Info
        public string? Remarks { get; set; }
        public int? AssignedUserId { get; set; }

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