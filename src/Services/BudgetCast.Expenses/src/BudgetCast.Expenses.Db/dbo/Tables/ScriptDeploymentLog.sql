﻿CREATE TABLE [dbo].[ScriptDeploymentLog]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ScriptName] VARCHAR(250) NOT NULL,
	[TaskId] VARCHAR(50) NULL,
	[CreatedOnUTC] DateTime NOT NULL DEFAULT(GETUTCDATE()),
	[AppliedTimes] INT NOT NULL DEFAULT(1)
)