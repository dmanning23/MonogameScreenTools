using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MonogameScreenTools
{
	public static class FileSystemHelper
	{
		public static async Task<bool> AskForExternalFilesystemPermission()
		{
#if __IOS__ || ANDROID
			var available = false;
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
				if (status != PermissionStatus.Granted)
				{
					var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
					status = results[Permission.Storage];
				}

				if (status == PermissionStatus.Granted)
				{
					//TODO: check if media is mounted
					available = true;
				}
			}
			catch (Exception)
			{
			}
			return available;
#else
			return true;
#endif
		}

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
			var fullFilename = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			fullFilename = Path.Combine(fullFilename, filename);

			return fullFilename;
		}
	}
}
