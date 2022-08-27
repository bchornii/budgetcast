namespace BudgetCast.Common.Domain;

public abstract class Entity : IAuditableEntity
{
    private int? _requestedHashCode;
    private List<IDomainEvent> _domainEvents;
    private List<IDomainEventNotification> _domainEventNotifications;

    public long Id { get; set; }

    public IReadOnlyCollection<IDomainEvent>? DomainEvents => _domainEvents?.AsReadOnly();

    public IReadOnlyCollection<IDomainEventNotification>? DomainEventNotifications =>
        _domainEventNotifications?.AsReadOnly();

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    protected Entity()
    {
        _domainEvents = default!;
        _domainEventNotifications = default!;
        CreatedBy = default!;
        LastModifiedBy = default!;
    }

    #region Equality methods

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

    #endregion

    #region Domain events

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents = _domainEvents ?? new List<IDomainEvent>();
        _domainEvents.Add(eventItem);
    }

    public void AddDomainEventNotification(IDomainEventNotification notification)
    {
        _domainEventNotifications = _domainEventNotifications ?? new List<IDomainEventNotification>();
        _domainEventNotifications.Add(notification);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem) 
        => _domainEvents?.Remove(eventItem);

    public void RemoveDomainEventNotification(IDomainEventNotification notification)
        => _domainEventNotifications?.Remove(notification);

    public void ClearDomainEvents() 
        => _domainEvents?.Clear();

    public void ClearDomainEventNotifications()
        => _domainEventNotifications?.Clear();

    #endregion
    
    private bool IsTransient() => Id == default;
}