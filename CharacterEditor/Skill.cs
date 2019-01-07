using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CharacterEditor
{
	/// <summary>
	/// Skill class for accessing raw skill data
	/// </summary>
	public class Skill : IEnumerable
	{
		/// <summary>
		/// Each byte in skillBytes represents level of a skill
		/// </summary>
		private byte[] skillBytes;

		/// <summary>
		/// Gets or sets the skill at specified index
		/// </summary>
		/// <param name="index">Index of skill to access</param>
		/// <returns>Value of skill at specified index</returns>
		public byte this[int index]
		{
			get
			{
				return skillBytes[index+2];
			}
			set
			{
				skillBytes[index+2] = value;
			}
		}

		/// <summary>
		/// Number of skills
		/// </summary>
		public int Length
		{
			get { return skillBytes.Length - 2; }
		}

		/// <summary>
		/// Creates a new skill class for accessing raw skill data
		/// </summary>
		/// <param name="skillBytes">Raw skill data from save file</param>
		/// <remarks>Skill data exists between the end of stat data and beginning of item data</remarks>
		public Skill(byte[] skillBytes)
		{
			if (skillBytes[0] != 'i' || skillBytes[1] != 'f')
			{
				throw new Exception("SkillByte data missing if header");
			}

			this.skillBytes = skillBytes;
		}

		/// <summary>
		/// Converts all skill data into raw data for save file
		/// </summary>
		/// <returns>Raw skill data ready for insertion into save file</returns>
		public byte[] GetSkillBytes()
		{
			return skillBytes;
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			for (int i = 0; i < skillBytes.Length; i++)
			{
				yield return skillBytes[i];
			}
		}

		#endregion
	}
}
