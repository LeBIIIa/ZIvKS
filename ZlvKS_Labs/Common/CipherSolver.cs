using System;
using System.IO;
using System.Text;

namespace ZlvKS_Labs
{
    public class CipherSolver
    {
        readonly StringBuilder sb;

        public CipherSolver()
        {
            sb = new StringBuilder();
        }

        public string RunAffine(string cipherText)
        {
            sb.Clear();
            sb.AppendLine("\n");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine("   AFFINE CIPHER");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine("");

            // create a Frequency Analysis object
            FrequencyAnalysis freqAnal = new FrequencyAnalysis();
            freqAnal.AverageChange += (string text) => sb.AppendLine(text);

            sb.AppendLine("// ########## ########## ########## ########## ########## ########## ########## ########## //");
            sb.AppendLine(FrequencyAnalysis.CalculatePrevStats(cipherText));
            sb.AppendLine("// ########## ########## ########## ########## ########## ########## ########## ########## //");
            sb.AppendLine();

            // iterate the possible shift caesars
            sb.AppendLine("\n -- [ITERATE KEYS] --");
            int[] coprimes = { 1, 3, 5, 7, 9, 11, 15, 17, 19, 21, 23, 25 };     // these are the possible values of a, as shown on the wiki
            foreach (int var_a in coprimes)
            {
                for (int i = 0; i < 26; i++)
                {
                    string resultant = Ciphers.DecipherAffine(cipherText, var_a, i);
                    freqAnal.AddOption(var_a.ToString() + "," + i.ToString(), resultant);
                    sb.AppendLine(string.Format("[OPTION:{0}:{1}]", var_a, i));
                    sb.AppendLine(resultant);
                }
            }

            // nice friendly lil' header for console output...
            sb.AppendLine("\n");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine("     LOWEST FREQUENCY DIFFERENCE");
            sb.AppendLine("       (most probable result)");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine("");

            string[] solution = freqAnal.Solution().Split(',');
            sb.AppendLine(string.Format("AFFINE KEY BEST FIT: a={0}, b={1}\n", solution[0], solution[1]));

            sb.AppendLine(Ciphers.DecipherAffine(cipherText, int.Parse(solution[0]), int.Parse(solution[1])));
            return sb.ToString();
        }
        public string RunCaesar(string cipherText)
        {
            sb.Clear();
            sb.AppendLine("\n");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine("     CAESAR SHIFT CIPHER");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine();

            sb.AppendLine("// ########## ########## ########## ########## ########## ########## ########## ########## //");
            sb.AppendLine(FrequencyAnalysis.CalculatePrevStats(cipherText));
            sb.AppendLine("// ########## ########## ########## ########## ########## ########## ########## ########## //");
            sb.AppendLine();

            // create a Frequency Analysis object
            FrequencyAnalysis freqAnal = new FrequencyAnalysis();
            freqAnal.AverageChange += (string text) => sb.AppendLine(text);

            // iterate the possible shift caesars
            sb.AppendLine("\n -- [ITERATE KEYS] --");
            for (int i = 0; i < 26; i++)
            {
                string resultant = Ciphers.DecipherCaesar(cipherText, i);
                freqAnal.AddOption(i.ToString(), resultant);
                sb.AppendLine(string.Format("[OPTION:{0}]", i));
                sb.AppendLine(resultant);
            }

            // nice friendly lil' header for console output...
            sb.AppendLine("\n");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine("     LOWEST FREQUENCY DIFFERENCE");
            sb.AppendLine("       (most probable result)");
            sb.AppendLine("// ########## ########## ########## //");
            sb.AppendLine("");

            sb.AppendLine(freqAnal.Solution());

            sb.AppendLine(Ciphers.DecipherCaesar(cipherText, int.Parse(freqAnal.Solution())));
            return sb.ToString();
        }
    }
}
