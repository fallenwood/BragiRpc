namespace BragiRpc;

using System.Text;

public class BragiClient
{
    private readonly SerializeHelper helper;

    public BragiClient(SerializeHelper helper)
    {
        this.helper = helper;
    }

    public async Task<TResponse> InvokeAsync<TRequest, TResponse>(TRequest request, BinaryReader reader, BinaryWriter writer, SerializationType serializationType = SerializationType.Json)
        where TRequest: BaseRequest
        where TResponse : BaseResponse
    {
        var requestData = serializationType switch
        {
            SerializationType.Json => Encoding.UTF8.GetBytes(request.Serialize()),
            SerializationType.MessagePack => request.SerializeMessagePack(),
        };

        // Console.WriteLine(requestMessage);

        writer.Write(requestData.Length);
        writer.Write(requestData);

        var responseDataLength = reader.ReadInt32();
        var responseData = reader.ReadBytes(responseDataLength);

        var response = serializationType switch
        {
            SerializationType.Json => helper.DeserializeJson<BaseResponse>(responseData),
            SerializationType.MessagePack => helper.DeserializeMessagePack<BaseResponse>(responseData),
        };

        return response as TResponse;
    }
}
