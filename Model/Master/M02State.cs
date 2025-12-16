namespace atmglobalapi.Model.Master
{
    public class M02State
    {
        public int Type { get; set; }          // 1–5
        public int? Id { get; set; }
        public int? CountryId { get; set; }
        public string? StateName { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public string? StateCode { get; set; }
        public bool? is_active { get; set; }
    }
}
