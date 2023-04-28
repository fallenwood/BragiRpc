namespace BragiRpc;

using System.Text;

public class BragiClient
{
    private readonly SerializeHelper helper;

    public BragiClient(SerializeHelper helper)
    {
        this.helper = helper;
    }

    public Task SendRequestAsync<TRequest>(TRequest request, BinaryWriter writer, SerializationType serializationType)
        where TRequest : BaseRequest
    {
        var requestData = serializationType switch
        {
            SerializationType.Json => Encoding.UTF8.GetBytes(request.Serialize()),
            SerializationType.MessagePack => request.SerializeMessagePack(),
        };

        writer.Write(requestData.Length);
        writer.Write(requestData);

        return Task.CompletedTask;
    }

    public Task<TResponse> RetrieveResponseAsync<TResponse>(BinaryReader reader, SerializationType serializationType)
        where TResponse : BaseResponse
    {
        var responseDataLength = reader.ReadInt32();
        var responseData = reader.ReadBytes(responseDataLength);

        var response = serializationType switch
        {
            SerializationType.Json => helper.DeserializeJson<BaseResponse>(responseData),
            SerializationType.MessagePack => helper.DeserializeMessagePack<BaseResponse>(responseData),
        };

        return Task.FromResult<TResponse>(response as TResponse);
    }

    public async Task<TResponse> InvokeUnaryAsync<TRequest, TResponse>(TRequest request, BinaryReader reader, BinaryWriter writer, SerializationType serializationType = SerializationType.Json)
        where TRequest: BaseRequest
        where TResponse : BaseResponse
    {
        await this.SendRequestAsync(request, writer, serializationType);
        return await this.RetrieveResponseAsync<TResponse>(reader, serializationType);
    }

    public async IAsyncEnumerable<TResponse> InvokeServerStreamingAsync<TRequest, TResponse>(TRequest request, BinaryReader reader, BinaryWriter writer, SerializationType serializationType = SerializationType.Json)
        where TRequest : BaseRequest
        where TResponse : BaseResponse
    {
        await this.SendRequestAsync(request, writer, serializationType);

        while (true)
        {
            TResponse response;
            try
            {
                response = await this.RetrieveResponseAsync<TResponse>(reader, serializationType);
            }
            catch (Exception ex)
            {
                break;
            }

            yield return response;
        }
    }
}
