using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Utils.Serialization.Resolvers
{
    public class ContractResolver : DefaultContractResolver
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IEnumerable<IConfigureProperty> _propertyConfigurers;

        public ContractResolver(JsonSerializerSettings serializerSettings, IEnumerable<IConfigureProperty> propertyConfigurers)
        {
            _serializerSettings = serializerSettings;
            _propertyConfigurers = propertyConfigurers;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (member is PropertyInfo propertyInfo)
            {
                _propertyConfigurers.Iter(propertyConfigurer => propertyConfigurer.ConfigureProperty(property, propertyInfo, _serializerSettings));
            }

            return property;
        }
    }
}
