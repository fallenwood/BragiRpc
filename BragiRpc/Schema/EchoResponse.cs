﻿namespace BragiRpc;

using MessagePack;

[MessagePackObject]
public class EchoResponse : BaseResponse
{
    [Key(0)]
    public string Message { get; set; }
}