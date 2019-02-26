using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace MediaRadar.API.SDK.Serialization
{
    public class MediaRadarContractResolver : DefaultContractResolver
    {
        public static readonly MediaRadarContractResolver Instance = new MediaRadarContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member,
            MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {
                property.DefaultValueHandling = DefaultValueHandling.Include;
            }

            return property;
        }
    }
}
