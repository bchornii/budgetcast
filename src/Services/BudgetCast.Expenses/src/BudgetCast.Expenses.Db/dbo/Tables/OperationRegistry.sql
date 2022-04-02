CREATE TABLE [dbo].[OperationsRegistry]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1), 
    [Operations] NVARCHAR(MAX) NULL, 
    [IdempodentOperationName] NVARCHAR(200) NOT NULL,
    [StartedOn] DATETIME NOT NULL, 
    [CorrelationId] NVARCHAR(200) NOT NULL, 
    [OperationResult] NVARCHAR(MAX) NULL
)