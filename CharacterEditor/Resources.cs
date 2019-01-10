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
				if (IsValidResourceSet(value))
				{
					resourceSet = value;
				}
			}
		}

		/// <summary>
		/// Obtains a list of all available resource sets. Resources must be marked as Resource when building project.
		/// </summary>
		/// <example>Resources/es300_R6D/AllItems.txt -> es300_R6D</example>
		/// <returns></returns>
		/// http://blogs.microsoft.co.il/blogs/alex_golesh/archive/2009/11/13/silverlight-tip-enumerating-embedded-resources.aspx
		private static List<string> GetResourceSetsSilverlight()
		{
			List<string> resourceNames = new List<string>();
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			string[] resources = currentAssembly.GetManifestResourceNames();

			foreach (var resource in resources)
			{
				ResourceManager rm = new ResourceManager(resource.Replace(".resources", ""), currentAssembly);
				rm.GetStream("app.xaml"); // can't seem to GetResourceSet without getting a stream -> see above link where code comes from
				ResourceSet rs = rm.GetResourceSet(Thread.CurrentThread.CurrentCulture, false, true);

				foreach (DictionaryEntry item in rs)
				{
					string resourceName = (string)item.Key;

					// "Resources/es300_R6D/AllItems.txt"
					Regex regex = new Regex(@"^(?<root>[^/]+)/(?<set>[^/]+)/(?<file>.*)$");

					if (regex.IsMatch(resourceName))
					{
						Match match = regex.Match(resourceName);

						if (match.Groups["file"].Value.ToLower() == "allitems.txt")
						{
							resourceNames.Add(match.Groups["set"].Value);
						}
					}
				}

				if (resourceNames.Count > 0)
				{
					return resourceNames;
				}
			}

			return resourceNames;
		}

#if !SILVERLIGHT
		private static List<string> GetResourceSetsDesktop()
		{
			List<string> resourceNames = new List<string>();
			string[] directories = Directory.GetDirectories("Resources/");

			foreach (var dir in directories)
			{
				if (File.Exists(dir + "/" + "allitems.txt"))
				{
					resourceNames.Add(Path.GetFileName(dir));
				}
			}

			return resourceNames;
		}
#endif

	    public static string CurrentDirectory
	    {
	        get
	        {
                return Directory.GetCurrentDirectory();
	        }
            set
            {
                Directory.SetCurrentDirectory(value);
            }
	    }

		public static List<string> GetResourceSets()
		{
#if SILVERLIGHT
			return GetResourceSetsSilverlight();
#else
			return GetResourceSetsDesktop();
#endif
		}


		internal static bool IsValidResourceSet(string value)
		{
#if SILVERLIGHT
			// Pretty lengthy process, but is rarely called
			return GetResourceSetsSilverlight().Contains(value.ToLower());
#else
			return Directory.Exists("Resources/" + value);
#endif
		}

        public Stream OpenResource(string resourceName)
		{
			string resourcePath = "Resources/" + resourceSet + "/" + resourceName;

			return ResourceUtils.OpenResource("CharacterEditor.Silverlight", resourcePath);
		}

		public StreamReader OpenResourceText(string resourceName)
		{
			string resourcePath = "Resources/" + resourceSet + "/" + resourceName;

			return ResourceUtils.OpenResourceText("CharacterEditor.Silverlight", resourcePath);
		}

		public List<string> ReadAllLines(string resourceName)
		{
			string resourcePath = "Resources/" + resourceSet + "/" + resourceName;

			return ResourceUtils.ReadAllLines("CharacterEditor.Silverlight", resourcePath);
		}
    }
}
