using log4net;

namespace Loot
{
	internal class Log4c
	{
		public static ILog Logger => Loot.Instance.Logger;
	}
}
