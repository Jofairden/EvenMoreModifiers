namespace Loot.Api.Builder
{
	/// <summary>
	/// Defines the interface for a PropertyBuilder of generic <see cref="T"/>
	/// </summary>
	/// <typeparam name="T">A class type this builder can build</typeparam>
	public interface IPropertyBuilder<T> where T : class
	{
		void WithDefault(IPropertyBuilder<T> defaultBuilder);
		void WithDefault(T defaultValue);
		T Build();
	}
}
