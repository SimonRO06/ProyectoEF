using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Api.Converts;
public class FlexibleEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (Enum.TryParse<T>(stringValue, true, out var resultFromString))
                        return resultFromString;
                    throw new JsonException($"Value '{stringValue}' is not valid for enum {typeof(T).Name}");

                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var intValue))
                    {
                        if (Enum.IsDefined(typeof(T), intValue))
                            return (T)Enum.ToObject(typeof(T), intValue);
                        throw new JsonException($"Value {intValue} is not valid for enum {typeof(T).Name}");
                    }
                    throw new JsonException("Invalid number format for enum");

                default:
                    throw new JsonException($"Unable to convert {reader.TokenType} to enum {typeof(T).Name}");
            }
        }
        catch (Exception ex)
        {
            throw new JsonException($"Error converting to enum {typeof(T).Name}. Valid values: {string.Join(", ", Enum.GetNames(typeof(T)))}", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}