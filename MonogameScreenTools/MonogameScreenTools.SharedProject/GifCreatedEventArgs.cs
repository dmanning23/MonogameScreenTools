using System;

namespace MonogameScreenTools
{
	public class GifCreatedEventArgs : EventArgs
	{
		public string Filename { get; set; }
		public TimeSpan ResizeTime { get; set; }
		public TimeSpan TotalTime  { get; set; }

		public GifCreatedEventArgs()
		{
		}

		public GifCreatedEventArgs(string filename, TimeSpan resizeTime, TimeSpan totalTime)
		{
			Filename = filename;
			ResizeTime = resizeTime;
			TotalTime = totalTime;
		}
	}
}
