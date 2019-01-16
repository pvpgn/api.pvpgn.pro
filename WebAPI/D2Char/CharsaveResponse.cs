using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CharacterEditor;

namespace WebAPI.D2Char
{
    public class CharSaveResponse : CharacterResponse
    {
        public CharSaveResponse() { }

        public CharSaveResponse(SaveReader playerData, string hash = null)
        {
            Data = playerData.GetBytes(true); // true is important!
            Hash = (hash != null) ? hash : Helper.MD5(Data);
            HashWithOriginalStat = (hash != null) ? hash : Helper.MD5(playerData.GetBytes(true, true)); // true is important!;

            Version = playerData.Version;
            TimeStamp = playerData.TimeStamp;

            // items
            foreach (var i in playerData.Inventory.PlayerItems)
                PlayerItems.Add(new CharItemResponse(i));
            foreach (var i in playerData.Inventory.CorpseItems)
                CorpseItems.Add(new CharItemResponse(i));
            foreach (var i in playerData.Inventory.GolemItems)
                GolemItems.Add(new CharItemResponse(i));
            foreach (var i in playerData.Inventory.MercItems)
                MercItems.Add(new CharItemResponse(i));

            Name = playerData.Character.Name;
            Class = playerData.Character.Class;
            Died = playerData.Character.Died;
            Expansion = playerData.Character.Expansion;
            Hardcore = playerData.Character.Hardcore;
            Ladder = playerData.Character.Ladder;
            HasMercenary = playerData.Character.HasMercenary;
            MercenaryExp = playerData.Character.MercenaryExp;
            MercenaryNameId = playerData.Character.MercenaryNameId;
            MercenaryType = playerData.Character.MercenaryType;
            Progression = playerData.Character.Progression;
            BaseHitpoints = playerData.Stat.BaseHitpoints;
            BaseMana = playerData.Stat.BaseMana;
            BaseStamina = playerData.Stat.BaseStamina;
            DeathCount = playerData.Stat.DeathCount;
            Dexterity = playerData.Stat.Dexterity;
            Energy = playerData.Stat.Energy;
            Experience = playerData.Stat.Experience;
            Gold = playerData.Stat.Gold;
            GoldBank = playerData.Stat.GoldBank;
            Hitpoints = playerData.Stat.Hitpoints;
            KillCount = playerData.Stat.KillCount;
            Level = playerData.Stat.Level;
            Mana = playerData.Stat.Mana;
            SkillPoints = playerData.Stat.SkillPoints;
            Stamina = playerData.Stat.Stamina;
            StatPoints = playerData.Stat.StatPoints;
            Strength = playerData.Stat.Strength;
            Vitality = playerData.Stat.Vitality;
            
            for (var i = 0; i < playerData.Skill.Length; i++)
                Skills.Add(byte.Parse(playerData.Skill[i].ToString()));
        }

        /// <summary>
        /// Replace character stats and items
        /// </summary>
        /// <param name="playerData"></param>
        public SaveReader GetCharacter(SaveReader playerData)
        {
            playerData.Inventory.PlayerItems.Clear();
            foreach (var i in PlayerItems)
                playerData.Inventory.PlayerItems.Add(i.GetItem(Item.NewItem(i.Data)));

            playerData.Inventory.CorpseItems.Clear();
            foreach (var i in CorpseItems)
                playerData.Inventory.CorpseItems.Add(i.GetItem(Item.NewItem(i.Data)));

            playerData.Inventory.GolemItems.Clear();
            foreach (var i in GolemItems)
                playerData.Inventory.GolemItems.Add(i.GetItem(Item.NewItem(i.Data)));

            playerData.Inventory.MercItems.Clear();
            foreach (var i in MercItems)
                playerData.Inventory.MercItems.Add(i.GetItem(Item.NewItem(i.Data))); 

            playerData.Character.Name = Name;
            playerData.Character.Class = Class;
            playerData.Character.Died = Died;
            playerData.Character.Expansion = Expansion;
            playerData.Character.Hardcore = Hardcore;
            playerData.Character.Ladder = Ladder;
            //playerData.Character.HasMercenary = HasMercenary; // readonly
            //playerData.Character.MercenaryExp = MercenaryExp; // readonly
            //playerData.Character.MercenaryNameId = MercenaryNameId; // readonly
            //playerData.Character.MercenaryType = MercenaryType; // readonly
            playerData.Character.Progression = Progression;
            playerData.Stat.BaseHitpoints = BaseHitpoints;
            playerData.Stat.BaseMana = BaseMana;
            playerData.Stat.BaseStamina = BaseStamina;
            playerData.Stat.DeathCount = DeathCount;
            playerData.Stat.Dexterity = Dexterity;
            playerData.Stat.Energy = Energy;
            playerData.Stat.Experience = Experience;
            playerData.Stat.Gold = Gold;
            playerData.Stat.GoldBank = GoldBank;
            playerData.Stat.Hitpoints = Hitpoints;
            playerData.Stat.KillCount = KillCount;
            playerData.Stat.Level = Level;
            playerData.Stat.Mana = Mana;
            playerData.Stat.SkillPoints = SkillPoints;
            playerData.Stat.Stamina = Stamina;
            playerData.Stat.StatPoints = StatPoints;
            playerData.Stat.Strength = Strength;
            playerData.Stat.Vitality = Vitality;

            // start from 2-d byte (skip "if" header on the destination)
            for (var i = 0; i < Skills.Count; i++)
                playerData.Skill[i] = Skills[i];

            return playerData;
        }

        public List<CharItemResponse> PlayerItems = new List<CharItemResponse>();
        public List<CharItemResponse> CorpseItems = new List<CharItemResponse>();
        public List<CharItemResponse> GolemItems = new List<CharItemResponse>();
        public List<CharItemResponse> MercItems = new List<CharItemResponse>();

        public new String FileType = "charsave";
        public String Hash;
        public String HashWithOriginalStat;
        public byte[] Data;
        public int Version;
        public int TimeStamp;

        public String Name;

        public Character.CharacterClass Class;
        public string ClassTitle => Class.ToString();
        public bool Died;
        public bool Expansion;
        public bool Hardcore;
        public bool Ladder;
        public bool HasMercenary;
        public uint MercenaryExp;
        public ushort MercenaryNameId;
        public ushort MercenaryType;
        public byte Progression;

        public string CharTitle => Character.GetCharTitle(Progression, Expansion, Hardcore, Class);
        public string DifficultyTitle => Character.GetDiffucultyTitle(Progression, Expansion);

        public int BaseHitpoints;
        public int BaseMana;
        public int BaseStamina;
        public int DeathCount;
        public int Dexterity;
        public int Energy;
        public uint Experience;
        public uint Gold;
        public uint GoldBank;
        public int Hitpoints;
        public int KillCount;
        public int Level;
        public int Mana;
        public int SkillPoints;
        public int Stamina;
        public int StatPoints;
        public int Strength;
        public int Vitality;

        public List<byte> Skills = new List<byte>();
    }


}