using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WeText.Common.Repositories
{
    public interface IRepository
    {
        Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : class, IAggregateRootWithEventSourcing;

        Task<TAggregateRoot> GetByIdAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : class, IAggregateRootWithEventSourcing;
    }
}
