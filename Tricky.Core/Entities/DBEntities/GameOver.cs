using System;
using System.Collections.Generic;
using System.Text;

namespace Tricky.Core.Entities.DBEntities
{
    public class GameOver
    {
        public string id                            { get; set; }
        public DateTime startDate                   { get; set; }
        public string namePlayer1                   { get; set; }
        public string namePlayer2                   { get; set; }
        public int duration                         { get; set; }
        public int totalMovements                   { get; set; }
        public string[,] dataCurrentMovements       { get; set; }
        public string winner                        { get; set; }
        public GameOver()
        {
            dataCurrentMovements = new string[3, 3];
        }
    }
}
