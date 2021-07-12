using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Entities.DBEntities;
using Tricky.Core.Entities.Exceptions;
using Tricky.Core.Entities.Response;
using Tricky.Core.Services.Data.CurrentGameService;
using Tricky.Core.Utilities;

namespace Tricky.Core.Business.Services
{
    public class CurrentGameServiceCore
    {
        private readonly CurrentGameService _currentGameService;
        private readonly IQueueService _queueService;
        private readonly string EndpointSqs = "https://sqs.us-east-1.amazonaws.com/214697756788/SaveGameEnd";

        public CurrentGameServiceCore(CurrentGameService currentGameService, IQueueService queueService)
        {
            _currentGameService = currentGameService;
            _queueService = queueService;
        }

        public CurrentGameServiceCore()
        {
        }

        public async Task<CurrentGame> CreateGame(CurrentGame currentGame)
        {
            try
            {

                currentGame.id = Guid.NewGuid().ToString();
                currentGame.startDate = DateTime.UtcNow;
                currentGame = BuildCurrentGame(currentGame);
                await _currentGameService.Set(currentGame.id, JsonConvert.SerializeObject(currentGame), TimeSpan.FromSeconds(300));
                return currentGame;
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error while creating start Game");
                throw;
            }
        }

        public async Task<CurrentGame> GetCurrentGame(string id)
        {
            try
            {
                if (await _currentGameService.Exists(id))
                {
                    CurrentGame currentGame = JsonConvert.DeserializeObject<CurrentGame>((await _currentGameService.Get(id)).ToString());
                    return currentGame;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error while add movements");
                throw;
            }
        }

        private CurrentGame BuildCurrentGame(CurrentGame currentGame)
        {
            currentGame.status = "PLAYING";
            currentGame.turn = GetFirtsTurn();
            if (currentGame.turn == "player1")
            {
                currentGame.player1.firstMovement = true;
            }
            else
            {
                currentGame.player2.firstMovement = true;
            }
            currentGame.player1.indicator = "x";
            currentGame.player2.indicator = "o";
            return currentGame;
        }

        private string GetFirtsTurn()
        {
            Random r = new Random();
            int num = r.Next(1, 3);
            string response = $"player{num}";
            return response;
        }

        public async Task<CurrentGame> AddMovementsCurrentGame(AddMovement addMovement)
        {
            try
            {
                if (await _currentGameService.Exists(addMovement.idGame))
                {
                    CurrentGame currentGame = JsonConvert.DeserializeObject<CurrentGame>((await _currentGameService.Get(addMovement.idGame)).ToString());
                    if (currentGame.status == "GAMEOVER")
                    {
                        return currentGame;
                    }
                    if (currentGame.turn == "player1")
                    {
                        currentGame.player1.movements.Add($"{addMovement.movementx},{addMovement.movementy}");
                        currentGame.dataCurrentMovements = AddListMovements(currentGame, addMovement, currentGame.player1.indicator);
                        if (ValidateEndGame(currentGame.dataCurrentMovements))
                        {
                            currentGame.winner = currentGame.turn;
                            currentGame.status = "GAMEOVER";
                            //await _currentGameService.Set(currentGame.id, JsonConvert.SerializeObject(currentGame), TimeSpan.FromSeconds(60));                            
                             _queueService.sendMessageAsync(currentGame, EndpointSqs, 0);
                            return currentGame;
                        }
                        currentGame.turn = "player2";
                    }
                    else
                    {
                        currentGame.player2.movements.Add($"{addMovement.movementx},{addMovement.movementy}");
                        currentGame.dataCurrentMovements = AddListMovements(currentGame, addMovement, currentGame.player2.indicator);
                        if (ValidateEndGame(currentGame.dataCurrentMovements))
                        {
                            currentGame.winner = currentGame.turn;
                            currentGame.status = "GAMEOVER";
                            //await _currentGameService.Set(currentGame.id, JsonConvert.SerializeObject(currentGame), TimeSpan.FromSeconds(60));
                            _queueService.sendMessageAsync(currentGame, EndpointSqs, 0);
                            return currentGame;
                        }
                        currentGame.turn = "player1";
                    }
                    await _currentGameService.Set(currentGame.id, JsonConvert.SerializeObject(currentGame), TimeSpan.FromSeconds(300));
                    return currentGame;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error while add movements");
                throw;
            }
        }
        private string[,] AddListMovements(CurrentGame currentGame, AddMovement addMovement, string indicator)
        {
            string[,] movements = new string[3, 3];
            movements = currentGame.dataCurrentMovements;
            if (!string.IsNullOrEmpty(movements[addMovement.movementx, addMovement.movementy]))
            {
                throw new GameTrickyException("Position is not empty");
            }
            if (addMovement.movementx >= 3 && addMovement.movementx >= 3)
            {
                throw new GameTrickyException("Position not exist");
            }
            movements[addMovement.movementx, addMovement.movementy] = indicator;
            return movements;
        }

        public bool ValidateEndGame(string[,] matrix)
        {
            bool validation = false;
            ResponseGeneral response = new ResponseGeneral();
            if (MatrixUtils.IsGameOver(matrix))
            {
                return true;
            }
            return validation;
        }       
    }
}
