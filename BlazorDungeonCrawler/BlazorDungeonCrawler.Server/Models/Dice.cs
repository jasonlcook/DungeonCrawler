﻿using System.Text.RegularExpressions;

namespace BlazorDungeonCrawler.Server.Models {
    public static class Dice {
        public static int RandomNumber(int min, int max) {
            Guid guid = Guid.NewGuid();
            string guidDigits = Regex.Replace(guid.ToString(), @"\D0*", "");
            int number = int.Parse(guidDigits.Substring(0, 8));
            Random rnd = new Random(number);

            return rnd.Next(min, max + 1);
        }

        public static int RollDSix() {
            return RandomNumber(1, 6);
        }

    }
}