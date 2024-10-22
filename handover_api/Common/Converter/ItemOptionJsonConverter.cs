using handover_api.Service.ValueObject;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace handover_api.Common.Converter
{
    public class ItemOptionJsonConverter : JsonConverter<ItemOption>
    {
        public override ItemOption Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var itemOption = new ItemOption();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                string propertyName = reader.GetString();
                reader.Read(); // 讀取屬性值

                switch (propertyName)
                {
                    case "optionName":
                        itemOption.OptionName = reader.GetString();
                        break;
                    case "type":
                        itemOption.Type = reader.GetString();
                        break;
                    case "comment":
                        itemOption.Comment = reader.GetString();
                        break;
                    case "tableInfo":
                        // 讀取 tableInfo JSON 並轉換為 Dictionary
                        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                        {
                            itemOption.TableInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(doc.RootElement.GetRawText(), options);
                        }
                        break;
                }
            }

            return itemOption;
        }



        public override void Write(Utf8JsonWriter writer, ItemOption value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("optionName", value.OptionName);
            writer.WriteString("type", value.Type);
            writer.WriteString("comment", value.Comment);

            if (value.TableInfo != null)
            {
                writer.WritePropertyName("tableInfo");
                JsonSerializer.Serialize(writer, value.TableInfo, options); // 直接序列化 Dictionary
            }

            writer.WriteEndObject();
        }


    }



}
