namespace BragiRpc;

using System.Net;
using System.Net.Sockets;

internal class Program
{
    static async Task Main(string[] args)
    {
        var listener = new TcpListener(IPAddress.Loopback, 9876);
        var service = new BragiDispatcher();
        var server = new BragiServer(service, new SerializeHelper());

        listener.Start();

        Console.WriteLine("Server ready...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();

            _ = Task.Run(async () =>
            {
                await HandleAsync(client, server);
            });
        }
    }

    public static async Task HandleAsync(TcpClient tcpClient, BragiServer service)
    {
        var reader = new BinaryReader(tcpClient.GetStream());
        var writer = new BinaryWriter(tcpClient.GetStream());

        try
        {
            while (true)
            {
                await service.HandleAsync(reader, writer, SerializationType.MessagePack);
            }
        } catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
        }
    }
}