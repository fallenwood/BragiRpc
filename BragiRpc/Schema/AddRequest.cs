namespace BragiRpc;

using MessagePack;

[MessagePackObject]
public class AddRequest : BaseRequest
{
    [Key(0)]
    public int A { get; set; }

    [Key(1)]
    public int B { get; set; }
}