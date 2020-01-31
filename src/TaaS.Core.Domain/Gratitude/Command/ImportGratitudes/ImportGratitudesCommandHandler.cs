using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OperationResult;

namespace TaaS.Core.Domain.Gratitude.Command.ImportGratitudes
{
    public class ImportGratitudesCommandHandler : IRequestHandler<ImportGratitudesCommand, Status<string>>
    {
        public Task<Status<string>> Handle(ImportGratitudesCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}