using System.ComponentModel.DataAnnotations;

namespace CBT.API.Model
{
    public class TestResultHistory 
    {
        [Key]
        public int id { get; set; }
        public int test_result_id { get; set; }
        public int question_id { get; set; }
        public int question_option_id { get; set; }
        public Boolean IsCorrect { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
