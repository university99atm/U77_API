namespace atmglobalapi.Model.Master
{
    public class M03District
    {
        public int Type { get; set; }          // 1-Insert,2-Update,3-Delete,4-Archive,5-Get
        public int? Id { get; set; }
        public int? StateId { get; set; }
        public string? DistrictName { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? is_active { get; set; }
    }
}
