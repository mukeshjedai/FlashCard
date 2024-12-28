using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MyMVCApp.Controllers
{
    public class HomeController : Controller
    {
        
        public class QuestionModel
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public string Answer { get; set; }
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
        public ActionResult SubmitAnswer(int id, string selectedOption)
        {
            var correct = CheckAnswer(id, selectedOption);
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
