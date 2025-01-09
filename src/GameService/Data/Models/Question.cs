namespace GameService.Data.Models;

public class Question
{
    public string Text { get; set; }
    public int AnswerIndex { get; set; }
    public string[] Answers { get; set; }
}