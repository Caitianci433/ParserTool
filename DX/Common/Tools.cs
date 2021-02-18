using DX.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DX.Common
{
    class Tools
    {
        public static string BytesToShowBytes(byte[] byteArray)
        {
            StringBuilder str = new StringBuilder();
            str.Append("ADDRESS   |00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F\r\n");
            str.Append("\r\n");
            for (int i = 0, j = 0, k = 0, l = 0; i < byteArray.Length; i++, k++)
            {
                if (k % 16 == 0)
                {
                    str.Append(l.ToString("X8"));
                    str.Append("  |");
                    l += 1;
                }
                str.Append(byteArray[i].ToString("X2"));
                str.Append(" ");
                j += 1;
                if (j == 16)
                {
                    str.Append("\r\n");
                    j = 0;
                }
            }

            return str.ToString();
        }
        public static byte[] GetPartialBytes(byte[] bytes, int start, int count = -1)
        {
            if (bytes != null && start >= 0 && start < bytes.Length && count < 0)
            {
                count = bytes.Length - start;
            }

            if (bytes == null || count < 0 || start < 0 || start >= bytes.Length || start + count > bytes.Length)
                return null;

            byte[] ret = new byte[count];
            Array.Copy(bytes, start, ret, 0, count);

            return ret;
        }

        public static int BytesToInt(byte[] src, int offset)
        {
            int value;
            value = (int)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }

        public static ulong BytesToUlong(byte[] src, int offset)
        {
            ulong value;

             value = (ulong)(((src[offset] & 0xffuL) << 32)
                    | ((src[offset + 1] & 0xffuL) << 40)
                    | ((src[offset + 2] & 0xffuL) << 48)
                    | ((src[offset + 3] & 0xffuL) << 56)
                    | ((src[offset + 4] & 0xffuL))
                    | ((src[offset + 5] & 0xffuL) << 8)
                    | ((src[offset + 6] & 0xffuL) << 16)
                    | ((src[offset + 7] & 0xffuL) << 24));

            return value;
        }

        public static ulong BytesToUlong2(byte[] src, int offset)
        {
            byte[] a3 = new byte[] { src[offset+4], src[offset+5], src[offset+6], src[offset+7], src[offset], src[offset+1], src[offset+2], src[offset+3] };

            return BitConverter.ToUInt64(a3,0);
        }

        public static ulong BytesToUlong3(byte[] src, int offset)
        {
            ulong value;

            var x = ((src[offset] & 0xFFUL) << 32)
                    | ((src[offset + 1] & 0xFFUL) << 40)
                    | ((src[offset + 2] & 0xFFUL) << 48)
                    | ((src[offset + 3] & 0xFFUL) << 56)
                    | ((src[offset + 4] & 0xFFUL))
                    | ((src[offset + 5] & 0xFFUL) << 8)
                    | ((src[offset + 6] & 0xFFUL) << 16)
                    | ((src[offset + 7] & 0xFFUL) << 24);
            value = (ulong)x;
            return value;
        }

        public static int Bytes2ToInt(byte high, byte low)
        {
            int value;
            value = (int)((low & 0xFF)
                    | ((high & 0xFF) << 8));
            return value;
        }

        static public byte[] ReadPcapngFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            long size = fs.Length;
            byte[] array = new byte[size];
            fs.Read(array, 0, array.Length);
            fs.Close();
            return array;
        }

        static public List<Block> BytesToBlock(byte[] byteArray)
        {
            List<Block> BlockList = new List<Block>();
            for (int i = 0; i < byteArray.Length;)
            {
                int offset = BytesToInt(byteArray, i + 4);
                Block block = new Block(GetPartialBytes(byteArray,i,offset));
                BlockList.Add(block);
                i += offset;
            }
            return BlockList;
        }

        



    }
}
