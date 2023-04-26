namespace BragiRpc.Tcp;

using System.Net;
using System.Net.Sockets;
using BragiRpc;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Client ready, press any key to send requests...");
        Console.ReadKey();
        var socket = new TcpClient();
        await socket.ConnectAsync(IPAddress.Parse("127.0.0.1"), 9876);

        var client = new BragiClient(new SerializeHelper());

        var request = new AddRequest
        {
            A = 7,
            B = 4,
        };

        Console.WriteLine($"Requesting {request.GetType().FullName} with {request.A}, {request.B}");

        var reader = new BinaryReader(socket.GetStream());
        var writer = new BinaryWriter(socket.GetStream());

        var response = await client.InvokeAsync<AddRequest, AddResponse>(request, reader, writer, SerializationType.MessagePack);

        Console.WriteLine(response.Sum);

        await Task.Delay(Timeout.Infinite);
    }
}
