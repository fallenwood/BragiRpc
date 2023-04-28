namespace BragiRpc;

using MessagePack;

[MessagePackObject]
public class FibResponse : BaseResponse
{
    [Key(0)]
    public int Id { get; set; }

    [Key(1)]
    public long Value { get; set; }
}