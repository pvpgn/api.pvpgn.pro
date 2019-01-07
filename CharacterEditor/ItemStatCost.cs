using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace CharacterEditor
{
	public class ItemStatCost
	{
		public string Stat { get; set; }
		public int ID { get; set; }
		public bool SendOther { get; set; }
		public bool Signed { get; set; }
		public int SendBits { get; set; }
		public int SendParamBits { get; set; }
		public bool UpdateAnimRate { get; set; }
		public bool Saved { get; set; }
		public bool CSvSigned { get; set; }
		public int CSvBits { get; set; }
		public int CSvParam { get; set; }
		public bool FCallback { get; set; }
		public bool FMin { get; set; }
		public int MinAccr { get; set; }
		public int Encode { get; set; }
		public int Add { get; set; }
		public int Multiply { get; set; }
		public int Divide { get; set; }
		public int ValShift { get; set; }
		public int OldSaveBits { get; set; }
		public int OldSaveAdd { get; set; }
		public int SaveBits { get; set; }
		public int SaveAdd { get; set; }
		public int SaveParamBits { get; set; }
		public int Keepzero { get; set; }
		public int Op { get; set; }
		public int OpParam { get; set; }
		public string OpBase { get; set; }
		public string OpStat1 { get; set; }
		public string OpStat2 { get; set; }
		public string OpStat3 { get; set; }
		public bool Direct { get; set; }
		public string Maxstat { get; set; }
		public bool Itemspecific { get; set; }
		public bool Damagerelated { get; set; }
		public string Itemevent1 { get; set; }
		public int Itemeventfunc1 { get; set; }
		public string Itemevent2 { get; set; }
		public int Itemeventfunc2 { get; set; }
		public int Descpriority { get; set; }
		public int Descfunc { get; set; }
		public int Descval { get; set; }
		public string Descstrpos { get; set; }
		public string Descstrneg { get; set; }
		public string Descstr2 { get; set; }
		public int Dgrp { get; set; }
		public int Dgrpfunc { get; set; }
		public int Dgrpval { get; set; }
		public string Dgrpstrpos { get; set; }
		public string Dgrpstrneg { get; set; }
		public string Dgrpstr2 { get; set; }
		public int Stuff { get; set; }
		public bool Eol { get; set; }

		public ItemStatCost()
		{

		}

		public ItemStatCost(string[] data)
		{
			int i = 0;

			// TODO: Horrible cut+paste+replace mess
			Stat = (string)data[i++];
			ID = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			SendOther = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			Signed = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			SendBits = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			SendParamBits = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			UpdateAnimRate = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			Saved = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			CSvSigned = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			CSvBits = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			CSvParam = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			FCallback = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			FMin = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			MinAccr = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Encode = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Add = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Multiply = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Divide = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			ValShift = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			OldSaveBits = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			OldSaveAdd = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			SaveBits = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			SaveAdd = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			SaveParamBits = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Keepzero = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Op = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			OpParam = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			OpBase = (string)data[i++];
			OpStat1 = (string)data[i++];
			OpStat2 = (string)data[i++];
			OpStat3 = (string)data[i++];
			Direct = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			Maxstat = (string)data[i++];
			Itemspecific = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			Damagerelated = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
			Itemevent1 = (string)data[i++];
			Itemeventfunc1 = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Itemevent2 = (string)data[i++];
			Itemeventfunc2 = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Descpriority = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Descfunc = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Descval = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Descstrpos = (string)data[i++];
			Descstrneg = (string)data[i++];
			Descstr2 = (string)data[i++];
			Dgrp = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Dgrpfunc = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Dgrpval = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Dgrpstrpos = (string)data[i++];
			Dgrpstrneg = (string)data[i++];
			Dgrpstr2 = (string)data[i++];
			Stuff = (int)StringUtils.ConvertFromString(data[i++], typeof(int));
			Eol = (bool)StringUtils.ConvertFromString(data[i++], typeof(bool));
		}

		public static List<ItemStatCost> Read(StreamReader stream)
		{
			List<ItemStatCost> statCosts = new List<ItemStatCost>();
			PropertyInfo[] properties = typeof(ItemStatCost).GetProperties();

			using (stream)
			{
				stream.ReadLine();

				while (!stream.EndOfStream)
				{
					string currentLine = stream.ReadLine();
					string[] splitLine = currentLine.Split('\t');


					statCosts.Add(new ItemStatCost(splitLine));
				}
			}

			return statCosts;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", ID, Stat);
		}
	}
}
