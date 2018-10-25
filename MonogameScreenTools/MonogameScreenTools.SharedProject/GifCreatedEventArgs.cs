using System;

namespace MonogameScreenTools
{
	public class GifCreatedEventArgs : EventArgs
	{
		public string Filename { get; set; }
		public TimeSpan ElpasedTime  { get; set; }

		public GifCreatedEventArgs()
		{
		}

		public GifCreatedEventArgs(string filename, TimeSpan time)
		{
			Filename = filename;
			ElpasedTime = time;
		}
	}
}
