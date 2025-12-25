namespace atmglobalapi.Model.Student
{
    public class S07StudentAddress
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public int? AddressTypeId { get; set; }

        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }

        public int? AreaId { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }

        public string? Pincode { get; set; }
        public bool? IsPrimary { get; set; }
    }
}
