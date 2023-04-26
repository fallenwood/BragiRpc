namespace BragiRpc;

using System.Text.Json.Serialization;

[JsonDerivedType(typeof(AddResponse), typeDiscriminator: 0)]
[JsonDerivedType(typeof(EchoResponse), typeDiscriminator: 1)]
public abstract class BaseResponse
{
    public virtual string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
}
