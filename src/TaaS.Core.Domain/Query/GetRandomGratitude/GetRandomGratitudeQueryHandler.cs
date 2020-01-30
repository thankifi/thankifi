using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace TaaS.Core.Domain.Query.GetRandomGratitude
{
    public class GetRandomGratitudeQueryHandler : IRequestHandler<GetRandomGratitudeQuery, string>
    {
        public Task<string> Handle(GetRandomGratitudeQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}