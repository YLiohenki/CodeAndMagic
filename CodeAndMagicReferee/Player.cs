using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeAndMagicReferee
{
    public class Player
    {
        public Player()
        {
        }
        public StreamWriter writer;
        public StreamReader reader;
        public double playerHealth = 30;
        public double playerMana = 0;
        public double playerMaxMana = 0;
        public double playerDeckSize = 0;
        public double playerRune = 25;
        public List<Card> cards = new List<Card>();
        public double draw = 1;
        public String ToInputString()
        {
            StringBuilder s = new StringBuilder();

            s.Append(playerHealth).Append(" ");
            s.Append(playerMana).Append(" ");
            s.Append(playerDeckSize).Append(" ");
            s.Append(playerRune).Append(" ");
            return s.ToString();
        }

    }
}
