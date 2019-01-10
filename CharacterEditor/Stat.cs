using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BKSystem.IO;
using System.ComponentModel;
using CharacterEditor.ResourceModels;

namespace CharacterEditor
{
	/// <summary>
	/// Controls access to character's stats
	/// </summary>
	public class Stat : INotifyPropertyChanged
	{
		/// <summary>
		/// Raw stat data from save file
		/// </summary>
		private byte[] statsBytes;
		/// <summary>
		/// Decoded stat values
		/// </summary>
		private Dictionary<int, int> statValues = new Dictionary<int, int>();

		public Stat(byte[] statsBytes)
		{
			if (statsBytes[0] != 'g' || statsBytes[1] != 'f')
			{
				throw new Exception("StatsByte data missing gf header");
			}

			this.statsBytes = statsBytes;
			ReadStats();
		}

		// TODO: Get rid of all of these individual properties?
		/// <summary>
		/// Base value of strength
		/// </summary>
		public int Strength
		{
			get { return GetStatValue("strength"); }
			set
			{
				SetStatValue("strength", value);
				OnPropertyChange("Strength");
			}
		}
		/// <summary>
		/// Base value of energy
		/// </summary>
		public int Energy
		{
			get { return GetStatValue("energy"); }
			set
			{
				SetStatValue("energy", value);
				OnPropertyChange("Energy");
			}
		}
		/// <summary>
		/// Base value of dexterity
		/// </summary>
		public int Dexterity
		{
			get { return GetStatValue("dexterity"); }
			set
			{
				SetStatValue("dexterity", value);
				OnPropertyChange("Dexterity");
			}
		}
		/// <summary>
		/// Base value of vitality
		/// </summary>
		public int Vitality
		{
			get { return GetStatValue("vitality"); }
			set
			{
				SetStatValue("vitality", value);
				OnPropertyChange("Vitality");
			}
		}
		/// <summary>
		/// Number of unallocated stat points
		/// </summary>
		public int StatPoints
		{
			get { return GetStatValue("statpts"); }
			set
			{
				SetStatValue("statpts", value);
				OnPropertyChange("StatPoints");
			}
		}
		/// <summary>
		/// Number of unallocated skill points
		/// </summary>
		public int SkillPoints
		{
			get { return GetStatValue("newskills"); }
			set
			{
				SetStatValue("newskills", value);
				OnPropertyChange("SkillPoints");
			}
		}
		/// <summary>
		/// Current value of hitpoints
		/// </summary>
		/// <remarks>Current value is usually higher than base value</remarks>
		public int Hitpoints
		{
			get { return GetStatValue("hitpoints"); }
			set
			{
				SetStatValue("hitpoints", value);
				OnPropertyChange("Hitpoints");
			}
		}
		/// <summary>
		/// Base value of hitpoints
		/// </summary>
		public int BaseHitpoints
		{
			get { return GetStatValue("maxhp"); }
			set
			{
				SetStatValue("maxhp", value);
				OnPropertyChange("BaseHitpoints");
			}
		}
		/// <summary>
		/// Current value of mana
		/// </summary>
		/// <remarks>Current value is usually higher than base value</remarks>
		public int Mana
		{
			get { return GetStatValue("mana"); }
			set
			{
				SetStatValue("mana", value);
				OnPropertyChange("Mana");
			}
		}
		/// <summary>
		/// Base value of mana
		/// </summary>
		public int BaseMana
		{
			get { return GetStatValue("maxmana"); }
			set
			{
				SetStatValue("maxmana", value);
				OnPropertyChange("BaseMana");
			}
		}
		/// <summary>
		/// Current value of stamina.
		/// </summary>
		/// <remarks>Current value is usually higher than base value</remarks>
		public int Stamina
		{
			get { return GetStatValue("stamina"); }
			set
			{
				SetStatValue("stamina", value);
				OnPropertyChange("Stamina");
			}
		}
		/// <summary>
		/// Base value of stamina
		/// </summary>
		public int BaseStamina
		{
			get { return GetStatValue("maxstamina"); }
			set
			{
				SetStatValue("maxstamina", value);
				OnPropertyChange("BaseStamina");
			}
		}
		/// <summary>
		/// Character's level
		/// </summary>
		public int Level
		{
			get { return GetStatValue("level"); }
			set
			{
				SetStatValue("level", value);
				OnPropertyChange("Level");
			}
		}
		/// <summary>
		/// Number of experience points character has
		/// </summary>
		public uint Experience
		{
			get { return (uint)GetStatValue("experience"); }
			set
			{
				SetStatValue("experience", (int)value);
				OnPropertyChange("Experience");
			}
		}
		/// <summary>
		/// Amount of gold character has in inventory
		/// </summary>
		public uint Gold
		{
			get { return (uint)GetStatValue("gold"); }
			set
			{
				SetStatValue("gold", (int)value);
				OnPropertyChange("Gold");
			}
		}
		/// <summary>
		/// Amount of gold character has in the bank
		/// </summary>
		public uint GoldBank
		{
			get { return (uint)GetStatValue("goldbank"); }
			set
			{
				SetStatValue("goldbank", (int)value);
				OnPropertyChange("GoldBank");
			}
		}
		/// <summary>
		/// Number of kills (Eastern sun only!)
		/// </summary>
		public int KillCount
		{
			get 
			{
				if (Resources.Instance.ResourceSet == "es300_r6d")
				{
					return GetStatValue("kill_counter");
				}
				return 0;
			}
			set
			{
				if (Resources.Instance.ResourceSet == "es300_r6d")
				{
					SetStatValue("kill_counter", value);
					OnPropertyChange("KillCount");
				}
			}
		}
		/// <summary>
		/// Times character has died (Eastern sun only!)
		/// </summary>
		public int DeathCount
		{
			get 
			{
				if (Resources.Instance.ResourceSet == "es300_r6d")
				{
					return GetStatValue("death_counter");
				}
				return 0;
			}
			set
			{
				if (Resources.Instance.ResourceSet == "es300_r6d")
				{
					SetStatValue("death_counter", value);
					OnPropertyChange("DeathCount");
				}
			}
		}

		/// <summary>
		/// Gets the specified stat's value
		/// </summary>
		/// <param name="name">Name of stat</param>
		/// <returns>Value of stat or 0 if stat is not present</returns>
		private int GetStatValue(string name)
		{
			int statId = ItemDefs.ItemStatCost[name].ID;

			if (!statValues.ContainsKey(statId))
			{
				return 0;
			}

			return statValues[statId];
		}

		/// <summary>
		/// Sets the specified stat to a given value or do nothing if stat is not present
		/// </summary>
		/// <param name="name">Name of stat</param>
		/// <param name="value">Value to set stat to</param>
		private void SetStatValue(string name, int value)
		{
            if (!ItemDefs.ItemStatCost.ContainsKey(name))
			{
				throw new Exception("SetStatValue: Invalid stat name " + name); 
			}

			int statId = ItemDefs.ItemStatCost[name].ID;

			// If value is 0, we assume user wants to delete the entry for that stat. 0 should 
			// be default if no record exists when diablo loads the save so it should work out?
			if (value == 0)
			{
				statValues.Remove(statId);
			}
			else
			{
				statValues[statId] = value;
			}
		}

		/// <summary>
		/// Parses raw character stat data
		/// </summary>
		/// <param name="headerBytes">Raw characte stat data found between "gf" and "if" near offset 765 in save file</param>
		/// <remarks>Bit lengths of stat types are found in the CSvBits column of ItemStatCost.txt</remarks>
		/// Source: http://phrozenkeep.hugelaser.com/forum/viewtopic.php?f=8&t=9011&start=50
		private void ReadStats()
		{
			BitStream bs = new BitStream(statsBytes);

			// Skip header bytes
			bs.SkipBits(8);
			bs.SkipBits(8);

			while (bs.RemainingBits >= 9)
			{
				// ID of stat (See ItemStatCost.txt)
				int statIndex = (int)bs.ReadReversed(9);
				// Value contains this many bits (See CSvBits in ItemStatCost.txt)
				int statValueBits = 0;
				// Value needs to be shifted by this amount
				int valShift = 0;

				// Terminating stat index
				if (statIndex == 0x1ff)
				{
					break;
				}

                var s = ItemDefs.ItemStatCost.Values.ElementAtOrDefault(statIndex);
                if (s == null)
                    throw new Exception("Invalid property ID " + statIndex);

                statValueBits = s.CSvBits;
				if (statValueBits == 0)
				{
					break;
				}

				valShift = s.ValShift;

				int statValue = (int)bs.ReadReversed(statValueBits);
				if (!statValues.ContainsKey(statIndex))
				{
					statValues.Add(statIndex, (statValue >> valShift));
				}
			}
		}

		/// <summary>
		/// Converts all stat data into raw data for save file
		/// </summary>
		/// <returns>Raw stat data ready for insertion into save file</returns>
		public byte[] GetStatBytes()
		{
			BitStream bits = new BitStream();

			bits.WriteReversed('g', 8);
			bits.WriteReversed('f', 8);

			var sortedValues = from n in statValues where true orderby n.Key select n;

			foreach (var stat in sortedValues)
			{
				bits.WriteReversed(stat.Key, 9);

				int valShift = 0;

                var s = ItemDefs.ItemStatCost.Values.ElementAtOrDefault(stat.Key);
                if (s == null)
                    throw new Exception("Invalid property ID " + stat.Key);

                int bitCount = s.CSvBits;
                valShift = s.ValShift;

                bits.WriteReversed((uint)((stat.Value << valShift)), bitCount);
			}

			// Write termining stat index
			bits.WriteReversed(0x1ff, 9);

			int remainingBitsForAlignment = 8 - (int)(bits.Position % 8);
			if (remainingBitsForAlignment > 0 && remainingBitsForAlignment < 8)
			{
				bits.WriteReversed(0, remainingBitsForAlignment);
			}
            

			return bits.ToReversedByteArray();
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Used to notify listeners that a property has changed. Mainly for data binding
		/// on the GUI
		/// </summary>
		/// <param name="propertyName">Name of property that was changed</param>
		private void OnPropertyChange(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}
