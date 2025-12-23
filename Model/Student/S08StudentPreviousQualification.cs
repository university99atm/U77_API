namespace atmglobalapi.Model.Student
{
    public class S08StudentPreviousQualification
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public int? DegreeId { get; set; }
        public int? QualificationId { get; set; }
        public int? InstituteId { get; set; }
        public int? EducationBoardId { get; set; }
        public int? MediumOfInstructionId { get; set; }

        public string? TCNumber { get; set; }
        public string? RollNumber { get; set; }

        public int? PassingYearId { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public decimal? PercentageOrCGPA { get; set; }

        public int? SectorId { get; set; }
        public string? ReasonForChange { get; set; }
    }
}
