using MediatR;
using Shared.Response;

namespace GameService.Features.Queries.PlayerQueries.GetEventPlayerState;

public class GetEventPlayerStateQuery : IRequest<BaseResponse<EventPlayerStateDto>>
{
    public string EventId { get; set; } = string.Empty;
}

public class EventPlayerStateDto
{
    public string EventId { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public int Tickets { get; set; }
    public int Diamonds { get; set; }
    public DateTime? LastShareTime { get; set; }
    public DateTime? NextResetTicketsTime { get; set; }
    public int QuizSessionsPlayed { get; set; }
    public int QuizWins { get; set; }
    public int HighestQuizScore { get; set; }
    public int TotalQuizScore { get; set; }
    public DateTime? LastPlayedAt { get; set; }
    public List<PlayerQuizHistoryItemDto> RecentQuizHistory { get; set; } = new();
}

public class PlayerQuizHistoryItemDto
{
    public string QuizSessionId { get; set; } = string.Empty;
    public string QuizSetId { get; set; } = string.Empty;
    public string? QuizSetTitle { get; set; }
    public int Score { get; set; }
    public bool IsWin { get; set; }
    public DateTime? PlayedAt { get; set; }
}
