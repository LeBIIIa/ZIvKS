using ZlvKS_Labs.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using ZlvKS_Labs.Hill;

namespace ZlvKS_Labs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            doJob2 = TranspositionСipher.Encipher;

            solver = new CipherSolver();
            doSolver = solver.RunCaesar;
        }

        #region Lab1
        private readonly string[] lower = new string[]
        {
            "а", "б", "в", "г", "ґ", "д", "е", "є",
            "ж", "з", "и", "і", "ї", "й",
            "к", "л", "м", "н", "о", "п",
            "р", "с", "т", "у", "ф", "х",
            "ц", "ч", "ш", "щ", "ь", "ю", "я"
        };
        private readonly string[] upper = new string[]
        {
            "А", "Б", "В", "Г", "Ґ", "Д", "Е", "Є",
            "Ж", "З", "И", "І", "Ї", "Й",
            "К", "Л", "М", "Н", "О", "П",
            "Р", "С", "Т", "У", "Ф", "Х",
            "Ц", "Ч", "Ш", "Щ", "Ь", "Ю", "Я"
        };
        private int step;
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void BtnProcess_Click(object sender, EventArgs e)
        {
            if (step == 0)
            {
                MessageBox.Show("Введіть крок!");
                return;
            }
            if (string.IsNullOrEmpty(tbMessage.Text.Trim()))
            {
                MessageBox.Show("Введіть повідомлення!");
                return;
            }
            try
            {
                string[] lower2 = lower.Skip(step).Concat(lower.Take(step)).ToArray();
                string[] upper2 = upper.Skip(step).Concat(upper.Take(step)).ToArray();
                string text = tbMessage.Text;
                StringBuilder sb = new StringBuilder();
                int i, j;
                CultureInfo culture = new CultureInfo("uk-UA");
                for (j = 0; j < text.Length; ++j)
                {
                    for (i = 0; i < lower2.Length; ++i)
                    {
                        if (string.Compare(text[j].ToString(), lower[i], false, culture) == 0)
                        {
                            sb.Append(lower2[i]);
                            break;
                        }
                        if (string.Compare(text[j].ToString(), upper[i], false, culture) == 0)
                        {
                            sb.Append(upper2[i]);
                            break;
                        }
                    }
                    if (i == lower2.Length)
                    {
                        sb.Append(text[j]);
                    }
                }
                tbResult.Text = sb.ToString();
            }
            catch
            {
                MessageBox.Show("Не вдалось зашифрувати повідомлення!");
            }
        }
        private void NumStep_ValueChanged(object sender, EventArgs e)
        {
            step = (int)numStep.Value;
        }
        #endregion

        #region Lab2
        private Func<string, string, string> doJob2;
        private void BtnProcess2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbMessage2.Text.Trim()))
                {
                    MessageBox.Show("Введіть повідомлення!");
                    return;
                }
                if (string.IsNullOrEmpty(tbKey2.Text.Trim()))
                {
                    MessageBox.Show("Введіть ключ!");
                    return;
                }
                string input = tbMessage2.Text;
                string key = tbKey2.Text;
                tbResult2.Text = doJob2.Invoke(input, key);
            }
            catch
            {
                MessageBox.Show("Не вдалось зашифрувати/розшифрувати повідомлення!");
            }
        }

        private void ChkbCrypto2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbCrypto2.Checked)
            {
                doJob2 = TranspositionСipher.Encipher;
            }
            else
            {
                doJob2 = TranspositionСipher.Decipher;
            }
        }
        #endregion

        #region Lab3
        Func<string, string> doSolver;
        readonly CipherSolver solver;
        
        private void BtnProcess3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbMessage3.Text.Trim()))
                {
                    MessageBox.Show("Введіть повідомлення!");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("\n");
                sb.AppendLine("// ########## ########## ########## //");
                sb.AppendLine("     ASSIGNMENT 1 :: SHIFT SOLVER");
                sb.AppendLine("// ########## ########## ########## //");
                sb.AppendLine("");

                sb.Append(doSolver.Invoke(tbMessage3.Text));
                tbResult3.Text = sb.ToString();
            }
            catch
            {
                MessageBox.Show("Не вдалось здійснити аналіз!");
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCaesar.Checked)
            {
                doSolver = solver.RunCaesar;
            }
            else if (rbAffine.Checked)
            {
                doSolver = solver.RunAffine;
            }
        }
        #endregion

        #region Lab4
        private BitArray ByteToBit(byte src)
        {
            BitArray bitArray = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                bool st;
                if ((src >> i & 1) == 1)
                {
                    st = true;
                }
                else st = false;
                bitArray[i] = st;
            }
            return bitArray;
        }
        private byte BitToByte(BitArray scr)
        {
            byte num = 0;
            for (int i = 0; i < scr.Count; i++)
                if (scr[i] == true)
                    num += (byte)Math.Pow(2, i);
            return num;
        }

        /// <summary>
        /// Перевіряє зашифрований файл, повертає true, якщо символ в першому пікслеле дорівнює / інакше false
        /// </summary>
        /// <param name="scr"></param>
        /// <returns></returns>
        private bool IsEncryption(Bitmap scr)
        {
            byte[] rez = new byte[1];
            Color color = scr.GetPixel(0, 0);
            BitArray colorArray = ByteToBit(color.R); //отримуємо байт кольору і перетворюємо в бітовий масив
            BitArray messageArray = ByteToBit(color.R); ;//створюємо результуючий бітовий масив
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = ByteToBit(color.G);//отримуємо байт кольору і перетворимо в бітовий масив
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = ByteToBit(color.B);//отримуємо байт кольору і перетворимо в бітовий масив
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            rez[0] = BitToByte(messageArray); //отримуємо байт символу, записаного в 1 пікселі
            string m = Encoding.UTF8.GetString(rez);
            if (m == "/")
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Записує кількість символів для шифрування в перші біти картинки
        /// </summary>
        /// <param name="count"></param>
        /// <param name="src"></param>
        private void WriteCountText(int count, Bitmap src)
        {
            byte[] CountSymbols = Encoding.UTF8.GetBytes(count.ToString().PadLeft(3, '0'));
            for (int i = 0; i < 3; i++)
            {
                BitArray bitCount = ByteToBit(CountSymbols[i]); //біти кількості символів
                Color pColor = src.GetPixel(0, i + 1); //1, 2, 3 пікселі
                BitArray bitsCurColor = ByteToBit(pColor.R); //біт кольорів поточного пікселя
                bitsCurColor[0] = bitCount[0];
                bitsCurColor[1] = bitCount[1];
                byte nR = BitToByte(bitsCurColor); //новий біт кольору пікселя

                bitsCurColor = ByteToBit(pColor.G);//біт кольорів поточного пікселя
                bitsCurColor[0] = bitCount[2];
                bitsCurColor[1] = bitCount[3];
                bitsCurColor[2] = bitCount[4];
                byte nG = BitToByte(bitsCurColor);//новий колір пікселя

                bitsCurColor = ByteToBit(pColor.B);//біт кольорів поточного пікселя
                bitsCurColor[0] = bitCount[5];
                bitsCurColor[1] = bitCount[6];
                bitsCurColor[2] = bitCount[7];
                byte nB = BitToByte(bitsCurColor);//новий колір пікселя

                Color nColor = Color.FromArgb(nR, nG, nB); //новий колір з отриманих бітів
                src.SetPixel(0, i + 1, nColor); //записали отриманий колір в картинку
            }
        }

        /// <summary>
        /// Читає кількість символів для дешифрування з перших біт картинки
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private int ReadCountText(Bitmap src)
        {
            byte[] rez = new byte[3]; //масив на 3 елементи, тобто максимум 999 символів шифрується
            for (int i = 0; i < rez.Length; i++)
            {
                Color color = src.GetPixel(0, i + 1); //цвет 1, 2, 3 пікселів 
                BitArray colorArray = ByteToBit(color.R); //біт кольора
                BitArray bitCount = ByteToBit(color.R); ; //ініціалізація результуючого масиву біт
                bitCount[0] = colorArray[0];
                bitCount[1] = colorArray[1];

                colorArray = ByteToBit(color.G);
                bitCount[2] = colorArray[0];
                bitCount[3] = colorArray[1];
                bitCount[4] = colorArray[2];

                colorArray = ByteToBit(color.B);
                bitCount[5] = colorArray[0];
                bitCount[6] = colorArray[1];
                bitCount[7] = colorArray[2];
                rez[i] = BitToByte(bitCount);
            }
            string m = Encoding.UTF8.GetString(rez);
            return Convert.ToInt32(m, 10);
        }
        private void BtnRead_Click(object sender, EventArgs e)
        {
            try
            {
                if (dPic.ShowDialog() == DialogResult.OK)
                {
                    string FilePic = dPic.FileName;
                    Bitmap bPic;
                    try
                    {
                        using (FileStream rFile = new FileStream(FilePic, FileMode.Open)) //открываем поток
                        {
                            bPic = new Bitmap(rFile);
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Помилка відкриття файлу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!IsEncryption(bPic))
                    {
                        MessageBox.Show("У файлі немає зашифрованої інформації", "Інформація", MessageBoxButtons.OK);
                        return;
                    }

                    int countSymbol = ReadCountText(bPic); //считали количество зашифрованных символов
                    byte[] message = new byte[countSymbol];
                    int index = 0;
                    bool st = false;
                    for (int i = 4; i < bPic.Width; i++)
                    {
                        for (int j = 0; j < bPic.Height; j++)
                        {
                            Color pixelColor = bPic.GetPixel(i, j);
                            if (index == message.Length)
                            {
                                st = true;
                                break;
                            }
                            BitArray colorArray = ByteToBit(pixelColor.R);
                            BitArray messageArray = ByteToBit(pixelColor.R);
                            messageArray[0] = colorArray[0];
                            messageArray[1] = colorArray[1];

                            colorArray = ByteToBit(pixelColor.G);
                            messageArray[2] = colorArray[0];
                            messageArray[3] = colorArray[1];
                            messageArray[4] = colorArray[2];

                            colorArray = ByteToBit(pixelColor.B);
                            messageArray[5] = colorArray[0];
                            messageArray[6] = colorArray[1];
                            messageArray[7] = colorArray[2];
                            message[index] = BitToByte(messageArray);
                            index++;
                        }
                        if (st)
                        {
                            break;
                        }
                    }
                    string strMessage = Encoding.UTF8.GetString(message);

                    tbMessage4.Text = strMessage;

                    picBox.Image = (Bitmap)bPic.Clone();
                    bPic.Dispose();
                }
            }
            catch
            {
                MessageBox.Show("Не вдалось записати повідомлення!");
            }
        }
        private void BtnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbMessage4.Text.Trim()))
                {
                    MessageBox.Show("Введіть повідомлення!");
                    return;
                }
                if (dPic.ShowDialog() == DialogResult.OK)
                {
                    string FilePic = dPic.FileName;

                    int CountText;
                    Bitmap bPic;
                    List<byte> bList;
                    try
                    {
                        using (FileStream rFile = new FileStream(FilePic, FileMode.Open))
                        {
                            bPic = new Bitmap(rFile);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (StreamWriter sw = new StreamWriter(ms))
                                {
                                    sw.Write(tbMessage4.Text);
                                    sw.Flush();
                                    ms.Position = 0;
                                    using (BinaryReader bText = new BinaryReader(ms, Encoding.UTF8))
                                    {
                                        bList = new List<byte>();
                                        //зчитуємо весь текстовий файл для шифрування в байтовий список
                                        while (bText.PeekChar() != -1)
                                        {
                                            bList.Add(bText.ReadByte());
                                        }
                                        CountText = bList.Count;//в CountText - кількість в байтах тексту, який потрібно закодувати
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Помилка відкриття файлу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //перевіряємо, поміститися вибраний текст в зображенні
                    if (CountText > (bPic.Width * bPic.Height))
                    {
                        MessageBox.Show("Вибрана картинка замала для розміщення вибраного тексту", "Інформація", MessageBoxButtons.OK);
                        return;
                    }

                    //перевіряємо, чи картинка вже зашифрована
                    if (IsEncryption(bPic))
                    {
                        MessageBox.Show("Файл вже зашифрований", "Інформація", MessageBoxButtons.OK);
                        return;
                    }

                    byte[] Symbol = Encoding.UTF8.GetBytes("/");
                    BitArray ArrBeginSymbol = ByteToBit(Symbol[0]);
                    Color curColor = bPic.GetPixel(0, 0);
                    BitArray tempArray = ByteToBit(curColor.R);
                    tempArray[0] = ArrBeginSymbol[0];
                    tempArray[1] = ArrBeginSymbol[1];
                    byte nR = BitToByte(tempArray);

                    tempArray = ByteToBit(curColor.G);
                    tempArray[0] = ArrBeginSymbol[2];
                    tempArray[1] = ArrBeginSymbol[3];
                    tempArray[2] = ArrBeginSymbol[4];
                    byte nG = BitToByte(tempArray);

                    tempArray = ByteToBit(curColor.B);
                    tempArray[0] = ArrBeginSymbol[5];
                    tempArray[1] = ArrBeginSymbol[6];
                    tempArray[2] = ArrBeginSymbol[7];
                    byte nB = BitToByte(tempArray);

                    Color nColor = Color.FromArgb(nR, nG, nB);
                    bPic.SetPixel(0, 0, nColor);
                    //тобто в першому пікселі буде символ /, який говорить про те, що картинка зашифрована

                    WriteCountText(CountText, bPic); //записуємо кількість символів для шифрування

                    int index = 0;
                    bool st = false;
                    for (int i = 4; i < bPic.Width; i++)
                    {
                        for (int j = 0; j < bPic.Height; j++)
                        {
                            Color pixelColor = bPic.GetPixel(i, j);
                            if (index == bList.Count)
                            {
                                st = true;
                                break;
                            }
                            BitArray colorArray = ByteToBit(pixelColor.R);
                            BitArray messageArray = ByteToBit(bList[index]);
                            colorArray[0] = messageArray[0]; //міняємо
                            colorArray[1] = messageArray[1]; // в нашому кольорі біти
                            byte newR = BitToByte(colorArray);

                            colorArray = ByteToBit(pixelColor.G);
                            colorArray[0] = messageArray[2];
                            colorArray[1] = messageArray[3];
                            colorArray[2] = messageArray[4];
                            byte newG = BitToByte(colorArray);

                            colorArray = ByteToBit(pixelColor.B);
                            colorArray[0] = messageArray[5];
                            colorArray[1] = messageArray[6];
                            colorArray[2] = messageArray[7];
                            byte newB = BitToByte(colorArray);

                            Color newColor = Color.FromArgb(newR, newG, newB);
                            bPic.SetPixel(i, j, newColor);
                            index++;
                        }
                        if (st)
                        {
                            break;
                        }
                    }
                    picBox.Image = (Bitmap)bPic.Clone();

                    if (dSavePic.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (FileStream wFile = new FileStream(dSavePic.FileName, FileMode.Create)) //записуємо результат
                            {
                                bPic.Save(wFile, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Помилка відкриття файлу на запис", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    bPic.Dispose();
                }
            }
            catch
            {
                MessageBox.Show("Не вдалось записати повідомлення!");
            }
        }
        #endregion

        #region Lab5
        private void DoJob5(Func<byte[], string, Func<byte, byte, byte>, string, byte[]> job)
        {
            try
            {
                if (string.IsNullOrEmpty(tbKey5.Text.Trim()))
                {
                    MessageBox.Show("Введіть ключ!");
                    return;
                }
                string key = tbKey5.Text;
                if (dOpenFile.ShowDialog() == DialogResult.OK)
                {
                    byte[] fileInBytes = File.ReadAllBytes(dOpenFile.FileName);

                    tbInBytes5.Text = string.Join(" ", fileInBytes);

                    if (dSaveFile.ShowDialog() == DialogResult.OK)
                    {
                        byte[] fileOutBytes = job(fileInBytes, dSaveFile.FileName, FeistelCipher.FunctionF, key);
                        tbOutBytes5.Text = string.Join(" ", fileOutBytes);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Не вдалось зашифрувати/розшифрувати повідомлення!");
            }
        }
        private void BtnEncrypt5_Click(object sender, EventArgs e)
        {
            DoJob5(FeistelCipher.EncryptFile);
        }
        private void BtnDecrypt5_Click(object sender, EventArgs e)
        {
            DoJob5(FeistelCipher.DecryptFile);
        }
        #endregion

        #region Lab6-7
        private const int rank = 3;
        readonly SecurityAlgorithm security = new Hill.Hill(new []{ Hill.Hill.SymbolType.Mask, Hill.Hill.SymbolType.Mask, Hill.Hill.SymbolType.Character }, rank);
        private void DoJob67(Func<string, string, string> job)
        {
            try
            {
                if (string.IsNullOrEmpty(tbKey67.Text.Trim()))
                {
                    MessageBox.Show("Введіть ключ!");
                    return;
                }
                if (tbKey67.Text.Length != 9)
                {
                    MessageBox.Show("Введіть ключ!");
                    return;
                }
                if (dOpenFile.ShowDialog() == DialogResult.OK)
                {
                    if (dSaveFile.ShowDialog() == DialogResult.OK)
                    {
                        string key = tbKey67.Text;
                        string input = File.ReadAllText(dOpenFile.FileName);
                        if (string.IsNullOrEmpty(input))
                        {
                            MessageBox.Show("Немає що шифрувати!");
                            return;
                        }
                        tbMessage67.Text = input;
                        string output = job(input, key);
                        File.WriteAllText(dSaveFile.FileName, output);
                        tbResult67.Text = output;
                    }
                    MessageBox.Show("Виконано!");
                }
            }
            catch (ArgumentException ar)
            {
                MessageBox.Show(ar.Message);
            }
            catch
            {
                MessageBox.Show("Не вдалось зашифрувати/розшифрувати повідомлення!");
            }
        }

        private void btnEncrypt67_Click(object sender, EventArgs e)
        {
            DoJob67(security.Encrypt);
        }
        private void btnDecrypt67_Click(object sender, EventArgs e)
        {
            DoJob67(security.Decrypt);
        }
        #endregion

        #region Lab8
        private const byte lenIV = 16;
        private static void AesCtrTransform(
            byte[] key, byte[] salt, Stream inputStream, Stream outputStream)
        {
            SymmetricAlgorithm aes =
                new AesManaged { Mode = CipherMode.ECB, Padding = PaddingMode.None };

            int blockSize = aes.BlockSize / 8;

            if (salt.Length != blockSize)
            {
                throw new ArgumentException(
                    string.Format(
                        "Salt size must be same as block size (actual: {0}, expected: {1})",
                        salt.Length, blockSize));
            }

            byte[] counter = (byte[])salt.Clone();

            Queue<byte> xorMask = new Queue<byte>();

            var zeroIv = new byte[blockSize];
            ICryptoTransform counterEncryptor = aes.CreateEncryptor(key, zeroIv);

            int b;
            while ((b = inputStream.ReadByte()) != -1)
            {
                if (xorMask.Count == 0)
                {
                    var counterModeBlock = new byte[blockSize];

                    counterEncryptor.TransformBlock(
                        counter, 0, counter.Length, counterModeBlock, 0);

                    for (var i2 = counter.Length - 1; i2 >= 0; i2--)
                    {
                        if (++counter[i2] != 0)
                        {
                            break;
                        }
                    }

                    foreach (var b2 in counterModeBlock)
                    {
                        xorMask.Enqueue(b2);
                    }
                }

                var mask = xorMask.Dequeue();
                outputStream.WriteByte((byte)(((byte)b) ^ mask));
            }
        }
        
        private void DoCipher()
        {

            try
            {
                if (string.IsNullOrEmpty(tbKey8.Text.Trim()))
                {
                    MessageBox.Show("Введіть ключ!");
                    return;
                }
                if (string.IsNullOrEmpty(tbSalt8.Text.Trim()))
                {
                    MessageBox.Show("Введіть IV!");
                    return;
                }

                byte[] keyBytes = Encoding.UTF8.GetBytes(tbKey8.Text);

                int len = keyBytes.Length;
                if (len != 16 && len != 24 && len != 32)
                {
                    MessageBox.Show("Розмір ключа повинен бути 16 літер(128-bit), 24 літер(192-bit), 32 літер(256-bit)! (" + len + ")");
                    return;
                }

                string salt = tbSalt8.Text.Substring(0, Math.Min(tbSalt8.Text.Length, lenIV));
                salt = salt.PadRight(16, salt[salt.Length - 1]);

                tbSalt8.Enabled = false;
                tbSalt8.Text = salt;
                tbSalt8.Enabled = true;

                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

                if (dOpenFile.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = File.OpenRead(dOpenFile.FileName))
                    {
                        if (dSaveFile.ShowDialog() == DialogResult.OK)
                        {
                            using (FileStream fs2 = File.OpenWrite(dSaveFile.FileName))
                            {
                                AesCtrTransform(keyBytes, saltBytes, fs, fs2);
                            }
                        }
                    }
                    MessageBox.Show("Виконано!");
                }
            }
            catch
            {
                MessageBox.Show("Не вдалось зашифрувати/розшифрувати повідомлення!");
            }
        }

        private void BtnEncrypt8_Click(object sender, EventArgs e)
        {
            DoCipher();
        }
        private void BtnDecrypt8_Click(object sender, EventArgs e)
        {
            DoCipher();
        }
        #endregion
    }
}
