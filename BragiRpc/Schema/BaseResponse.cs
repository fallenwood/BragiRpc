namespace BragiRpc;

using MessagePack;
using System.Text.Json.Serialization;

[Union(0, typeof(AddResponse))]
[Union(1, typeof(EchoResponse))]
[Union(2, typeof(FibResponse))]
[JsonDerivedType(typeof(AddResponse), typeDiscriminator: 0)]
[JsonDerivedType(typeof(EchoResponse), typeDiscriminator: 1)]
[JsonDerivedType(typeof(FibResponse), typeDiscriminator: 2)]
public abstract class BaseResponse
{
    public virtual string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
    public virtual byte[] SerializeMessagePack() => MessagePackSerializer.Serialize(this);
}
