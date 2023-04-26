namespace BragiRpc;

using System.Text;
using System.Text.Json;

public class BragiClient
{
    public async Task<TResponse> InvokeAsync<TRequest, TResponse>(TRequest request, BinaryReader reader, BinaryWriter writer)
        where TRequest: BaseRequest
        where TResponse : BaseResponse
    {
        var requestMessage = request.Serialize();
        var requestData = Encoding.UTF8.GetBytes(requestMessage);

        Console.WriteLine(requestMessage);

        writer.Write(requestData.Length);
        writer.Write(requestData);

        var responseDataLength = reader.ReadInt32();
        var responseData = reader.ReadBytes(responseDataLength);
        var responseMessage = Encoding.UTF8.GetString(responseData);

        var response = JsonSerializer.Deserialize<TResponse>(responseMessage)!;

        return response;
    }
}
