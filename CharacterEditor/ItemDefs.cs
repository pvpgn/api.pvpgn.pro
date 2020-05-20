using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Linq;
using CharacterEditor.ResourceModels;
using CsvHelper;

namespace CharacterEditor
{
	public class ItemDefs
	{
		private static Dictionary<string, string> itemDescriptions = new Dictionary<string, string>();
		private static Dictionary<string, string> setDescriptions = new Dictionary<string, string>();
		private static Dictionary<string, HashSet<string>> itemCodeSets = new Dictionary<string, HashSet<string>>();

        public static bool Loaded { get; private set; }

        /// <summary>
        /// CSV data
        /// </summary>
        public static Dictionary<string, Misc> Misc;
        public static Dictionary<string, ItemTypes> ItemTypes;
        public static List<UniqueItems> UniqueItems;
        public static Dictionary<string, Weapons> Weapons;
        public static Dictionary<string, Armor> Armor;
        public static Dictionary<string, ItemStatCost> ItemStatCost;
        public static List<Experience> Experience;

        public static TblReader StringTables;


		// TODO: Temp fix! Get rid of static class and move to singleton based on current resource
		//  set (es300_r6d, rot_1.a3.1, etc)
		internal static void LoadItemDefs()
        {
            Loaded = true;

			// Temp!
			itemDescriptions = new Dictionary<string, string>();
			setDescriptions = new Dictionary<string, string>();
			itemCodeSets = new Dictionary<string, HashSet<string>>();

			List<String> fileContents = Resources.Instance.ReadAllLines("AllItems.txt");

			foreach (String str in fileContents)
			{
				string cleanStr = str;

				while (true)
				{
					// SpecialChar is the color character (followed by 2 other characters), this
					//  is just stripping it since it looks bad
					int specialCharLocation = cleanStr.IndexOf('\xfffd');

					if (specialCharLocation == -1)
						break;

					cleanStr = cleanStr.Remove(specialCharLocation, 3);
				}

				itemDescriptions.Add(cleanStr.Substring(0, 3), cleanStr.Substring(4));
			}

			// Just so it's guaranteed that these sections exist
			itemCodeSets.Add("weapons", new HashSet<string>());
			itemCodeSets.Add("armor", new HashSet<string>());
			itemCodeSets.Add("stackable", new HashSet<string>());
			itemCodeSets.Add("monsterparts", new HashSet<string>());
			itemCodeSets.Add("scrolltome", new HashSet<string>());
			itemCodeSets.Add("charms", new HashSet<string>());

			ReadItemCodeSet(itemCodeSets, "ItemGroups.txt");

			foreach (string str in Resources.Instance.ReadAllLines("Sets.txt"))
			{
				string itemCode = str.Substring(0, 3);

				if (!setDescriptions.ContainsKey(itemCode))
					setDescriptions.Add(itemCode, str.Substring(4));
			}

            ReadCsvData();

            StringTables = new TblReader(new string[] {
                "string.tbl", 
                "expansionstring.tbl", 
                "patchstring.tbl"
            });
        }

        private static void ReadCsvData()
        {
            using (var reader = Resources.Instance.OpenResourceText("Misc.txt"))
            using (var csv = new CsvReader(reader) { Configuration = { Delimiter = "\t" } })
                Misc = csv.GetRecords<Misc>().ToDictionary(t => t.code, t => t);

            using (var reader = Resources.Instance.OpenResourceText("ItemTypes.txt"))
            using (var csv = new CsvReader(reader) { Configuration = { Delimiter = "\t" } })
                ItemTypes = csv.GetRecords<ItemTypes>().GroupBy(t => t.Code).ToDictionary(t => t.Key, t => t.First()); // ignore empty duplicates

            using (var reader = Resources.Instance.OpenResourceText("UniqueItems.txt"))
            using (var csv = new CsvReader(reader) { Configuration = { Delimiter = "\t" } })
                UniqueItems = csv.GetRecords<UniqueItems>().ToList();

            using (var reader = Resources.Instance.OpenResourceText("Weapons.txt"))
            using (var csv = new CsvReader(reader) { Configuration = { Delimiter = "\t" } })
                Weapons = csv.GetRecords<Weapons>().ToDictionary(t => t.code, t => t);

            using (var reader = Resources.Instance.OpenResourceText("Armor.txt"))
            using (var csv = new CsvReader(reader) { Configuration = { Delimiter = "\t" } })
                Armor = csv.GetRecords<Armor>().ToDictionary(t => t.code, t => t);

            using (var reader = Resources.Instance.OpenResourceText("ItemStatCost.txt"))
            using (var csv = new CsvReader(reader) { Configuration = { Delimiter = "\t" } })
                ItemStatCost = csv.GetRecords<ItemStatCost>().ToDictionary(t => t.Stat, t => t);

            using (var reader = Resources.Instance.OpenResourceText("Experience.txt"))
            using (var csv = new CsvReader(reader) { Configuration = { Delimiter = "\t" } })
                Experience = csv.GetRecords<Experience>().ToList();
        }


        static ItemDefs()
		{
            //LoadItemDefs(); // TODO: No more static class
        }

		/// <summary>
		/// Returns the name of the specified property ID
		/// </summary>
		/// <param name="id">ID of property</param>
		/// <returns>Name of property with specified ID</returns>
		public static string GetPropertyName(int id)
        {
            var p = ItemStatCost.Values.ElementAtOrDefault(id);
            if (p == null)
			{
				if (id == 0x1ff)
				{
					return "EOF";
				}
				return "UNKNOWN";
			}
			return p.Stat;
		}

		/// <summary>
		/// Returns the description of the specified item code
		/// </summary>
		/// <param name="itemCode">Item code</param>
		/// <returns>Item description</returns>
		public static string GetItemDescription(string itemCode)
		{
			if (itemDescriptions.ContainsKey(itemCode))
			{
				return itemDescriptions[itemCode];
			}

			return "UNKNOWN";
		}

		/// <summary>
		/// Returns the name of the set item with specified item code
		/// </summary>
		/// <param name="itemCode">Item code</param>
		/// <returns>Name of set item</returns>
		public static string GetUniqueSetName(string itemCode)
		{
			if (!setDescriptions.ContainsKey(itemCode))
			{
				return "UNKNOWN SET";
			}

			return setDescriptions[itemCode];
        }

        /// <summary>
        /// Returns gfx for the item
        ///
        /// (HarpyWar) How to find gfx for d2item:
        ///   1) Find in misc, armor, weapon. If found then take invfile.
        ///       If quality = "set" then try find setinv
        ///       If quality = "unique" then try find uniqueinv
        ///   2) If isGraphic = true then find item by "type" in itemtypes and get invfileX where X = graphicid.If found then replace.
        ///   3) If quality = "unique" then try find item in uniqueitems by uniqueid (index) and if found then replace it.
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Name of image file</returns>
        public static string GetItemImageFile(Item item)
        {
            string fileName = string.Empty;

            if (IsArmor(item.ItemCode))
            {
                if (Armor.ContainsKey(item.ItemCode))
                {
                    var r = Armor[item.ItemCode];
                    fileName = r.invfile;
                    if (item.Quality == (uint)Item.ItemQuality.Unique && !string.IsNullOrWhiteSpace(r.uniqueinvfile))
                        fileName = r.uniqueinvfile;
                    if (item.Quality == (uint)Item.ItemQuality.Set && !string.IsNullOrWhiteSpace(r.setinvfile))
                        fileName = r.setinvfile;
                    if (item.HasGraphic)
                        findInvFileByItemType(ref fileName, r.type, item.Graphic);
                }
            }
            if (IsWeapon(item.ItemCode))
            {
                if (Weapons.ContainsKey(item.ItemCode))
                {
                    var r = Weapons[item.ItemCode];
                    fileName = r.invfile;
                    if (item.Quality == (uint)Item.ItemQuality.Unique && !string.IsNullOrWhiteSpace(r.uniqueinvfile))
                        fileName = r.uniqueinvfile;
                    if (item.Quality == (uint)Item.ItemQuality.Set && !string.IsNullOrWhiteSpace(r.setinvfile))
                        fileName = r.setinvfile;
                    if (item.HasGraphic)
                        findInvFileByItemType(ref fileName, r.type, item.Graphic);
                }
            }
            if (!IsArmor(item.ItemCode) && !IsWeapon(item.ItemCode))
            {
                if (Misc.ContainsKey(item.ItemCode))
                {
                    var r = Misc[item.ItemCode];
                    fileName = r.invfile;
                    if (item.Quality == (uint)Item.ItemQuality.Unique && !string.IsNullOrWhiteSpace(r.uniqueinvfile))
                        fileName = r.uniqueinvfile;
                    if (item.HasGraphic)
                        findInvFileByItemType(ref fileName, r.type, item.Graphic);
                }
            }
            if (item.Quality == (uint)Item.ItemQuality.Unique)
            {
                var r = UniqueItems.ElementAtOrDefault((int) item.UniqueSetId);
                if (r != null)
                {
                    if (!string.IsNullOrWhiteSpace(r.invfile))
                        fileName = r.invfile;
                }
            }
            return fileName;
        }

        // if graphic > 0 then find in ItemTypes
        private static void findInvFileByItemType(ref string fileName, string itemType, uint graphic)
        {
            if (ItemTypes.ContainsKey(itemType) && ItemTypes[itemType].VarInvGfx > 0)
            {
                var inv = ItemTypes[itemType];
                switch (graphic)
                {
                    case 0:
                        if (!string.IsNullOrWhiteSpace(inv.InvGfx1))
                            fileName = inv.InvGfx1;
                        break;
                    case 1:
                        if (!string.IsNullOrWhiteSpace(inv.InvGfx2))
                            fileName = inv.InvGfx2;
                        break;
                    case 2:
                        if (!string.IsNullOrWhiteSpace(inv.InvGfx3))
                            fileName = inv.InvGfx3;
                        break;
                    case 3:
                        if (!string.IsNullOrWhiteSpace(inv.InvGfx4))
                            fileName = inv.InvGfx4;
                        break;
                    case 4:
                        if (!string.IsNullOrWhiteSpace(inv.InvGfx5))
                            fileName = inv.InvGfx5;
                        break;
                    case 5:
                        if (!string.IsNullOrWhiteSpace(inv.InvGfx6))
                            fileName = inv.InvGfx6;
                        break;
                }
            }
        }


        /// <summary>
        /// Item is armor
        /// </summary>
        /// <param name="itemCode">Item code</param>
        /// <returns>True if item is armor</returns>
        public static bool IsArmor(string itemCode)
		{
			return itemCodeSets["armor"].Contains(itemCode);
		}

		/// <summary>
		/// Item is a weapon
		/// </summary>
		/// <param name="itemCode">Item code</param>
		/// <returns>True if item is a weapon</returns>
		public static bool IsWeapon(string itemCode)
		{
			return itemCodeSets["weapons"].Contains(itemCode);
		}


		/// <summary>
		/// Item is stackable
		/// </summary>
		/// <param name="itemCode">Item code</param>
		/// <returns>True if item is stackable</returns>
		public static bool IsStackable(string itemCode)
		{
			return itemCodeSets["stackable"].Contains(itemCode);
		}

		/// <summary>
		/// Item is a monster part
		/// </summary>
		/// <param name="itemCode">Item code</param>
		/// <returns>True if item is a monster part</returns>
		public static bool IsMonsterPart(string itemCode)
		{
			return itemCodeSets["monsterparts"].Contains(itemCode);
		}

		/// <summary>
		/// Item is a scroll or tome
		/// </summary>
		/// <param name="itemCode">Item code</param>
		/// <returns>True if item is a scroll or tome</returns>
		public static bool IsScrollOrTome(string itemCode)
		{
			return itemCodeSets["scrolltome"].Contains(itemCode);
		}

		/// <summary>
		/// Item is a charm
		/// </summary>
		/// <param name="itemCode">Item code</param>
		/// <returns>True if item is a charm</returns>
		public static bool IsCharm(string itemCode)
		{
			return itemCodeSets["charms"].Contains(itemCode);
		}

		/// <summary>
		/// Reads multiple sets of item codes from disk. Each set has a unique section name defined in brackets (eg. [sectionName]).
		/// If multiple identical section names are exist, then the data for these sections are combined into the same set.
		/// </summary>
		/// <param name="itemCodeSets">Table of sets to store data</param>
		/// <param name="filePath">Path containing sets of item codes</param>
		/// <example>
		/// 
		/// [armor]
		/// cap Cap/hat
		/// skp Skull Cap
		/// 
		/// [weapons]
		/// hax Hand Axe 
		/// axe Axe 
		/// 
		/// </example>
		public static void ReadItemCodeSet(Dictionary<string, HashSet<string>> itemCodeSets, string filePath)
		{
			List<string> lines = Resources.Instance.ReadAllLines(filePath);
			HashSet<string> currentSection = null;

			foreach (var line in lines)
			{
				if (line.Length <= 2)
				{
					continue;
				}

				if (line[0] == '[' && line[line.Length - 1] == ']')
				{
					string sectionName = line.Substring(1, line.Length - 2).ToLower();

					if (!itemCodeSets.ContainsKey(sectionName))
					{
						itemCodeSets.Add(sectionName, new HashSet<string>());
					}

					currentSection = itemCodeSets[sectionName];
				}
				else if (currentSection != null)
				{
					string itemCode = line.Substring(0, 3).ToLower();

					currentSection.Add(itemCode);
				}
			}
		}

        /// <summary>
        /// TODO: unfinished https://github.com/pvpgn/api.pvpgn.pro/issues/1#issuecomment-552119449
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static string GetPropDescription(Item.PropertyInfo prop)
        {
            // EOF
            if (prop.ID == 511)
                return "EOF";

            var s = ItemDefs.ItemStatCost.Values.ElementAtOrDefault(prop.ID);
            if (s == null)
                return string.Empty;

            /* 73 is max durability, 159 and 160 are throw damage, 356 is something
               having to do with whether a quest item corresponds to
               normal/nightmare/hell difficulty, and 92 is level requirement which I
               don't think is in the game anyway. All of these don't need to be
               printed and are ignored. */
            if (prop.ID == 73 || prop.ID == 159 || prop.ID == 160 || prop.ID == 356 || prop.ID == 92)
            {
                return string.Empty;
            }
            
            /* 140 is extra blood, and while it's not printed in the game, I figured,
                why the hell not print it here? */
            if (prop.ID == 140)
            {
                return "Extra Blood";
            }

            /* This looks up the entry's string from its string index. The indexes are
               stuff like 'ModStr1a' and need to be translated with any one of three
               string tables. If these aren't loaded correctly, it'll just print
               'unknown' and exit. If there's no string index in the table, it'll
               continue with the name of the property instead of the official string. */
            var str = "";
            if (!string.IsNullOrEmpty(s.descstrpos))
            {
                var strpos = ItemDefs.StringTables.FindString(s.descstrpos);
                var strneg = ItemDefs.StringTables.FindString(s.descstrpos);
                if (string.IsNullOrEmpty(strpos) || string.IsNullOrEmpty(strpos))
                {
                    if (prop.ParamValue > 0)
                        return string.Format("unknown: {0}: {1}, {2} ({3})", s.Stat, prop.Value, prop.ParamValue, s.descval);
                    else
                        return string.Format("unknown: {0}: {1} ({2})", s.Stat, prop.Value, s.descval);
                }
                str = prop.Value > 0 ? strpos : strneg;
            }
            else
            {
                str = prop.PropertyName;
            }
            
            ///* Some min/max pairs need special treatment. */
            //switch (prop.ID)
            //{
            //    case 17:
            //        return string.Format() "+{0}%% Enhanced Damage", (prop->min + prop->max) / 2);
            //        return 1;
            //    case 21:
            //    case 23:
            //        min = prop->val;
            //        length = 0;
            //        /* If they appear in pairs, print both and return two. */
            //        if (prop->next && ((prop->id == 21 && (prop->next)->id == 22) ||
            //                  (prop->id == 23 && (prop->next)->id == 24)))
            //        {
            //            prop = prop->next;
            //            max = prop->val;
            //            fprintf(file, "Adds %d-%d damage", min, max);
            //            length += 2;
            //            /* If there's also a two-handed damage pair attached, return four. */
            //            if (prop->next && (prop->next)->id == 23 && (prop->next)->next && ((prop->next)->next)->id == 24)
            //            {
            //                length += 2;
            //            }
            //            return length;
            //        }
            //        /* If only min or max is present, along with its two-handed counterpart,
            //           print one of them and return two. */
            //        else if (prop->next && ((prop->id == 21 && (prop->next)->id == 23) ||
            //                   (prop->id == 22 && (prop->next)->id == 24)))
            //        {
            //            fprintf(file, "+%d %s", val, str);
            //            return 2;
            //        }
            //        else
            //        {
            //            fprintf(file, "+%d %s", val, str);
            //        }
            //        return 1;
            //    case 48:
            //        fprintf(file, "Adds %d-%d fire damage", prop->min, prop->max);
            //        return 1;
            //    case 50:
            //        fprintf(file, "Adds %d-%d lightning damage", prop->min, prop->max);
            //        return 1;
            //    case 52:
            //        fprintf(file, "Adds %d-%d magic damage", prop->min, prop->max);
            //        return 1;
            //    case 54:
            //        /* Length isn't printed, just like in the game. */
            //        fprintf(file, "Adds %d-%d cold damage", prop->min, prop->max);
            //        return 1;
            //    case 57:
            //        /* Poison damage is tricky, the game prints it in two different ways,
            //           and the values are based on the length. More details can be found at
            //           http://www.xmission.com/~trevin/DiabloIIv1.09_Magic_Properties.shtml */
            //        if (prop->max == prop->min)
            //        {
            //            fprintf(file, "+%d poison damage over %d seconds", (int)floor(((prop->min + prop->max) / 2.0 * prop->length / 256.0) + 0.5), (int)floor((prop->length / 25.0) + 0.5));
            //        }
            //        else
            //        {
            //            fprintf(file, "Adds %d-%d poison damage over %d seconds", (int)floor(((prop->min + prop->min) / 2.0 * prop->length / 256.0) + 0.5), (int)floor(((prop->max + prop->max) / 2.0 * prop->length / 256.0) + 0.5), (int)floor((prop->length / 25.0) + 0.5));
            //        }
            //        return 1;
            //    case 252:
            //        /* This wasn't displaying correctly, I can't remember why, so it's been
            //           hardcoded. */
            //        if (prop->val == 0)
            //        {
            //            break;
            //        }
            //        fprintf(file, "Repairs %d durability in %d seconds", 1, (int)floor((100.0 / prop->val) + 0.5));
            //        return 1;
            //    default:
            //        break;
            //}


   //         /* Every prop entry has a descfunc val that determines how it should be
   //output. I'll give an example of a property with that descfunc for each */
   //         switch (entry->str_type)
   //         {
   //             /* '+10 to Strength' */
   //             case 1: output = val > 0 ? "+%d" : "%d"; break;
   //             /* 'Damage Reduced by 10%' */
   //             case 2: output = "%d%%"; break;
   //             /* 'Magic Damage Reduced by 5' */
   //             case 3: output = "%d"; break;
   //             /* 'Fire Resist +20%' */
   //             case 4:
   //             /* 'Hit Causes Monster to Flee +25%' */
   //             case 5: output = val > 0 ? "+%d%%" : "%d%%"; break;
   //             /* '+10 to Dexterity (based on character level)' */
   //             case 6:
   //                 /* AR bonuses and damage to undead/demons are stored in halves */
   //                 if (prop->id == 224 || prop->id == 245 || prop->id == 246)
   //                 {
   //                     val *= clvl / 2;
   //                 }
   //                 /* everything else is in eighths */
   //                 else
   //                 {
   //                     val *= clvl / 8;
   //                 }
   //                 output = val > 0 ? "+%d" : "%d";
   //                 tail = "(based on character level)";
   //                 break;
   //             /* 'Fire Resist 30% (based on character level)' */
   //             case 7:
   //                 /* AR percent bonuses are stored in halves */
   //                 if (prop->id == 225)
   //                 {
   //                     val *= clvl / 2;
   //                 }
   //                 /* everything else is in eighths */
   //                 else
   //                 {
   //                     val *= clvl / 8;
   //                 }
   //                 output = "%d%%";
   //                 tail = "(based on character level)";
   //                 break;
   //             /* 'Heal Stamina Plus +50% (based on character level)' */
   //             case 8:
   //                 val *= clvl / 8;
   //                 output = val > 0 ? "+%d%%" : "%d%%%";
   //                 tail = "(based on character level)";
   //                 break;
   //             /* 'Attacker Takes Damage of 5 (based on character level)' */
   //             case 9:
   //                 val *= clvl / 8;
   //                 output = "%d";
   //                 tail = "(based on character level)";
   //                 break;
   //             /* 'Repairs 5 durability per second' (no, that's not a typo) */
   //             case 11:
   //                 if (find_str(str, "%d") == 1)
   //                 {
   //                     sprintf(aux_output, str, val);
   //                 }
   //                 break;
   //             /* 'Freezes target +5' */
   //             case 12: output = val > 0 ? "+%d" : "%d"; break;
   //             /* '+3 to Amazon Skill Levels' */
   //             case 13:
   //                 output = val > 0 ? "+%d" : "%d";
   //                 switch (prop->param)
   //                 {
   //                     case 0: str = "to Amazon Skill Levels"; break;
   //                     case 1: str = "to Sorceress Skill Levels"; break;
   //                     case 2: str = "to Necromancer Skill Levels"; break;
   //                     case 3: str = "to Paladin Skill Levels"; break;
   //                     case 4: str = "to Barbarian Skill Levels"; break;
   //                     case 5: str = "to Druid Skill Levels"; break;
   //                     case 6: str = "to Assassin Skill Levels"; break;
   //                     default: str = "to (unknown class) Skill Levels"; break;
   //                 }
   //                 break;
   //             /* '+1 to Bow and Crossbow Skills (Amazon Only)' */
   //             case 14:
   //                 output = val > 0 ? "+%d" : "%d";
   //                 switch ((prop->param & 0x7) + (prop->param >> 3 & 0x1ff) * 3)
   //                 {
   //                     case 0: str = "to Bow and Crossbow Skills (Amazon Only)"; break;
   //                     case 1: str = "to Passive and Magic Skills (Amazon Only)"; break;
   //                     case 2: str = "to Javelin and Spear Skills (Amazon Only)"; break;
   //                     case 3: str = "to Fire Skills (Sorceress Only)"; break;
   //                     case 4: str = "to Lightning Skills (Sorceress Only)"; break;
   //                     case 5: str = "to Cold Skills (Sorceress Only)"; break;
   //                     case 6: str = "to Curses (Necromancer Only)"; break;
   //                     case 7: str = "to Poison and Bone Skills (Necromancer Only)"; break;
   //                     case 8: str = "to Summoning Skills (Necromancer Only)"; break;
   //                     case 9: str = "to Combat Skills (Paladin Only)"; break;
   //                     case 10: str = "to Offensive Auras (Paladin Only)"; break;
   //                     case 11: str = "to Defensive Auras (Paladin Only)"; break;
   //                     case 12: str = "to Masteries (Barbarian Only)"; break;
   //                     case 13: str = "to Combat Skills (Barbarian Only)"; break;
   //                     case 14: str = "to Warcries (Barbarian Only)"; break;
   //                     case 15: str = "to Summoning Skills (Druid Only)"; break;
   //                     case 16: str = "to Shape Shifting Skills (Druid Only)"; break;
   //                     case 17: str = "to Elemental Skills (Druid Only)"; break;
   //                     case 18: str = "to Traps (Assassin Only)"; break;
   //                     case 19: str = "to Shadow Disciplines (Assassin Only)"; break;
   //                     case 20: str = "to Martial Arts (Assassin Only)"; break;
   //                     default: str = "to (unknown tab) Skills (unknown class Only)"; break;
   //                 }
   //                 break;
   //             /* '5% chance to cast Level 1 Amplify Damage on striking'*/
   //             case 15:
   //                 /* these strings are stored in the table with %d and %s tokens.
   //                    so, make sure they're really there first, and then use sprintf
   //                    directly for the desired results after looking up the skill ID. */
   //                 if (find_str(str, "%d") == 2 && find_str(str, "%s") == 1)
   //                 {
   //                     if (lookup_skill_by_id(skill_id))
   //                     {
   //                         skill_name = lookup_skill_by_id(skill_id)->name;
   //                         if (lookup_skill_by_id(skill_id)->str_name && lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name))
   //                         {
   //                             skill_name = lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name);
   //                         }
   //                     }
   //                     sprintf(aux_output, str, val, prop->skill_level, skill_name);
   //                 }
   //                 break;
   //             /* Level 1 Meditate Aura When Equipped */
   //             case 16:
   //                 /* same thing, roughly */
   //                 if (find_str(str, "%d") == 1 && find_str(str, "%s") == 1)
   //                 {
   //                     if (lookup_skill_by_id(prop->param))
   //                     {
   //                         skill_name = lookup_skill_by_id(prop->param)->name;
   //                         if (lookup_skill_by_id(prop->param)->str_name && lookup_string_by_name(lookup_skill_by_id(prop->param)->str_name))
   //                         {
   //                             skill_name = lookup_string_by_name(lookup_skill_by_id(prop->param)->str_name);
   //                         }
   //                     }
   //                     sprintf(aux_output, str, val, skill_name);
   //                 }
   //                 break;
   //             /* '+50 Defense (Increases During Daytime)' */
   //             case 17:
   //                 val = prop->val_off_time;
   //                 output = val > 0 ? "+%d" : "%d";
   //                 switch (prop->time)
   //                 {
   //                     case 0: tail = "(Increases During Daytime)"; break;
   //                     case 1: tail = "(Increases Near Dusk)"; break;
   //                     case 2: tail = "(Increases During Nighttime)"; break;
   //                     case 3: tail = "(Increases Near Dawn)"; break;
   //                     default: tail = "(Increases at unknown time)"; break;
   //                 }
   //             /* '+5% bonus to Attack Rating (Increase Near Dusk)' */
   //             case 18:
   //                 val = prop->val_off_time;
   //                 output = val > 0 ? "+%d%%" : "%d%%%";
   //                 switch (prop->time)
   //                 {
   //                     case 0: tail = "(Increases During Daytime)"; break;
   //                     case 1: tail = "(Increases Near Dusk)"; break;
   //                     case 2: tail = "(Increases During Nighttime)"; break;
   //                     case 3: tail = "(Increases Near Dawn)"; break;
   //                     default: tail = "(Increases at unknown time)"; break;
   //                 }
   //                 break;
   //             /* '-5% to Enemy Fire Resistance' (why is this in its own func? no idea) */
   //             case 20: output = val > 0 ? "+%d%%" : "%d%%"; break;
   //             /* '+50% to Damage versus Rakanishu' or something ... */
   //             case 22:
   //                 output = val > 0 ? "+%d%%" : "%d%%";
   //                 /* you don't really expect me to read in monster tables just for the one
   //                    oddball item that might have this property, that's likely not even in
   //                    the Goddamned game, do you */
   //                 tail = "(certain monster types)";
   //                 break;
   //             /* 'Reanimate as: Dracula' (or something) */
   //             case 23:
   //                 output = "%d%%";
   //                 tail = "(certain monster)";
   //                 break;
   //             /* 'Level 1 Teleport (50/50 Charges)' */
   //             case 24:
   //                 if (lookup_skill_by_id(skill_id))
   //                 {
   //                     skill_name = lookup_skill_by_id(skill_id)->name;
   //                     if (lookup_skill_by_id(skill_id)->str_name && lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name))
   //                     {
   //                         skill_name = lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name);
   //                     }
   //                 }
   //                 sprintf(aux_output, "Level %d %s (%d/%d Charges)", prop->skill_level, skill_name, prop->charges, prop->max_charges);
   //                 break;
   //             /* '+5000 to Blessed Hammer (Paladin Only)' */
   //             case 27:
   //                 if (lookup_skill_by_id(skill_id))
   //                 {
   //                     skill_name = lookup_skill_by_id(skill_id)->name;
   //                     if (lookup_skill_by_id(skill_id)->str_name && lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name))
   //                     {
   //                         skill_name = lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name);
   //                     }
   //                 }
   //                 sprintf(aux_output, "+%d to %s", prop->skill_level, skill_name);
   //                 if (skill_id >= 6 && skill_id < 36)
   //                 {
   //                     tail = "(Amazon Only)";
   //                 }
   //                 else if (skill_id >= 36 && skill_id < 66)
   //                 {
   //                     tail = "(Sorceress Only)";
   //                 }
   //                 else if (skill_id >= 66 && skill_id < 96)
   //                 {
   //                     tail = "(Necromancer Only)";
   //                 }
   //                 else if (skill_id >= 96 && skill_id < 126)
   //                 {
   //                     tail = "(Paladin Only)";
   //                 }
   //                 else if (skill_id >= 126 && skill_id < 156)
   //                 {
   //                     tail = "(Barbarian Only)";
   //                 }
   //                 else if (skill_id >= 221 && skill_id < 251)
   //                 {
   //                     tail = "(Druid Only)";
   //                 }
   //                 else if (skill_id >= 251 && skill_id < 281)
   //                 {
   //                     tail = "(Assassin Only)";
   //                 }
   //                 else
   //                 {
   //                     tail = "(unknown class Only)";
   //                 }
   //                 break;
   //             /* '+20 to Inferno' (no class restriction, and if the class uses it, they
   //                                  only receive +3 at most) */
   //             case 28:
   //                 if (lookup_skill_by_id(skill_id))
   //                 {
   //                     skill_name = lookup_skill_by_id(skill_id)->name;
   //                     if (lookup_skill_by_id(skill_id)->str_name && lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name))
   //                     {
   //                         skill_name = lookup_string_by_name(lookup_skill_by_id(skill_id)->str_name);
   //                     }
   //                 }
   //                 sprintf(aux_output, "+%d to %s", prop->skill_level, skill_name);
   //                 break;
   //             default:
   //                 fprintf(file, "unknown: %s (%d)", str, entry->str_val);
   //                 return 1;
   //         }







   //         /* Some display functions set their own output instead. */
   //         if (output[0] == '\0')
   //         {
   //             fprintf(file, "%s", aux_output);
   //         }
   //         /* 188 includes properties like '+3 to Summoning Skills (Necromancer Only)',
   //            I can't entirely remember why this shim was necessary */
   //         else if (prop->id == 188)
   //         {
   //             fprintf(file, output, val);
   //             fprintf(file, " %s", str);
   //         }
   //         /* str_val is another table entry that determines what direction / order 
   //            to print the value and the string. 0 means 'no value', 1 means 'value
   //            before string' and 2 means 'value after string'. */
   //         else if (entry->str_val == 0)
   //         {
   //             fprintf(file, "%s", str);
   //         }
   //         else if (entry->str_val == 1)
   //         {
   //             fprintf(file, output, val);
   //             fprintf(file, " %s", str);
   //         }
   //         else
   //         {
   //             fprintf(file, "%s ", str);
   //             fprintf(file, output, val);
   //         }
   //         /* Finally, some display functions set a tail, like '(based on character
   //            level)', so if necessary print that */
   //         if (tail[0] != '\0')
   //         {
   //             fprintf(file, " %s", tail);
   //         }
   //         return 1;





            return str;
        }
    }
}
