namespace BragiRpc;

using MessagePack;
using System.Text.Json.Serialization;

[Union(0, typeof(AddRequest))]
[Union(1, typeof(EchoRequest))]
[JsonDerivedType(typeof(AddRequest), typeDiscriminator: 0)]
[JsonDerivedType(typeof(EchoRequest), typeDiscriminator: 1)]
public abstract class BaseRequest
{
    public virtual string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
    public virtual byte[] SerializeMessagePack() => MessagePackSerializer.Serialize(this);
}
