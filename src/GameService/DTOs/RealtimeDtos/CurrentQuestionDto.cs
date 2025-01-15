namespace GameService.DTOs.RealtimeDtos;

public class CurrentQuestionDto
{
    public string Content { get; set; }
    public DateTime QuestionTime { get; set; }
    public int AnswerIndex { get; set; }
}