using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nezam.Refahi.Shared.Infrastructure.Services;

/// <summary>
/// Centralized JSON serialization service with optimized settings for dynamic objects and dictionaries
/// </summary>
public class JsonSerializationService
{
    private static readonly JsonSerializerOptions _defaultOptions = CreateDefaultOptions();
    private static readonly JsonSerializerOptions _camelCaseOptions = CreateCamelCaseOptions();

    /// <summary>
    /// Default JSON serializer options with PascalCase naming
    /// </summary>
    public static JsonSerializerOptions DefaultOptions => _defaultOptions;

    /// <summary>
    /// JSON serializer options with camelCase naming for API responses
    /// </summary>
    public static JsonSerializerOptions CamelCaseOptions => _camelCaseOptions;

    /// <summary>
    /// Serializes an object to JSON string using default options
    /// </summary>
    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, DefaultOptions);
    }

    /// <summary>
    /// Serializes an object to JSON string using camelCase options
    /// </summary>
    public static string SerializeCamelCase<T>(T value)
    {
        return JsonSerializer.Serialize(value, CamelCaseOptions);
    }

    /// <summary>
    /// Deserializes JSON string to object using default options
    /// </summary>
    public static T? Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }

    /// <summary>
    /// Deserializes JSON string to object using camelCase options
    /// </summary>
    public static T? DeserializeCamelCase<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, CamelCaseOptions);
    }

    /// <summary>
    /// Deserializes JSON string to object using type information
    /// </summary>
    public static object? Deserialize(string json, Type returnType)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize(json, returnType, DefaultOptions);
    }

    /// <summary>
    /// Creates default JSON serializer options with PascalCase naming
    /// </summary>
    private static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            // Property naming policy
            PropertyNamingPolicy = null, // PascalCase (default)
            
            // Dictionary handling
            DictionaryKeyPolicy = null, // PascalCase for dictionary keys
            
            // Write options
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            
            // Read options
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            
            // Number handling
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            
            // Date handling
            DefaultBufferSize = 16 * 1024, // 16KB buffer
            
            // Converters for complex types
            Converters =
            {
                new JsonStringEnumConverter(),
                new DateTimeConverter(),
                new GuidConverter(),
                new DictionaryStringStringConverter(),
                new DynamicObjectConverter()
            }
        };

        return options;
    }

    /// <summary>
    /// Creates JSON serializer options with camelCase naming
    /// </summary>
    private static JsonSerializerOptions CreateCamelCaseOptions()
    {
        var options = new JsonSerializerOptions
        {
            // Property naming policy
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            
            // Dictionary handling
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            
            // Write options
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            
            // Read options
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            
            // Number handling
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            
            // Date handling
            DefaultBufferSize = 16 * 1024, // 16KB buffer
            
            // Converters for complex types
            Converters =
            {
                new JsonStringEnumConverter(),
                new DateTimeConverter(),
                new GuidConverter(),
                new DictionaryStringStringConverter(),
                new DynamicObjectConverter()
            }
        };

        return options;
    }
}

/// <summary>
/// Custom converter for DateTime to handle UTC properly
/// </summary>
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (DateTime.TryParse(stringValue, out var dateTime))
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTime.FromOADate(reader.GetDouble());
        }

        throw new JsonException($"Unable to convert {reader.TokenType} to DateTime");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
    }
}

/// <summary>
/// Custom converter for Guid
/// </summary>
public class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (Guid.TryParse(stringValue, out var guid))
            {
                return guid;
            }
        }

        throw new JsonException($"Unable to convert {reader.TokenType} to Guid");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// Custom converter for Dictionary<string, string> to handle edge cases
/// </summary>
public class DictionaryStringStringConverter : JsonConverter<Dictionary<string, string>>
{
    public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return new Dictionary<string, string>();
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected StartObject, got {reader.TokenType}");
        }

        var dictionary = new Dictionary<string, string>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected PropertyName, got {reader.TokenType}");
            }

            var key = reader.GetString() ?? string.Empty;
            
            reader.Read();
            var value = reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString() ?? string.Empty,
                JsonTokenType.Number => reader.GetInt32().ToString(),
                JsonTokenType.True => "true",
                JsonTokenType.False => "false",
                JsonTokenType.Null => string.Empty,
                _ => reader.GetString() ?? string.Empty
            };

            dictionary[key] = value;
        }

        throw new JsonException("Unexpected end of JSON");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        foreach (var kvp in value)
        {
            writer.WriteString(kvp.Key, kvp.Value);
        }
        
        writer.WriteEndObject();
    }
}

/// <summary>
/// Custom converter for dynamic objects
/// </summary>
public class DynamicObjectConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null!,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.TryGetInt32(out var intValue) ? intValue : reader.GetDouble(),
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.StartObject => ReadObject(ref reader),
            JsonTokenType.StartArray => ReadArray(ref reader),
            _ => throw new JsonException($"Unexpected token type: {reader.TokenType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }

    private static Dictionary<string, object> ReadObject(ref Utf8JsonReader reader)
    {
        var obj = new Dictionary<string, object>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return obj;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected PropertyName, got {reader.TokenType}");
            }

            var key = reader.GetString() ?? string.Empty;
            reader.Read();
            
            obj[key] = ReadValue(ref reader);
        }

        throw new JsonException("Unexpected end of JSON");
    }

    private static List<object> ReadArray(ref Utf8JsonReader reader)
    {
        var list = new List<object>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return list;
            }

            list.Add(ReadValue(ref reader));
        }

        throw new JsonException("Unexpected end of JSON");
    }

    private static object ReadValue(ref Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null!,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.TryGetInt32(out var intValue) ? intValue : reader.GetDouble(),
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.StartObject => ReadObject(ref reader),
            JsonTokenType.StartArray => ReadArray(ref reader),
            _ => throw new JsonException($"Unexpected token type: {reader.TokenType}")
        };
    }
}
