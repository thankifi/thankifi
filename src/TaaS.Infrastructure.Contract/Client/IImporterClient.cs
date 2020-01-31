using System.Threading.Tasks;
using TaaS.Infrastructure.Contract.Model;

namespace TaaS.Infrastructure.Contract.Client
{
    public interface IImporterClient
    {
        Task<ImportResponse> GetData();
    }
}