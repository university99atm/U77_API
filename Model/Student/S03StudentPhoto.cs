namespace atmglobalapi.Model.Student
{
    public class S03StudentPhoto
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public string EnrollmentId { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? DocumentSerialNo { get; set; }
        public string DocumentBase64 { get; set; }
        public bool? IsActive { get; set; }
    }
}
