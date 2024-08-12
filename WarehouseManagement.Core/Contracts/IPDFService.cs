using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Contracts;

public interface IPDFService
{
    public byte[] GenerateBarcodePdfReport(DeliveryPDFModelDto model);
}
