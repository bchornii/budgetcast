using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.AnemicModel;
using BudgetCast.Dashboard.Domain.ReadModel.Tags;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BudgetCast.Dashboard.ReadAccessors
{
    public class DefaultTagReadAccessor : IDefaultTagReadAccessor
    {
        private readonly BudgetCastContext _context;

        public DefaultTagReadAccessor(BudgetCastContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<string>> GetExistingTags(string[] tags)
        {
            return await _context.DefaultTags
                .AsQueryable()
                .Where(t => tags.Contains(t.Name))
                .Select(t => t.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<string>> GetTags(
            string userId, string term, int amount)
        {
            var filter = string.IsNullOrWhiteSpace(term)
                ? FilterByUser(userId)
                : FilterByUserAndName(userId, term);

            return await _context.DefaultTags
                .Find(filter)
                .Limit(amount)
                .SortBy(t => t.Name)
                .Project(t => t.Name)
                .ToListAsync();
        }

        private FilterDefinition<DefaultTag> FilterByUser(string userId) => 
            Builders<DefaultTag>.Filter.Where(t => t.UserId == userId);

        private FilterDefinition<DefaultTag> FilterByUserAndName(string userId, string term)
        {
            var regex = BsonRegularExpression.Create(
                new Regex(term, RegexOptions.IgnoreCase));

            return Builders<DefaultTag>.Filter
              .Regex(t => t.Name, regex) & FilterByUser(userId);
        }
    }
}