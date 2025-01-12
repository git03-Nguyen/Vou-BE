using Shared.Domain;

namespace GameService.Data.Models;

public class PlayerQuizSession : BaseEntity
{
    public string QuizSessionId { get; set; }
    public string PlayerId { get; set; }
    public int Score { get; set; } = 0;
    public bool IsWin { get; set; } = false;
}