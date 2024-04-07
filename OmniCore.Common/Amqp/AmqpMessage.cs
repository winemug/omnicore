using OmniCore.Common.Amqp;
using OmniCore.Shared.Api;
using System.Text;

namespace OmniCore.Services.Interfaces.Amqp;

public record AmqpMessage
{
    public AmqpMessage(AmqpDestination destination, string route)
    {
        Destination = destination;
        Exchange = GetExchange(destination);
        Route = route;
    }

    public AmqpMessage(AmqpDestination destination, string route, string? type, string fromUserId)
    {
        UserId = fromUserId;
        Destination = destination;
        Type = type;
        Exchange = GetExchange(destination);
        Route = route;
    }

    private string GetExchange(AmqpDestination destination)
    {
        switch (destination)
        {
            case AmqpDestination.Response:
                return "e_responses";
            case AmqpDestination.Request:
                return "e_requests";
            case AmqpDestination.Sync:
                return "e_sync";
            default:
                throw new InvalidOperationException();
        }
    }

    public string UserId { get; private set; }
    public string Route { get; private set; }
    public string Exchange { get; private set; }
    public AmqpDestination Destination { get; }
    public string? Type { get; private set; }
    public byte[] Body { get; set; } = new byte[0];

    public string Text
    {
        get => Encoding.UTF8.GetString(Body);
        set => Body = Encoding.UTF8.GetBytes(value);
    }
    public Task OnPublishConfirmed { get; set; }
}