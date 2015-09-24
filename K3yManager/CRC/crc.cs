using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3yManager.CRC
{
    
        public class crc
        {

            /// <summary>
            /// CRC码计算,返回数组中,0为低位,1为高位
            /// </summary>
            /// <param name="data"> </param>
            /// <returns> </returns>
            public static byte[] CRC16(byte[] data)
            {
                byte CRC16Lo = 0;
                byte CRC16Hi = 0;
                byte CL = 0;
                byte CH = 0;
                byte SaveHi = 0;
                byte SaveLo = 0;
                // short i = 0;
                short Flag = 0;
                byte[] ReturnData = new byte[1 + 1];
                // CRC寄存器
                // 多项式码&HA001
                CRC16Lo = (byte)0xFF;
                CRC16Hi = (byte)0xFF;
                CL = (byte)0x1;
                CH = (byte)0xA0;
                for (int i = 0; i <= data.Length - 3; i++)
                {
                    CRC16Lo = Convert.ToByte(CRC16Lo ^ data[i]);
                    // 每一个数据与CRC寄存器进行异或
                    for (Flag = (short)0;
                    Flag <= 7.0;
                    Flag = Convert.ToInt16(Flag + 1))
                    {
                        SaveHi = CRC16Hi;
                        SaveLo = CRC16Lo;
                        CRC16Hi = Convert.ToByte(CRC16Hi / 2);
                        // 高位右移一位
                        CRC16Lo = Convert.ToByte(CRC16Lo / 2);
                        // 低位右移一位
                        if ((SaveHi & 0x1) == 0x1)
                        {
                            // 如果高位字节最后一位为1
                            CRC16Lo = Convert.ToByte(CRC16Lo | 0x80);
                            // 则低位字节右移后前面补1
                        }
                        // 否则自动补0
                        if ((SaveLo & 0x1) == 0x1)
                        {
                            // 如果LSB为1，则与多项式码进行异或
                            CRC16Hi = Convert.ToByte(CRC16Hi ^ CH);
                            CRC16Lo = Convert.ToByte(CRC16Lo ^ CL);
                        }
                        // Debug.Print Str(i) & ":", CRC16Lo, CRC16Hi
                    }
                    // Debug.Print CRC16Lo, CRC16Hi
                }
                ReturnData[0] = CRC16Lo;
                // CRC低位
                ReturnData[1] = CRC16Hi;
                // CRC高位
                return ReturnData;
            }
        }

    
    
}
