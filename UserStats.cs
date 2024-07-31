using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class UserStats
    {
        public ulong UserId { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }

}