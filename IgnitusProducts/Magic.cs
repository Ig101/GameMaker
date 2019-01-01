using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public static class Magic
    {
        struct Word
        {
            public byte[] letters;
            public Word(byte l1, byte l2, byte l3, byte l4)
            {
                letters = new byte[] { l1, l2, l3, l4 };
            }
        }
        #region consts
        const int Nb = 4;
        const int Nk = 4;
        const int Nr = 10;
        const int c = 99;
        static byte[] Sbox = new byte[]{
        0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76, 
        0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0, 
        0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15, 
        0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75, 
        0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84, 
        0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf, 
        0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8, 
        0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2, 
        0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73, 
        0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb, 
        0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79, 
        0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08, 
        0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a, 
        0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e, 
        0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf, 
        0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
        };
        static byte[] InvSbox = new byte[]{
        0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
        0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
        0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
        0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
        0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
        0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
        0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
        0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
        0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
        0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
        0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
        0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
        0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
        0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
        0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
        0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
        };
        static byte[,] Rcon = new byte[,]{
        {0x00, 0x00, 0x00, 0x00},
        {0x01, 0x00, 0x00, 0x00},
        {0x02, 0x00, 0x00, 0x00},
        {0x04, 0x00, 0x00, 0x00},
        {0x08, 0x00, 0x00, 0x00},
        {0x10, 0x00, 0x00, 0x00},
        {0x20, 0x00, 0x00, 0x00},
        {0x40, 0x00, 0x00, 0x00},
        {0x80, 0x00, 0x00, 0x00},
        {0x1b, 0x00, 0x00, 0x00},
        {0x36, 0x00, 0x00, 0x00}
        };
        static byte[] key = new byte[] { 54, 45, 77, 98, 66, 31, 128, 14, 68, 32, 06, 38, 79, 63, 188, 214 };
        #endregion

        public static byte[] Restore(string path)
        {
            Word[] w = KeyExpansion(key);
            byte[] bytes = File.ReadAllBytes(path);
            List<byte> block = new List<byte>();
            for (int i = 0; i < bytes.Length; i++)
            {
                block.Add(bytes[i]);
                if (block.Count >= 16)
                {
                    byte[] newBlock = Magic.InvCipher(block.ToArray(), w);
                    for (int j = 0; j < 16; j++)
                    {
                        bytes[i + j - 15] = newBlock[j];
                    }
                    block = new List<byte>();
                }
                if (i == bytes.Length - 1 && block.Count > 0)
                {
                    byte[] blockUpd = new byte[16];
                    for (int j = 0; j < block.Count; j++)
                    {
                        blockUpd[j] = block[j];
                    }
                    byte[] newBlock = Magic.InvCipher(blockUpd, w);
                    for (int j = 0; j < 16; j++)
                    {
                        bytes[i + j - 15] = newBlock[j];
                    }
                }
            }
            int symNum = bytes.Length;
            while (bytes[symNum - 1] == 0)
            {
                symNum--;
            }
            if (symNum % 2 == 1) symNum++;
            byte[] newBytes = new byte[symNum];
            for (int i = 0; i < newBytes.Length; i++)
            {
                newBytes[i] = bytes[i];
            }
            return newBytes;
        }

        public static void Act(string path, byte[] rawBytes)
        {
            Word[] w = KeyExpansion(key);
            byte[] bytes = new byte[16 * ((long)(rawBytes.Length / 16) + ((rawBytes.Length % 16 == 0) ? 0 : 1))];
            for (int i = 0; i < rawBytes.Length; i++)
            {
                bytes[i] = rawBytes[i];
            }
            List<byte> block = new List<byte>();
            for (int i = 0; i < bytes.Length; i++)
            {
                block.Add(bytes[i]);
                if (block.Count >= 16)
                {
                    byte[] newBlock = Magic.Cipher(block.ToArray(), w);
                    for (int j = 0; j < 16; j++)
                    {
                        bytes[i + j - 15] = newBlock[j];
                    }
                    block = new List<byte>();
                }
                if (i == bytes.Length - 1 && block.Count > 0)
                {
                    byte[] blockUpd = new byte[16];
                    for (int j = 0; j < block.Count; j++)
                    {
                        blockUpd[j] = block[j];
                    }
                    byte[] newBlock = Magic.Cipher(blockUpd, w);
                    for (int j = 0; j < 16; j++)
                    {
                        bytes[i + j - block.Count + 1] = newBlock[j];
                    }
                }
            }
            File.WriteAllBytes(path, bytes);
        }

        #region Cipher

        #region Magic
        static byte ReversalWithM(byte divider)
        {
            int shifted = divider << 1;
            int m = 283; //x^8+x^4+x^3+x^1+1
            return shifted > 255 ? (byte)(shifted ^ m) : (byte)shifted;
        }

        static byte MultiplyWithM(byte b1, byte b2)
        {
            int m = 283;
            int[] nums = new int[8];
            nums[0] = b2;
            for (int i = 1; i < 8; i++)
            {
                nums[i] = nums[i - 1] << 1;
                if (nums[i] > 255) nums[i] ^= m;
            }
            int sum = 0;
            for (int i = 7; i >= 0; i--)
            {
                if ((b1 & (1 << i)) != 0)
                {
                    sum ^= nums[i];
                }
            }
            return (byte)sum;
        }

        static bool[] Byte2Bit(byte value)
        {
            bool[] bits = new bool[8];
            for (int i = 7; i >= 0; i--)
            {
                if ((value & (1 << i)) != 0)
                {
                    bits[i] = true;
                }
                else
                {
                    bits[i] = false;
                }
            }
            return bits;
        }

        static byte Bit2Byte(bool[] value)
        {
            byte res = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i]) res += (byte)Math.Pow(2, i);
            }
            return res;
        }
        #endregion

        #region Code
        static Word RotWord(Word start)
        {
            return new Word(start.letters[1], start.letters[2], start.letters[3], start.letters[0]);
        }

        static Word SubWord(Word start)
        {
            Word end = new Word(0, 0, 0, 0);
            for (int i = 0; i < start.letters.Length; i++)
            {
                int sbox_row = (int)(start.letters[i] / 0x10);
                int sbox_col = start.letters[i] % 0x10;
                end.letters[i] = Sbox[16 * sbox_row + sbox_col];
            }
            return end;
        }

        static Word[] KeyExpansion(byte[] key)
        {
            Word[] w = new Word[Nb * (Nr + 1)];
            Word temp;
            for (int i = 0; i < Nk; i++)
            {
                w[i] = new Word(key[4 * i], key[4 * i + 1], key[4 * i + 2], key[4 * i + 3]);
            }
            for (int i = Nk; i < Nb * (Nr + 1); i++)
            {
                temp = w[i - 1];
                if (i % Nk == 0)
                {
                    temp = SubWord(RotWord(temp));//Rcon[i / Nk];
                    for (int k = 0; k < temp.letters.Length; k++)
                    {
                        temp.letters[k] ^= Rcon[i / Nk, k];
                    }
                }
                else if (Nk > 6 && i % Nk == 4)
                {
                    temp = SubWord(temp);
                }
                w[i] = new Word(0, 0, 0, 0);
                for (int k = 0; k < temp.letters.Length; k++)
                {
                    w[i].letters[k] = (byte)(w[i - Nk].letters[k] ^ temp.letters[k]);
                }
            }
            return w;
        }

        static void AddRoundKey(byte[,] state, Word[] w, int start, int finish)
        {
            byte[,] key = new byte[4, Nb];
            for (int i = start; i <= finish; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    key[j, i - start] = w[i].letters[j];
                }
            }
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    state[i, j] ^= key[i, j];
                }
            }
        }

        static void SubBytes(byte[,] state, bool inv)
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    int sbox_row = (int)(state[i, j] / 0x10);
                    int sbox_col = state[i, j] % 0x10;
                    state[i, j] = inv ? InvSbox[16 * sbox_row + sbox_col] : Sbox[16 * sbox_row + sbox_col];
                }
            }
        }

        static void ShiftRows(byte[,] state, bool inv)
        {
            for (int i = 1; i < state.GetLength(0); i++)
            {
                byte[] row = new byte[state.GetLength(1)];
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    row[j] = state[i, j];
                }
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    int number;
                    if (inv)
                    {
                        number = j - i;
                        if (number < 0) number += row.Length;
                    }
                    else
                    {
                        number = j + i;
                        if (number >= row.Length) number -= row.Length;
                    }
                    state[i, j] = row[number];
                }
            }
        }

        static void MixColumns(byte[,] state, bool inv)
        {
            byte[] native = inv ? new byte[] { 14, 11, 13, 9 } : new byte[] { 2, 3, 1, 1 };
            for (int j = 0; j < state.GetLength(1); j++)
            {
                byte[] cols = new byte[state.GetLength(0)];
                for (int i = 0; i < state.GetLength(0); i++)
                {
                    cols[i] = state[i, j];
                }
                for (int i = 0; i < state.GetLength(0); i++)
                {
                    int newValue = 0;
                    for (int k = 0; k < state.GetLength(0); k++)
                    {
                        int number = (i + k > 3) ? (i + k - 4) : (i + k);
                        newValue ^= MultiplyWithM(cols[number], native[k]);
                    }
                    state[i, j] = (byte)newValue;
                }
            }
        }

        static byte[] Cipher(byte[] input, Word[] w)
        {
            byte[,] state = new byte[4, Nb];
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    state[i, j] = input[i + 4 * j];
                }
            }
            AddRoundKey(state, w, 0, Nb - 1);
            for (int i = 1; i <= Nr - 1; i++)
            {
                SubBytes(state, false);
                ShiftRows(state, false);
                MixColumns(state, false);
                AddRoundKey(state, w, i * Nb, (i + 1) * Nb - 1);
            }
            SubBytes(state, false);
            ShiftRows(state, false);
            AddRoundKey(state, w, Nr * Nb, (Nr + 1) * Nb - 1);
            byte[] output = new byte[4 * Nb];
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    output[i + 4 * j] = state[i, j];
                }
            }
            return output;
        }

        static byte[] InvCipher(byte[] input, Word[] w)
        {
            byte[,] state = new byte[4, Nb];
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    state[i, j] = input[i + 4 * j];
                }
            }
            AddRoundKey(state, w, Nr * Nb, (Nr + 1) * Nb - 1);
            for (int i = Nr - 1; i > 0; i--)
            {
                ShiftRows(state, true);
                SubBytes(state, true);
                AddRoundKey(state, w, Nb * i, Nb * (i + 1) - 1);
                MixColumns(state, true);
            }
            ShiftRows(state, true);
            SubBytes(state, true);
            AddRoundKey(state, w, 0, Nb - 1);
            byte[] output = new byte[4 * Nb];
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    output[i + 4 * j] = state[i, j];
                }
            }
            return output;
        }
        #endregion
        #endregion
    }
}
