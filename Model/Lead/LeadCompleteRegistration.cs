namespace atmglobalapi.Model.Lead
{
    public class LeadCompleteRegistration
    {
        public LeadInfo? Lead { get; set; }
        public List<LeadCourseInfo>? Courses { get; set; }
        public bool? System { get; set; }
    }

    public class LeadInfo
    {
        public int? TitleId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNo1 { get; set; }
        public string? MobileNo2 { get; set; }
        public string? EmailId { get; set; }
        public int? GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public int? AreaId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Pincode { get; set; }
        public string? Remarks { get; set; }
        public int? AssignedUserId { get; set; }
    }

    public class LeadCourseInfo
    {
        public int? OrganizationId { get; set; }
        public int? CollegeId { get; set; }
        public int? BranchId { get; set; }
        public int? CourseTypeId { get; set; }
        public int? UniversityId { get; set; }
        public int? SectorId { get; set; }
        public int? CourseId { get; set; }
        public int? PriorityOrder { get; set; }
    }
}