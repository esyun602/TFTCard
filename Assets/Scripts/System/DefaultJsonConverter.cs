using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class DefaultJsonConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(object);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		switch (reader.TokenType)
		{
			case JsonToken.Integer:
				long val = Convert.ToInt64(reader.Value);
				if (val <= int.MaxValue && val >= int.MinValue)
					return (int)val;
				return val;
			case JsonToken.Float:
				return Convert.ToSingle(reader.Value);

			case JsonToken.String:
				return reader.Value?.ToString();

			case JsonToken.Boolean:
				return (bool)reader.Value;

			case JsonToken.StartArray:
				var list = new List<object>();
				reader.Read();
				while (reader.TokenType != JsonToken.EndArray)
				{
					list.Add(ReadJson(reader, typeof(object), null, serializer));
					reader.Read();
				}

				return list;

			case JsonToken.StartObject:
				var dict = new Dictionary<string, object>();
				reader.Read();
				while (reader.TokenType != JsonToken.EndObject)
				{
					string propName = reader.Value.ToString();
					reader.Read();
					dict[propName] = ReadJson(reader, typeof(object), null, serializer);
					reader.Read();
				}

				return dict;

			case JsonToken.Null:
				return null;
		}

		return null;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		serializer.Serialize(writer, value);
	}
}