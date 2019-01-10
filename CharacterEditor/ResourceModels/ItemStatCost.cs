using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace CharacterEditor.ResourceModels
{
    public class ItemStatCost
    {
        public string Stat { get; set; }
        [Default(0)]
        public int ID { get; set; }
        [Name("Send Other")]
        public string SendOther { get; set; }
        public string Signed { get; set; }
        [Name("Send Bits")]
        public string SendBits { get; set; }
        [Name("Send Param Bits")]
        public string SendParamBits { get; set; }
        public string UpdateAnimRate { get; set; }
        public string Saved { get; set; }
        public string CSvSigned { get; set; }
        [Default(0)]
        public int CSvBits { get; set; }
        public string CSvParam { get; set; }
        public string fCallback { get; set; }
        public string fMin { get; set; }
        public string MinAccr { get; set; }
        public string Encode { get; set; }
        public string Add { get; set; }
        public string Multiply { get; set; }
        public string Divide { get; set; }
        [Default(0)]
        public int ValShift { get; set; }
        [Name("1.09-Save Bits")]
        public string SaveBits109 { get; set; }
        [Name("1.09-Save Add")]
        public string SaveAdd109 { get; set; }
        [Name("Save Bits")]
        [Default(0)]
        public int SaveBits { get; set; }
        [Name("Save Add")]
        [Default(0)]
        public int SaveAdd { get; set; }
        [Name("Save Param Bits")]
        [Default(0)]
        public int SaveParamBits { get; set; }
        public string keepzero { get; set; }
        public string op { get; set; }
        [Name("op param")]
        public string opParam { get; set; }
        [Name("op base")]
        public string opBase { get; set; }
        [Name("op stat1")]
        public string opStat1 { get; set; }
        [Name("op stat2")]
        public string opStat2 { get; set; }
        [Name("op stat3")]
        public string opStat3 { get; set; }
        public string direct { get; set; }
        public string maxstat { get; set; }
        public string itemspecific { get; set; }
        public string damagerelated { get; set; }
        public string itemevent1 { get; set; }
        public string itemeventfunc1 { get; set; }
        public string itemevent2 { get; set; }
        public string itemeventfunc2 { get; set; }
        public string descpriority { get; set; }
        public string descfunc { get; set; }
        public string descval { get; set; }
        public string descstrpos { get; set; }
        public string descstrneg { get; set; }
        public string descstr2 { get; set; }
        public string dgrp { get; set; }
        public string dgrpfunc { get; set; }
        public string dgrpval { get; set; }
        public string dgrpstrpos { get; set; }
        public string dgrpstrneg { get; set; }
        public string dgrpstr2 { get; set; }
        public string stuff { get; set; }
        [Name("*eol")]
        public string eol { get; set; }
    }
}
