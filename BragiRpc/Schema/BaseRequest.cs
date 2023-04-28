namespace BragiRpc;

using MessagePack;
using System.Text.Json.Serialization;

[Union(0, typeof(AddRequest))]
[Union(1, typeof(EchoRequest))]
[Union(2, typeof(FibRequest))]
[JsonDerivedType(typeof(AddRequest), typeDiscriminator: 0)]
[JsonDerivedType(typeof(EchoRequest), typeDiscriminator: 1)]
[JsonDerivedType(typeof(FibRequest), typeDiscriminator: 2)]
public abstract class BaseRequest
{
    [Key(0)]
    public abstract RequestType RequestType { get; set; }
    public virtual string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
    public virtual byte[] SerializeMessagePack() => MessagePackSerializer.Serialize(this);
}
