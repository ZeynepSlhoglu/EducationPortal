namespace EducationPortalAPI.Models
{
    public class Education
    {
        public int ID { get; set; } 
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public int UserId { get; set; }
        public int Capacity { get; set; }
        public int Duration { get; set; }
        public float Price { get; set; }
    }
}

