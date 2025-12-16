namespace atmglobalapi.Model.Master
{
    public class M07College
    {
        public int Type { get; set; }            // 1=Insert,2=Update,3=Delete,4=Archive,5=Get
        public int? Id { get; set; }
        public int? OrganizationId { get; set; }
        public string? CollegeName { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? is_active { get; set; }
        public int? Created_by { get; set; }
        public int? Updated_by { get; set; }
    }
}
