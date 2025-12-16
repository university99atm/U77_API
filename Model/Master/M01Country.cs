namespace atmglobalapi.Model.Master
{
    public class M01Country
    {
        public int? Id { get; set; }
        public string? CountryName { get; set; }
        public bool? status { get; set; }
        public bool? archive { get; set; }
        public string? Country_Code { get; set; }
        public bool? is_active { get; set; }

        // operation type (1–5)
        public int Type { get; set; }
    }
}
