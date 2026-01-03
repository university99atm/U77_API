namespace atmglobalapi.Model.Master
{
    public class M02State
    {
        public int? Id { get; set; }
        public int? CountryId { get; set; }
        public string? StateName { get; set; }
        public string? StateCode { get; set; }

        // 🔹 Status
        // 1 = Active, 0 = Inactive, 2 = Deleted
        public int? Status { get; set; }

        // 🔹 Pagination & Search (Type 7)
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // 🔹 Audit
        public bool? System { get; set; }
        public string? IPAddress { get; set; }

        // 🔹 Operation
        public int Type { get; set; }
        public int? OperationBy { get; set; }
    }
}
