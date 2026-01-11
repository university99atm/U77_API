namespace atmglobalapi.Model.Master
{
    public class M27ExternalInstitute
    {
        public int? Id { get; set; }
        public string? InstituteName { get; set; }
        public string? InstituteType { get; set; }

        // Foreign Keys
        public int? EducationBoardId { get; set; }
        public int? MediumOfInstructionId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }

        // Address
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
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
