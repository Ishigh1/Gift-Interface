using System.Reflection;
using Archipelago.Gifting.Net.Gifts;
using Newtonsoft.Json;
	
namespace Gift_Interface;

public class JsonGiftConverter : JsonConverter<GiftItem>
{
		public override GiftItem ReadJson(JsonReader reader, Type typeToConvertobjectType, GiftItem? existingValue, bool hasExistingValue,
			JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, GiftItem? value, JsonSerializer serializer)
		{
			writer.WriteStartObject();

			// Serialize all public properties except for "Amount"
			PropertyInfo[] properties = typeof(GiftItem).GetProperties();
			foreach (PropertyInfo property in properties)
			{
				if (property.Name != "Amount")
				{
					writer.WritePropertyName(property.Name);
					serializer.Serialize(writer, property.GetValue(value));
				}
			}

			writer.WriteEndObject();
		}
}