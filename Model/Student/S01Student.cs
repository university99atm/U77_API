namespace atmglobalapi.Model.Student
{
    public class S01Student
    {
        public long? Id { get; set; }

        // Personal Info
        public int? TitleId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }

        // Contact Info
        public string? PersonalEmail { get; set; }
        public string? CollegeEmail { get; set; }
        public string? MobileNo1 { get; set; }
        public string? MobileNo2 { get; set; }

        // Additional Info
        public int? BloodGroupId { get; set; }
        public int? CategoryId { get; set; }
        public int? ReligionId { get; set; }
        public int? MotherTongueId { get; set; }
        public int? NationalityId { get; set; }
        public int? MaritalStatusId { get; set; }

        // Password for User Creation (Type 1 only)
        public string? Password { get; set; }

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