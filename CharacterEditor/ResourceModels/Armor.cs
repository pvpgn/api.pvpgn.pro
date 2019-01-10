using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace CharacterEditor.ResourceModels
{
    public class Armor
    {
        public string name { get; set; }
        public string version { get; set; }
        public string compactsave { get; set; }
        public string rarity { get; set; }
        public string spawnable { get; set; }
        public string minac { get; set; }
        public string maxac { get; set; }
        public string absorbs { get; set; }
        public string speed { get; set; }
        public string reqstr { get; set; }
        public string block { get; set; }
        public string durability { get; set; }
        public string nodurability { get; set; }
        public string level { get; set; }
        public string levelreq { get; set; }
        public string cost { get; set; }
        [Name("gamble cost")]
        public string gambleCost { get; set; }
        public string code { get; set; }
        public string namestr { get; set; }
        [Name("magic lvl")]
        public string magicLvl { get; set; }
        [Name("auto prefix")]
        public string autoPrefix { get; set; }
        public string alternategfx { get; set; }
        public string OpenBetaGfx { get; set; }
        public string normcode { get; set; }
        public string ubercode { get; set; }
        public string ultracode { get; set; }
        public string spelloffset { get; set; }
        public string component { get; set; }
        public string invwidth { get; set; }
        public string invheight { get; set; }
        public string hasinv { get; set; }
        public string gemsockets { get; set; }
        public string gemapplytype { get; set; }
        public string flippyfile { get; set; }
        public string invfile { get; set; }
        public string uniqueinvfile { get; set; }
        public string setinvfile { get; set; }
        public string rArm { get; set; }
        public string lArm { get; set; }
        public string Torso { get; set; }
        public string Legs { get; set; }
        public string rSPad { get; set; }
        public string lSPad { get; set; }
        public string useable { get; set; }
        public string throwable { get; set; }
        public string stackable { get; set; }
        public string minstack { get; set; }
        public string maxstack { get; set; }
        public string type { get; set; }
        public string type2 { get; set; }
        public string dropsound { get; set; }
        public string dropsfxframe { get; set; }
        public string usesound { get; set; }
        public string unique { get; set; }
        public string transparent { get; set; }
        public string transtbl { get; set; }
        public string quivered { get; set; }
        public string lightradius { get; set; }
        public string belt { get; set; }
        public string quest { get; set; }
        public string missiletype { get; set; }
        public string durwarning { get; set; }
        public string qntwarning { get; set; }
        [Index(62)]
        public string mindam1 { get; set; }
        [Index(63)]
        public string maxdam1 { get; set; }
        public string StrBonus { get; set; }
        public string DexBonus { get; set; }
        public string gemoffset { get; set; }
        public string bitfield1 { get; set; }
        public string CharsiMin { get; set; }
        public string CharsiMax { get; set; }
        public string CharsiMagicMin { get; set; }
        public string CharsiMagicMax { get; set; }
        public string CharsiMagicLvl { get; set; }
        public string GheedMin { get; set; }
        public string GheedMax { get; set; }
        public string GheedMagicMin { get; set; }
        public string GheedMagicMax { get; set; }
        public string GheedMagicLvl { get; set; }
        public string AkaraMin { get; set; }
        public string AkaraMax { get; set; }
        public string AkaraMagicMin { get; set; }
        public string AkaraMagicMax { get; set; }
        public string AkaraMagicLvl { get; set; }
        public string FaraMin { get; set; }
        public string FaraMax { get; set; }
        public string FaraMagicMin { get; set; }
        public string FaraMagicMax { get; set; }
        public string FaraMagicLvl { get; set; }
        public string LysanderMin { get; set; }
        public string LysanderMax { get; set; }
        public string LysanderMagicMin { get; set; }
        public string LysanderMagicMax { get; set; }
        public string LysanderMagicLvl { get; set; }
        public string DrognanMin { get; set; }
        public string DrognanMax { get; set; }
        public string DrognanMagicMin { get; set; }
        public string DrognanMagicMax { get; set; }
        public string DrognanMagicLvl { get; set; }
        public string HraltiMin { get; set; }
        public string HraltiMax { get; set; }
        public string HraltiMagicMin { get; set; }
        public string HraltiMagicMax { get; set; }
        public string HratliMagicLvl { get; set; }
        public string AlkorMin { get; set; }
        public string AlkorMax { get; set; }
        public string AlkorMagicMin { get; set; }
        public string AlkorMagicMax { get; set; }
        public string AlkorMagicLvl { get; set; }
        public string OrmusMin { get; set; }
        public string OrmusMax { get; set; }
        public string OrmusMagicMin { get; set; }
        public string OrmusMagicMax { get; set; }
        public string OrmusMagicLvl { get; set; }
        public string ElzixMin { get; set; }
        public string ElzixMax { get; set; }
        public string ElzixMagicMin { get; set; }
        public string ElzixMagicMax { get; set; }
        public string ElzixMagicLvl { get; set; }
        public string AshearaMin { get; set; }
        public string AshearaMax { get; set; }
        public string AshearaMagicMin { get; set; }
        public string AshearaMagicMax { get; set; }
        public string AshearaMagicLvl { get; set; }
        public string CainMin { get; set; }
        public string CainMax { get; set; }
        public string CainMagicMin { get; set; }
        public string CainMagicMax { get; set; }
        public string CainMagicLvl { get; set; }
        public string HalbuMin { get; set; }
        public string HalbuMax { get; set; }
        public string HalbuMagicMin { get; set; }
        public string HalbuMagicMax { get; set; }
        public string HalbuMagicLvl { get; set; }
        public string JamellaMin { get; set; }
        public string JamellaMax { get; set; }
        public string JamellaMagicMin { get; set; }
        public string JamellaMagicMax { get; set; }
        public string JamellaMagicLvl { get; set; }
        public string LarzukMin { get; set; }
        public string LarzukMax { get; set; }
        public string LarzukMagicMin { get; set; }
        public string LarzukMagicMax { get; set; }
        public string LarzukMagicLvl { get; set; }
        public string MalahMin { get; set; }
        public string MalahMax { get; set; }
        public string MalahMagicMin { get; set; }
        public string MalahMagicMax { get; set; }
        public string MalahMagicLvl { get; set; }
        public string DrehyaMin { get; set; }
        public string DrehyaMax { get; set; }
        public string DrehyaMagicMin { get; set; }
        public string DrehyaMagicMax { get; set; }
        public string DrehyaMagicLvl { get; set; }
        [Name("Source Art")]
        public string SourceArt { get; set; }
        [Name("Game Art")]
        public string GameArt { get; set; }
        public string Transform { get; set; }
        public string InvTrans { get; set; }
        public string SkipName { get; set; }
        public string NightmareUpgrade { get; set; }
        public string HellUpgrade { get; set; }
        [Index(160)]
        public string mindam2 { get; set; }
        [Index(161)]
        public string maxdam2 { get; set; }
        public string nameable { get; set; }
    }
}
