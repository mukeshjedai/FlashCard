namespace MyMvcApp.Models;

using System.Collections.Generic;

public class WrongAnswers
{

    public int Id { get; set; }

    public string QuestionText { get; set; }

    public List<string> Options { get; set; }

    public string Answer { get; set; }
    
    public string SoftDelete { get; set; }



    public WrongAnswers()
    {

        Options = new List<string>(); 

    }



    public WrongAnswers(int id, string questionText, List<string> options, string answer, string softDelete)

    {

        Id = id;

        QuestionText = questionText;

        Options = options;

        Answer = answer;
        SoftDelete = softDelete;
        
    }

}


