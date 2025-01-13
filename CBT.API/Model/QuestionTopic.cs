using System.ComponentModel.DataAnnotations;

namespace CBT.API.Model
{
    public class QuestionTopic
    {
        [Key]
        public int id { get; set; }
        public string topic_name { get; set; }
        public string difficulty_level { get; set; }
        public string duration { get; set; }
        public string total_questions { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }

        public List<Question> questions { get; set; } = new List<Question>();
    }
}
