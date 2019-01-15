using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CharacterEditor
{
	public class ResourceUtils
	{
		public static Stream OpenResource(string resourcePath)
		{
			return File.Open(resourcePath, FileMode.Open);
		}

		public static StreamReader OpenResourceText(string resourcePath)
		{
			return new StreamReader(OpenResource(resourcePath));
		}

		public static BinaryReader OpenResourceBinary(string resourcePath)
		{
			return new BinaryReader(OpenResource(resourcePath));
		}

		public static List<string> ReadAllLines(string resourcePath)
		{
			List<string> lines = new List<string>();

			using (StreamReader sr = OpenResourceText(resourcePath))
			{
				while (!sr.EndOfStream)
				{
					lines.Add(sr.ReadLine());
				}
			}

			return lines;
		}

		public static string ReadToEnd(string resourcePath)
		{
			using (StreamReader sr = OpenResourceText(resourcePath))
			{
				return sr.ReadToEnd();
			}
		}

		public static byte[] ReadAllBytes(string resourcePath)
		{
			using (BinaryReader br = OpenResourceBinary(resourcePath))
			{
				return br.ReadBytes((int)br.BaseStream.Length);
			}
		}
	}
}