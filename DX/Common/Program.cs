using System;
using System.IO;
using System.Text;

namespace tool
{
    class Program
    {
     // static void Main(string[] args)
     // {
     //     byte[] buffer = new byte[4] {0xc8,0x00,0x00,0x00};
     //     byte[] ByteArray = ReadPcapngFile("D://DX//NJ5-1500_V1.19_ProjectDownload_1.pcapng");
     //     Console.WriteLine(ByteArray.Length);
     //     Console.WriteLine("------------------------");
     //     int isreqres = 1;
     //
     //
     //     StreamWriter sw = new StreamWriter("D://DX//NJ5-1500_V1.19_ProjectDownload_1.txt");
     //
     //     for (int i = 0; i < ByteArray.Length; )
     //     {
     //         int offset = bytesToInt(ByteArray, i + 4);
     //         if (bytesToInt(ByteArray, i) != 6 || ByteArray[i+51]!= 0b00000110)
     //         {                   
     //         }
     //         else
     //         {
     //             string str = Encoding.ASCII.GetString(GetPartialBytes(ByteArray, i + 82, offset - 86));
     //             if (str.Contains("HTTP"))
     //             {
     //                 if (isreqres==1) 
     //                 {
     //                     sw.WriteLine();
     //                     sw.WriteLine("----http--req---------------------------");
     //                     isreqres = 0;
     //                 }
     //                 else
     //                 {
     //                     sw.WriteLine();
     //                     sw.WriteLine("-----http-res--------------------------");
     //                     isreqres = 1;
     //                 }
     //                 
     //             }
     //             
     //             sw.Write(str);
     //             
     //             
     //
     //         }
     //         
     //         i += offset;
     //     }
     //
     //     sw.Flush();
     //     sw.Close();
     //     Console.ReadKey();
     // }
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
        public static int bytesToInt(byte[] src, int offset)
        {
            int value;
            value = (int)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }

        static public byte[] ReadPcapngFile(string path) 
        {
            FileStream fs = new FileStream(path,FileMode.Open, FileAccess.Read);
            long size = fs.Length;
            byte[] array = new byte[size];
            fs.Read(array, 0, array.Length);
            fs.Close();
            return array;
        }

    }
}
