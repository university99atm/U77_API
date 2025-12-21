namespace atmglobalapi.Model.Master
{
    public class M39StudentPhotoType
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public string? DocumentTypeCode { get; set; }
        public string? DocumentTypeName { get; set; }
        public string? Description { get; set; }

        public bool? Status { get; set; }
        public bool? Archive { get; set; }
    }
}
