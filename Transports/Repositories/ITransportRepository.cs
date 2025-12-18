using server.Transports.Models;

namespace server.Transports.Repositories;

public interface ITransportRepository
{
    Task<IEnumerable<TransportData>> GetAllTransports(TransportFilterQuery filter);
    Task<TransportDetail?> GetTransportById(int id);
    Task CreateTransport(Transport transport);
    Task DeleteTransport(int id);
}
