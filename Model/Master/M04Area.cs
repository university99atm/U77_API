namespace atmglobalapi.Model.Master
{
    public class M04Area
    {
        public int Type { get; set; }        // 1=Insert,2=Update,3=Delete,4=Archive,5=Get
        public int? Id { get; set; }
        public int? DistrictId { get; set; }
        public string? AreaName { get; set; }
        public string? Pincode { get; set; }
        public int? CityId { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? is_active { get; set; }
    }
}
