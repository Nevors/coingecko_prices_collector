using Confluent.Kafka;
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
        using var ms = new MemoryStream();

        var jsonString = JsonSerializer.Serialize(data, serializationSettings);
        var writer = new StreamWriter(ms);

        writer.Write(jsonString);
        writer.Flush();
        ms.Position = 0;

        return ms.ToArray();
    }
}
