using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Entities.DBEntities;
using Tricky.Core.Entities.Exceptions;
using Tricky.Core.Entities.Response;

namespace Tricky.Core.Business.Services
{
    public class PlayerServiceCore
    {
        private readonly IplayerService _playerService;

        public PlayerServiceCore(IplayerService playerService)
        {
            _playerService = playerService;
        }

        public async Task<ResponseGeneral> CreatePlayer(Player player)
        {
            try
            {
                player.id = Guid.NewGuid().ToString();                
                await _playerService.CreatePlayer(player);
                ResponseGeneral response = new ResponseGeneral();
                response.message = "Ok";
                response.description = "Successful player creation";
                response.id = player.id;
                response.namePlayer = player.namePlayer;
                return response;
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error while creating the player");
                throw;
            }
        }        

        public async Task<Player> Readplayer(string namePlayer)
        {
            try
            {
                Player player = await _playerService.ReadPlayer(namePlayer);
                if (player != null)
                {
                    return player;
                }
                else
                    throw new GameTrickyException("It is no posible to get the specified player, because this not exists in the data base");
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error getting the player from the database");
                throw;
            }
        }
    }
}
