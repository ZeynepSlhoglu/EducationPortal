namespace EducationPortalAPI.Models
{
    public class Education
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int InstructorID { get; set; }
        public int Capacity { get; set; }
        public decimal Cost { get; set; }
        public int Duration { get; set; }
    }
}
