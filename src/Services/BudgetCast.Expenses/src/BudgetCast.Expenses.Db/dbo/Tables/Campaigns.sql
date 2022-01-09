CREATE TABLE [dbo].[Campaigns] (
    [TenantId]       BIGINT         NOT NULL,
    [Id]             BIGINT         NOT NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [StartsAt]       DATETIME       NOT NULL,
    [CompletesAt]    DATETIME       NULL,
    [CreatedBy]      NVARCHAR (100) NOT NULL,
    [CreatedOn]      DATETIME       NOT NULL,
    [LastModifiedBy] NVARCHAR (100) NOT NULL,
    [LastModifiedOn] DATETIME       NULL,
    CONSTRAINT [PK_Campaigns_TenantId_Id] PRIMARY KEY CLUSTERED ([TenantId] ASC, [Id] ASC)
);

