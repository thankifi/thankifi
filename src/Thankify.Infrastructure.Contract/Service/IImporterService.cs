using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OperationResult;
using Thankify.Infrastructure.Contract.Model;

namespace Thankify.Infrastructure.Contract.Service
{
    public interface IImporterService
    {
        Task<Result<(List<Gratitude>, List<Category>), string>> Fetch(CancellationToken cancellationToken = default);
        Task<Result<string, string>> FindCurrentVersion(CancellationToken cancellationToken = default);
    }
}