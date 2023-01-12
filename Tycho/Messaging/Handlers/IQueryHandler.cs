using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    public interface IQueryHandler { }

    public interface IQueryHandler<in Query, Response>
        : IQueryHandler
        where Query : class, IQuery<Response>
    {
        Task<Response> Handle(Query query, CancellationToken cancellationToken = default);
    }
}
