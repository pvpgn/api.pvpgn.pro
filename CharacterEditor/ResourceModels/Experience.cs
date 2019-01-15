using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace CharacterEditor.ResourceModels
{
    public class Experience
    {
        [Name("Level")]
        public string LevelStr { get; set; }
        [Ignore]
        public byte Level
        {
            get
            {
                byte result;
                byte.TryParse(LevelStr, out result);
                return result;
            }
        }
        public uint Amazon { get; set; }
        public uint Sorceress { get; set; }
        public uint Necromancer { get; set; }
        public uint Paladin { get; set; }
        public uint Barbarian { get; set; }
        public uint Druid { get; set; }
        public uint Assassin { get; set; }
        public int ExpRatio { get; set; }
    }
}
