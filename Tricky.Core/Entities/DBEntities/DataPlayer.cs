using System.Collections.Generic;

namespace Tricky.Core.Entities.DBEntities
{
    public class DataPlayer
    {
        public string id                { get; set; }
        public string namePlayer        { get; set; }
        public bool firstMovement       { get; set; }
        public string indicator         { get; set; }
        public List<string> movements   { get; set; }

        public DataPlayer()
        {
            movements = new List<string>();
        }
    }
}
