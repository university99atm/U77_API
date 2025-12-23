namespace atmglobalapi.Model.Student
{
    public class S01StudentEnrollment
    {
        public int Type { get; set; }
        public int? StudentId { get; set; }

        // Student
        public string? SRN { get; set; }
        public string? StudentCode { get; set; }
        public int? TitleId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CollegeEmail { get; set; }
        public string? MobileNo1 { get; set; }
        public string? MobileNo2 { get; set; }
        public string? BloodGroup { get; set; }
        public int? CategoryId { get; set; }
        public int? HighestQualificationId { get; set; }
        public string? MaritalStatus { get; set; }
        public int? NationalityId { get; set; }
        public int? MotherTongueId { get; set; }
        public int? ReligionId { get; set; }
        public int? FinancialYearId { get; set; }
        public int? StatusId { get; set; }
        public string? Remarks { get; set; }

        // User
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public int? RollId { get; set; }
    }
}
