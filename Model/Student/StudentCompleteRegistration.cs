namespace atmglobalapi.Model.Student
{
    public class StudentCompleteRegistration
    {
        // Student Basic Info
        public StudentInfo? Student { get; set; }
        
        // Course Enquiries (Multiple)
        public List<EnquiryInfo>? Enquiries { get; set; }
        
        // Parent/Guardian Info (Multiple)
        public List<ParentInfo>? Parents { get; set; }
        
        // Addresses (Multiple)
        public List<AddressInfo>? Addresses { get; set; }
        
        // Academic History (Multiple)
        public List<AcademicInfo>? AcademicHistory { get; set; }
        
        // Last School Info
        public SchoolInfo? LastSchool { get; set; }
        
        // Audit
        public bool? System { get; set; }
    }

    // Nested Classes
    public class StudentInfo
    {
        public int? TitleId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CollegeEmail { get; set; }
        public string? MobileNo1 { get; set; }
        public string? MobileNo2 { get; set; }
        public int? BloodGroupId { get; set; }
        public int? CategoryId { get; set; }
        public int? ReligionId { get; set; }
        public int? MotherTongueId { get; set; }
        public int? NationalityId { get; set; }
        public int? MaritalStatusId { get; set; }
        public string? Password { get; set; } // Plain password (will be encrypted)
    }

    public class EnquiryInfo
    {
        public int? OrganizationId { get; set; }
        public int? CollegeId { get; set; }
        public int? BranchId { get; set; }
        public int? CourseTypeId { get; set; }
        public int? UniversityId { get; set; }
        public int? SectorId { get; set; }
        public int? CourseId { get; set; }
    }

    public class ParentInfo
    {
        public int? RelationId { get; set; }
        public string? FullName { get; set; }
        public string? MobileNo { get; set; }
        public string? MobileNo2 { get; set; }
        public string? EmailId { get; set; }
        public string? Qualification { get; set; }
        public string? Profession { get; set; }
        public string? Designation { get; set; }
        public string? CompanyName { get; set; }
        public string? OfficeAddress { get; set; }
        public int? AnnualIncomeId { get; set; }
        public decimal? ExactIncome { get; set; }
    }

    public class AddressInfo
    {
        public int? AddressTypeId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int? AreaId { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? Pincode { get; set; }
        public bool? IsCurrent { get; set; }
    }

    public class AcademicInfo
    {
        public int? DegreeId { get; set; }
        public int? QualificationId { get; set; }
        public bool? IsMinQualification { get; set; }
        public bool? IsHighestQualification { get; set; }
        public int? InstituteId { get; set; }
        public int? BoardUniversityId { get; set; }
        public string? RollNumber { get; set; }
        public int? PassingYearId { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public decimal? PercentageOrCGPA { get; set; }
    }

    public class SchoolInfo
    {
        public int? InstituteId { get; set; }
        public int? SectorId { get; set; }
        public string? BatchCode { get; set; }
        public string? PrincipalName { get; set; }
        public string? PrincipalMobile { get; set; }
    }
}