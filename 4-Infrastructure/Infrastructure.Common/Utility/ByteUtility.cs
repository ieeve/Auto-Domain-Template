using System.Text;

namespace Infrastructure.Common.Utility
{
    public class ByteUtility
    {
        public static byte[] CRC16(byte[] data)
        {
            int len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;

                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8); //高位置
                byte lo = (byte)(crc & 0x00FF); //低位置

                return BitConverter.IsLittleEndian ? new byte[] { lo, hi } : new byte[] { hi, lo };
            }
            return new byte[] { 0, 0 };
        }
        /// <summary>  
        /// 累加校验和，只保留低8位  
        /// </summary>  
        /// <param name="memorySpage">需要校验的数据</param>  
        /// <returns>返回校验和结果</returns>  
        public static byte GetByteCrcSum(byte[] memorySpage)
        {
            int num = 0;
            for (int i = 0; i < memorySpage.Length; i++)
            {
                num = (num + memorySpage[i]) % 0xffff;
            }
            //实际上num 这里已经是结果了，如果只是取int 可以直接返回了  
            memorySpage = BitConverter.GetBytes(num);
            //返回累加校验和  
            return memorySpage[0];
        }

        /// <summary>
        /// 判断校检位是否正确
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool CheckCrcCorrect(byte[] bytes)
        {
            byte[] arrNew = new byte[bytes.Length - 2]; //去掉后2位的新数组
            Array.Copy(bytes, arrNew, arrNew.Length); // 数据复制
            byte crcSum = GetByteCrcSum(arrNew);
            byte crcVal2 = bytes[bytes.Length - 2]; //取指令的倒数第二位
            return crcSum == crcVal2;
        }

        /// <summary> 
        /// 字符串转16进制字节数组 ,以空格分隔或横杠分隔或无分隔的
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
        public static byte[] HexStr2Byte(string hexString)
        {
            hexString = hexString.Replace("-", "");
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 根据Int类型的值，返回用1或0(对应True或Flase)填充的数组
        /// <remarks>从右侧开始向左索引(0~31)</remarks>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<bool> GetBitList(int value)
        {
            var list = new List<bool>(32);
            for (var i = 0; i <= 31; i++)
            {
                var val = 1 << i;
                list.Add((value & val) == val);
            }
            return list;
        }

        /// <summary>
        /// 返回Int数据中某一位是否为1
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index">32位数据的从右向左的偏移位索引(0~31)</param>
        /// <returns>true表示该位为1，false表示该位为0</returns>
        public static bool GetBitValue(int value, ushort index)
        {
            if (index > 31) throw new ArgumentOutOfRangeException("index"); //索引出错
            var val = 1 << index;
            return (value & val) == val;
        }

        /// <summary>
        /// 返回Byte数据中某一位是否为1
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index">8位数据的从右向左的偏移位索引(0~31)</param>
        /// <returns>true表示该位为1，false表示该位为0</returns>
        public static bool GetBitValue(byte value, ushort index)
        {
            if (index > 8) throw new ArgumentOutOfRangeException("index"); //索引出错
            var val = 1 << index;
            return (value & val) == val;
        }

        /// <summary>
        /// 设定Int数据中某一位的值
        /// </summary>
        /// <param name="value">位设定前的值</param>
        /// <param name="index">32位数据的从右向左的偏移位索引(0~31)</param>
        /// <param name="bitValue">true设该位为1,false设为0</param>
        /// <returns>返回位设定后的值</returns>
        public static int SetBitValue(int value, ushort index, bool bitValue)
        {
            if (index > 31) throw new ArgumentOutOfRangeException("index"); //索引出错
            var val = 1 << index;
            return bitValue ? (value | val) : (value & ~val);
        }

        //将整数转成32位的二进制字符串
        public static StringBuilder ConvertTo32Bits(int val)
        {
            //10000000 00000000 00000000 00000000
            int bitMask = 1 << 31;
            StringBuilder bitBuffer = new StringBuilder();
            for (int i = 1; i <= 32; i++)
            {
                //二进制与操作
                //由于bitBuffer是从后加入的所以当二进制遇到bitMask第一位的1才是1；
                //&和&&；|和||的区别C#
                if ((val & bitMask) == 0)
                    bitBuffer.Append("0");
                else
                    bitBuffer.Append("1");
                //将数字转成二进制比且位前移一位
                //简单的例子举几位表示下：3进制0011
                //而上述bitMask：1000
                //与操作：1.比较1000和0011所以是0
                //2.左移一位变成0110；1000与0110还是0
                //3.左移一位变成1100；1000与1100为1；
                //4.左移一位变成1000；1000与1000为1；
                val <<= 1;
                if ((i % 8) == 0)
                    bitBuffer.Append(" ");
            }
            return bitBuffer;
        }
    }
}
