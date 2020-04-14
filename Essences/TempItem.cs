using Terraria.ModLoader;

namespace Loot.Essences
{
	abstract class TempItem : ModItem
	{
		public override string Texture => "Loot/Placeholder";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(GetType().Namespace + "." + Name);
		}
	}
}
