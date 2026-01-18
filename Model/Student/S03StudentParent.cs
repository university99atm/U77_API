namespace atmglobalapi.Model.Student
{
    public class S03StudentParent
    {
        public long? Id { get; set; }

        // Student Info
        public long? StudentId { get; set; }
        public int? RelationId { get; set; }

        // Personal Info
        public string? FullName { get; set; }
        public string? MobileNo { get; set; }
        public string? MobileNo2 { get; set; }
        public string? EmailId { get; set; }
        public string? LandlineNo { get; set; }
        public string? AadhaarNo { get; set; }

        // Professional Info
        public string? Qualification { get; set; }
        public string? Profession { get; set; }
        public string? Designation { get; set; }
        public string? CompanyName { get; set; }
        public string? OfficeAddress { get; set; }

        // Income Info
        public int? AnnualIncomeId { get; set; }
        public decimal? ExactIncome { get; set; }

        // Status
        public int? Status { get; set; }

        // Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? System { get; set; }

        // Operation Type
        public int Type { get; set; }
    }
}