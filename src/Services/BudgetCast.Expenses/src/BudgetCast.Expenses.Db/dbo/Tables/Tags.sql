CREATE TABLE [dbo].[Tags] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [ExpenseTenantId] BIGINT         NOT NULL,
    [ExpenseId]       BIGINT         NOT NULL,
    CONSTRAINT [PK_Tags_TagId] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tag_TenantId_ExpenseId__Expenses_TenantId_Id] FOREIGN KEY ([ExpenseTenantId], [ExpenseId]) REFERENCES [dbo].[Expenses] ([TenantId], [Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Tags_TenantId_ExpenseId]
    ON [dbo].[Tags]([ExpenseTenantId] ASC, [ExpenseId] ASC);

