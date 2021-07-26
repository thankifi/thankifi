using System.Threading;
using System.Threading.Tasks;
using Thankify.Infrastructure.Contract.Model;

namespace Thankify.Infrastructure.Contract.Client
{
    public interface IImporterClient
    {
        Task<ImportResponse> GetData(CancellationToken cancellationToken = default);
        Task<VersionResponse> GetVersion(CancellationToken cancellationToken = default);
    }
}