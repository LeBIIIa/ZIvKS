using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZlvKS_Labs.Common
{
    public class FeistelCipher
    {
        #region Feistel
        public static byte Encrypt(byte msg, Func<byte, byte, byte> FunctionF, byte[] key)
        {
            byte step = msg;
            for (int i = 0; i < key.Length; i++)
            {
                step = FeistelStep(step, key[i], FunctionF);
            }

            return step;
        }
        public static byte Decrypt(byte msg, Func<byte, byte, byte> FunctionF, byte[] key)
        {
            byte step = msg;
            step = InversionLR(step);
            for (int i = key.Length - 1; i >= 0; i--)
            {
                step = FeistelStep(step, key[i], FunctionF);
            }
            step = InversionLR(step);

            return step;
        }
        public static byte FunctionF(byte x, byte key)
        {
            return Xor(x, key);
        }
        private static byte FeistelStep(byte msg, byte key, Func<byte, byte, byte> FunctionF)
        {
            var R = GetR(msg);
            var L = GetL(msg);

            var funcResult = OperateR(R, key, FunctionF);

            var xorResult = OperateL(L, funcResult, Xor);

            var finalResult = InversionLR(xorResult, R);

            return finalResult;
        }
        #endregion

        #region Bit Manipulation
        private static byte Xor(byte x, byte y)
        {
            return (byte)((int)x ^ (int)y);
        }
        private static byte GetR(byte x)
        {
            var temp = (byte)(((int)x) << 4);
            return (byte)(((int)temp) >> 4);
        }
        private static byte GetL(byte x)
        {
            var temp = (byte)(((int)x) >> 4);
            return (byte)(((int)temp) << 4);
        }
        private static byte InversionLR(byte l, byte r)
        {
            l = (byte)(((int)l) >> 4);
            r = (byte)(((int)r) << 4);
            return Xor(r, l);
        }
        private static byte InversionLR(byte msg)
        {
            var R = GetR(msg);
            var L = GetL(msg);
            return InversionLR(L, R);
        }
        private static byte OperateL(byte l, byte key, Func<byte, byte, byte> function)
        {
            key = (byte)(((int)key) << 4);
            return function(l, key);
        }
        private static byte OperateR(byte r, byte key, Func<byte, byte, byte> function)
        {
            return function(r, key);
        }
        #endregion

        #region Files
        public static byte[] EncryptFile(byte[] fileInBytes, string fileOutPath, Func<byte, byte, byte> FunctionF, string key)
        {
            byte[] encFile = new byte[fileInBytes.Length];
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < fileInBytes.Length; i++)
            {
                encFile[i] = Encrypt(fileInBytes[i], FunctionF, keyBytes);
            }
            File.WriteAllBytes(fileOutPath, encFile);
            return encFile;
        }
        public static byte[] DecryptFile(byte[] fileInBytes, string fileOutPath, Func<byte, byte, byte> FunctionF, string key)
        {
            byte[] decFile = new byte[fileInBytes.Length];
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < fileInBytes.Length; i++)
            {
                decFile[i] = Decrypt(fileInBytes[i], FunctionF, keyBytes);
            }
            File.WriteAllBytes(fileOutPath, decFile);
            return decFile;
        }
        #endregion
    }
}
