using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace CharacterEditor.ResourceModels
{

    public class UniqueItems
    {
        public string index { get; set; }
        public string version { get; set; }
        public string enabled { get; set; }
        public string ladder { get; set; }
        public string rarity { get; set; }
        public string nolimit { get; set; }
        public string lvl { get; set; }
        [Name("lvl req")]
        public string lvl_req { get; set; }
        public string code { get; set; }
        [Name("*type")]
        public string type { get; set; }
        [Name("*uber")]
        public string uber { get; set; }
        public string carry1 { get; set; }
        [Name("cost mult")]
        public string cost_mult { get; set; }
        [Name("cost add")]
        public string cost_add { get; set; }
        public string chrtransform { get; set; }
        public string invtransform { get; set; }
        public string flippyfile { get; set; }
        public string invfile { get; set; }
        public string dropsound { get; set; }
        public string dropsfxframe { get; set; }
        public string usesound { get; set; }
        public string prop1 { get; set; }
        public string par1 { get; set; }
        public string min1 { get; set; }
        public string max1 { get; set; }
        public string prop2 { get; set; }
        public string par2 { get; set; }
        public string min2 { get; set; }
        public string max2 { get; set; }
        public string prop3 { get; set; }
        public string par3 { get; set; }
        public string min3 { get; set; }
        public string max3 { get; set; }
        public string prop4 { get; set; }
        public string par4 { get; set; }
        public string min4 { get; set; }
        public string max4 { get; set; }
        public string prop5 { get; set; }
        public string par5 { get; set; }
        public string min5 { get; set; }
        public string max5 { get; set; }
        public string prop6 { get; set; }
        public string par6 { get; set; }
        public string min6 { get; set; }
        public string max6 { get; set; }
        public string prop7 { get; set; }
        public string par7 { get; set; }
        public string min7 { get; set; }
        public string max7 { get; set; }
        public string prop8 { get; set; }
        public string par8 { get; set; }
        public string min8 { get; set; }
        public string max8 { get; set; }
        public string prop9 { get; set; }
        public string par9 { get; set; }
        public string min9 { get; set; }
        public string max9 { get; set; }
        public string prop10 { get; set; }
        public string par10 { get; set; }
        public string min10 { get; set; }
        public string max10 { get; set; }
        public string prop11 { get; set; }
        public string par11 { get; set; }
        public string min11 { get; set; }
        public string max11 { get; set; }
        public string prop12 { get; set; }
        public string par12 { get; set; }
        public string min12 { get; set; }
        public string max12 { get; set; }
        [Name("*eol")]
        public string eol { get; set; }

    }
}
