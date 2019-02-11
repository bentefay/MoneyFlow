using System;

namespace Web.Utils.Serialization.Converters.SumTypes
{
    /// <summary>
    /// This attribute has no purpose other than for enforcing conventions for our sum types that are not attributed with a JsonConverter attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SumTypeAttribute : Attribute
    {
        public string DiscriminatorPropertyName { get; }

        public SumTypeAttribute(string discriminatorPropertyName)
        {
            DiscriminatorPropertyName = discriminatorPropertyName;
        }
    }
}
