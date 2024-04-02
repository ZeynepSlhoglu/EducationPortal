﻿namespace EducationPortalUI.Models
{
    public class Participation
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int EducationID { get; set; }
        public bool RequestStatus { get; set; }
        public bool CompletionStatus { get; set; }
    }
}
