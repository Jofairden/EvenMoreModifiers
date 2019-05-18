using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Api.Builder
{
	public interface IPropertyBuilder<T> where T : class
	{
		void WithDefault(IPropertyBuilder<T> defaultBuilder);
		void WithDefault(T defaultValue);
		T Build();
	}
}
