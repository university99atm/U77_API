namespace atmglobalapi.Model.Student
{
    public class S05StudentAcademicHistory
    {
        public long? Id { get; set; }

        // Student Info
        public long? StudentId { get; set; }
        public int? DegreeId { get; set; }
        public int? QualificationId { get; set; }

        // Qualification Flags
        public bool? IsMinQualification { get; set; }
        public bool? IsHighestQualification { get; set; }

        // Institute Info
        public int? InstituteId { get; set; }
        public int? BoardUniversityId { get; set; }
        public int? MediumOfInstructionId { get; set; }

        // Academic Details
        public string? RollNumber { get; set; }
        public int? PassingYearId { get; set; }

        // Marks
        public decimal? TotalMarks { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public decimal? PercentageOrCGPA { get; set; }

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