namespace atmglobalapi.Model.Master
{
    public class M12Course
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public int? CourseTypeId { get; set; }
        public int? CollegeId { get; set; }
        public int? MinQualificationId { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public int? DurationYears { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}