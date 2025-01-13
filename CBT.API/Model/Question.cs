using System.ComponentModel.DataAnnotations;

namespace CBT.API.Model
{
    public class Question
    {
        [Key]
        public int id { get; set; } = 0;
        public string question { get; set; }
        public int weight { get; set; }
        public int question_topic_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; } 

        public List<QuestionOption> question_options { get; set; } 
    }
}
