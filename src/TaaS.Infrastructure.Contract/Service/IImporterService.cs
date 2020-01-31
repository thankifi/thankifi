using System.Collections.Generic;
using System.Threading.Tasks;
using OperationResult;
using TaaS.Infrastructure.Contract.Model;

namespace TaaS.Infrastructure.Contract.Service
{
    public interface IImporterService
    {
        Task<Result<(List<Gratitude>, List<Category>), string>> Fetch();
    }
}