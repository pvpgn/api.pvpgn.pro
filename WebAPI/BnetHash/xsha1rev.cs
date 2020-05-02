using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.BnetHash
{
    /// <summary>
    /// XSHA1 Reverser algorithm by Steve "Sc00bz" Thomas
    /// https://www.tobtu.com/revxsha1.php
    /// </summary>
    public class xsha1rev
    {
        /// <summary>
        /// Crack max 16 characters
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string Crack16(string hash)
        {
            var hashInt = convertToIntArray(hash);

            uint invalidCount = 0;
            string pass = "";
            var count = calcNorm16(hashInt, ref invalidCount, false, true, false, ref pass);
            return pass;
        }

        /// <summary>
        /// Crack max 20 characters (may take a long time)
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string Crack20(string hash)
        {
            var hashInt = convertToIntArray(hash);

            uint invalidCount = 0;
            string pass = "";
            var count = calcNorm20(hashInt, ref invalidCount, false, true, false, ref pass);
            return pass;
        }


        #region MACROS

        private void EXPANSION(ref uint[] x, uint i)
        {
            x[i + 16] = (uint)1 << (int)((x[i] ^ x[i + 2] ^ x[i + 8] ^ x[i + 13]) & 31);
        }
        private uint ROT(uint n, int s)
        {
            return ((n << (s)) | (n >> (32 - (s))));
        }
        private void SHA1_FF_REV(uint a, ref uint b, uint c, uint d, ref uint e, uint x)
        {
            b = ROT(b, 2); e -= (((c ^ d) & b) ^ d) + x + 0x5a827999 + ROT(a, 5);
        }
        private void SHA1_GG_REV(uint a, ref uint b, uint c, uint d, ref uint e, uint x)
        {
            b = ROT(b, 2); e -= (b ^ c ^ d) + x + 0x6ed9eba1 + ROT(a, 5);
        }
        private void SHA1_HH_REV(uint a, ref uint b, uint c, uint d, ref uint e, uint x)
        {
            b = ROT(b, 2); e -= (((b | c) & d) | (b & c)) + x + 0x8f1bbcdc + ROT(a, 5);
        }
        private void SHA1_II_REV(uint a, ref uint b, uint c, uint d, ref uint e, uint x)
        {
            b = ROT(b, 2); e -= (b ^ c ^ d) + x + 0xca62c1d6 + ROT(a, 5);
        }

        #endregion


        #region CRACK_16

        private uint calcNorm16(uint[] hash, ref uint invalidCount, bool onlyValid, bool onlyPrintable, bool noCollisions, ref string result)
        {
            result = "";
            uint a, b, c, d, e, x0, x1, count = 0, invalid;
            uint[] x = new uint[80];

            uint hash0, hash1, hash2, hash3, hash4;
            uint t17a, t17b, t17c, t17d, t17e;
            uint t16a, t16b, t16c, t16d, t16e;

            invalidCount = 0;
            hash0 = hash[0] - 0x67452301;
            hash1 = hash[1] - 0xefcdab89;
            hash2 = hash[2] - 0x98badcfe;
            hash3 = hash[3] - 0x10325476;
            hash4 = hash[4] - 0xc3d2e1f0;

            for (x[3] = 0; x[3] < 32; x[3]++)
            {
                for (x1 = 0; x1 < 5; x1++)
                {
                    x[1] = x1 ^ x[3];
                    for (x[2] = 0; x[2] < 32; x[2]++)
                    {
                        for (x0 = 0; /*x0 <= 5*/; x0++)
                        {
                            x[0] = x0 ^ x[2];
                            x[16] = (uint)1 << (int)x0;
                            x[17] = (uint)1 << (int)x1;
                            x[18] = (uint)1 << (int)x[2];
                            x[19] = (uint)1 << (int)((x[3] ^ x[16]) & 31);
                            x[20] = (uint)1 << (int)((x[17]) & 31);
                            x[21] = (uint)1 << (int)((x[18]) & 31);
                            x[22] = (uint)1 << (int)((x[19]) & 31);
                            x[23] = (uint)1 << (int)((x[20]) & 31);
                            x[24] = (uint)1 << (int)((x[16] ^ x[21]) & 31);
                            x[25] = (uint)1 << (int)((x[17] ^ x[22]) & 31);
                            x[26] = (uint)1 << (int)((x[18] ^ x[23]) & 31);
                            x[27] = (uint)1 << (int)((x[19] ^ x[24]) & 31);
                            x[28] = (uint)1 << (int)((x[20] ^ x[25]) & 31);
                            x[29] = (uint)1 << (int)((x[21] ^ x[26]) & 31);
                            x[30] = (uint)1 << (int)((x[16] ^ x[22] ^ x[27]) & 31);
                            x[31] = (uint)1 << (int)((x[17] ^ x[23] ^ x[28]) & 31);
                            EXPANSION(ref x, 16); EXPANSION(ref x, 17); EXPANSION(ref x, 18); EXPANSION(ref x, 19);
                            EXPANSION(ref x, 20); EXPANSION(ref x, 21); EXPANSION(ref x, 22); EXPANSION(ref x, 23);
                            EXPANSION(ref x, 24); EXPANSION(ref x, 25); EXPANSION(ref x, 26); EXPANSION(ref x, 27);
                            EXPANSION(ref x, 28); EXPANSION(ref x, 29); EXPANSION(ref x, 30); EXPANSION(ref x, 31);
                            EXPANSION(ref x, 32); EXPANSION(ref x, 33); EXPANSION(ref x, 34); EXPANSION(ref x, 35);
                            EXPANSION(ref x, 36); EXPANSION(ref x, 37); EXPANSION(ref x, 38); EXPANSION(ref x, 39);
                            EXPANSION(ref x, 40); EXPANSION(ref x, 41); EXPANSION(ref x, 42); EXPANSION(ref x, 43);
                            EXPANSION(ref x, 44); EXPANSION(ref x, 45); EXPANSION(ref x, 46); EXPANSION(ref x, 47);
                            EXPANSION(ref x, 48); EXPANSION(ref x, 49); EXPANSION(ref x, 50); EXPANSION(ref x, 51);
                            EXPANSION(ref x, 52); EXPANSION(ref x, 53); EXPANSION(ref x, 54); EXPANSION(ref x, 55);
                            EXPANSION(ref x, 56); EXPANSION(ref x, 57); EXPANSION(ref x, 58); EXPANSION(ref x, 59);
                            EXPANSION(ref x, 60); EXPANSION(ref x, 61); EXPANSION(ref x, 62); EXPANSION(ref x, 63);

                            a = hash0;
                            b = hash1;
                            c = hash2;
                            d = hash3;
                            e = hash4;

                            SHA1_II_REV(b, ref c, d, e, ref a, x[79]); SHA1_II_REV(c, ref d, e, a, ref b, x[78]); SHA1_II_REV(d, ref e, a, b, ref c, x[77]); SHA1_II_REV(e, ref a, b, c, ref d, x[76]); SHA1_II_REV(a, ref b, c, d, ref e, x[75]);
                            SHA1_II_REV(b, ref c, d, e, ref a, x[74]); SHA1_II_REV(c, ref d, e, a, ref b, x[73]); SHA1_II_REV(d, ref e, a, b, ref c, x[72]); SHA1_II_REV(e, ref a, b, c, ref d, x[71]); SHA1_II_REV(a, ref b, c, d, ref e, x[70]);
                            SHA1_II_REV(b, ref c, d, e, ref a, x[69]); SHA1_II_REV(c, ref d, e, a, ref b, x[68]); SHA1_II_REV(d, ref e, a, b, ref c, x[67]); SHA1_II_REV(e, ref a, b, c, ref d, x[66]); SHA1_II_REV(a, ref b, c, d, ref e, x[65]);
                            SHA1_II_REV(b, ref c, d, e, ref a, x[64]); SHA1_II_REV(c, ref d, e, a, ref b, x[63]); SHA1_II_REV(d, ref e, a, b, ref c, x[62]); SHA1_II_REV(e, ref a, b, c, ref d, x[61]); SHA1_II_REV(a, ref b, c, d, ref e, x[60]);

                            SHA1_HH_REV(b, ref c, d, e, ref a, x[59]); SHA1_HH_REV(c, ref d, e, a, ref b, x[58]); SHA1_HH_REV(d, ref e, a, b, ref c, x[57]); SHA1_HH_REV(e, ref a, b, c, ref d, x[56]); SHA1_HH_REV(a, ref b, c, d, ref e, x[55]);
                            SHA1_HH_REV(b, ref c, d, e, ref a, x[54]); SHA1_HH_REV(c, ref d, e, a, ref b, x[53]); SHA1_HH_REV(d, ref e, a, b, ref c, x[52]); SHA1_HH_REV(e, ref a, b, c, ref d, x[51]); SHA1_HH_REV(a, ref b, c, d, ref e, x[50]);
                            SHA1_HH_REV(b, ref c, d, e, ref a, x[49]); SHA1_HH_REV(c, ref d, e, a, ref b, x[48]); SHA1_HH_REV(d, ref e, a, b, ref c, x[47]); SHA1_HH_REV(e, ref a, b, c, ref d, x[46]); SHA1_HH_REV(a, ref b, c, d, ref e, x[45]);
                            SHA1_HH_REV(b, ref c, d, e, ref a, x[44]); SHA1_HH_REV(c, ref d, e, a, ref b, x[43]); SHA1_HH_REV(d, ref e, a, b, ref c, x[42]); SHA1_HH_REV(e, ref a, b, c, ref d, x[41]); SHA1_HH_REV(a, ref b, c, d, ref e, x[40]);

                            SHA1_GG_REV(b, ref c, d, e, ref a, x[39]); SHA1_GG_REV(c, ref d, e, a, ref b, x[38]); SHA1_GG_REV(d, ref e, a, b, ref c, x[37]); SHA1_GG_REV(e, ref a, b, c, ref d, x[36]); SHA1_GG_REV(a, ref b, c, d, ref e, x[35]);
                            SHA1_GG_REV(b, ref c, d, e, ref a, x[34]); SHA1_GG_REV(c, ref d, e, a, ref b, x[33]); SHA1_GG_REV(d, ref e, a, b, ref c, x[32]); SHA1_GG_REV(e, ref a, b, c, ref d, x[31]); SHA1_GG_REV(a, ref b, c, d, ref e, x[30]);
                            SHA1_GG_REV(b, ref c, d, e, ref a, x[29]); SHA1_GG_REV(c, ref d, e, a, ref b, x[28]); SHA1_GG_REV(d, ref e, a, b, ref c, x[27]); SHA1_GG_REV(e, ref a, b, c, ref d, x[26]); SHA1_GG_REV(a, ref b, c, d, ref e, x[25]);
                            SHA1_GG_REV(b, ref c, d, e, ref a, x[24]); SHA1_GG_REV(c, ref d, e, a, ref b, x[23]); SHA1_GG_REV(d, ref e, a, b, ref c, x[22]); SHA1_GG_REV(e, ref a, b, c, ref d, x[21]); SHA1_GG_REV(a, ref b, c, d, ref e, x[20]);

                            SHA1_FF_REV(b, ref c, d, e, ref a, x[19]); SHA1_FF_REV(c, ref d, e, a, ref b, x[18]); SHA1_FF_REV(d, ref e, a, b, ref c, x[17]);

                            if (x0 == 5)
                            {
                                break;
                            }

                            SHA1_FF_REV(e, ref a, b, c, ref d, x[16]);
                            SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); // 4

                            if (a == 0x59d148c0 && finishRev16Norm(b, c, d, e, x) > 0)
                            {
                                invalid = outputPassword(x, 4, onlyValid, onlyPrintable, hash, ref result);
                                invalidCount += invalid;
                                count++;
                                if (invalid == 0 && noCollisions)
                                {
                                    return count - invalidCount;
                                }
                            }
                        }

                        SHA1_FF_REV(e, ref a, b, c, ref d, 0); // 16
                        t16a = a;
                        t16b = b;
                        t16c = c;
                        t16d = d;
                        t16e = e;
                        // x[16] = (1 << 5) to (1 << 31)
                        for (; x[16] != 0; x[16] <<= 1, x[0] = ++x0 ^ x[2])
                        {
                            a = t16a;
                            b = t16b;
                            c = t16c;
                            d = t16d - x[16];
                            e = t16e;

                            SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); // 4

                            if (a == 0x59d148c0 && finishRev16Norm(b, c, d, e, x) > 0)
                            {
                                invalid = outputPassword(x, 4, onlyValid, onlyPrintable, hash, ref result);
                                invalidCount += invalid;
                                count++;
                                if (invalid == 0 && noCollisions)
                                {
                                    return count - invalidCount;
                                }
                            }
                        }
                    }
                }
                x[1] = 5 ^ x[3];
                for (x[2] = 0; x[2] < 32; x[2]++)
                {
                    for (x0 = 0; /*x0 <= 5*/; x0++)
                    {
                        x[0] = x0 ^ x[2];
                        x[16] = (uint)1 << (int)x0;
                        x[17] = (uint)1 << (int)5;
                        x[18] = (uint)1 << (int)x[2];
                        x[19] = (uint)1 << (int)((x[3] ^ x[16]) & 31);
                        x[20] = 1;
                        x[21] = (uint)1 << (int)((x[18]) & 31);
                        x[22] = (uint)1 << (int)((x[19]) & 31);
                        x[23] = (uint)1 << (int)((x[20]) & 31);
                        x[24] = (uint)1 << (int)((x[16] ^ x[21]) & 31);
                        x[25] = (uint)1 << (int)((x[22]) & 31);
                        x[26] = (uint)1 << (int)((x[18] ^ x[23]) & 31);
                        x[27] = (uint)1 << (int)((x[19] ^ x[24]) & 31);
                        x[28] = (uint)1 << (int)((x[20] ^ x[25]) & 31);
                        x[29] = (uint)1 << (int)((x[21] ^ x[26]) & 31);
                        x[30] = (uint)1 << (int)((x[16] ^ x[22] ^ x[27]) & 31);
                        x[31] = (uint)1 << (int)((x[23] ^ x[28]) & 31);
                        EXPANSION(ref x, 16); EXPANSION(ref x, 17); EXPANSION(ref x, 18); EXPANSION(ref x, 19);
                        EXPANSION(ref x, 20); EXPANSION(ref x, 21); EXPANSION(ref x, 22); EXPANSION(ref x, 23);
                        EXPANSION(ref x, 24); EXPANSION(ref x, 25); EXPANSION(ref x, 26); EXPANSION(ref x, 27);
                        EXPANSION(ref x, 28); EXPANSION(ref x, 29); EXPANSION(ref x, 30); EXPANSION(ref x, 31);
                        EXPANSION(ref x, 32); EXPANSION(ref x, 33); EXPANSION(ref x, 34); EXPANSION(ref x, 35);
                        EXPANSION(ref x, 36); EXPANSION(ref x, 37); EXPANSION(ref x, 38); EXPANSION(ref x, 39);
                        EXPANSION(ref x, 40); EXPANSION(ref x, 41); EXPANSION(ref x, 42); EXPANSION(ref x, 43);
                        EXPANSION(ref x, 44); EXPANSION(ref x, 45); EXPANSION(ref x, 46); EXPANSION(ref x, 47);
                        EXPANSION(ref x, 48); EXPANSION(ref x, 49); EXPANSION(ref x, 50); EXPANSION(ref x, 51);
                        EXPANSION(ref x, 52); EXPANSION(ref x, 53); EXPANSION(ref x, 54); EXPANSION(ref x, 55);
                        EXPANSION(ref x, 56); EXPANSION(ref x, 57); EXPANSION(ref x, 58); EXPANSION(ref x, 59);
                        EXPANSION(ref x, 60); EXPANSION(ref x, 61); EXPANSION(ref x, 62); EXPANSION(ref x, 63);

                        a = hash0;
                        b = hash1;
                        c = hash2;
                        d = hash3;
                        e = hash4;

                        SHA1_II_REV(b, ref c, d, e, ref a, x[79]); SHA1_II_REV(c, ref d, e, a, ref b, x[78]); SHA1_II_REV(d, ref e, a, b, ref c, x[77]); SHA1_II_REV(e, ref a, b, c, ref d, x[76]); SHA1_II_REV(a, ref b, c, d, ref e, x[75]);
                        SHA1_II_REV(b, ref c, d, e, ref a, x[74]); SHA1_II_REV(c, ref d, e, a, ref b, x[73]); SHA1_II_REV(d, ref e, a, b, ref c, x[72]); SHA1_II_REV(e, ref a, b, c, ref d, x[71]); SHA1_II_REV(a, ref b, c, d, ref e, x[70]);
                        SHA1_II_REV(b, ref c, d, e, ref a, x[69]); SHA1_II_REV(c, ref d, e, a, ref b, x[68]); SHA1_II_REV(d, ref e, a, b, ref c, x[67]); SHA1_II_REV(e, ref a, b, c, ref d, x[66]); SHA1_II_REV(a, ref b, c, d, ref e, x[65]);
                        SHA1_II_REV(b, ref c, d, e, ref a, x[64]); SHA1_II_REV(c, ref d, e, a, ref b, x[63]); SHA1_II_REV(d, ref e, a, b, ref c, x[62]); SHA1_II_REV(e, ref a, b, c, ref d, x[61]); SHA1_II_REV(a, ref b, c, d, ref e, x[60]);

                        SHA1_HH_REV(b, ref c, d, e, ref a, x[59]); SHA1_HH_REV(c, ref d, e, a, ref b, x[58]); SHA1_HH_REV(d, ref e, a, b, ref c, x[57]); SHA1_HH_REV(e, ref a, b, c, ref d, x[56]); SHA1_HH_REV(a, ref b, c, d, ref e, x[55]);
                        SHA1_HH_REV(b, ref c, d, e, ref a, x[54]); SHA1_HH_REV(c, ref d, e, a, ref b, x[53]); SHA1_HH_REV(d, ref e, a, b, ref c, x[52]); SHA1_HH_REV(e, ref a, b, c, ref d, x[51]); SHA1_HH_REV(a, ref b, c, d, ref e, x[50]);
                        SHA1_HH_REV(b, ref c, d, e, ref a, x[49]); SHA1_HH_REV(c, ref d, e, a, ref b, x[48]); SHA1_HH_REV(d, ref e, a, b, ref c, x[47]); SHA1_HH_REV(e, ref a, b, c, ref d, x[46]); SHA1_HH_REV(a, ref b, c, d, ref e, x[45]);
                        SHA1_HH_REV(b, ref c, d, e, ref a, x[44]); SHA1_HH_REV(c, ref d, e, a, ref b, x[43]); SHA1_HH_REV(d, ref e, a, b, ref c, x[42]); SHA1_HH_REV(e, ref a, b, c, ref d, x[41]); SHA1_HH_REV(a, ref b, c, d, ref e, x[40]);

                        SHA1_GG_REV(b, ref c, d, e, ref a, x[39]); SHA1_GG_REV(c, ref d, e, a, ref b, x[38]); SHA1_GG_REV(d, ref e, a, b, ref c, x[37]); SHA1_GG_REV(e, ref a, b, c, ref d, x[36]); SHA1_GG_REV(a, ref b, c, d, ref e, x[35]);
                        SHA1_GG_REV(b, ref c, d, e, ref a, x[34]); SHA1_GG_REV(c, ref d, e, a, ref b, x[33]); SHA1_GG_REV(d, ref e, a, b, ref c, x[32]); SHA1_GG_REV(e, ref a, b, c, ref d, x[31]); SHA1_GG_REV(a, ref b, c, d, ref e, x[30]);
                        SHA1_GG_REV(b, ref c, d, e, ref a, x[29]); SHA1_GG_REV(c, ref d, e, a, ref b, x[28]); SHA1_GG_REV(d, ref e, a, b, ref c, x[27]); SHA1_GG_REV(e, ref a, b, c, ref d, x[26]); SHA1_GG_REV(a, ref b, c, d, ref e, x[25]);
                        SHA1_GG_REV(b, ref c, d, e, ref a, x[24]); SHA1_GG_REV(c, ref d, e, a, ref b, x[23]); SHA1_GG_REV(d, ref e, a, b, ref c, x[22]); SHA1_GG_REV(e, ref a, b, c, ref d, x[21]); SHA1_GG_REV(a, ref b, c, d, ref e, x[20]);

                        SHA1_FF_REV(b, ref c, d, e, ref a, x[19]); SHA1_FF_REV(c, ref d, e, a, ref b, x[18]);

                        SHA1_FF_REV(d, ref e, a, b, ref c, 0); // 17
                        t17a = a;
                        t17b = b;
                        t17c = c;
                        t17d = d;
                        t17e = e;

                        if (x0 == 5)
                        {
                            break;
                        }

                        // x[17] = (1 << 5) to (1 << 31)
                        for (; x[17] != 0; x[17] <<= 1, x[1] = ++x1 ^ x[3])
                        {
                            a = t17a;
                            b = t17b;
                            c = t17c - x[17];
                            d = t17d;
                            e = t17e;

                            SHA1_FF_REV(e, ref a, b, c, ref d, x[16]);

                            SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); // 4                        

                            if (a == 0x59d148c0 && finishRev16Norm(b, c, d, e, x) > 0)
                            {
                                invalid = outputPassword(x, 4, onlyValid, onlyPrintable, hash, ref result);
                                invalidCount += invalid;
                                count++;
                                if (invalid == 0 && noCollisions)
                                {
                                    return count - invalidCount;
                                }
                            }
                        }
                        x1 = 5;
                        x[1] = 5 ^ x[3];
                    }

                    // x[17] = (1 << 5) to (1 << 31)
                    for (; x[17] != 0; x[17] <<= 1, x[1] = ++x1 ^ x[3])
                    {
                        a = t17a;
                        b = t17b;
                        c = t17c - x[17];
                        d = t17d;
                        e = t17e;

                        SHA1_FF_REV(e, ref a, b, c, ref d, 0); // 16
                        t16a = a;
                        t16b = b;
                        t16c = c;
                        t16d = d;
                        t16e = e;
                        // x[16] = (1 << 5) to (1 << 31)
                        for (; x[16] != 0; x[16] <<= 1, x[0] = ++x0 ^ x[2])
                        {
                            a = t16a;
                            b = t16b;
                            c = t16c;
                            d = t16d - x[16];
                            e = t16e;

                            SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                            SHA1_FF_REV(b, ref c, d, e, ref a, 0); // 4

                            if (a == 0x59d148c0 && finishRev16Norm(b, c, d, e, x) > 0)
                            {
                                invalid = outputPassword(x, 4, onlyValid, onlyPrintable, hash, ref result);
                                invalidCount += invalid;
                                count++;
                                if (invalid == 0 && noCollisions)
                                {
                                    return count - invalidCount;
                                }
                            }
                        }
                        x0 = 5;
                        x[0] = 5 ^ x[2];
                        x[16] = 1 << 5;
                    }
                    x1 = 5;
                    x[1] = 5 ^ x[3];
                }
            }
            return count - invalidCount;
        }


        uint finishRev16Norm(uint t, uint a, uint b, uint c, uint[] x)
        {
            //x[4] = ROT(c, 32 - 30) - (0xc3d2e1f0 + ((0xefcdab89 & 0x98badcfe) | ((~0xefcdab89) & 0x10325476)) + ROT(0x67452301, 5) + 0x5a827999);
            x[4] = ROT(c, 32 - 30) - 0x9fb498b3;
            if ((x[4] & 31) == x[0])
            {
                b = ROT(b, 2);
                //x[5] = b - (0x10325476 + ((0x67452301 & 0x7bf36ae2) | ((~0x67452301) & 0x98badcfe)) + ROT(ROT(c, 32 - 30), 5) + 0x5a827999);
                x[5] = b - (ROT(ROT(c, 32 - 30), 5) + 0x66b0cd0d);
                if ((x[5] & 31) == x[1])
                {
                    //x[6] = a - (0x98badcfe + ((ROT(c, 32 - 30) & 0x59d148c0) | ((~ROT(c, 32 - 30)) & 0x7bf36ae2)) + ROT(b, 5) + 0x5a827999);
                    x[6] = a - (((ROT(c, 32 - 30) & 0x59d148c0) | ((~ROT(c, 32 - 30)) & 0x7bf36ae2)) + ROT(b, 5) + 0xf33d5697);
                    if ((x[6] & 31) == x[2])
                    {
                        //x[7] = t - (0x7bf36ae2 + ((b & c) | ((~b) & 0x59d148c0)) + ROT(a, 5) + 0x5a827999);
                        x[7] = t - (((b & c) | ((~b) & 0x59d148c0)) + ROT(a, 5) + 0xd675e47b);
                        if ((x[7] & 31) == x[3])
                        {
                            x[8] = 0;
                            x[9] = 0;
                            return 1;
                        }
                    }
                }
            }
            return 0;
        }

        #endregion

        #region CRACK_20

        private uint calcNorm20(uint[] hash, ref uint invalidCount, bool onlyValid, bool onlyPrintable, bool noCollisions, ref string result)
        {
            result = "";
            uint a, b, c, d, e, x0, x1, count = 0, invalid;
            uint[] x = new uint[80];

            uint hash0, hash1, hash2, hash3, hash4;
            uint t17a, t17b, t17c, t17d, t17e;
            uint t16a, t16b, t16c, t16d, t16e;

            invalidCount = 0;
            hash0 = hash[0] - 0x67452301;
            hash1 = hash[1] - 0xefcdab89;
            hash2 = hash[2] - 0x98badcfe;
            hash3 = hash[3] - 0x10325476;
            hash4 = hash[4] - 0xc3d2e1f0;

            for (x[4] = 0; x[4] < 32; x[4]++)
            {
                for (x[3] = 0; x[3] < 32; x[3]++)
                {
                    for (x1 = 0; x1 < 5; x1++)
                    {
                        x[1] = x1 ^ x[3];
                        for (x[2] = 0; x[2] < 32; x[2]++)
                        {
                            for (x0 = 0; /*x0 <= 5*/; x0++)
                            {
                                x[0] = x0 ^ x[2];
                                x[16] = (uint)1 << (int)x0;
                                x[17] = (uint)1 << (int)x1;
                                x[18] = (uint)1 << (int)(x[2] ^ x[4]);
                                x[19] = (uint)1 << (int)((x[3] ^ x[16]) & 31);
                                x[20] = (uint)1 << (int)((x[4] ^ x[17]) & 31);
                                x[21] = (uint)1 << (int)((x[18]) & 31);
                                x[22] = (uint)1 << (int)((x[19]) & 31);
                                x[23] = (uint)1 << (int)((x[20]) & 31);
                                x[24] = (uint)1 << (int)((x[16] ^ x[21]) & 31);
                                x[25] = (uint)1 << (int)((x[17] ^ x[22]) & 31);
                                x[26] = (uint)1 << (int)((x[18] ^ x[23]) & 31);
                                x[27] = (uint)1 << (int)((x[19] ^ x[24]) & 31);
                                x[28] = (uint)1 << (int)((x[20] ^ x[25]) & 31);
                                x[29] = (uint)1 << (int)((x[21] ^ x[26]) & 31);
                                x[30] = (uint)1 << (int)((x[16] ^ x[22] ^ x[27]) & 31);
                                x[31] = (uint)1 << (int)((x[17] ^ x[23] ^ x[28]) & 31);
                                EXPANSION(ref x, 16); EXPANSION(ref x, 17); EXPANSION(ref x, 18); EXPANSION(ref x, 19);
                                EXPANSION(ref x, 20); EXPANSION(ref x, 21); EXPANSION(ref x, 22); EXPANSION(ref x, 23);
                                EXPANSION(ref x, 24); EXPANSION(ref x, 25); EXPANSION(ref x, 26); EXPANSION(ref x, 27);
                                EXPANSION(ref x, 28); EXPANSION(ref x, 29); EXPANSION(ref x, 30); EXPANSION(ref x, 31);
                                EXPANSION(ref x, 32); EXPANSION(ref x, 33); EXPANSION(ref x, 34); EXPANSION(ref x, 35);
                                EXPANSION(ref x, 36); EXPANSION(ref x, 37); EXPANSION(ref x, 38); EXPANSION(ref x, 39);
                                EXPANSION(ref x, 40); EXPANSION(ref x, 41); EXPANSION(ref x, 42); EXPANSION(ref x, 43);
                                EXPANSION(ref x, 44); EXPANSION(ref x, 45); EXPANSION(ref x, 46); EXPANSION(ref x, 47);
                                EXPANSION(ref x, 48); EXPANSION(ref x, 49); EXPANSION(ref x, 50); EXPANSION(ref x, 51);
                                EXPANSION(ref x, 52); EXPANSION(ref x, 53); EXPANSION(ref x, 54); EXPANSION(ref x, 55);
                                EXPANSION(ref x, 56); EXPANSION(ref x, 57); EXPANSION(ref x, 58); EXPANSION(ref x, 59);
                                EXPANSION(ref x, 60); EXPANSION(ref x, 61); EXPANSION(ref x, 62); EXPANSION(ref x, 63);

                                a = hash0;
                                b = hash1;
                                c = hash2;
                                d = hash3;
                                e = hash4;

                                SHA1_II_REV(b, ref c, d, e, ref a, x[79]); SHA1_II_REV(c, ref d, e, a, ref b, x[78]); SHA1_II_REV(d, ref e, a, b, ref c, x[77]); SHA1_II_REV(e, ref a, b, c, ref d, x[76]); SHA1_II_REV(a, ref b, c, d, ref e, x[75]);
                                SHA1_II_REV(b, ref c, d, e, ref a, x[74]); SHA1_II_REV(c, ref d, e, a, ref b, x[73]); SHA1_II_REV(d, ref e, a, b, ref c, x[72]); SHA1_II_REV(e, ref a, b, c, ref d, x[71]); SHA1_II_REV(a, ref b, c, d, ref e, x[70]);
                                SHA1_II_REV(b, ref c, d, e, ref a, x[69]); SHA1_II_REV(c, ref d, e, a, ref b, x[68]); SHA1_II_REV(d, ref e, a, b, ref c, x[67]); SHA1_II_REV(e, ref a, b, c, ref d, x[66]); SHA1_II_REV(a, ref b, c, d, ref e, x[65]);
                                SHA1_II_REV(b, ref c, d, e, ref a, x[64]); SHA1_II_REV(c, ref d, e, a, ref b, x[63]); SHA1_II_REV(d, ref e, a, b, ref c, x[62]); SHA1_II_REV(e, ref a, b, c, ref d, x[61]); SHA1_II_REV(a, ref b, c, d, ref e, x[60]);

                                SHA1_HH_REV(b, ref c, d, e, ref a, x[59]); SHA1_HH_REV(c, ref d, e, a, ref b, x[58]); SHA1_HH_REV(d, ref e, a, b, ref c, x[57]); SHA1_HH_REV(e, ref a, b, c, ref d, x[56]); SHA1_HH_REV(a, ref b, c, d, ref e, x[55]);
                                SHA1_HH_REV(b, ref c, d, e, ref a, x[54]); SHA1_HH_REV(c, ref d, e, a, ref b, x[53]); SHA1_HH_REV(d, ref e, a, b, ref c, x[52]); SHA1_HH_REV(e, ref a, b, c, ref d, x[51]); SHA1_HH_REV(a, ref b, c, d, ref e, x[50]);
                                SHA1_HH_REV(b, ref c, d, e, ref a, x[49]); SHA1_HH_REV(c, ref d, e, a, ref b, x[48]); SHA1_HH_REV(d, ref e, a, b, ref c, x[47]); SHA1_HH_REV(e, ref a, b, c, ref d, x[46]); SHA1_HH_REV(a, ref b, c, d, ref e, x[45]);
                                SHA1_HH_REV(b, ref c, d, e, ref a, x[44]); SHA1_HH_REV(c, ref d, e, a, ref b, x[43]); SHA1_HH_REV(d, ref e, a, b, ref c, x[42]); SHA1_HH_REV(e, ref a, b, c, ref d, x[41]); SHA1_HH_REV(a, ref b, c, d, ref e, x[40]);

                                SHA1_GG_REV(b, ref c, d, e, ref a, x[39]); SHA1_GG_REV(c, ref d, e, a, ref b, x[38]); SHA1_GG_REV(d, ref e, a, b, ref c, x[37]); SHA1_GG_REV(e, ref a, b, c, ref d, x[36]); SHA1_GG_REV(a, ref b, c, d, ref e, x[35]);
                                SHA1_GG_REV(b, ref c, d, e, ref a, x[34]); SHA1_GG_REV(c, ref d, e, a, ref b, x[33]); SHA1_GG_REV(d, ref e, a, b, ref c, x[32]); SHA1_GG_REV(e, ref a, b, c, ref d, x[31]); SHA1_GG_REV(a, ref b, c, d, ref e, x[30]);
                                SHA1_GG_REV(b, ref c, d, e, ref a, x[29]); SHA1_GG_REV(c, ref d, e, a, ref b, x[28]); SHA1_GG_REV(d, ref e, a, b, ref c, x[27]); SHA1_GG_REV(e, ref a, b, c, ref d, x[26]); SHA1_GG_REV(a, ref b, c, d, ref e, x[25]);
                                SHA1_GG_REV(b, ref c, d, e, ref a, x[24]); SHA1_GG_REV(c, ref d, e, a, ref b, x[23]); SHA1_GG_REV(d, ref e, a, b, ref c, x[22]); SHA1_GG_REV(e, ref a, b, c, ref d, x[21]); SHA1_GG_REV(a, ref b, c, d, ref e, x[20]);

                                SHA1_FF_REV(b, ref c, d, e, ref a, x[19]); SHA1_FF_REV(c, ref d, e, a, ref b, x[18]); SHA1_FF_REV(d, ref e, a, b, ref c, x[17]);

                                if (x0 == 5)
                                {
                                    break;
                                }

                                SHA1_FF_REV(e, ref a, b, c, ref d, x[16]);
                                SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                // 5

                                if (finishRev20Norm(a, b, c, d, e, x) > 0)
                                {
                                    invalid = outputPassword(x, 5, onlyValid, onlyPrintable, hash, ref result);
                                    invalidCount += invalid;
                                    count++;
                                    if (invalid == 0 && noCollisions)
                                    {
                                        return count - invalidCount;
                                    }
                                }
                            }

                            SHA1_FF_REV(e, ref a, b, c, ref d, 0); // 16
                            t16a = a;
                            t16b = b;
                            t16c = c;
                            t16d = d;
                            t16e = e;
                            // x[16] = (1 << 5) to (1 << 31)
                            for (; x[16] != 0; x[16] <<= 1, x[0] = ++x0 ^ x[2])
                            {
                                a = t16a;
                                b = t16b;
                                c = t16c;
                                d = t16d - x[16];
                                e = t16e;

                                SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                // 5

                                if (finishRev20Norm(a, b, c, d, e, x) > 0)
                                {
                                    invalid = outputPassword(x, 5, onlyValid, onlyPrintable, hash, ref result);
                                    invalidCount += invalid;
                                    count++;
                                    if (invalid == 0 && noCollisions)
                                    {
                                        return count - invalidCount;
                                    }
                                }
                            }
                        }
                    }
                    x[1] = 5 ^ x[3];
                    for (x[2] = 0; x[2] < 32; x[2]++)
                    {
                        for (x0 = 0; /*x0 <= 5*/; x0++)
                        {
                            x[0] = x0 ^ x[2];
                            x[16] = (uint)1 << (int)x0;
                            x[17] = (uint)1 << (int)5;
                            x[18] = (uint)1 << (int)(x[2] ^ x[4]);
                            x[19] = (uint)1 << (int)((x[3] ^ x[16]) & 31);
                            x[20] = (uint)1 << (int)((x[4]) & 31);
                            x[21] = (uint)1 << (int)((x[18]) & 31);
                            x[22] = (uint)1 << (int)((x[19]) & 31);
                            x[23] = (uint)1 << (int)((x[20]) & 31);
                            x[24] = (uint)1 << (int)((x[16] ^ x[21]) & 31);
                            x[25] = (uint)1 << (int)((x[22]) & 31);
                            x[26] = (uint)1 << (int)((x[18] ^ x[23]) & 31);
                            x[27] = (uint)1 << (int)((x[19] ^ x[24]) & 31);
                            x[28] = (uint)1 << (int)((x[20] ^ x[25]) & 31);
                            x[29] = (uint)1 << (int)((x[21] ^ x[26]) & 31);
                            x[30] = (uint)1 << (int)((x[16] ^ x[22] ^ x[27]) & 31);
                            x[31] = (uint)1 << (int)((x[23] ^ x[28]) & 31);
                            EXPANSION(ref x, 16); EXPANSION(ref x, 17); EXPANSION(ref x, 18); EXPANSION(ref x, 19);
                            EXPANSION(ref x, 20); EXPANSION(ref x, 21); EXPANSION(ref x, 22); EXPANSION(ref x, 23);
                            EXPANSION(ref x, 24); EXPANSION(ref x, 25); EXPANSION(ref x, 26); EXPANSION(ref x, 27);
                            EXPANSION(ref x, 28); EXPANSION(ref x, 29); EXPANSION(ref x, 30); EXPANSION(ref x, 31);
                            EXPANSION(ref x, 32); EXPANSION(ref x, 33); EXPANSION(ref x, 34); EXPANSION(ref x, 35);
                            EXPANSION(ref x, 36); EXPANSION(ref x, 37); EXPANSION(ref x, 38); EXPANSION(ref x, 39);
                            EXPANSION(ref x, 40); EXPANSION(ref x, 41); EXPANSION(ref x, 42); EXPANSION(ref x, 43);
                            EXPANSION(ref x, 44); EXPANSION(ref x, 45); EXPANSION(ref x, 46); EXPANSION(ref x, 47);
                            EXPANSION(ref x, 48); EXPANSION(ref x, 49); EXPANSION(ref x, 50); EXPANSION(ref x, 51);
                            EXPANSION(ref x, 52); EXPANSION(ref x, 53); EXPANSION(ref x, 54); EXPANSION(ref x, 55);
                            EXPANSION(ref x, 56); EXPANSION(ref x, 57); EXPANSION(ref x, 58); EXPANSION(ref x, 59);
                            EXPANSION(ref x, 60); EXPANSION(ref x, 61); EXPANSION(ref x, 62); EXPANSION(ref x, 63);

                            a = hash0;
                            b = hash1;
                            c = hash2;
                            d = hash3;
                            e = hash4;

                            SHA1_II_REV(b, ref c, d, e, ref a, x[79]); SHA1_II_REV(c, ref d, e, a, ref b, x[78]); SHA1_II_REV(d, ref e, a, b, ref c, x[77]); SHA1_II_REV(e, ref a, b, c, ref d, x[76]); SHA1_II_REV(a, ref b, c, d, ref e, x[75]);
                            SHA1_II_REV(b, ref c, d, e, ref a, x[74]); SHA1_II_REV(c, ref d, e, a, ref b, x[73]); SHA1_II_REV(d, ref e, a, b, ref c, x[72]); SHA1_II_REV(e, ref a, b, c, ref d, x[71]); SHA1_II_REV(a, ref b, c, d, ref e, x[70]);
                            SHA1_II_REV(b, ref c, d, e, ref a, x[69]); SHA1_II_REV(c, ref d, e, a, ref b, x[68]); SHA1_II_REV(d, ref e, a, b, ref c, x[67]); SHA1_II_REV(e, ref a, b, c, ref d, x[66]); SHA1_II_REV(a, ref b, c, d, ref e, x[65]);
                            SHA1_II_REV(b, ref c, d, e, ref a, x[64]); SHA1_II_REV(c, ref d, e, a, ref b, x[63]); SHA1_II_REV(d, ref e, a, b, ref c, x[62]); SHA1_II_REV(e, ref a, b, c, ref d, x[61]); SHA1_II_REV(a, ref b, c, d, ref e, x[60]);

                            SHA1_HH_REV(b, ref c, d, e, ref a, x[59]); SHA1_HH_REV(c, ref d, e, a, ref b, x[58]); SHA1_HH_REV(d, ref e, a, b, ref c, x[57]); SHA1_HH_REV(e, ref a, b, c, ref d, x[56]); SHA1_HH_REV(a, ref b, c, d, ref e, x[55]);
                            SHA1_HH_REV(b, ref c, d, e, ref a, x[54]); SHA1_HH_REV(c, ref d, e, a, ref b, x[53]); SHA1_HH_REV(d, ref e, a, b, ref c, x[52]); SHA1_HH_REV(e, ref a, b, c, ref d, x[51]); SHA1_HH_REV(a, ref b, c, d, ref e, x[50]);
                            SHA1_HH_REV(b, ref c, d, e, ref a, x[49]); SHA1_HH_REV(c, ref d, e, a, ref b, x[48]); SHA1_HH_REV(d, ref e, a, b, ref c, x[47]); SHA1_HH_REV(e, ref a, b, c, ref d, x[46]); SHA1_HH_REV(a, ref b, c, d, ref e, x[45]);
                            SHA1_HH_REV(b, ref c, d, e, ref a, x[44]); SHA1_HH_REV(c, ref d, e, a, ref b, x[43]); SHA1_HH_REV(d, ref e, a, b, ref c, x[42]); SHA1_HH_REV(e, ref a, b, c, ref d, x[41]); SHA1_HH_REV(a, ref b, c, d, ref e, x[40]);

                            SHA1_GG_REV(b, ref c, d, e, ref a, x[39]); SHA1_GG_REV(c, ref d, e, a, ref b, x[38]); SHA1_GG_REV(d, ref e, a, b, ref c, x[37]); SHA1_GG_REV(e, ref a, b, c, ref d, x[36]); SHA1_GG_REV(a, ref b, c, d, ref e, x[35]);
                            SHA1_GG_REV(b, ref c, d, e, ref a, x[34]); SHA1_GG_REV(c, ref d, e, a, ref b, x[33]); SHA1_GG_REV(d, ref e, a, b, ref c, x[32]); SHA1_GG_REV(e, ref a, b, c, ref d, x[31]); SHA1_GG_REV(a, ref b, c, d, ref e, x[30]);
                            SHA1_GG_REV(b, ref c, d, e, ref a, x[29]); SHA1_GG_REV(c, ref d, e, a, ref b, x[28]); SHA1_GG_REV(d, ref e, a, b, ref c, x[27]); SHA1_GG_REV(e, ref a, b, c, ref d, x[26]); SHA1_GG_REV(a, ref b, c, d, ref e, x[25]);
                            SHA1_GG_REV(b, ref c, d, e, ref a, x[24]); SHA1_GG_REV(c, ref d, e, a, ref b, x[23]); SHA1_GG_REV(d, ref e, a, b, ref c, x[22]); SHA1_GG_REV(e, ref a, b, c, ref d, x[21]); SHA1_GG_REV(a, ref b, c, d, ref e, x[20]);

                            SHA1_FF_REV(b, ref c, d, e, ref a, x[19]); SHA1_FF_REV(c, ref d, e, a, ref b, x[18]);

                            SHA1_FF_REV(d, ref e, a, b, ref c, 0); // 17
                            t17a = a;
                            t17b = b;
                            t17c = c;
                            t17d = d;
                            t17e = e;

                            if (x0 == 5)
                            {
                                break;
                            }

                            // x[17] = (1 << 5) to (1 << 31)
                            for (; x[17] != 0; x[17] <<= 1, x[1] = ++x1 ^ x[3])
                            {
                                a = t17a;
                                b = t17b;
                                c = t17c - x[17];
                                d = t17d;
                                e = t17e;

                                SHA1_FF_REV(e, ref a, b, c, ref d, x[16]);

                                SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                // 5                        

                                if (finishRev20Norm(a, b, c, d, e, x) > 0)
                                {
                                    invalid = outputPassword(x, 5, onlyValid, onlyPrintable, hash, ref result);
                                    invalidCount += invalid;
                                    count++;
                                    if (invalid == 0 && noCollisions)
                                    {
                                        return count - invalidCount;
                                    }
                                }
                            }
                            x1 = 5;
                            x[1] = 5 ^ x[3];
                        }

                        // x[17] = (1 << 5) to (1 << 31)
                        for (; x[17] != 0; x[17] <<= 1, x[1] = ++x1 ^ x[3])
                        {
                            a = t17a;
                            b = t17b;
                            c = t17c - x[17];
                            d = t17d;
                            e = t17e;

                            SHA1_FF_REV(e, ref a, b, c, ref d, 0); // 16
                            t16a = a;
                            t16b = b;
                            t16c = c;
                            t16d = d;
                            t16e = e;
                            // x[16] = (1 << 5) to (1 << 31)
                            for (; x[16] != 0; x[16] <<= 1, x[0] = ++x0 ^ x[2])
                            {
                                a = t16a;
                                b = t16b;
                                c = t16c;
                                d = t16d - x[16];
                                e = t16e;

                                SHA1_FF_REV(a, ref b, c, d, ref e, 0); // 15
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                SHA1_FF_REV(b, ref c, d, e, ref a, 0); SHA1_FF_REV(c, ref d, e, a, ref b, 0); SHA1_FF_REV(d, ref e, a, b, ref c, 0); SHA1_FF_REV(e, ref a, b, c, ref d, 0); SHA1_FF_REV(a, ref b, c, d, ref e, 0);
                                // 5

                                if (finishRev20Norm(a, b, c, d, e, x) > 0)
                                {
                                    invalid = outputPassword(x, 5, onlyValid, onlyPrintable, hash, ref result);
                                    invalidCount += invalid;
                                    count++;
                                    if (invalid == 0 && noCollisions)
                                    {
                                        return count - invalidCount;
                                    }
                                }
                            }
                            x0 = 5;
                            x[0] = 5 ^ x[2];
                            x[16] = 1 << 5;
                        }
                        x1 = 5;
                        x[1] = 5 ^ x[3];
                    }
                }
            }
            return count - invalidCount;
        }

        uint finishRev20Norm(uint t, uint a, uint b, uint c, uint d, uint[] x)
        {
            //x[5] = ROT(d, 32 - 30) - (0xc3d2e1f0 + ((0xefcdab89 & 0x98badcfe) | ((~0xefcdab89) & 0x10325476)) + ROT(0x67452301, 5) + 0x5a827999);
            x[5] = ROT(d, 32 - 30) - 0x9fb498b3;
            if ((x[5] & 31) == x[0])
            {
                //x[6] = ROT(c, 32 - 30) - (0x10325476 + ((0x67452301 & 0x7bf36ae2) | ((~0x67452301) & 0x98badcfe)) + ROT(ROT(d, 32 - 30), 5) + 0x5a827999);
                x[6] = ROT(c, 32 - 30) - (ROT(ROT(d, 32 - 30), 5) + 0x66b0cd0d);
                if ((x[6] & 31) == x[1])
                {
                    b = ROT(b, 32 - 30);
                    //x[7] = b - (0x98badcfe + ((ROT(d, 32 - 30) & 0x59d148c0) | ((~ROT(d, 32 - 30)) & 0x7bf36ae2)) + ROT(ROT(c, 32 - 30), 5) + 0x5a827999);
                    x[7] = b - (((ROT(d, 32 - 30) & 0x59d148c0) | ((~ROT(d, 32 - 30)) & 0x7bf36ae2)) + ROT(ROT(c, 32 - 30), 5) + 0xf33d5697);
                    if ((x[7] & 31) == x[2])
                    {
                        //x[8] = a - (0x7bf36ae2 + ((ROT(c, 32 - 30) & d) | ((~ROT(c, 32 - 30)) & 0x59d148c0)) + ROT(b, 5) + 0x5a827999);
                        x[8] = a - (((ROT(c, 32 - 30) & d) | ((~ROT(c, 32 - 30)) & 0x59d148c0)) + ROT(b, 5) + 0xd675e47b);
                        if ((x[8] & 31) == x[3])
                        {
                            //x[9] = t - (0x59d148c0 + ((b & c) | ((~b) & d)) + ROT(a, 5) + 0x5a827999);
                            x[9] = t - (((b & c) | ((~b) & d)) + ROT(a, 5) + 0xb453c259);
                            if ((x[9] & 31) == x[4])
                            {
                                x[10] = 0;
                                return 1;
                            }
                        }
                    }
                }
            }
            return 0;
        }

        #endregion


        uint outputPassword(uint[] x, int offset, bool onlyValid, bool onlyPrintable, uint[] hash, ref string result)
        {
            int len = 20;
            uint last = x[4 + offset];
            char ch;

            if (x[4 + offset] == 0)
            {
                len -= 4;
                last = x[3 + offset];
                if (x[3 + offset] == 0)
                {
                    len -= 4;
                    last = x[2 + offset];
                    if (x[2 + offset] == 0)
                    {
                        len -= 4;
                        last = x[1 + offset];
                        if (x[1 + offset] == 0)
                        {
                            len -= 4;
                            last = x[0 + offset];
                        }
                    }
                }
            }
            if (last == 0)
            {
                result += "";
                //fprintf(fout, "%08x%08x%08x%08x%08x::\n", hash[0], hash[1], hash[2], hash[3], hash[4]);
                return 0;
            }
            if ((last & 0xff000000) == 0)
            {
                len--;
                if ((last & 0x00ff0000) == 0)
                {
                    len--;
                    if ((last & 0x0000ff00) == 0)
                    {
                        len--;
                    }
                }
            }

            List<byte> bytes = new List<byte>();
            for (var i = offset; i < x.Length; i++)
            {
                if (x[i] == 0)
                    break;
                bytes.AddRange(BitConverter.GetBytes(x[i]));
            }

            var str = Encoding.UTF8.GetString(bytes.GetRange(0, len).ToArray());

            // valid check
            if (onlyPrintable)
            {
                foreach (var c in str)
                {
                    var isPrintable = (Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || Char.IsWhiteSpace(c));
                    if (!isPrintable)
                        return 1;
                }
            }
            //else if (onlyValid)
            //{
            //    for (int a = 0; a<len; a++)
            //    {
            //        if (bytes[a] < ' ' || bytes[a] >= 'A' && bytes[a] <= 'Z')
            //        {
            //	        return 1;
            //        }
            //    }
            //}

            //fprintf(fout, "%08x%08x%08x%08x%08x:", hash[0], hash[1], hash[2], hash[3], hash[4]);
            result += str;
            return 0;
        }


        private uint[] convertToIntArray(string a)
        {
            List<uint> x = new List<uint>();
            for (int i = 0; i < a.Length - 1; i += 8)
                x.Add(uint.Parse(a.Substring(i, 8), System.Globalization.NumberStyles.HexNumber));

            return x.ToArray();
        }
    }
}
