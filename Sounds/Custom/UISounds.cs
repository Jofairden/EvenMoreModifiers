using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace Loot.Sounds.Custom
{
	static class SoundMaker
	{
		public static SoundEffectInstance MakeSoundInstance(ref SoundEffectInstance instance, float volume, float level)
		{
			instance.Volume = volume * level;
			return instance;
		}
	}

	internal class CloseUI : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			return SoundMaker.MakeSoundInstance(ref soundInstance, volume, 0.5f);
		}
	}

	internal class Decline : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			return SoundMaker.MakeSoundInstance(ref soundInstance, volume, 0.5f);
		}
	}

	internal class Notif : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			return SoundMaker.MakeSoundInstance(ref soundInstance, volume, 0.5f);
		}
	}

	internal class OpenUI : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			return SoundMaker.MakeSoundInstance(ref soundInstance, volume, 0.5f);
		}
	}

	internal class Receive : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			return SoundMaker.MakeSoundInstance(ref soundInstance, volume, 0.5f);
		}
	}

	internal class Redeem : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			return SoundMaker.MakeSoundInstance(ref soundInstance, volume, 0.5f);
		}
	}
}
