using System.ComponentModel.DataAnnotations;

namespace CBT.API.Model
{
    public class Schedule
    {
        [Key]
        public int id { get; set; }
        public int question_topic_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
