using Microsoft.Xna.Framework;

namespace MonogameScreenTools
{
	public interface IScreenGrabber
	{
		bool Enabled { get; set; }
		ImageList CurrentImageList { get; set; }

		void BeginDraw();
		void Draw(GameTime gameTime);
	}
}