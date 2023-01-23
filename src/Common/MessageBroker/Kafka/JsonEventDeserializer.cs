using Confluent.Kafka;
using MessageBroker.Contracts.Abstractions;
using System.Text;
using System.Text.Json;

namespace Kafka;

internal class JsonEventDeserializer<T> : IDeserializer<T>
{
    private readonly JsonSerializerOptions? serializationSettings;

    public JsonEventDeserializer(JsonSerializerOptions? serializationSettings = null)
    {
        this.serializationSettings = serializationSettings;
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var rawData = Encoding.UTF8.GetString(data);

        return GetMessage(rawData) ?? throw new EventDeserializationException(rawData);

        T? GetMessage(string rawData)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(rawData, serializationSettings);
            }
            catch (Exception ex)
            {
                throw new EventDeserializationException(rawData, ex);
            }
        }
    }
}
