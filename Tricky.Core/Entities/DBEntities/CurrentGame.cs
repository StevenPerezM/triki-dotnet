using System;
using System.Collections.Generic;
using System.Text;

namespace Tricky.Core.Entities.DBEntities
{
    public class CurrentGame
    {
        public string id                                        { get; set; }
        public DateTime startDate                               { get; set; }
        public string turn                                      { get; set; }
        public string status                                    { get; set; }
        public DataPlayer player1                               { get; set; }
        public DataPlayer player2                               { get; set; }
        public string[,] dataCurrentMovements                   { get; set; }
        public string winner                                    { get; set; }
        public CurrentGame()
        {
            dataCurrentMovements = new string[3, 3];
        }
    }
}
