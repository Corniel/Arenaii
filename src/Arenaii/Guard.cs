using System;
using System.IO;

namespace Arenaii
{
	public static class Guard
	{
		public static T NotNull<T>(T value, string name) where T : class
		{
			if (value == null) { throw new ArgumentNullException(name); }
			return value;
		}

		public static FileInfo Exists(FileInfo file, string name)
		{
			NotNull(file, name);
			if (!file.Exists)
			{
				throw new ArgumentException(string.Format("The File '{0}' does not exist.", file), name);
			}
			return file;
		}
	}
}
