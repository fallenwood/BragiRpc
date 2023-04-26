namespace BragiRpc;

using System.Text.Json.Serialization;

[JsonDerivedType(typeof(AddRequest), typeDiscriminator: 0)]
[JsonDerivedType(typeof(EchoRequest), typeDiscriminator: 1)]
public abstract class BaseRequest
{
    public virtual string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
}
