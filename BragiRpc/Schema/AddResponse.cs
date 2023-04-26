namespace BragiRpc;

using MessagePack;

[MessagePackObject]
public class AddResponse : BaseResponse
{
    [Key(0)]
    public int Sum { get; set; }
}