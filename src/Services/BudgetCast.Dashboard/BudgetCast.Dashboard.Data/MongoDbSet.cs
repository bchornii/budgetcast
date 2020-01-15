using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.SeedWork;
using MediatR;
using MongoDB.Driver;

namespace BudgetCast.Dashboard.Data
{
    public class MongoDbSet<TDocument> where TDocument : AggregateRoot
    {
        private readonly IMediator _mediator;
        private readonly string _userId;
        public IMongoCollection<TDocument> Collection { get; }

        public MongoDbSet(IMongoCollection<TDocument> collection, IMediator mediator, string userId)
        {
            _mediator = mediator;
            _userId = userId;
            Collection = collection;
        }

        public IFindFluent<TDocument, TDocument> Find(Expression<Func<TDocument, bool>> filter,
            FindOptions options = null)
        {
            return Collection.Find(filter, options);
        }

        public async Task<TDocument> InsertOneAsync(TDocument document, InsertOneOptions options = null,
            CancellationToken cancellationToken = default)
        {
            SetCreateMetaInformation(document, _userId);

            await Collection.InsertOneAsync(
                document, options, cancellationToken);
            await PublishDomainEvents(document);
            return document;
        }

        public async Task<TDocument> FindOneAndReplaceAsync(Expression<Func<TDocument, bool>> filter,
            TDocument replacement, FindOneAndReplaceOptions<TDocument, TDocument> options = null,
            CancellationToken cancellationToken = default)
        {
            SetUpdateMetaInformation(replacement, _userId);

            var result = await Collection.FindOneAndReplaceAsync(
                filter, replacement, options, cancellationToken);
            await PublishDomainEvents(replacement);
            return result;
        }

        public async Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter, TDocument document,
            CancellationToken cancellationToken = default)
        {
            var result = await Collection.DeleteOneAsync(filter, cancellationToken);
            await PublishDomainEvents(document);
            return result;
        }

        private async Task PublishDomainEvents(TDocument document)
        {
            var domainEvents = document.DomainEvents ??
                Array.Empty<INotification>();

            document.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }

        private void SetUpdateMetaInformation(TDocument document, string userId)
        {
            GetPropertyInfo<TDocument>("_updatedBy")
                ?.SetValue(document, userId);

            GetPropertyInfo<TDocument>("_updatedAt")
                ?.SetValue(document, DateTime.Now);
        }

        private void SetCreateMetaInformation(TDocument document, string userId)
        {
            GetPropertyInfo<TDocument>("_createdBy")
                ?.SetValue(document, userId);

            GetPropertyInfo<TDocument>("_createdAt")
                ?.SetValue(document, DateTime.Now);
        }

        private static FieldInfo GetPropertyInfo<T>(string propertyName)
        {
            return typeof(T).BaseType?
                .GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}
