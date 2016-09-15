using System.Configuration;
using System.IO;

namespace Arenaii.Configuration
{
	public static class AppConfig
	{
		public static DirectoryInfo CompetitionDirectory
		{
			get
			{
				try
				{
					return new DirectoryInfo(ConfigurationManager.AppSettings["Competition.Dir"]);
				}
				catch
				{
					return new DirectoryInfo("competition");
				}
			}
		}

		public static DirectoryInfo BotsDirectory { get { return new DirectoryInfo(Path.Combine(CompetitionDirectory.FullName, "bots")); } }

		public static DirectoryInfo GamesDirectory { get { return new DirectoryInfo(Path.Combine(CompetitionDirectory.FullName, "games")); } }

		public static FileInfo ResultsFile { get { return new FileInfo(Path.Combine(CompetitionDirectory.FullName, "results.txt")); } }
	}
}
