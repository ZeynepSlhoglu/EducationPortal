namespace EducationPortalAPI.Models
{
    public class EducationContent
    {
        public int ID { get; set; }
        public int EducationID { get; set; }
        public string ContentName { get; set; }
        public string ContentType { get; set; }
        public string ContentPath { get; set; }
    }
}
