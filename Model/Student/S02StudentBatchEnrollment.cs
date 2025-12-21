namespace atmglobalapi.Model.Student
{
    public class S02StudentBatchEnrollment
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public int? BatchId { get; set; }
        public string StudentRollNo { get; set; }
        public string EnrollmentId { get; set; }
        public string UniversityEnrollNo { get; set; }
        public string FormNo { get; set; }
        public string Specialization { get; set; }
        public int? SectionId { get; set; }
        public int? FeeCategoryId { get; set; }
        public DateTime? GovernmentFeesDate { get; set; }
        public int? GovernmentFreezeBy { get; set; }
        public string GovernmentFreezeIp { get; set; }
        public string EnrollIpAddress { get; set; }
        public string Campaign { get; set; }
        public string SubCampaign { get; set; }
        public bool? DropIn { get; set; }
        public bool? AllSemesters { get; set; }
        public int? BatchEnrollBy { get; set; }
        public DateTime? BatchEnrollDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public int? MobilizedBy { get; set; }
        public bool? IsPlaced { get; set; }
        public bool? AssessmentAttendance { get; set; }
        public string AssessmentResult { get; set; }
    }
}
