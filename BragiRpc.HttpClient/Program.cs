namespace BragiRpc;


internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Client ready, press any key to send requests...");
        Console.ReadKey();
        var httpClient = new HttpClient();

        var client = new BragiClient(new SerializeHelper());

        var request = new AddRequest
        {
            A = 7,
            B = 4,
        };

        Console.WriteLine($"Requesting {request.GetType().FullName} with {request.A}, {request.B}");

        using var memoryStream = new MemoryStream();
        var binaryWriter = new BinaryWriter(memoryStream);

        var serializationType = SerializationType.MessagePack;
        var contentType = serializationType switch
        {
            SerializationType.Json => "application/octet-stream",
            SerializationType.MessagePack => "application/messagepack"
        };
        var contentTypeHeader = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(contentType);

        await client.SendRequestAsync(request, binaryWriter, serializationType);

        memoryStream.Seek(0, SeekOrigin.Begin);

        var content = new StreamContent(memoryStream);
        content.Headers.ContentType = contentTypeHeader;

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:5151/rpc");
        requestMessage.Content = content;
        requestMessage.Headers.Accept.Add(contentTypeHeader);

        var httpResponse = await httpClient.PostAsync("http://127.0.0.1:5151/rpc", content);

        var responseStream = await httpResponse.Content.ReadAsStreamAsync();
        responseStream.Seek(0, SeekOrigin.Begin);

        var response = await client.RetrieveResponseAsync<AddResponse>(new BinaryReader(responseStream), serializationType);

        Console.WriteLine(response.Sum);

        await Task.Delay(Timeout.Infinite);
    }
}
