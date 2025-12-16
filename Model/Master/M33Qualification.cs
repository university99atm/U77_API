namespace atmglobalapi.Model.Master
{
    public class M33Qualification
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public int? DegreeId { get; set; }
        public string? QualificationName { get; set; }
        public string? QualificationCode { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}