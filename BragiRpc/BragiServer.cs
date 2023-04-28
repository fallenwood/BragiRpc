namespace BragiRpc;

using System;
using System.Text;
using System.Threading.Tasks;

public class BragiServer
{
    private readonly SerializeHelper helper;
    private readonly BragiDispatcher service;

    public BragiServer(BragiDispatcher service, SerializeHelper helper)
    {
        this.service = service;
        this.helper = helper;
    }

    public async Task<TResponse> HandleUnaryAsync<TRequest, TResponse>(TRequest request)
        where TRequest: BaseRequest
        where TResponse: BaseResponse
    {
        var method = service.GetType().GetMethods().FirstOrDefault(e => string.Equals(e.Name[..^5], request.GetType().Name[..^7], StringComparison.OrdinalIgnoreCase));

        var responseTask = method.Invoke(service, new object[] { request }) as Task<BaseResponse>;
        var response = await responseTask;
        return response as TResponse;
    }

    public async IAsyncEnumerable<TResponse> HandleServerStreamingAsync<TRequest, TResponse>(TRequest request)
        where TRequest : BaseRequest
        where TResponse : BaseResponse
    {
        var method = service.GetType().GetMethods().FirstOrDefault(e => string.Equals(e.Name[..^5], request.GetType().Name[..^7], StringComparison.OrdinalIgnoreCase));

        var responses = method.Invoke(service, new object[] { request }) as IAsyncEnumerable<BaseResponse>;

        await foreach (var response in responses)
        {
            yield return response as TResponse;
        }
    }

    public async Task HandleAsync(BinaryReader reader, BinaryWriter writer, SerializationType serializationType = SerializationType.Json)
    {
        var dataLength = reader.ReadInt32();
        var data = reader.ReadBytes(dataLength);

        var request = serializationType switch
        {
            SerializationType.Json => helper.DeserializeJson<BaseRequest>(data),
            SerializationType.MessagePack => helper.DeserializeMessagePack<BaseRequest>(data),
        };

        if (request.RequestType == RequestType.Unary)
        {
            var response = await this.HandleUnaryAsync<BaseRequest, BaseResponse>(request);

            var bytes = serializationType switch
            {
                SerializationType.Json => Encoding.UTF8.GetBytes(response.Serialize()),
                SerializationType.MessagePack => response.SerializeMessagePack(),
            };

            writer.Write(bytes.Length);
            writer.Write(bytes);
        }
        else if (request.RequestType == RequestType.ServerStreaming)
        {
            var responses = this.HandleServerStreamingAsync<BaseRequest, BaseResponse>(request);

            await foreach (var response in responses)
            {
                var bytes = serializationType switch
                {
                    SerializationType.Json => Encoding.UTF8.GetBytes(response.Serialize()),
                    SerializationType.MessagePack => response.SerializeMessagePack(),
                };

                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }
    }
}
