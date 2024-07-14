using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.JSON
{
    /// <summary>
    /// <inheritdoc/> 
    /// Converts <see cref="Color"/> or <see cref="Nullable"/>&lt;<see cref="Color"/>&gt;.
    /// </summary>
    public class ColorConverter : JsonConverter
    {
        /// <inheritdoc/>
        /// <remarks>Will return <see langword="true"/>, if the given <paramref name="objectType"/> is either 
        /// <see cref="Color"/> or <see cref="Nullable"/>&lt;<see cref="Color"/>&gt;.</remarks>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color) || objectType == typeof(Color?);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            
            JObject jObject = JObject.Load(reader);

            return jObject.ToObject<Color>();
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (!(value is Color color))
            {
                throw new JsonSerializationException($"Converter cannot write specified value to JSON. {typeof(Color)} is required.");
            }

            JObject jObject = JObject.FromObject(color);
            serializer.Serialize(writer, jObject, color.GetType());
        }
    }
}
