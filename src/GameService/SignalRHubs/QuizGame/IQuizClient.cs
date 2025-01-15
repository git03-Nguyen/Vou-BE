using GameService.DTOs;
using GameService.DTOs.RealtimeDtos;

namespace GameService.SignalRHubs.QuizGame;

public interface IQuizClient
{
    // On any player join or leave
    Task WaitingPlayers(IEnumerable<WaitingPlayerDto> players);
    
    // On new question, 10s
    Task NewQuestion(string id, string content, string[] answers);
    
    // On question result
    Task Result(string id, int score);
    
    // On game end
    Task GameEnd(int totalScore, VoucherDto? voucher);
}