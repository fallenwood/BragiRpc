namespace BragiRpc.Tcp;

using System.Net;
using System.Net.Sockets;
using BragiRpc;

internal class Program
{
    public static async Task HandleAddRquestAsync(TcpClient socket)
    {
        var client = new BragiClient(new SerializeHelper());

        var request = new AddRequest
        {
            A = 7,
            B = 4,
        };

        Console.WriteLine($"Requesting {request.GetType().FullName} with {request.A}, {request.B}");

        var reader = new BinaryReader(socket.GetStream());
        var writer = new BinaryWriter(socket.GetStream());

        var response = await client.InvokeUnaryAsync<AddRequest, AddResponse>(request, reader, writer, SerializationType.MessagePack);

        Console.WriteLine(response.Sum);

    }

    public static async Task HandleFibRquestAsync(TcpClient socket)
    {
        var client = new BragiClient(new SerializeHelper());

        var request = new FibRequest
        {
        };

        Console.WriteLine($"Requesting {request.GetType().FullName}");

        var reader = new BinaryReader(socket.GetStream());
        var writer = new BinaryWriter(socket.GetStream());

        var responses = client.InvokeServerStreamingAsync<FibRequest, FibResponse>(request, reader, writer, SerializationType.MessagePack);

        await foreach (var response in responses)
        {
            Console.WriteLine($"Fib {response.Id}: {response.Value}");
        }
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine("Client ready, press any key to send requests...");
        Console.ReadKey();
        var socket = new TcpClient();
        await socket.ConnectAsync(IPAddress.Parse("127.0.0.1"), 9876);

        await HandleFibRquestAsync(socket);

        await HandleAddRquestAsync(socket);

        await Task.Delay(Timeout.Infinite);
    }
}
