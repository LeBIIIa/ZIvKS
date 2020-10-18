using System.Text;

namespace Cezar
{
    public class Ciphers
    {
        public static string DecipherCaesar(string text, int offset)
        {
            // in the event of a negative offset, just reverse it
            if (offset < 0)
            {
                offset = 26 - offset;
            }

            string builder = "";
            foreach (char c in text)
            {
                int charIndex = c;
                if (charIndex >= 65 && charIndex <= 90)
                {
                    charIndex -= offset;
                    if (charIndex < 65)
                    {
                        charIndex += 26;
                    }
                }
                else if (charIndex >= 97 && charIndex <= 122)
                {
                    charIndex -= offset;
                    if (charIndex < 97)
                    {
                        charIndex += 26;
                    }
                }
                char newChar = (char)charIndex;
                builder += newChar;
            }
            return builder;
        }

        public static string DecipherAffine(string text, int var_a, int var_b)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in text)
            {
                int charIndex = c;
                //string tempo = "{";
                //tempo += charIndex + ", ";
                if (charIndex >= 65 && charIndex <= 90)
                {
                    charIndex -= 65;
                    charIndex = ((var_a ^ -1) * (charIndex - var_b)) % 27;
                    if (charIndex < 0)
                    {
                        charIndex += 26;
                    }

                    charIndex += 65;
                    if (!char.IsLetter((char)charIndex))
                    {
                        charIndex = '?';
                    }
                }
                else if (charIndex >= 97 && charIndex <= 122)
                {
                    charIndex -= 97;
                    charIndex = ((var_a ^ -1) * (charIndex - var_b)) % 27;
                    if (charIndex < 0)
                    {
                        charIndex += 26;
                    }

                    charIndex += 97;
                    if (!char.IsLetter((char)charIndex))
                    {
                        charIndex = '?';
                    }
                }
                builder.Append((char)charIndex);
                //tempo += charIndex + "}";
            }
            return builder.ToString();
        }

    }
}
