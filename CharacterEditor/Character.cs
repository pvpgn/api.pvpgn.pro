using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;

namespace CharacterEditor
{
	public class Character : INotifyPropertyChanged
	{
		private byte[] characterBytes;
		private byte characterFlags;

		/// <summary>
		/// Character classes
		/// </summary>
		public enum CharacterClass
		{
			Amazon,
			Sorceress,
			Necromancer,
			Paladin,
			Barbarian,
			Druid,
			Assassin,
		}

		/// <summary>
		/// Character's name
		/// </summary>
		public String Name
		{
			get
			{
				string name = UnicodeEncoding.UTF8.GetString(characterBytes, 0x14, 16);
				return name.Substring(0, name.IndexOf('\0'));
			}
			set
			{
				byte[] paddedName = new byte[16];
				byte[] newName = UnicodeEncoding.UTF8.GetBytes(value);

				//15 instead of 16 to keep trailing null character
				Array.Copy(newName, paddedName, newName.Length < 15 ? newName.Length : 15);
				Array.Copy(paddedName, 0, characterBytes, 0x14, paddedName.Length);
				OnPropertyChange("Name");
			}
		}

		/// <summary>
		/// Character is in hardcore mode
		/// </summary>
		public bool Hardcore
		{
			get
			{
				return (characterFlags & 0x04) > 0;
			}
			set
			{
				if (value)
				{
					characterFlags |= 0x04;
				}
				else
				{
					characterFlags &= 0xfb;
				}
				OnPropertyChange("Hardcore");
			}
		}

		/// <summary>
		/// Character has died before
		/// </summary>
		public bool Died
		{
			get
			{
				return (characterFlags & 0x08) > 0;
			}
			set
			{
				if (value)
				{
					characterFlags |= 0x08;
				}
				else
				{
					characterFlags &= 0xf7;
				}
				OnPropertyChange("Died");
			}
		}

        /// <summary>
        /// Character has died before
        /// </summary>
        public bool Ladder
        {
            get
            {
                return (characterFlags & 0x40) > 0;
            }
            set
            {
                if (value)
                {
                    characterFlags |= 0x40;
                }
                else
                {
                    characterFlags &= 0xbf;
                }
                OnPropertyChange("Ladder");
            }
        }


        /// <summary>
        /// Character is expansion character
        /// </summary>
        public bool Expansion
		{
			get
			{
				return (characterFlags & 0x20) > 0;
			}
			set
			{
				if (value)
				{
					characterFlags |= 0x20;
				}
				else
				{
					characterFlags &= 0xDF;
				}
				OnPropertyChange("Expansion");
			}
		}

		/// <summary>
		/// Collection of unknown flags
		/// </summary>
		public byte UnknownFlags
		{
			get
			{
				return (byte)(characterFlags & (byte)0xd3);
			}
			set
			{
				characterFlags &= 0x2C;
				characterFlags |= value;
				OnPropertyChange("UnknownFlags");
			}
		}

		/// <summary>
		/// Character's title / progression
		/// </summary>
		public byte Progression
		{
			get { return characterBytes[37]; }
			set
			{
				characterBytes[37] = value;
				OnPropertyChange("Progression");
			}
		}

		/// <summary>
		/// Character's class
		/// </summary>
		public CharacterClass Class
		{
			get { return (CharacterClass)characterBytes[40]; }
			set
			{
				characterBytes[40] = (byte)value;
				OnPropertyChange("Class");
			}
		}

		/// <summary>
		/// Level displayed on character list ?
		/// </summary>
		public byte LevelDisplay
		{
			get { return characterBytes[43]; }
			set
			{
				characterBytes[43] = value;
				OnPropertyChange("LevelDisplay");
			}
		}

		/// <summary>
		/// Chracter has a mercenary
		/// </summary>
		public bool HasMercenary
		{
			get
			{
				return BitConverter.ToUInt32(characterBytes, 179) != 0;
			}
		}

		/// <summary>
		/// ID Of mercenary's name
		/// </summary>
		public ushort MercenaryNameId
		{
			get
			{
				return BitConverter.ToUInt16(characterBytes, 183);
			}
		}

		/// <summary>
		/// Mercenary type
		/// </summary>
		public ushort MercenaryType
		{
			get
			{
				return BitConverter.ToUInt16(characterBytes, 185);
			}
		}

		/// <summary>
		/// Mercenary's experience points
		/// </summary>
		public uint MercenaryExp
		{
			get
			{
				return BitConverter.ToUInt32(characterBytes, 185);
			}
		}

		/// <summary>
		/// Creates a new character reader with specified header
		/// </summary>
		/// <param name="characterBytes">Raw character bytes from save file</param>
		public Character(byte[] characterBytes)
		{
			if (characterBytes[0] != 0x55 || characterBytes[1] != 0xAA || characterBytes[2] != 0x55 || characterBytes[3] != 0xAA)
			{
				throw new Exception("CharacterByte data missing 0x55AA55AA header");
			}

			this.characterBytes = characterBytes;
			characterFlags = characterBytes[0x24];
		}

		/// <summary>
		/// Returns modified character bytes for use in save file
		/// </summary>
		/// <returns>Raw character bytes</returns>
		public byte[] GetCharacterBytes()
		{
			characterBytes[0x24] = characterFlags;

			return characterBytes;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

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
