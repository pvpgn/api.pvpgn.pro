using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Linq;

namespace CharacterEditor
{
	public class ItemDefs
	{
		private static Dictionary<string, string> itemDescriptions = new Dictionary<string, string>();
		private static Dictionary<string, string> setDescriptions = new Dictionary<string, string>();
		private static Dictionary<string, HashSet<string>> itemCodeSets = new Dictionary<string, HashSet<string>>();
		private static Dictionary<string, ItemStatCost> itemStatCostsByName = new Dictionary<string, ItemStatCost>();
		private static Dictionary<int, ItemStatCost> itemStatCostsById = new Dictionary<int, ItemStatCost>();

		/// <summary>
		/// List of ItemStatCost records based on Name
		/// </summary>
		public static Dictionary<string, ItemStatCost> ItemStatCostsByName
		{
			get { return itemStatCostsByName; }
		}

		/// <summary>
		/// List of ItemStatCost records based on ID
		/// </summary>
		public static Dictionary<int, ItemStatCost> ItemStatCostsById
		{
			get { return itemStatCostsById; }
		}

		// TODO: Temp fix! Get rid of static class and move to singleton based on current resource
		//  set (es300_r6d, rot_1.a3.1, etc)
		internal static void LoadItemDefs()
		{
			// Temp!
			itemDescriptions = new Dictionary<string, string>();
			setDescriptions = new Dictionary<string, string>();
			itemCodeSets = new Dictionary<string, HashSet<string>>();
			itemStatCostsByName = new Dictionary<string, ItemStatCost>();
			itemStatCostsById = new Dictionary<int, ItemStatCost>();

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

			ReadItemStatCost();
		}

		static ItemDefs()
		{
			//LoadItemDefs(); // TODO: No more static class
		}

		/// <summary>
		/// Read ItemStatCost data
		/// </summary>
		private static void ReadItemStatCost()
		{
			List<ItemStatCost> statCosts = ItemStatCost.Read(Resources.Instance.OpenResourceText("ItemStatCost.txt"));

			itemStatCostsByName = statCosts.ToDictionary(v => v.Stat, v => v);
			itemStatCostsById = statCosts.ToDictionary(v => v.ID, v => v);
		}

		/// <summary>
		/// Returns the name of the specified property ID
		/// </summary>
		/// <param name="id">ID of property</param>
		/// <returns>Name of property with specified ID</returns>
		public static string GetPropertyName(int id)
		{
			if (!itemStatCostsById.ContainsKey(id))
			{
				if (id == 0x1ff)
				{
					return "EOF";
				}

				return "UNKNOWN";
			}

			return itemStatCostsById[id].Stat;
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
