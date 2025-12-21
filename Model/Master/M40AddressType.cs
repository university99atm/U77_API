namespace atmglobalapi.Model.Master
{
    public class M40AddressType
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public string? AddressTypeCode { get; set; }
        public string? AddressTypeName { get; set; }

        public bool? Status { get; set; }
        public bool? Archive { get; set; }
    }
}
