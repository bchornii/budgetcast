using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.SeedWork;
using MongoDB.Driver;

namespace BudgetCast.Dashboard.Repository
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot
    {
        protected readonly BudgetCastContext Context;

        public Repository(BudgetCastContext context)
        {
            Context = context;
        }

        public Task<T> Add(T aggregate)
        {
            return Context.GetDbSet<T>().InsertOneAsync(aggregate);
        }

        public Task Update(T aggregate)
        {
            return Context.GetDbSet<T>().FindOneAndReplaceAsync(
                e => e.Id == aggregate.Id, aggregate);
        }

        public Task<T> GetAsync(string id)
        {
            return Context.GetDbSet<T>()
                .Find(e => e.Id == id).FirstOrDefaultAsync();
        }
    }
}