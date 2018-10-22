
namespace MonogameScreenTools
{
	public interface IScreenShotHelper
	{
		string SaveScreenshot(string filename = "", bool appendTimeStamp = true);
	}
}