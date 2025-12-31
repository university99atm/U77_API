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

        // 🔹 NEW
        public bool? System { get; set; }        // 0 = User, 1 = System
        public string? IPAddress { get; set; }

        // operation type (1–6)
        public int Type { get; set; }

        public int? operation_by { get; set; }
    }
}
