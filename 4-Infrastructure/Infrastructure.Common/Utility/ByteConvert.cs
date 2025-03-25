using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Utility
{
    public static class ByteConvert
    {
        /// <summary>
        /// 每两位反转，modebus小端转大端，
        /// 配合 if (BitConverter.IsLittleEndian) {}
        /// 若为 小端模式，则使用次函数转换
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static byte[] ReversePer2Byte(byte[] byteArr)
        {
            List<byte> ret = new List<byte>();
            for (int i = 0; i < byteArr.Length; i = i + 2)
            {
                ret.Add(byteArr[i + 1]);
                ret.Add(byteArr[i]);
            }
            return ret.ToArray();
        }
        /// <summary>
        ///     字符串转换为Hex字符串
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="encode">编码类型</param>
        /// <returns></returns>
        public static string StringToHexString(string s, Encoding encode)
        {
            var b = encode.GetBytes(s); //按照指定编码将string编程字节数组
            return b.Aggregate(string.Empty, (current, t) => current + "%" + Convert.ToString(t, 16));
        }

        /// <summary>
        ///     Hex字符串转换为字符串
        /// </summary>
        /// <param name="hs">Hex字符串</param>
        /// <param name="encode">编码类型</param>
        /// <returns></returns>
        public static string HexStringToString(string hs, Encoding encode)
        {
            //以%分割字符串，并去掉空字符
            var chars = hs.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
            var b = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (var i = 0; i < chars.Length; i++) b[i] = Convert.ToByte(chars[i], 16);

            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }

        /// <summary>
        ///     字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString">Hex字符串</param>
        /// <returns></returns>
        public static byte[] StringToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
                hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        ///     字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes)
        {
            const string returnStr = "";
            return bytes == null ? returnStr : bytes.Aggregate(returnStr, (current, t) => current + t.ToString("X2"));
        }

        /// <summary>
        ///     将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int BytesToInt32(byte[] data)
        {
            //如果传入的字节数组长度小于4,则返回0
            if (data.Length < 4) return 0;

            //定义要返回的整数
            var num = 0;
            //如果传入的字节数组长度大于4,需要进行处理
            if (data.Length < 4) return num;
            //创建一个临时缓冲区
            var tempBuffer = new byte[4];
            //将传入的字节数组的前4个字节复制到临时缓冲区
            Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);
            //将临时缓冲区的值转换成整数，并赋给num
            num = BitConverter.ToInt32(tempBuffer, 0);
            //返回整数
            return num;
        }

        /// <summary>
        ///     bytes数据转换为float类型
        /// </summary>
        /// <param name="data">byte数据</param>
        /// <returns></returns>
        public static float ValueConvertToFloat(byte[] data)
        {
            var shuju = BytesToHexString(data);
            var num = uint.Parse(shuju, NumberStyles.AllowHexSpecifier);
            var floatValues = BitConverter.GetBytes(num);
            return BitConverter.ToSingle(floatValues, 0);
        }

        /// <summary>
        ///     bytes数据转换为long类型
        /// </summary>
        /// <param name="data">byte数据</param>
        /// <returns></returns>
        public static long ValueConvertToLong(byte[] data)
        {
            var shuju = BytesToHexString(data);
            var num = ulong.Parse(shuju, NumberStyles.AllowHexSpecifier);
            return (long)num;
        }

        public static byte[] CRC16(byte[] data)
        {
            ushort crc = 0xFFFF; //set all 1

            var len = data.Length;
            if (len <= 0)
            {
                crc = 0;
            }
            else
            {
                len--;
                uint ix;
                for (ix = 0; ix <= len; ix++)
                {
                    crc = (ushort)(crc ^ data[ix]);
                    uint iy;
                    for (iy = 0; iy <= 7; iy++)
                        if ((crc & 1) != 0)
                            crc = (ushort)((crc >> 1) ^ 0xA001);
                        else
                            crc = (ushort)(crc >> 1); //
                }
            }

            var buf1 = (byte)((crc & 0xff00) >> 8); //高位置
            var buf2 = (byte)(crc & 0x00ff); //低位置
            crc = (ushort)(buf1 << 8);
            crc += buf2;
            var strA = crc.ToString("x4");
            var result = StringToHexByte(strA);
            var b = new byte[2];
            b[0] = result[1];
            b[1] = result[0];
            return b;
        }
    }
}
