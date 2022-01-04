using MediatR;

namespace BudgetCast.Common.Domain
{
    public abstract class Entity
    {
        private int? _requestedHashCode;
        private List<INotification> _domainEvents;

        public string Id { get; set; }
        public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

        protected Entity()
        {
            Id = UniqueIdFactory.GetId();
            _domainEvents = default!;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            Entity item = (Entity)obj;

            return !item.IsTransient() && !IsTransient() && item.Id == Id;
        }
        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                {
                    _requestedHashCode = Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                }

                return _requestedHashCode.Value;
            }
            else
            {
                return base.GetHashCode();
            }
        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null) ? true : false;
            }
            else
            {
                return left.Equals(right);
            }
        }
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        private bool IsTransient() => Id == default;

    }
}
