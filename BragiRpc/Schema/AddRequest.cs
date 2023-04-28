namespace BragiRpc;

using MessagePack;

[MessagePackObject]
public class AddRequest : BaseRequest
{
    [Key(0)]
    public override RequestType RequestType { get; set; } = RequestType.Unary;

    [Key(1)]
    public int A { get; set; }

    [Key(2)]
    public int B { get; set; }

}