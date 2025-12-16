namespace atmglobalapi.Model.Master
{
    public class M09CourseType
    {
        public int Type { get; set; }          // 1=Insert,2=Update,3=Delete,4=Archive,5=Get
        public int? Id { get; set; }
        public string? CourseTypeName { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? is_active { get; set; }
        public int? Created_by { get; set; }
        public int? Updated_by { get; set; }
    }
}
