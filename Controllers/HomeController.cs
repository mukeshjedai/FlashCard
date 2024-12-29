using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyMvcApp.Models;


namespace MyMVCApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public void InsertWrongAnswer([FromBody]WrongAnswers wrongAnswer)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string enableIdentityInsert = "SET IDENTITY_INSERT WrongAnswers ON;";
                string query = "INSERT INTO WrongAnswers (Id, Question, Options, Answer, SoftDelete) VALUES (@Id, @Question, @Options, @Answer, @SoftDelete)";
                string disableIdentityInsert = "SET IDENTITY_INSERT WrongAnswers OFF;";

                SqlCommand enableCommand = new SqlCommand(enableIdentityInsert, connection);
                SqlCommand command = new SqlCommand(query, connection);
                SqlCommand disableCommand = new SqlCommand(disableIdentityInsert, connection);

                command.Parameters.AddWithValue("@Id", wrongAnswer.Id);
                command.Parameters.AddWithValue("@Question", "wrongAnswer.Question");
                command.Parameters.AddWithValue("@Options", string.Join(";", wrongAnswer.Options));
                command.Parameters.AddWithValue("@Answer", "wrongAnswer.Answer");
                command.Parameters.AddWithValue("@SoftDelete", 1);

                connection.Open();

                enableCommand.ExecuteNonQuery();
                command.ExecuteNonQuery();
                disableCommand.ExecuteNonQuery();
            }
        }


        public class QuestionModel
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public string Answer { get; set; }
        }

        public class WrongAnswers
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public string Answer { get; set; }
            public bool SoftDelete { get; set; }

            public WrongAnswers()
            {
                Options = new List<string>();
            }

            public WrongAnswers(int id, string question, List<string> options, string answer, bool softDelete)
            {
                Id = id;
                Question = question;
                Options = options;
                Answer = answer;
                SoftDelete = softDelete;
            }
        }

        private readonly string connectionString = "Server=tcp:flashdbserver.database.windows.net,1433;Initial Catalog=flashcardsdb;Persist Security Info=False;User ID=admin123;Password=Mukesh123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        // GET: Home
        public ActionResult Index()
        {
            var questions = GetQuestions();
            return View(questions);
        }

        private List<QuestionModel> GetQuestions()
        {
            var questions = new List<QuestionModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Question, Options, Answer FROM Questions";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var question = new QuestionModel
                        {
                            Id = reader.GetInt32(0),
                            Question = reader.GetString(1),
                            Options = reader.GetString(2).Split(';').ToList(),
                            Answer = reader.GetString(3)
                        };
                        questions.Add(question);
                    }
                }
            }

            return questions;
        }
        
        [HttpPost]
        public ActionResult SubmitAnswer([FromBody] WrongAnswers wrongAnswerData)
        {
            var correct = CheckAnswer(wrongAnswerData.Id, wrongAnswerData.Answer);

            if (!correct)
            {
                InsertWrongAnswer(new WrongAnswers
                {
                    Id = wrongAnswerData.Id,
                    Question = wrongAnswerData.Question,
                    Options = wrongAnswerData.Options,
                    Answer = wrongAnswerData.Answer,
                   
                });
            }

            return Json(new { Correct = correct });
        }

        

        private bool CheckAnswer(int id, string selectedOption)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Answer FROM Questions WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                var answer = command.ExecuteScalar() as string;

                return string.Equals(answer, selectedOption, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
