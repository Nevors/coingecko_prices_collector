using MessageBroker.Contracts.Abstractions;

namespace MessageBroker.Contracts;

public record EventMessage<T>(T Value) : IMessage<T>;

public record EventConsumeMessage<T>(T Value, DateTime Timestamp)
    : EventMessage<T>(Value), IConsumeMessage<T>;
