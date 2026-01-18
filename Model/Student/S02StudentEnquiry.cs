namespace atmglobalapi.Model.Student
{
    public class S02StudentEnquiry
    {
        public long? Id { get; set; }

        // Student Info
        public long? StudentId { get; set; }
        public string? SRN { get; set; }

        // Academic Info
        public int? OrganizationId { get; set; }
        public int? CollegeId { get; set; }
        public int? BranchId { get; set; }
        public int? CourseTypeId { get; set; }
        public int? UniversityId { get; set; }
        public int? SectorId { get; set; }
        public int? CourseId { get; set; }

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