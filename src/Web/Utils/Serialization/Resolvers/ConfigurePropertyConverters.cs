using System.Collections.Generic;

namespace Web.Utils.Serialization.Resolvers
{
    public class ConfigurePropertyConverters
    {
        public static readonly IReadOnlyList<IConfigureProperty> All = new IConfigureProperty[]
        {
            new ConfigureOptionProperty(),
            new ConfigureTinyTypeProperty()
        };
    }
}