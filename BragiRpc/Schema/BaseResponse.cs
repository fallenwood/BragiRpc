namespace BragiRpc;

using MessagePack;
using System.Text.Json.Serialization;

[Union(0, typeof(AddResponse))]
[Union(1, typeof(EchoResponse))]
[JsonDerivedType(typeof(AddResponse), typeDiscriminator: 0)]
[JsonDerivedType(typeof(EchoResponse), typeDiscriminator: 1)]
public abstract class BaseResponse
{
    public virtual string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
    public virtual byte[] SerializeMessagePack() => MessagePackSerializer.Serialize(this);
}
