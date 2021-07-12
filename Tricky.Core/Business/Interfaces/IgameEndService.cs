using System.Threading.Tasks;
using Tricky.Core.Entities.DBEntities;

namespace Tricky.Core.Business.Interfaces
{
    public interface IgameEndService
    {
        Task CreateGameOver(GameOver gameOver);
    }
}
