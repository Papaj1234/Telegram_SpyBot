using TelegramSpyBot.App.Game.Interfaces;
using TelegramSpyBot.App.Game.Models;
using TelegramSpyBot.Data.Models;
using TelegramSpyBot.Data.UnitOfWork;
using DataPlayer = TelegramSpyBot.Data.Models.Player;
using DataGameStatus = TelegramSpyBot.Data.Models.GameStatus;

namespace TelegramSpyBot.App.Game;

public class GameService : IGameService
{
    private readonly IUnitOfWork unitOfWork;

    public GameService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<GameStartResult?> StartGameAsync(Lobby lobby)
    {
        var character = await unitOfWork.Characters
            .GetRandomByUniverseAsync(lobby.UniverseId);

        if (character is null)
        {
            return null;
        }

        var players = lobby.Players.ToList();
        var spyIndex = Random.Shared.Next(players.Count);

        var session = new GameSession
        {
            ChatId = lobby.ChatId,
            CharacterId = character.Id,
            Status = DataGameStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var roles = new List<PlayerRole>();

        for (var i = 0; i < players.Count; i++)
        {
            var isSpy = i == spyIndex;
            var lobbyPlayer = players[i];

            session.Players.Add(new DataPlayer
            {
                TelegramUserId = lobbyPlayer.TelegramUserId,
                Username = lobbyPlayer.Username,
                FirstName = lobbyPlayer.FirstName,
                IsSpy = isSpy
            });

            roles.Add(new PlayerRole
            {
                TelegramUserId = lobbyPlayer.TelegramUserId,
                IsSpy = isSpy
            });
        }

        await unitOfWork.GameSessions.AddAsync(session);
        await unitOfWork.SaveChangesAsync();

        return new GameStartResult
        {
            CharacterName = character.Name,
            CharacterImageUrl = character.ImageUrl,
            Roles = roles
        };
    }
}