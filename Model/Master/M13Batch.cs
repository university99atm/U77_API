namespace atmglobalapi.Model.Master
{
    public class M13Batch
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public int? OrganizationId { get; set; }
        public int? CollegeId { get; set; }
        public int? BranchId { get; set; }
        public int? CourseTypeId { get; set; }
        public int? UniversityId { get; set; }
        public int? SectorId { get; set; }
        public int? CourseId { get; set; }
        public string? BatchCode { get; set; }
        public string? BatchName { get; set; }
        public string? BatchType { get; set; }
        public int? SoftSkillCapacity { get; set; }
        public int? CoreSkillCapacity { get; set; }
        public int? usp_softskill { get; set; }
        public int? usp_coreskill { get; set; }
        public string? SDMSBatchId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? BatchStatus { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}