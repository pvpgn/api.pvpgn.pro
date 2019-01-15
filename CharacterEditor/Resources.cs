using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using CsvHelper.Configuration.Attributes;

namespace CharacterEditor
{
	public class Resources
	{
		private static readonly Resources instance = new Resources();
		public static Resources Instance
		{
			get { return instance; }
		}

		private Resources()
		{

		}

		private string resourceSet;
		public string ResourceSet
		{
			get
			{
				return resourceSet;
			}
			set
            {
                var newValue = Path.Combine(ResourcePath, "Resources", value);
                if (IsValidResourceSet(newValue))
				{
					resourceSet = newValue;
				}
			}
		}

        /// <summary>
        /// General path of resources
        /// </summary>
        public static string ResourcePath { get; set; }


		internal static bool IsValidResourceSet(string value)
		{
			return Directory.Exists(value);
		}

        public Stream OpenResource(string resourceName)
		{
			string resourcePath = Path.Combine(resourceSet, resourceName);

			return ResourceUtils.OpenResource(resourcePath);
		}

		public StreamReader OpenResourceText(string resourceName)
		{
			string resourcePath = Path.Combine(resourceSet, resourceName);

            return ResourceUtils.OpenResourceText(resourcePath);
		}

		public List<string> ReadAllLines(string resourceName)
		{
			string resourcePath = Path.Combine(resourceSet, resourceName);

            return ResourceUtils.ReadAllLines(resourcePath);
		}
    }
}
