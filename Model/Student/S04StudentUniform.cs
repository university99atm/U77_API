namespace atmglobalapi.Model.Student
{
    public class S04StudentUniform
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public int? BatchEnrollmentId { get; set; }
        public int? GenderId { get; set; }
        public string UniformType { get; set; }

        public string Size { get; set; }
        public string Length { get; set; }
        public string Chest { get; set; }
        public string Waist { get; set; }
        public string Hip { get; set; }

        public bool? IsReceived { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public int? HandedOverBy { get; set; }
    }
}
