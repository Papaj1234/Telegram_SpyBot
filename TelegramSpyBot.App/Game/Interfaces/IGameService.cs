using TelegramSpyBot.App.Game.Models;

namespace TelegramSpyBot.App.Game.Interfaces;

public interface IGameService
{
    Task<GameStartResult?> StartGameAsync(Lobby lobby);
}