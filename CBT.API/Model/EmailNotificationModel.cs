namespace CBT.API.Model
{
    public class EmailNotificationModel
    {
        public List<string> Recipients { get; set; } = new();
        public int TemplateId { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TopicName { get; set; }
        public string Duration { get; set; }
        public string LoginLink { get; set; }
    }
}
