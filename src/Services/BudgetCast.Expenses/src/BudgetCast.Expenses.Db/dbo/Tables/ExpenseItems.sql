CREATE TABLE [dbo].[ExpenseItems] (
    [Id]              BIGINT          NOT NULL,
    [Title]           NVARCHAR (150)  NOT NULL,
    [Note]            NVARCHAR (300)  NULL,
    [Price]           DECIMAL (18, 5) NOT NULL,
    [Quantity]        INT             NOT NULL,
    [ExpenseTenantId] BIGINT          NOT NULL,
    [ExpenseId]       BIGINT          NOT NULL,
    [CreatedBy]       NVARCHAR (100)  NULL,
    [CreatedOn]       DATETIME        NOT NULL,
    [LastModifiedBy]  NVARCHAR (100)  NOT NULL,
    [LastModifiedOn]  DATETIME        NULL,
    CONSTRAINT [PK_ExpenseItems_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExpenseItems_TenantId_ExpenseId__Expenses_TenantId_Id] FOREIGN KEY ([ExpenseTenantId], [ExpenseId]) REFERENCES [dbo].[Expenses] ([TenantId], [Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExpenseItems_ExpenseTenantId_ExpenseId]
    ON [dbo].[ExpenseItems]([ExpenseTenantId] ASC, [ExpenseId] ASC);

