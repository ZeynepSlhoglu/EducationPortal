namespace EducationPortalAPI.Models
{
    public class Participation
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int EducationID { get; set; }
        public bool RequestStatus { get; set; }
    }
}
