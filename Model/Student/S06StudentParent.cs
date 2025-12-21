namespace atmglobalapi.Model.Student
{
    public class S06StudentParent
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public int? RelationId { get; set; }

        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public string MobileNo2 { get; set; }
        public string EmailId { get; set; }
        public string LandlineNo { get; set; }
        public string AadhaarNo { get; set; }

        public string Qualification { get; set; }
        public string Profession { get; set; }
        public string Designation { get; set; }
        public string CompanyName { get; set; }
        public string OfficeAddress { get; set; }

        public string Industry { get; set; }
        public string FunctionalArea { get; set; }
        public string RoleName { get; set; }

        public decimal? AnnualIncome { get; set; }
    }
}
