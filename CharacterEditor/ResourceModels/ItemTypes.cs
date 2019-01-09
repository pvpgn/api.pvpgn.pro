using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace CharacterEditor.ResourceModels
{
    public class ItemTypes
    {
        public string ItemType { get; set; }
        public string Code { get; set; }
        public string Equiv1 { get; set; }
        public string Equiv2 { get; set; }
        public string Repair { get; set; }
        public string Body { get; set; }
        public string BodyLoc1 { get; set; }
        public string BodyLoc2 { get; set; }
        public string Shoots { get; set; }
        public string Quiver { get; set; }
        public string Throwable { get; set; }
        public string Reload { get; set; }
        public string ReEquip { get; set; }
        public string AutoStack { get; set; }
        public string Magic { get; set; }
        public string Rare { get; set; }
        public string Normal { get; set; }
        public string Charm { get; set; }
        public string Gem { get; set; }
        public string Beltable { get; set; }
        public string MaxSock1 { get; set; }
        public string MaxSock25 { get; set; }
        public string MaxSock40 { get; set; }
        public string TreasureClass { get; set; }
        public string Rarity { get; set; }
        public string StaffMods { get; set; }
        public string CostFormula { get; set; }
        public string Class { get; set; }
        [Default(0)]
        public int VarInvGfx { get; set; }
        public string InvGfx1 { get; set; }
        public string InvGfx2 { get; set; }
        public string InvGfx3 { get; set; }
        public string InvGfx4 { get; set; }
        public string InvGfx5 { get; set; }
        public string InvGfx6 { get; set; }
        public string StorePage { get; set; }
        [Name("*eol")]
        public string eol { get; set; }
    }
}
