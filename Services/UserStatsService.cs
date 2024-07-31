using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public static class UserStatsService
    {
        private static readonly Dictionary<ulong, UserStats> _userStats = new Dictionary<ulong, UserStats>();

        public static UserStats GetUserStats(ulong userId)
        {
            if (!_userStats.ContainsKey(userId))
            {
                _userStats[userId] = new UserStats { UserId = userId };
            }
            return _userStats[userId];
        }

        public static void UpdateUserStats(ulong userId, bool won)
        {
            var stats = GetUserStats(userId);
            if (won)
            {
                stats.Wins++;
            }
            else
            {
                stats.Losses++;
            }
        }
    }
}