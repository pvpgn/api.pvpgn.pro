using System;
using System.IO;
using System.Text;

namespace WebAPI.BnetHash
{
    /**
    * C# implementation of the PvPGN Password Hash Algorithm.
    * Copyright 2011 HarpyWar (harpywar@gmail.com)
    * http://harpywar.com
    *
    * Based on the MBNCSUtil XSha function
    * This code is available under the GNU Lesser General Public License:
    * http://www.gnu.org/licenses/lgpl.txt
*/

    /// <summary>
    /// Provides an implementation of Battle.net's "broken" (nonstandard) SHA-1 
    /// implementation.  This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This class does not derive from the standard
    /// .NET <see cref="System.Security.Cryptography.SHA1">SHA1</see>
    /// class, and also does not provide adequate security for independent
    /// security solutions.  See the System.Security.Cryptography 
    /// namespace for more information.
    /// </remarks>
    /// <threadsafety>This type is safe for multithreaded operations.</threadsafety>
    public static class PvpgnHash
    {
        /**
         * Returns the 20 byte hash based on the passed in byte[] data.
         *
         * @param pass The data to hash.
         * @return The 20 bytes of hashed data.
         */
        public static byte[] GetHash(byte[] pass)
        {
            String tmp = Encoding.UTF8.GetString(pass);
            return CalculateHash(tmp);
        }

        /**
         * Returns hash based on the passed in String data.
         *
         * @param pass The data to hash.
         * @return The 40 symbols hex String of hashed data.
         */
        public static String GetHash(String pass)
        {
            byte[] tmp = CalculateHash(pass);
            return asHex(tmp);
        }


        /// <summary>
        /// Calculates the "broken" SHA-1 hash used by Battle.net.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <returns>A 20-byte array containing the hashed result.</returns>
        private static byte[] CalculateHash(string data)
        {
            byte[] input = Encoding.UTF8.GetBytes(toLowerUnicode(data).ToCharArray());

            if (input.Length > 1024) throw new ArgumentOutOfRangeException("data", "The input size must be less than 1024 bytes.");

            return SafeHash(input);
        }

        private static byte[] SafeHash(byte[] input)
        {
            byte[] data = new byte[1024];
            Array.Copy(input, 0, data, 0, input.Length);

            int i;
            MemoryStream mdata = new MemoryStream(data, true);
            BinaryReader br = new BinaryReader(mdata);
            BinaryWriter bw = new BinaryWriter(mdata);

            uint a, b, c, d, e, g;

            for (i = 0; i < 64; i++)
            {
                mdata.Seek((i * 4), SeekOrigin.Begin);
                // mdata now at ldata[i]
                uint expr_ldata_i = br.ReadUInt32();
                // mdata now at ldata[i+1]
                mdata.Seek(1 * 4, SeekOrigin.Current);
                // mdata now at ldata[i+2]
                uint expr_ldata_i_2 = br.ReadUInt32();
                // mdata now at ldata[i+3]
                mdata.Seek(5 * 4, SeekOrigin.Current);
                // mdata now at ldata[i+8]
                uint expr_ldata_i_8 = br.ReadUInt32();
                // mdata now at ldata[i+9]
                mdata.Seek(4 * 4, SeekOrigin.Current);
                // mdata now at ldata[i+13]
                uint expr_ldata_i_13 = br.ReadUInt32();
                // mdata now at ldata[i+14]
                int shiftVal = (int)((expr_ldata_i ^ expr_ldata_i_8 ^ expr_ldata_i_2 ^
                    expr_ldata_i_13) & 0x1f);
                mdata.Seek(2 * 4, SeekOrigin.Current);
                // mdata now at ldata[i+16]
                bw.Write(ROL(1, shiftVal));
            }

            a = 0x67452301;
            b = 0xefcdab89;
            c = 0x98badcfe;
            d = 0x10325476;
            e = 0xc3d2e1f0;
            g = 0;

            mdata.Seek(0, SeekOrigin.Begin);

            // loop 1
            for (i = 0; i < 20; i++)
            {
                g = br.ReadUInt32() + ROL(a, 5) + e + ((b & c) | (~b & d)) + 0x5A827999;
                e = d;
                d = c;
                c = ROL(b, 30);
                b = a;
                a = g;
            }

            // loop 1
            for (i = 0; i < 20; i++)
            {
                g = (d ^ c ^ b) + e + ROL(g, 5) + br.ReadUInt32() + 0x6ed9eba1;
                e = d;
                d = c;
                c = ROL(b, 30);
                b = a;
                a = g;
            }

            // loop 3
            for (i = 0; i < 20; i++)
            {
                g = br.ReadUInt32() + ROL(g, 5) + e + ((c & b) | (d & c) | (d & b)) - 0x70E44324;
                e = d;
                d = c;
                c = ROL(b, 30);
                b = a;
                a = g;
            }

            // loop 4
            for (i = 0; i < 20; i++)
            {
                g = (d ^ c ^ b) + e + ROL(g, 5) + br.ReadUInt32() - 0x359d3e2a;
                e = d;
                d = c;
                c = ROL(b, 30);
                b = a;
                a = g;
            }

            br.Close();
            bw.Close();
            mdata.Close();

            byte[] result = new byte[20];
            mdata = new MemoryStream(result, 0, 20, true, true);
            bw = new BinaryWriter(mdata);
            unchecked
            {
                bw.Write(ReverseBytes(0x67452301 + a));
                bw.Write(ReverseBytes(0xefcdab89 + b));
                bw.Write(ReverseBytes(0x98badcfe + c));
                bw.Write(ReverseBytes(0x10325476 + d));
                bw.Write(ReverseBytes(0xc3d2e1f0 + e));
            }

            mdata.Close();
            bw.Close();

            return result;
        }

        private static uint ROL(uint val, int shift)
        {
            shift &= 0x1f;
            val = (val >> (0x20 - shift)) | (val << shift);
            return val;
        }


        // reverse byte order (16-bit)
        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }
        // reverse byte order (32-bit)
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        // reverse byte order (64-bit)
        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        /***
         * Converts byte array to hex string
         * @param buf
         * @return
         */
        private static String asHex(byte[] buf)
        {
            string hex = BitConverter.ToString(buf).Replace("-", string.Empty).ToLower();
            return hex;
        }

        /***
         * PvPGN hash is case insensitive but only for ASCII characters
         * @param str
         * @return
         */
        private static String toLowerUnicode(String str)
        {
            char c = new char();
            for (int i = 0; i < str.Length; i++)
            {
                c = str[i];
                if (c < 128)
                {
                    str = str.Substring(0, i) + c.ToString().ToLower() + str.Substring(i + 1);
                }
            }
            return str;
        }

    }
}
