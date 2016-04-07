using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaWinRate
{
    static class ListExtensions
    {
        public static IList<T> FastRemove<T>(this IList<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return list;
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[Program.rand.Next(list.Count)];
        }
    }
    class Player
    {
        public void ChangeWinLoss(int newWins, int newLosses)
        {
            var currentList = Program.currentPlayers[wins, losses];
            var newList = Program.currentPlayers[newWins, newLosses];
            currentList.FastRemove(listIndex);
            if (listIndex < currentList.Count)
            {
                currentList[listIndex].listIndex = listIndex;
            }

            newList.Add(this);
            listIndex = newList.Count - 1;

            wins = newWins;
            losses = newLosses;
        }

        public void Reset()
        {
            ChangeWinLoss(0, 0);
            ability = Program.rand.Next(100);
        }

        public void Retire()
        {
            Program.totalWins += wins;
            Program.totalRuns++;
            Reset();
        }

        public void Win()
        {
            if (wins < 11)
            {
                ChangeWinLoss(wins + 1, losses);
            }
            else
            {
                Program.total12Wins++;
                Retire();
            }
        }

        public void Lose()
        {
            if (losses < 2)
            {
                ChangeWinLoss(wins, losses + 1);
            }
            else
            {
                Retire();
            }
        }

        public int wins = 0;
        public int losses = 0;
        public int ability = Program.rand.Next(100);
        public int listIndex = 0;
        public List<Player> currentList
        {
            get
            {
                return Program.currentPlayers[wins, losses];
            }
        }
    }

    class Program
    {
        public const int TOTAL_PLAYERS = 100000;
        public const long TOTAL_GAMES = 100000000;

        public static Random rand = new Random();
        public static List<Player> allPlayers = new List<Player>();
        public static List<Player>[,] currentPlayers = new List<Player>[12,3];
        public static int totalWins = 0;
        public static int totalRuns = 0;
        public static int total12Wins = 0;

        static void Main(string[] args)
        {
            for (int wins = 0; wins < 12; wins++)
            {
                for (int losses = 0; losses < 3; losses++)
                {
                    currentPlayers[wins, losses] = new List<Player>(TOTAL_PLAYERS);
                }
            }

            // Populate starting pool of players
            allPlayers.Capacity = TOTAL_PLAYERS;
            for (int i = 0; i < TOTAL_PLAYERS; i++)
            {
                Player newPlayer = new Player();
                currentPlayers[0,0].Add(newPlayer);
                newPlayer.listIndex = currentPlayers[0, 0].Count - 1;
                allPlayers.Add(newPlayer);
            }

            // Let's play lots of games
            for (long i = 0; i < TOTAL_GAMES; i++)
            {
                if (i % (TOTAL_GAMES / 100) == 0)
                {
                    Console.Out.WriteLineAsync(String.Format("{0}% complete", i / (TOTAL_GAMES / 100)));
                }
                Player player1 = allPlayers.RandomElement();
                while (player1.currentList.Count == 1)
                {
                    player1 = allPlayers.RandomElement();
                }
                Player player2 = player1;
                while (player1 == player2)
                {
                    player2 = player1.currentList.RandomElement();
                }

                int player1Val = rand.Next(20) + player1.ability;
                int player2Val = rand.Next(20) + player2.ability;
                if (player1Val == player2Val)
                {
                    player1Val += rand.Next(2) == 1 ? 1 : -1;
                }
                if (player1Val > player2Val)
                {
                    player1.Win();
                    player2.Lose();
                }
                else if (player1Val > player2Val)
                {
                    player1.Lose();
                    player2.Win();
                }
            }

            // Print average # wins
            Console.Out.WriteLine(String.Format("Total Wins: {0}", totalWins));
            Console.Out.WriteLine(String.Format("Total Runs: {0}", totalRuns));
            Console.Out.WriteLine(String.Format("Total 12 Win Runs: {0}", total12Wins));
            Console.Out.WriteLine(String.Format("Average Wins Per Run: {0}", (double)totalWins / (double)totalRuns));
        }
    }
}
