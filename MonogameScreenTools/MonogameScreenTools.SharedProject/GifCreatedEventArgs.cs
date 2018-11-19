using System;

namespace MonogameScreenTools
{
	public class GifCreatedEventArgs : EventArgs
	{
		/// <summary>
		/// Whether or not the gif was created successfully
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// The error message that was generated if/when the gif creation failed
		/// </summary>
		public string ErrorMessage { get; set; }

		public string Filename { get; set; }
		public TimeSpan TotalTime  { get; set; }

		public GifCreatedEventArgs()
		{
			Success = true;
		}

		public GifCreatedEventArgs(string filename, TimeSpan totalTime) : this()
		{
			Filename = filename;
			TotalTime = totalTime;
		}
	}
}
