namespace BragiRpc.Http;

using System.Text.Json;
using BragiRpc;
using Microsoft.AspNetCore.Server.Kestrel.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<BragiServer>();
        builder.Services.AddSingleton<BragiDispatcher>();
        builder.Services.AddSingleton<SerializeHelper>();

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapPost("/rpc", async (HttpContext context) =>
        {
            if (context.Request.ContentType == "application/json")
            {
                await HandleJsonContentAsync(context);
            }
            else if (context.Request.ContentType == "application/octet-stream")
            {
                await HandleBinaryContentAsync(context, SerializationType.Json);
            }
            else if (context.Request.ContentType == "application/messagepack")
            {
                await HandleBinaryContentAsync(context, SerializationType.MessagePack);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        })
            .WithName("rpc")
            .WithOpenApi();

        app.Run();
    }

    public static async Task HandleJsonContentAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        var service = context.RequestServices.GetService<BragiServer>();

        var request = await JsonSerializer.DeserializeAsync<BaseRequest>(context.Request.Body);
        var response = await service.HandleUnaryAsync<BaseRequest, BaseResponse>(request);

        await JsonSerializer.SerializeAsync(context.Response.Body, response, typeof(BaseResponse));
    }

    public static async Task HandleBinaryContentAsync(HttpContext context, SerializationType serializationType)
    {
        context.Response.ContentType = serializationType switch
        {
            SerializationType.Json => "application/octet-stream",
            SerializationType.MessagePack => "application/messagepack",
        };

        var service = context.RequestServices.GetService<BragiServer>();
        var reader = new BinaryReader(context.Request.Body);
        var writer = new BinaryWriter(context.Response.Body);

        await service.HandleAsync(reader, writer, serializationType);
    }
}