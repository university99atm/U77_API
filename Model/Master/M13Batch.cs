namespace atmglobalapi.Model.Master
{
    public class M13Batch
    {
        public int? Id { get; set; }
        public int? SectorId { get; set; }
        public int? UniversityId { get; set; }
        public int? CourseTypeId { get; set; }
        public int? CourseId { get; set; }
        public int? CollegeId { get; set; }
        public int? BranchId { get; set; }
        public string? BatchName { get; set; }
        public string? BatchType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? BatchStatus { get; set; }

        // 1=Active, 0=Inactive, 2=Archived
        public int? Status { get; set; }

        // Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? System { get; set; }

        public int Type { get; set; }
    }
}