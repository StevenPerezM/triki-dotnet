using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tricky.Core.Entities.DBEntities;

namespace Tricky.Core.Business.Interfaces
{
    public interface IplayerService
    {
        Task CreatePlayer(Player player);
        Task<Player> ReadPlayer(string namePlayer);        
    }
}
