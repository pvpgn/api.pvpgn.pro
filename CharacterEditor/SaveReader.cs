using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CharacterEditor
{
    public class SaveReader : ICloneable
    {
        /// <summary>
        /// Character's inventory
        /// </summary>
        private Inventory inventory;
        /// <summary>
        /// General character information
        /// </summary>
        private Character character;
        /// <summary>
        /// Character's stats
        /// </summary>
        private Stat stat;
        /// <summary>
        /// Character's skills
        /// </summary>
        private Skill skill;
        /// <summary>
        /// Unmodified all data from save file
        /// </summary>
        private byte[] OriginalBytes;
        /// <summary>
        /// Unmodified character data from save file
        /// </summary>
        private byte[] OriginalCharacterBytes;
        /// <summary>
        /// Unmodified skill data from save file
        /// </summary>
        private byte[] OriginalSkillBytes;
        /// <summary>
        /// Unmodified inventory data from save file
        /// </summary>
        private byte[] OriginalInventoryBytes;

        public int Version
        {
            get
            {
                return BitConverter.ToInt32(OriginalBytes, 4);
            }
        }
        public int TimeStamp
        {
            get
            {
                return BitConverter.ToInt32(OriginalBytes, 48);
            }
        }

        /// <summary>
        /// Character's inventory
        /// </summary>
        public Inventory Inventory
        {
            get { return inventory; }
        }

        /// <summary>
        /// General character information
        /// </summary>
        public Character Character
        {
            get { return character; }
        }

        /// <summary>
        /// Character's stats (Str, dex, etc)
        /// </summary>
        public Stat Stat
        {
            get { return stat; }
        }

        /// <summary>
        /// Character's skills
        /// </summary>
        public Skill Skill
        {
            get { return skill; }
        }

        /// <summary>
        /// Position of the start of stat information (This should be hardcoded in all save files)
        /// </summary>
        private static int StatListBegin
        {
            get { return 765; }
        }

        /// <summary>
        /// Failed to decode character data
        /// </summary>
        public bool FailedCharacterDecoding { get; protected set; }

        /// <summary>
        /// Failed to decode skill data
        /// </summary>
        public bool FailedSkillDecoding { get; protected set; }

        /// <summary>
        /// Failed to decode inventory data
        /// </summary>
        public bool FailedInventoryDecoding { get; protected set; }


		/// <summary>
		/// Creates a new SaveReader
		/// </summary>
		public SaveReader(string resourceSet)
        {
            // load only once
            if (ItemDefs.Loaded)
                return;
			Resources.Instance.ResourceSet = resourceSet;
			ItemDefs.LoadItemDefs();
		}

		/// <summary>
		/// Creates a new SaveReader with specified save file bytes and begins to process the data
		/// </summary>
		/// <param name="rawCharacterData">Raw bytes from save file</param>
		public SaveReader(byte[] rawCharacterData)
		{
			Read(rawCharacterData);
		}

		/// <summary>
		/// Read character save from disk
		/// </summary>
		/// <param name="filePath">Path of save file</param>
		public void Read(byte[] rawCharacterData)
		{
			ReadHeaders(rawCharacterData);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skipFailedData">with true it return original bytes of "incorrect" parts</param>
        /// <param name="originalStat">FIXME: (HarpyWar) Often statbytes differs from original,
        /// so with this flag it returns with original stat bytes (not modified, if you modify it).</param>
        /// <returns></returns>
        public byte[] GetBytes(bool skipFailedData = false, bool originalStat = false)
        {
            byte[] characterBytes = (skipFailedData && FailedCharacterDecoding) ? OriginalCharacterBytes : Character.GetCharacterBytes();
            byte[] statsBytes = originalStat ? GetStatBytes(OriginalBytes) : Stat.GetStatBytes();
            byte[] skillBytes = (skipFailedData && FailedSkillDecoding) ? OriginalSkillBytes : Skill.GetSkillBytes();
            byte[] inventoryBytes = (skipFailedData && FailedInventoryDecoding) ? OriginalInventoryBytes : Inventory.GetInventoryBytes(Character.HasMercenary);
            byte[] rawCharacterData = new byte[characterBytes.Length + statsBytes.Length + skillBytes.Length + inventoryBytes.Length];

            Array.Copy(characterBytes, rawCharacterData, characterBytes.Length);
            Array.Copy(statsBytes, 0, rawCharacterData, characterBytes.Length, statsBytes.Length);
            Array.Copy(skillBytes, 0, rawCharacterData, characterBytes.Length + statsBytes.Length, skillBytes.Length);
            Array.Copy(inventoryBytes, 0, rawCharacterData, characterBytes.Length + statsBytes.Length + skillBytes.Length, inventoryBytes.Length);

            FixHeaders(ref rawCharacterData);

            return rawCharacterData;
        }

        /// <summary>
        /// Saves player data to specified path
        /// </summary>
        /// <param name="filePath">Path to save character data as</param>
        public void Write(Stream saveFile, bool skipFailedData = false)
		{
		    var rawCharacterData = GetBytes();

			using (BinaryWriter bw = new BinaryWriter(saveFile))
			{
				bw.Write(rawCharacterData);
			}

			//File.WriteAllBytes(filePath, rawCharacterData);
		}

		/// <summary>
		/// Splits character data into several sections for easier parsing
		/// </summary>
		private void ReadHeaders(byte[] rawCharacterData)
		{
			if (rawCharacterData[0] != 0x55 || rawCharacterData[1] != 0xAA || rawCharacterData[2] != 0x55 || rawCharacterData[3] != 0xAA)
			{
				throw new Exception("Not a Diablo II Save file");
			}
            if (rawCharacterData.Length == 130)
            {
                throw new EndOfStreamException("Brand new D2GS characters are not supported");
            }

            OriginalBytes = rawCharacterData;
            OriginalCharacterBytes = null;
            OriginalSkillBytes = null;
			OriginalInventoryBytes = null;

			byte[] statBytes = GetStatBytes(rawCharacterData);
			byte[] characterBytes = GetCharacterBytes(rawCharacterData);
			byte[] skillBytes = GetSkillBytes(rawCharacterData);
			byte[] inventoryBytes = GetInventoryBytes(rawCharacterData);

			inventory = new Inventory(inventoryBytes);
			character = new Character(characterBytes);
			stat = new Stat(statBytes);
			skill = new Skill(skillBytes);

			// Stats will always be different, we're not reading the fractional portion of hp/mana/stamina
			FailedCharacterDecoding = !characterBytes.SequenceEqual(character.GetCharacterBytes());
			FailedSkillDecoding = !skillBytes.SequenceEqual(skill.GetSkillBytes());
			FailedInventoryDecoding = !inventoryBytes.SequenceEqual(inventory.GetInventoryBytes(character.HasMercenary));

			if (FailedCharacterDecoding)
			{
				OriginalCharacterBytes = characterBytes;
			}
			if (FailedInventoryDecoding)
			{
				OriginalInventoryBytes = inventoryBytes;
			}
			if (FailedSkillDecoding)
			{
				OriginalSkillBytes = skillBytes;
			}
		}

		/// <summary>
		/// Obtains general character information from raw save file data
		/// </summary>
		/// <param name="rawCharacterData"></param>
		/// <returns></returns>
		private static byte[] GetCharacterBytes(byte[] rawCharacterData)
		{
			byte[] characterBytes = new byte[StatListBegin];
			Array.Copy(rawCharacterData, characterBytes, characterBytes.Length);

			return characterBytes;
		}

		/// <summary>
		/// Copies character's raw stat data from raw header bytes
		/// </summary>
		/// <param name="rawCharacterBytes">Raw header data from save file</param>
		/// <returns>Raw stat data</returns>
		private static byte[] GetStatBytes(byte[] rawCharacterBytes)
		{
			byte[] statsSection;
			int statsSectionLength = FindStatListEnd(rawCharacterBytes) - StatListBegin;

			statsSection = new byte[statsSectionLength];
			Array.Copy(rawCharacterBytes, StatListBegin, statsSection, 0, statsSection.Length);

			return statsSection;
		}

		/// <summary>
		/// Obtains skill information from raw save file data
		/// </summary>
		/// <param name="rawCharacterData"></param>
		/// <returns></returns>
		private static byte[] GetSkillBytes(byte[] rawCharacterData)
		{
			int itemListBegin = FindItemListBegin(rawCharacterData);
			int statListEnd = FindStatListEnd(rawCharacterData);

			byte[] skillBytes = new byte[itemListBegin - statListEnd];
			Array.Copy(rawCharacterData, statListEnd, skillBytes, 0, skillBytes.Length);

			return skillBytes;
		}

		/// <summary>
		/// Obtains inventory information from raw save file data
		/// </summary>
		/// <param name="rawCharacterData"></param>
		/// <returns></returns>
		private static byte[] GetInventoryBytes(byte[] rawCharacterData)
		{
			int itemListBegin = FindItemListBegin(rawCharacterData);

			byte[] inventoryBytes = new byte[rawCharacterData.Length - itemListBegin];
			Array.Copy(rawCharacterData, itemListBegin, inventoryBytes, 0, inventoryBytes.Length);

			return inventoryBytes;
		}

		/// <summary>
		/// Corrects checksum of new player data
		/// </summary>
		/// <param name="rawCharacterData">Raw player save data</param>
		private void FixHeaders(ref byte[] rawCharacterData)
		{
			byte[] fileSizeBytes = BitConverter.GetBytes(rawCharacterData.Length);
			Array.Copy(fileSizeBytes, 0, rawCharacterData, 8, 4);

			uint checksum = CalculateChecksum(rawCharacterData);

			byte[] checksumBytes = BitConverter.GetBytes(checksum);
			Array.Copy(checksumBytes, 0, rawCharacterData, 12, 4);
		}

		// 
		/// <summary>
		/// Calculates a new checksum for specified data
		/// </summary>
		/// <param name="fileBytes">Raw character data</param>
		/// <returns>Checksum for specified data</returns>
		/// <remarks>Source: ehertlein ( http://forums.diii.net/showthread.php?t=532037&page=41 )</remarks>
		private static uint CalculateChecksum(byte[] fileBytes)
		{
			uint hexTest = 0x80000000;
			uint checksum = 0;

			// clear out the old checksum
			fileBytes[12] = 0;
			fileBytes[13] = 0;
			fileBytes[14] = 0;
			fileBytes[15] = 0;

			foreach (byte currentByte in fileBytes)
			{
				if ((checksum & hexTest) == hexTest)
				{
					checksum = checksum << 1;
					checksum = checksum + 1;
				}
				else
				{
					checksum = checksum << 1;
				}

				checksum += currentByte;
			}

			return checksum;
		}

		/// <summary>
		/// Returns the location of the inventory data
		/// </summary>
		/// <param name="rawCharacterData">Raw bytes from save file</param>
		/// <returns>Location of inventory data</returns>
		private static int FindItemListBegin(byte[] rawCharacterData)
		{
			for (int i = StatListBegin; i < rawCharacterData.Length - 5; i++)
			{
				if (rawCharacterData[i] == 'J' && rawCharacterData[i + 1] == 'M')
				{
					// JM..JM is the pattern we're looking for
					if (rawCharacterData[i + 4] == 'J' && rawCharacterData[i + 5] == 'M')
					{
						return i;
					}
				}
			}

			return 0;
		}

		/// <summary>
		/// Returns location right after the end of the character's stat list
		/// </summary>
		/// <param name="rawCharacterBytes">Raw data from save file</param>
		/// <returns>Location of the end of character's stat list</returns>
		private static int FindStatListEnd(byte[] rawCharacterBytes)
		{
			int itemListBegin = FindItemListBegin(rawCharacterBytes);

			if (rawCharacterBytes[itemListBegin - 37] == 'i' && rawCharacterBytes[itemListBegin - 36] == 'f')
			{
				return itemListBegin - 37;
			}

			for (int i = FindItemListBegin(rawCharacterBytes); i > StatListBegin; i--)
			{
				if (rawCharacterBytes[i] == 'i' && rawCharacterBytes[i + 1] == 'f')
				{
					return i;
				}
			}

			throw new Exception("End of stat list not found!");
		}

		/// <summary>
		/// Move all allocated skill and stat points to pool of unspent points. Fails if golem exists
		/// </summary>
		/// TODO: Untested!
		public void Respec()
		{
			if (Inventory.GolemItems.Count > 0)
			{
				throw new Exception("Respec: Cannot respec with an active golem");
			}

			int totalSkillPoints = Stat.SkillPoints;
			int totalStatPoints = stat.StatPoints;

			for (int i = 0; i < Skill.Length; i++)
			{
				totalSkillPoints += Skill[i];
				Skill[i] = 0;
			}

			totalStatPoints += Stat.Strength;
			totalStatPoints += Stat.Dexterity;
			totalStatPoints += Stat.Vitality;
			totalStatPoints += Stat.Energy;

			Stat.Strength = 0;
			Stat.Dexterity = 0;
			Stat.Vitality = 0;
			Stat.Energy = 0;

			Stat.SkillPoints = totalSkillPoints;
			Stat.StatPoints = totalStatPoints;
		}

        public object Clone()
        {
            return this.MemberwiseClone();
        }
	}
}
