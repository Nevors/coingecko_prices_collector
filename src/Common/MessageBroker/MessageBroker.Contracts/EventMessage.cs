using MessageBroker.Contracts.Abstractions;

namespace MessageBroker.Contracts;

public record EventMessage<T>(T Value) : IMessage<T>;
