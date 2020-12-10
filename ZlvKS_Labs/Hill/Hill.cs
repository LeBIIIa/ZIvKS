using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZlvKS_Labs.Hill
{
    public class Hill : SecurityAlgorithm
    {
        public enum SymbolType
        {
            Character,
            Mask
        }

        readonly int rank;
        int[,] key;
        readonly SymbolType[] template;
        readonly int countOT;



        public Hill(int[,] matrix)
        {
            key = matrix;
        }
        public Hill(SymbolType[] template, int rank)
        {
            if (template.Length != rank)
                throw new ArgumentException("Incorrect size of template pattern!");
            if (!template.Contains(SymbolType.Character))
                throw new ArgumentException("Must be at least 1 character!");
            this.rank = rank;
            this.template = template;
            countOT = template.Count(s => s == SymbolType.Character);
        }
        public Hill(string key, SymbolType[] template, int rank)
        {
            if (string.IsNullOrEmpty(key) && key.Length % rank != 0)
                throw new ArgumentException("Incorrect rank for this key or key is empty!");
            if (template.Length != rank)
                throw new ArgumentException("Incorrect size of template pattern!");
            if (!template.Contains(SymbolType.Character))
                throw new ArgumentException("Must be at least 1 character!");
            this.rank = rank;
            this.key = GetKeyMatrix(key, rank);
            this.template = template;
            countOT = template.Count(s => s == SymbolType.Character);
        }
        private int[,] GetKeyMatrix(string key, int rank)
        {
            int[,] keyMatrix = new int[rank, rank];
            int k = 0;
            for (int i = 0; i < rank; ++i)
            {
                for (int j = 0; j < rank; ++j)
                {
                    keyMatrix[i, j] = alphabet[key[k++]];
                }
            }
            return keyMatrix;
        }

        #region Public Methods

        public override string Encrypt(string plainText)
        {
            if (plainText.Length % countOT != 0)
                plainText = string.Concat(plainText, "-");
            MatrixClass matrix = new MatrixClass(key);
            int charPosition;
            StringBuilder result = new StringBuilder();
            int matrixSize = key.GetLength(0);
            Random random = new Random();

            for (int r = 0; r < plainText.Length / countOT; ++r)
            {
                int[] temp = new int[3];
                int posSubstring = 0;
                for (int k = 0; k < matrixSize; ++k)
                {
                    if (template[k] == SymbolType.Character)
                        temp[k] = alphabet[plainText[(r * countOT) + posSubstring++]];
                    else
                        temp[k] = alphabet[(char)random.Next(0, alphabet.Count)];
                }

                for (int i = 0; i < matrixSize; ++i)
                {
                    charPosition = 0;

                    for (int j = 0; j < matrixSize; ++j)
                    {
                        charPosition += (int)matrix[j, i].Numerator * temp[j];
                    }

                    result.Append(alphabet.Keys.ElementAt(charPosition % alphabet.Count));
                }
            }

            return result.ToString();
        }
        public override string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrEmpty(key) && key.Length % rank != 0)
                throw new ArgumentException("Incorrect rank for this key or key is empty!");
            this.key = GetKeyMatrix(key, rank); 
            return Encrypt(plainText);
        }

        public override void Encrypt(StreamReader inputStream, StreamWriter outputStream)
        {
            MatrixClass matrix = new MatrixClass(key);
            int charPosition;
            int matrixSize = key.GetLength(0);
            Random random = new Random();

            char b;
            int countBytes = 0;
            char[] temp = new char[3];
            int k = 0;

            while (true)
            {
                if (k < matrixSize)
                {
                    if (template[k] == SymbolType.Character)
                    {
                        b = (char)inputStream.Read(); 
                        ++countBytes;
                        if (b != char.MaxValue)
                            temp[k] = b;
                        else if (b == char.MaxValue && countBytes % countOT != 0)
                            temp[k] = '-';
                        else
                            break;
                    }
                    else if (template[k] == SymbolType.Mask)
                    {
                        temp[k] = (char)random.Next(0, alphabet.Count);
                    }
                    ++k;
                    continue;
                }

                for (int i = 0; i < matrixSize; ++i)
                {
                    charPosition = 0;

                    for (int j = 0; j < matrixSize; ++j)
                    {
                        charPosition += (int)matrix[j, i].Numerator * temp[j];
                    }

                    outputStream.Write(alphabet.Keys.ElementAt(charPosition % alphabet.Count));
                }
                k = 0;
            }

            outputStream.Flush();
        }
        public override void Encrypt(StreamReader inputStream, StreamWriter outputStream, string key)
        {
            if (string.IsNullOrEmpty(key) && key.Length % rank != 0)
                throw new ArgumentException("Incorrect rank for this key or key is empty!");
            this.key = GetKeyMatrix(key, rank);
            Encrypt(inputStream, outputStream);
        }

        public override string Decrypt(string cipher)
        {
            MatrixClass matrix = new MatrixClass(key);
            matrix = matrix.Inverse();

            int charPosition;
            StringBuilder result = new StringBuilder();
            int matrixSize = key.GetLength(0);

            for (int r = 0; r < cipher.Length / matrixSize; ++r)
            {
                for (int i = 0; i < matrixSize; ++i)
                {
                    charPosition = 0;

                    for (int j = 0; j < matrixSize; ++j)
                    {
                        charPosition += (int)matrix[j, i].Numerator * alphabet[cipher[(r * matrixSize) + j]];
                    }

                    if (template[i] == SymbolType.Character)
                        result.Append(alphabet.Keys.ElementAt(charPosition % alphabet.Count));
                }
            }

            return result.ToString();
        }
        public override string Decrypt(string cipher, string key)
        {
            if (string.IsNullOrEmpty(key) && key.Length % rank != 0)
                throw new ArgumentException("Incorrect rank for this key or key is empty!");
            this.key = GetKeyMatrix(key, rank);
            return Decrypt(cipher);
        }
        
        public override void Decrypt(StreamReader inputStream, StreamWriter outputStream)
        {
            MatrixClass matrix = new MatrixClass(key);
            matrix = matrix.Inverse();

            int charPosition;
            int matrixSize = key.GetLength(0);

            int b;
            char[] temp = new char[3];
            int k = 0;
            while ((b = inputStream.Read()) != -1)
            {
                if (k < matrixSize)
                {
                    temp[k++] = (char)b;
                    continue;
                }
                k = 0;
                for (int i = 0; i < matrixSize; ++i)
                {
                    charPosition = 0;

                    for (int j = 0; j < matrixSize; ++j)
                    {
                        charPosition += (int)matrix[j, i].Numerator * alphabet[temp[j]];
                    }

                    if (template[i] == SymbolType.Character)
                        outputStream.Write(alphabet.Keys.ElementAt(charPosition % alphabet.Count));
                }
            }
            outputStream.Flush();
        }
        public override void Decrypt(StreamReader inputStream, StreamWriter outputStream, string key)
        {
            if (string.IsNullOrEmpty(key) && key.Length % rank != 0)
                throw new ArgumentException("Incorrect rank for this key or key is empty!");
            this.key = GetKeyMatrix(key, rank);
            Decrypt(inputStream, outputStream);
        }

        #endregion
    }
}
