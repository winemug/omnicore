using System.Threading.Tasks;
using OmniCore.Common.Amqp;
using OmniCore.Services.Interfaces.Amqp;

namespace OmniCore.Services.Interfaces.Core;

public interface IAmqpService : ICoreService
{
    Task PublishMessage(AmqpMessage message, AmqpDestination destination);
    void RegisterMessageProcessor(Func<AmqpMessage, Task<bool>> function);
}