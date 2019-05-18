namespace Loot.Api.Builder
{
	public abstract class PropertyBuilder<T> : IPropertyBuilder<T> where T : class, new()
	{
		protected T Property = new T();
		protected abstract T DefaultProperty { set; }

		protected PropertyBuilder()
		{
		}

		protected PropertyBuilder(IPropertyBuilder<T> defaultPropertyBuilder)
		{
			WithDefault(defaultPropertyBuilder);
		}

		protected PropertyBuilder(T defaultValue)
		{
			WithDefault(defaultValue);
		}

		public void WithDefault(IPropertyBuilder<T> defaultPropertyBuilder)
		{
			DefaultProperty = defaultPropertyBuilder.Build();
		}

		public void WithDefault(T defaultValue)
		{
			DefaultProperty = defaultValue;
		}

		public virtual T Build()
		{
			return Property;
		}
	}
}
