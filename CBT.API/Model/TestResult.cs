using System.ComponentModel.DataAnnotations;

namespace CBT.API.Model
{
    public class TestResult
    {
        [Key]
        public int id { get; set; }
        public int user_management_id { get; set; }
        public int question_topic_id { get; set; }
        public int correct_score { get; set; }
        public int incorrect_score { get; set; }
        public int total_score { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }

        public List<TestResultHistory> test_result_history { get; set; } = new List<TestResultHistory>();
        //public List<UserManagement> user_management { get; set; } = new List<UserManagement>();
        //public List<QuestionTopic> question_topic { get; set; } = new List<QuestionTopic>();
    }
}
