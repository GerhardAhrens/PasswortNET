//-----------------------------------------------------------------------
// <copyright file="StringCompareHelper.cs" company="Lifeprojects.de">
//     Class: StringCompareHelper
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>11.02.2025 12:24:21</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows.Documents;

    public static class StringHelper
    {
        public static string InitialLetters(string name)
        {
            string result = string.Concat(name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Length >= 1 && char.IsLetter(x[0])).Select(x => char.ToUpper(x[0])));
            return result;
        }

        public static double StringMatchByLength(string a, string b)
        {
            if (a == b) //Same string, no iteration needed.
            {
                return 100;
            }

            if ((a.Length == 0) || (b.Length == 0))
            {
                return 0;
            }

            double maxLen = a.Length > b.Length ? a.Length : b.Length;
            int minLen = a.Length < b.Length ? a.Length : b.Length;
            int sameCharAtIndex = 0;
            for (int i = 0; i < minLen; i++) //Compare char by char
            {
                if (a[i] == b[i])
                {
                    sameCharAtIndex++;
                }
            }

            return sameCharAtIndex / maxLen * 100;
        }

        public static double StringMatchByPercentage(string a, string b)
        {
            List<string> pairs1 = WordLetterPairs(a.ToUpper());
            List<string> pairs2 = WordLetterPairs(b.ToUpper());

            int intersection = 0;
            int union = pairs1.Count + pairs2.Count;

            for (int i = 0; i < pairs1.Count; i++)
            {
                for (int j = 0; j < pairs2.Count; j++)
                {
                    if (pairs1[i] == pairs2[j])
                    {
                        intersection++;
                        pairs2.RemoveAt(j); //Must remove the match to prevent "AAAA" from appearing to match "AA" with 100% success
                        break;
                    }
                }
            }

            return (2.0 * intersection * 100) / union; //returns in percentage
                                                       //return (2.0 * intersection) / union; //returns in score from 0 to 1        }
        }

        private static List<string> WordLetterPairs(string str)
        {
            List<string> AllPairs = new List<string>();

            // Tokenize the string and put the tokens/words into an array
            string[] Words = Regex.Split(str, @"\s");

            // For each word
            for (int w = 0; w < Words.Length; w++)
            {
                if (!string.IsNullOrEmpty(Words[w]))
                {
                    // Find the pairs of characters
                    String[] PairsInWord = LetterPairs(Words[w]);

                    for (int p = 0; p < PairsInWord.Length; p++)
                    {
                        AllPairs.Add(PairsInWord[p]);
                    }
                }
            }

            return AllPairs;
        }

        private static string[] LetterPairs(string str)
        {
            int numPairs = str.Length - 1;
            string[] pairs = new string[numPairs];

            for (int i = 0; i < numPairs; i++)
            {
                pairs[i] = str.Substring(i, 2);
            }
            return pairs;
        }
    }
}
