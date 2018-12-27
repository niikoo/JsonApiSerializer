using System.Globalization;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json;
using JsonApiSerializer.ContractResolvers;

namespace JsonApiSerializer
{
    /// <summary>
    /// Provides a set of default settings to allow the deserialization of JsonApi formatted json.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonSerializerSettings" />
    public class JsonApiSerializerSettings : JsonSerializerSettings
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:JsonApiSerializer.JsonApiSerializerSettings" /> class.
        /// </summary>
        /// <param name="resourceObjectConverter">The converter to use when serializing/deserializing a JsonApi resource object</param>
        public JsonApiSerializerSettings(JsonConverter resourceObjectConverter)
        {
            NullValueHandling = NullValueHandling.Ignore;
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            FloatParseHandling = FloatParseHandling.Decimal;
            ContractResolver = new JsonApiContractResolver(resourceObjectConverter);
            DateParseHandling = DateParseHandling.DateTime;
            MissingMemberHandling = MissingMemberHandling.Ignore;
            DateTimeZoneHandling = DateTimeZoneHandling.Local;
            Culture = CultureInfo.InvariantCulture;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:JsonApiSerializer.JsonApiSerializerSettings" /> class.
        /// </summary>
        public JsonApiSerializerSettings() : this(new ResourceObjectConverter())
        {
            
        }


    }
}
