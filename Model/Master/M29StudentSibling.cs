namespace atmglobalapi.Model.Master
{
    public class M29StudentSibling
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public string? RelationshipName { get; set; }
        public string? RelationshipCode { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}