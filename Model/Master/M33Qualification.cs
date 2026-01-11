namespace atmglobalapi.Model.Master
{
    public class M33Qualification
    {
        public int? Id { get; set; }
        public int? DegreeId { get; set; }
        public string? QualificationName { get; set; }
        public string? Description { get; set; }

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