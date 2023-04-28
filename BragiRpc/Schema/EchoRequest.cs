namespace BragiRpc;

using MessagePack;

[MessagePackObject]
public class EchoRequest : BaseRequest
{
    [Key(0)]
    public override RequestType RequestType { get; set; } = RequestType.Unary;

    [Key(1)]
    public string Message { get; set; }
}
