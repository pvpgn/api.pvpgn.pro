using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite.Internal.UrlMatches;
using Microsoft.Extensions.Primitives;

namespace WebAPI.D2Char
{

    public class CharInfo : CharacterResponse
    {
        const int D2CHARINFO_FILESIZE = 192;
        const int D2CHARINFO_MAGICWORD = 0x12345678;
        const int D2CHARINFO_VERSION = 0x00010000;
        const int D2CHARINFO_PORTRAIT_PADSIZE = 30;
        const byte D2CHARINFO_PORTRAIT_PADBYTE = 0xff;
        const ushort D2CHARINFO_PORTRAIT_HEADER = 0x8084; // only for init
        const int MAX_CHARNAME_LEN = 16;
        const int MAX_USERNAME_LEN = 16;
        const int MAX_REALMNAME_LEN = 32;
        const byte D2CHARINFO_STATUS_FLAG_INIT = 0x01;
        const byte D2CHARINFO_STATUS_FLAG_EXPANSION = 0x20;
        const byte D2CHARINFO_STATUS_FLAG_LADDER = 0x40;
        const byte D2CHARINFO_STATUS_FLAG_HARDCORE = 0x04;
        const byte D2CHARINFO_STATUS_FLAG_DEAD = 0x08;

        public CharInfo() { }

        public new String FileType = "charinfo";

        public String Hash
        {
            get { return Helper.MD5(GetBytes()); }
        }

        public int CreatedTime;
        public int LastSeenTime;
        public int TotalPlayedMinutes; // minutes
        public string charName;
        public string UserName;
        public string RealmName;


        public CharClass Class;
        public string ClassTitle => Class.ToString();
        public int Level;
        public int Version;
        public int Experience;

        public string CharTitle
        {
            get
            {
                var v = Difficulty;
                if (Expansion)
                {
                    if (v >= 5 && v <= 8)
                        return Hardcore ? "Destroyer" : "Slayer";
                    if (v >= 10 && v <= 13)
                        return Hardcore ? "Conqueror" : "Champion";
                    if (v == 15)
                        return Hardcore
                            ? "Guardian"
                            : (Class == CharClass.Amazon || Class == CharClass.Assasin || Class == CharClass.Sorceress)
                                ? "Matriarch"
                                : "Patriarch";
                }
                else
                {
                    if (v >= 4 && v <= 7)
                        return (Class == CharClass.Amazon || Class == CharClass.Assasin || Class == CharClass.Sorceress)
                            ? Hardcore ? "Countess" : "Dame"
                            : Hardcore ? "Count" : "Sir";
                    if (v >= 8 && v <= 11)
                        return (Class == CharClass.Amazon || Class == CharClass.Assasin || Class == CharClass.Sorceress)
                            ? Hardcore ? "Duchess" : "Lady"
                            : Hardcore ? "Duke" : "Lord";
                    if (v >= 8 && v <= 11)
                        return (Class == CharClass.Amazon || Class == CharClass.Assasin || Class == CharClass.Sorceress)
                            ? Hardcore ? "Queen" : "Baroness"
                            : Hardcore ? "King" : "Baron";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Character is new. This flag is set to 0 by D2GS server after a first game
        /// </summary>
        public bool Init
        {
            get => (_statusByte & D2CHARINFO_STATUS_FLAG_INIT) > 0;
            set => _statusByte = (byte)(value ? _statusByte | D2CHARINFO_STATUS_FLAG_INIT : _statusByte & ~D2CHARINFO_STATUS_FLAG_INIT);
        }
        public bool Expansion
        {
            get => (_statusByte & D2CHARINFO_STATUS_FLAG_EXPANSION) > 0;
            set => _statusByte = (byte)(value ? _statusByte | D2CHARINFO_STATUS_FLAG_EXPANSION : _statusByte & ~D2CHARINFO_STATUS_FLAG_EXPANSION);
        }
        public bool Died
        {
            get => (_statusByte & D2CHARINFO_STATUS_FLAG_DEAD) > 0;
            set => _statusByte = (byte)(value ? _statusByte | D2CHARINFO_STATUS_FLAG_DEAD: _statusByte & ~D2CHARINFO_STATUS_FLAG_DEAD);
        }
        public bool Ladder
        {
            get => (_statusByte & D2CHARINFO_STATUS_FLAG_LADDER) > 0;
            set => _statusByte = (byte)(value ? _statusByte | D2CHARINFO_STATUS_FLAG_LADDER : _statusByte & ~D2CHARINFO_STATUS_FLAG_LADDER);
        }
        public bool Hardcore
        {
            get => (_statusByte & D2CHARINFO_STATUS_FLAG_HARDCORE) > 0;
            set => _statusByte = (byte)(value ? _statusByte | D2CHARINFO_STATUS_FLAG_HARDCORE : _statusByte & ~D2CHARINFO_STATUS_FLAG_HARDCORE);
        }

        public byte Difficulty;

        public string DifficultyTitle
        {
            get
            {
                switch ((Difficulty & 0x0f) / (Expansion ? 5 : 4))
                {
                    case 3: return "Hell";
                    case 2: return "Nightmare";
                    default: return "Normal"; // FIXME: 0 and 1
                }
            }
        }
        private byte _statusByte;

        public CharPortrait Portrait;

        public void Read(byte[] data)
        {
            // https://github.com/pvpgn/pvpgn-server/blob/36f99253577487fdcf803183de29f70ff034c45e/src/common/d2cs_d2gs_character.h
            using (var fs = new MemoryStream(data))
            using (var br = new BinaryReader(fs))
            {
                // header
                if (br.ReadInt32() != D2CHARINFO_MAGICWORD) // magic word
                    throw new Exception("Bad charinfo file");
                Version = br.ReadInt32(); // charinfo file version
                CreatedTime = br.ReadInt32(); // character creation time
                LastSeenTime = br.ReadInt32(); // character last access time
                br.ReadInt32(); // checksum
                TotalPlayedMinutes = br.ReadInt32(); // total in game play time
                br.ReadBytes(24); // reserved (skip)
                charName = Encoding.ASCII.GetString(br.ReadBytes(16)).TrimEnd('\0');    // 0x30
                UserName = Encoding.ASCII.GetString(br.ReadBytes(16)).TrimEnd('\0'); // 0x40
                RealmName = Encoding.ASCII.GetString(br.ReadBytes(32)).TrimEnd('\0');   // 0x50

                // portrait
                Portrait.Header = br.ReadInt16(); 
                Portrait.Gfx = br.ReadBytes(11);
                Portrait.ChClass = br.ReadByte();
                Portrait.Color = br.ReadBytes(11);
                Portrait.Level = br.ReadByte();
                Portrait.Status = br.ReadByte();
                Portrait.U1 = br.ReadBytes(3);
                Portrait.Ladder = br.ReadByte();
                Portrait.U2 = br.ReadBytes(2);
                br.ReadByte(); // end
                br.ReadBytes(D2CHARINFO_PORTRAIT_PADSIZE);

                // summary
                Experience = br.ReadInt32(); // 0xB0
                _statusByte = br.ReadByte(); // 0xB4
                Difficulty = br.ReadByte(); // 0xB4
                br.ReadInt16(); // skip
                Level = br.ReadInt32(); // 0xB8
                Class = (CharClass)br.ReadInt32(); // 0xBC
            }
        }

        public byte[] GetBytes()
        {
            byte[] buf = new byte[D2CHARINFO_FILESIZE];

            using (var fs = new MemoryStream(buf))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(D2CHARINFO_MAGICWORD);
                bw.Write(D2CHARINFO_VERSION);
                bw.Write(CreatedTime);
                bw.Write(LastSeenTime);
                bw.Write((int)0);
                bw.Write(TotalPlayedMinutes);
                bw.Write(new byte[24]); // reserved
                var charName = new char[MAX_CHARNAME_LEN];
                Encoding.Default.GetBytes(this.charName).CopyTo(charName, 0);
                bw.Write(charName);
                var userName = new char[MAX_USERNAME_LEN];
                Encoding.Default.GetBytes(UserName).CopyTo(userName, 0);
                bw.Write(userName);
                var realmName = new char[MAX_REALMNAME_LEN];
                Encoding.Default.GetBytes(RealmName).CopyTo(realmName, 0);
                bw.Write(realmName);

                // portrait
                bw.Write(Portrait.Header);
                bw.Write(Portrait.Gfx);
                bw.Write(Portrait.ChClass);
                bw.Write(Portrait.Color);
                bw.Write(Portrait.Level);
                bw.Write(Portrait.Status);
                bw.Write(Portrait.U1);
                bw.Write(Portrait.Ladder);
                bw.Write(Portrait.U2);
                bw.Write((byte)0); // end
                for (var i = 0; i < D2CHARINFO_PORTRAIT_PADSIZE; i++)
                    bw.Write((byte)0);

                // summary
                bw.Write(Experience);
                bw.Write(_statusByte);
                bw.Write(Difficulty);
                bw.Write((short)0);
                bw.Write(Level);
                bw.Write((int)Class);
            }
            return buf;
        }
    }

    public struct CharPortrait
    {
        public short Header;
        public byte[] Gfx;
        public byte ChClass;
        public byte[] Color;
        public byte Level;
        public byte Status;
        public byte[] U1;
        public byte Ladder; // need not be 0xff and 0 to make character displayed as ladder character
        // client only check this bit
        public byte[] U2;
    }


    public enum CharClass
    {
        Amazon = 0,
        Sorceress = 1,
        Necromancer = 2,
        Paladin = 3,
        Barbarian = 4,
        Druid = 5,
        Assasin = 6
    }
}
