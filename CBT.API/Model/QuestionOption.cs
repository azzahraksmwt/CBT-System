using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CBT.API.Model
{
    public class QuestionOption
    {
        [Key]
        public int id { get; set; } = 0;
        public int question_id { get; set; }
        public string option { get; set; }
        public Boolean IsCorrect { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
