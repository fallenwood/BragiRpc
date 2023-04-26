namespace BragiRpc;

using MessagePack;
using System.Text;
using System.Text.Json;

public class SerializeHelper
{
    public T DeserializeJson<T>(byte[] data)
    {
        var message = Encoding.UTF8.GetString(data);
        var payload = JsonSerializer.Deserialize<T>(message);
        return payload;
    }

    public T DeserializeMessagePack<T>(byte[] data)
    {
        var payload = MessagePackSerializer.Deserialize<T>(data);
        return payload;
    }
}
