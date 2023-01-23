using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace Kafka;

internal class JsonEventSerializer<T> : ISerializer<T>
{
    private readonly JsonSerializerOptions? serializationSettings;

    public JsonEventSerializer(JsonSerializerOptions? serializationSettings = null)
    {
        this.serializationSettings = serializationSettings;
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        var jsonString = JsonSerializer.Serialize(data, serializationSettings);

        return Encoding.UTF8.GetBytes(jsonString);
    }
}
