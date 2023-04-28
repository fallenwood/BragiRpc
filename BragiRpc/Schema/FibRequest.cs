namespace BragiRpc;

using MessagePack;


[MessagePackObject]
public class FibRequest : BaseRequest
{
    [Key(0)]
    public override RequestType RequestType { get; set; } = RequestType.ServerStreaming;
}
