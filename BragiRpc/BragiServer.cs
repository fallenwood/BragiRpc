namespace BragiRpc;

using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class BragiServer
{
    private readonly BragiDispatcher service;
    public BragiServer(BragiDispatcher service)
    {
        this.service = service;
    }

    public async Task<TResponse> HandleAsync<TRequest, TResponse>(TRequest request)
        where TRequest: BaseRequest
        where TResponse: BaseResponse
    {
        var method = service.GetType().GetMethods().FirstOrDefault(e => string.Equals(e.Name[..^5], request.GetType().Name[..^7], StringComparison.OrdinalIgnoreCase));

        var responseTask = method.Invoke(service, new object[] { request }) as Task<BaseResponse>;
        var response = await responseTask;
        return response as TResponse;
    }

    public async Task HandleAsync(BinaryReader reader, BinaryWriter writer)
    {
        var dataLength = reader.ReadInt32();
        var data = reader.ReadBytes(dataLength);
        var requestMessage = Encoding.UTF8.GetString(data);

        var request = JsonSerializer.Deserialize<BaseRequest>(requestMessage);

        var response = await this.HandleAsync<BaseRequest, BaseResponse>(request);

        var responseMsesage = response.Serialize();
        var bytes = Encoding.UTF8.GetBytes(responseMsesage);

        writer.Write(bytes.Length);
        writer.Write(bytes);
    }
}