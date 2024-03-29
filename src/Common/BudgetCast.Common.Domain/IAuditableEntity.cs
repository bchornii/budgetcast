﻿namespace BudgetCast.Common.Domain
{
    public interface IAuditableEntity
    {
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}
