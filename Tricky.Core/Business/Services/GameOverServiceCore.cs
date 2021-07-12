using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Entities.DBEntities;

namespace Tricky.Core.Business.Services
{
    public class GameOverServiceCore
    {        
        private readonly IgameEndService _gameEndService;

        public GameOverServiceCore(IgameEndService gameEndService)
        {
            _gameEndService = gameEndService;
        }

        public async Task<CurrentGame> EndGame(CurrentGame currentGame)
        {
            await _gameEndService.CreateGameOver(BuildGameOver(currentGame));
            return currentGame;
        }

        public GameOver BuildGameOver(CurrentGame currentGame)
        {
            return new GameOver
            {
                id = Guid.NewGuid().ToString(),
                startDate = currentGame.startDate,
                namePlayer1 = currentGame.player1.namePlayer,
                namePlayer2 = currentGame.player2.namePlayer,
                duration = 300,                 
                totalMovements = (currentGame.player1.movements.Count + currentGame.player2.movements.Count),
                dataCurrentMovements = currentGame.dataCurrentMovements,
                winner = currentGame.winner
            };
        }
    }
}
