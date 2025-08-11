using Service.Model;

namespace Orchestration.Saga;

public record OrchestrationInitializeSagaEvent(
    // CorrelationId 
    Guid OrderId, 
    //Payload Data
    IEnumerable<GoodViewModel> Goods, 
    Guid UserId, 
    string DeliveryAddress);