using System;

namespace MonogameScreenTools
{
	public class GifCreatedEventArgs : EventArgs
	{
		public string Filename { get; set; }
		public TimeSpan TotalTime  { get; set; }

		public GifCreatedEventArgs()
		{
		}

		public GifCreatedEventArgs(string filename, TimeSpan totalTime)
		{
			Filename = filename;
			TotalTime = totalTime;
		}
	}
}
