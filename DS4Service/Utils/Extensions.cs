using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Service
{
    public static class Extensions
    {
        public static byte ToByte(this int i)
        {            
            byte[] intBytes = BitConverter.GetBytes(i);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }
            return intBytes[0];
        }

        public static byte ToByte(this int? i)
        {
            byte[] intBytes = BitConverter.GetBytes(i ?? 0);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }
            return intBytes[0];
        }
    }
}
