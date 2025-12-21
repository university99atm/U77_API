namespace atmglobalapi.Model.Master
{
    public class M27ExternalInstitute
    {
        public int Type { get; set; }          // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }

        public string? InstituteName { get; set; }
        public string? InstituteCode { get; set; }
        public string? InstituteType { get; set; }

        public int? SectorId { get; set; }
        public int? EducationBoardId { get; set; }
        public int? MediumOfInstructionId { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }

        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Pincode { get; set; }

        public bool? Status { get; set; }
        public bool? Archive { get; set; }
    }
}
