CREATE TABLE dbo.IntegrationEventLog
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[EventId] NVARCHAR(64) NOT NULL,
	[EventTypeName] VARCHAR(240) NOT NULL,
	[State] INT NOT NULL,
	[TimesSent] INT NOT NULL,
	[CreationTime] DATETIME NOT NULL,
	[Content] NVARCHAR(MAX) NOT NULL,
	[ScopeId] NVARCHAR(64) NULL,
	[TransactionId] NVARCHAR(64) NULL
)

GO

CREATE NONCLUSTERED INDEX IX_IntegrationEventLog_ScopeId
ON [dbo].IntegrationEventLog(ScopeId)
WHERE ScopeId IS NOT NULL

GO

CREATE NONCLUSTERED INDEX IX_IntegrationEventLog_TransactionId
ON [dbo].IntegrationEventLog(TransactionId)
WHERE TransactionId IS NOT NULL