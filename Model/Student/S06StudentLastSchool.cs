namespace atmglobalapi.Model.Student
{
    public class S06StudentLastSchool
    {
        public long? Id { get; set; }

        // Student Info
        public long? StudentId { get; set; }
        public int? InstituteId { get; set; }
        public int? SectorId { get; set; }
        public string? BatchCode { get; set; }

        // Principal Info
        public string? PrincipalName { get; set; }
        public string? PrincipalMobile { get; set; }
        public string? PrincipalEmail { get; set; }

        // Best School Teacher
        public string? BestSchoolTeacherName { get; set; }
        public string? BestSchoolTeacherMobile { get; set; }
        public string? BestSchoolTeacherEmail { get; set; }

        // Best Coaching Teacher
        public string? BestCoachingTeacherName { get; set; }
        public string? BestCoachingTeacherMobile { get; set; }
        public string? BestCoachingTeacherEmail { get; set; }

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