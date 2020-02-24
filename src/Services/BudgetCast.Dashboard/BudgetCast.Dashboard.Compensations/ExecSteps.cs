namespace BudgetCast.Dashboard.Compensations
{
    public static class ExecSteps
    {
        public static class ProfileImage
        {
            public static class Upload
            {
                public const string BlobUploaded = nameof(BlobUploaded);
                public const string DbRecordAdded = nameof(DbRecordAdded);
            }

            public static class Delete
            {
                public const string BlobDeleted = nameof(BlobDeleted);
                public const string DbRecordDeleted = nameof(DbRecordDeleted);
                public const string OldBlobDeleted = nameof(OldBlobDeleted);
            }
        }
    }
}