using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FindFiles.Model
{
	class AppInfo
	{

		public string CurrentDirectory { get; set; } 
		public string SearchRegex { get; set; } 

		private static string _fileName { get; set; } = "AppInfo.json";

		public static void SaveInfo(string currentDirectory, string searchRegex)
		{
			if (File.Exists(_fileName))
				File.Delete(_fileName);
			File.Create(_fileName).Close();
			File.WriteAllText(_fileName, JsonConvert.SerializeObject(new AppInfo()
			{
				CurrentDirectory = currentDirectory,
				SearchRegex = searchRegex,
			}));
		}

		public static AppInfo GetInfo()
		{
			try
			{
				var json = File.ReadAllText(_fileName);
				return JsonConvert.DeserializeObject<AppInfo>(json);
			}
			catch (Exception)
			{
				return new();
			}
		}
	}
}
