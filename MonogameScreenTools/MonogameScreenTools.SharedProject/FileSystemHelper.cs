using System;
using System.IO;
#if ANDROID
using Android.OS;
using Environment = Android.OS.Environment;
#endif

namespace MonogameScreenTools
{
	public static class FileSystemHelper
	{
		/// <summary>
		/// Generate and save a screenshot.
		/// </summary>
		/// <param name="filename">The filename you'd like to use, with no path or file extension. Leave blank for the default filename</param>
		/// <param name="appendTimeStamp">Whether or not to append the current date/time to the end of the filename. </param>
		/// <returns>The full path, filename, and extension of the file that was created.</returns>
		public static string CreateFilename(string filename, string extension, bool appendTimeStamp = true)
		{
			//Setup the filename
			if (appendTimeStamp)
			{
				filename += DateTime.Now.ToFileTime();
			}

			//append the file extension
			filename += extension;

			//put the file in the Pictures folder. This assumes you have all the correct external storage permissions!!!
#if ANDROID
			var fullFilename = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures).AbsolutePath;
#else
			var fullFilename = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
#endif

			//create that directory if it doesn't exist
			if (!Directory.Exists(fullFilename))
			{
				Directory.CreateDirectory(fullFilename);
			}

			fullFilename = Path.Combine(fullFilename, filename);

			return fullFilename;
		}
	}
}
