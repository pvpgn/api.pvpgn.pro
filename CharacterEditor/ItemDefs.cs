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
    }
}
