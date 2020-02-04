using System.Threading;
using System.Threading.Tasks;
using TaaS.Infrastructure.Contract.Model;

namespace TaaS.Infrastructure.Contract.Client
{
    public interface IImporterClient
    {
        Task<ImportResponse> GetData(CancellationToken cancellationToken = default);
        Task<VersionResponse> GetVersion(CancellationToken cancellationToken = default);
    }
}