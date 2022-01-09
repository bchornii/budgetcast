CREATE TABLE [dbo].[Expenses] (
    [TenantId]         BIGINT          NOT NULL,
    [Id]               BIGINT          NOT NULL,
    [Description]      NVARCHAR (150)  NOT NULL,
    [AddedAt]          DATETIME        NOT NULL,
    [TotalPrice]       DECIMAL (18, 5) NOT NULL,
    [CampaignTenantId] BIGINT          NOT NULL,
    [CampaignId]       BIGINT          NOT NULL,
    [CreatedBy]        NVARCHAR (100)  NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModifiedBy]   NVARCHAR (100)  NOT NULL,
    [LastModifiedOn]   DATETIME        NULL,
    CONSTRAINT [PK_Expenses_TenantId_Id] PRIMARY KEY CLUSTERED ([TenantId] ASC, [Id] ASC),
    CONSTRAINT [FK_Expenses_TenantId_CampaignId__Campaigns_TenantId_Id] FOREIGN KEY ([CampaignTenantId], [CampaignId]) REFERENCES [dbo].[Campaigns] ([TenantId], [Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Expenses_TenantId_CampaignId]
    ON [dbo].[Expenses]([TenantId] ASC, [CampaignId] ASC);

