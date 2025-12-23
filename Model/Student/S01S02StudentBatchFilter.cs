namespace atmglobalapi.Model.Student
{
    public class S01S02StudentBatchFilter
    {
        /* ---------- STUDENT FILTERS ---------- */
        public int? StudentId { get; set; }
        public string? SRN { get; set; }
        public string? StudentName { get; set; }

        // 🔥 EXTRA S01 FILTERS (MISSING EARLIER)
        public int? NationalityId { get; set; }
        public int? ReligionId { get; set; }
        public int? CategoryId { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }

        /* ---------- ENROLLMENT FILTERS ---------- */
        public int? SectionId { get; set; }
        public int? FeeCategoryId { get; set; }
        public string? EnrollmentId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }

        /* ---------- BATCH FILTERS ---------- */
        public int? OrganizationId { get; set; }
        public int? CollegeId { get; set; }
        public int? BranchId { get; set; }
        public int? CourseTypeId { get; set; }
        public int? UniversityId { get; set; }
        public int? SectorId { get; set; }
        public int? CourseId { get; set; }
        public int? BatchId { get; set; }
        public string? BatchCode { get; set; }
        public string? BatchName { get; set; }
        public string? BatchType { get; set; }

        /* ---------- PAGINATION ---------- */
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
