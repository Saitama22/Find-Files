using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using FindFiles.Model;

namespace FindFiles.ViewModel
{
	class MainViewModel : INotifyPropertyChanged
	{
		private string _currentDirectory;
		private string _searchRegex;

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string p)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
		}

		public ICommand FindCommand => new RelayCommand(action =>
		{
			SearchInfo = string.Empty;
			_startTime = DateTime.Now;
			new Thread(() => { StartTimer(); }).Start();
			try
			{
				var t = Directory.GetDirectories(CurrentDirectory);
				IEnumerable<string> findFilesWithDirs = Directory.GetFiles(CurrentDirectory, "*", SearchOption.AllDirectories);
				CountAllFiles = findFilesWithDirs.Count();
				if (!string.IsNullOrEmpty(SearchRegex))
					findFilesWithDirs = findFilesWithDirs.Where(r => 
						new Regex(SearchRegex).IsMatch(r.Substring(r.LastIndexOf("\\"))));
				CountFindFiles = findFilesWithDirs.Count();

				findFilesWithDirs = findFilesWithDirs.Select(r => r.Replace(CurrentDirectory + "\\", string.Empty));
				var findFilesPaths = findFilesWithDirs.Select(r => r.Split("\\"));

				TreeViewNodes = new List<TreeViewNodes>();
				var maxFileLen = findFilesPaths.Select(r => r.Length).Max();

				foreach (var findFilesPath in findFilesPaths)
				{
					var currentTreeViewNodes = TreeViewNodes;
					foreach (var fileDir in findFilesPath)
					{
						var tempTreeViewNodes = currentTreeViewNodes.FirstOrDefault(r => r.Text == fileDir);
						if (tempTreeViewNodes == null)
						{
							tempTreeViewNodes = new()
							{
								Text = fileDir,
							};
							currentTreeViewNodes.Add(tempTreeViewNodes);
						}
						currentTreeViewNodes = tempTreeViewNodes.Children;
					}
				}
			}
			catch (Exception ex)
			{
				SearchInfo = "Не удалось найти файлы";
				TreeViewNodes = null;
				CountFindFiles = 0;
				CountAllFiles = 0;
			}
			finally
			{
				OnPropertyChanged(nameof(SearchInfo));
				OnPropertyChanged(nameof(TreeViewNodes));
				OnPropertyChanged(nameof(CountFindFiles));
				OnPropertyChanged(nameof(CountAllFiles));
			}
		}, canExecute => { return FindCommandWorking; });

		private void StartTimer()
		{
			while (true)
			{
				TimeInfo = (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss");
				OnPropertyChanged(nameof(TimeInfo));
			}
		}

		public string CurrentDirectory
		{
			get
			{
				_currentDirectory ??= AppInfo.GetInfo().CurrentDirectory;
				return _currentDirectory;
			}

			set
			{
				_currentDirectory = value;
				AppInfo.SaveInfo(_currentDirectory, _searchRegex);
			}
		}
		public string SearchRegex
		{
			get
			{
				_searchRegex ??= AppInfo.GetInfo().SearchRegex;
				return _searchRegex;
			}

			set
			{
				_searchRegex = value;
				AppInfo.SaveInfo(_currentDirectory, _searchRegex);
			}
		}
		public IList<TreeViewNodes> TreeViewNodes { get; set; }
		private bool FindCommandWorking { get; set; } = true;

		public int CountFindFiles { get; set; }
		public int CountAllFiles { get; set; }

		public string TimeInfo { get; set; } = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
		public DateTime _startTime { get; set; }

		public string SearchInfo { get; set; }
	}
}
