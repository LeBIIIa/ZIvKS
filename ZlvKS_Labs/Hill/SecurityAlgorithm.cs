using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZlvKS_Labs.Hill
{
    public abstract class SecurityAlgorithm
    {
        public static readonly int MOD = 127;
        protected readonly Dictionary<char, int> alphabet;

        public SecurityAlgorithm()
        {
            alphabet = new Dictionary<char, int>();
            char c = (char)0;

            for (int i = 0; i < MOD; ++i)
                alphabet.Add(c++, i);
        }

        public abstract string Encrypt(string plainText);
        public abstract string Decrypt(string cipher);

        public abstract string Encrypt(string plainText, string key);
        public abstract string Decrypt(string cipher, string key);

        public abstract void Encrypt(StreamReader inputStream, StreamWriter outputStream);
        public abstract void Encrypt(StreamReader inputStream, StreamWriter outputStream, string key);

        public abstract void Decrypt(StreamReader inputStream, StreamWriter outputStream);
        public abstract void Decrypt(StreamReader inputStream, StreamWriter outputStream, string key);
    }
}
