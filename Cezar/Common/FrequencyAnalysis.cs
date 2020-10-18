using System;
using System.Collections.Generic;
using System.Text;

namespace Cezar
{
    public class FrequencyAnalysis
    {
        public event Action<string> AverageChange;

        // define the standardised frequencies of characters in the English language
        private static readonly Dictionary<char, double> EngFrequencies = new Dictionary<char, double>() {
            { 'a', 8.1670 }, { 'b', 1.492 }, { 'c', 2.782 }, { 'd', 4.253 },
            { 'e', 12.702 }, { 'f', 2.228 }, { 'g', 2.015 }, { 'h', 6.094 },
            { 'i', 6.9660 }, { 'j', 0.153 }, { 'k', 0.772 }, { 'l', 4.025 },
            { 'm', 2.4060 }, { 'n', 6.749 }, { 'o', 7.507 }, { 'p', 1.929 },
            { 'q', 0.0950 }, { 'r', 5.987 }, { 's', 6.327 }, { 't', 9.056 },
            { 'u', 2.7580 }, { 'v', 0.978 }, { 'w', 2.361 }, { 'x', 0.150 },
            { 'y', 1.9740 }, { 'z', 0.074 }
        };

        private readonly Dictionary<string, int> Attempts = new Dictionary<string, int>();

        public static string CalculatePrevStats(string text)
        {
            StringBuilder sb = new StringBuilder(); 
            Dictionary<char, int> charCounter = new Dictionary<char, int>() 
            {
                { 'a', 0 }, { 'b', 0 }, { 'c', 0 }, { 'd', 0 },
                { 'e', 0 }, { 'f', 0 }, { 'g', 0 }, { 'h', 0 },
                { 'i', 0 }, { 'j', 0 }, { 'k', 0 }, { 'l', 0 },
                { 'm', 0 }, { 'n', 0 }, { 'o', 0 }, { 'p', 0 },
                { 'q', 0 }, { 'r', 0 }, { 's', 0 }, { 't', 0 },
                { 'u', 0 }, { 'v', 0 }, { 'w', 0 }, { 'x', 0 },
                { 'y', 0 }, { 'z', 0 }
            };
            int totalCharCount = 0;
            foreach (char c in text)
            {
                int charIndex = c;
                if ((charIndex >= 65 && charIndex <= 90) || (charIndex >= 97 && charIndex <= 122))
                    totalCharCount++;

                char newCharLower = char.ToLower(c);
                if (charCounter.ContainsKey(newCharLower))
                    charCounter[newCharLower]++;
            }
            foreach (KeyValuePair<char, int> item in charCounter)
            {
                double charPercentage = (100.00 / totalCharCount) * item.Value;
                double percentageDiff = ((EngFrequencies[item.Key] - charPercentage) / EngFrequencies[item.Key]) * 100;
                sb.AppendFormat("    Symbol: {0, 3}\t Text frequency {1,10:0.00}\t Standardised frequency {2,10:0.00}\t Diffrence {3,10:0.00}\r\n", item.Key, charPercentage, EngFrequencies[item.Key], percentageDiff);
            }
            return sb.ToString();
        }

        public void AddOption(string key, string text)
        {
            Dictionary<char, int> charCounter = new Dictionary<char, int>() {
                { 'a', 0 }, { 'b', 0 }, { 'c', 0 }, { 'd', 0 },
                { 'e', 0 }, { 'f', 0 }, { 'g', 0 }, { 'h', 0 },
                { 'i', 0 }, { 'j', 0 }, { 'k', 0 }, { 'l', 0 },
                { 'm', 0 }, { 'n', 0 }, { 'o', 0 }, { 'p', 0 },
                { 'q', 0 }, { 'r', 0 }, { 's', 0 }, { 't', 0 },
                { 'u', 0 }, { 'v', 0 }, { 'w', 0 }, { 'x', 0 },
                { 'y', 0 }, { 'z', 0 }
            };
            List<int> percentageDiffs = new List<int>();

            int totalCharCount = 0;
            foreach (char c in text)
            {
                int charIndex = c;
                if ((charIndex >= 65 && charIndex <= 90) || (charIndex >= 97 && charIndex <= 122))
                {
                    totalCharCount++;
                }

                char newCharLower = char.ToLower(c);
                if (charCounter.ContainsKey(newCharLower))
                {
                    charCounter[newCharLower]++;
                }
            }
            foreach (KeyValuePair<char, int> item in charCounter)
            {
                double charPercentage = (100.00 / totalCharCount) * item.Value;
                double percentageDiff = ((EngFrequencies[item.Key] - charPercentage) / EngFrequencies[item.Key]) * 100;
                percentageDiff = Math.Abs(percentageDiff);
                percentageDiffs.Add(Convert.ToInt32(percentageDiff));
            }
            int averageDiff = 0;
            foreach (int item in percentageDiffs)
            {
                averageDiff += item;
            }

            averageDiff /= 26;

            Attempts.Add(key, averageDiff);

            AverageChange?.Invoke(string.Format("!!!Difference {1}", key, averageDiff));
        }

        public string Solution()
        {
            string final_holder = "";
            int final_freq = -1;
            foreach (KeyValuePair<string, int> item in Attempts)
            {
                if (final_holder == "")
                {
                    final_holder = item.Key;
                    final_freq = item.Value;
                }
                else if (item.Value < final_freq)
                {
                    final_holder = item.Key;
                    final_freq = item.Value;
                }
            }
            return final_holder;
        }
    }
}
