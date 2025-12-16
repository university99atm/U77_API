namespace atmglobalapi.Model.Master
{
    public class M18FeeCategory
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public string? FeeCategoryName { get; set; }
        public string? FeeCategoryCode { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}