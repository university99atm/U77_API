namespace atmglobalapi.Model.Master
{
    public class M38BloodGroup
    {
        public int Type { get; set; }              // 1–5
        public int? Id { get; set; }
        public string? BloodGroupName { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}
