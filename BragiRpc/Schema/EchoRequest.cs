namespace BragiRpc;

using MessagePack;

[MessagePackObject]
public class EchoRequest : BaseRequest
{
    [Key(0)]
    public string Message { get; set; }
}
