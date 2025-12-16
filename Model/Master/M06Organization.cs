namespace atmglobalapi.Model.Master
{
    public class M06Organization
    {
        public int Type { get; set; }          // 1=Insert,2=Update,3=Delete,4=Archive,5=Get
        public int? Id { get; set; }
        public string? OrganizationName { get; set; }
        public string? OrganizationType { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? is_active { get; set; }
        public int? Created_by { get; set; }
        public int? Updated_by { get; set; }
    }
}
