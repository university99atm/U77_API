namespace atmglobalapi.Model.Master
{
    public class M12Course
    {
        public int? Id { get; set; }
        public int? CourseTypeId { get; set; }
        public int? CollegeId { get; set; }
        public int? MinQualificationId { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public int? DurationYears { get; set; }
        public string? Description { get; set; }

        // 1 = Active, 0 = Inactive, 2 = Deleted
        public int? Status { get; set; }

        // Pagination & Search
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Filters
        public int? CourseTypeFilter { get; set; }
        public int? CollegeFilter { get; set; }

        // Audit
        public bool? System { get; set; }
        public string? IPAddress { get; set; }

        public int Type { get; set; }
    }
}
