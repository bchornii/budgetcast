CREATE PROCEDURE [dbo].[spSaveScriptDeploymentLog]
	@ScriptName VARCHAR(250),
	@TaskId VARCHAR(50)
AS

DECLARE @ScriptId INT;
SELECT @ScriptId = Id FROM [dbo].[ScriptDeploymentLog] WHERE [ScriptName] = @ScriptName;

IF (@ScriptId IS NULL) -- Script runs the first time
	BEGIN
		INSERT INTO [dbo].[ScriptDeploymentLog]([ScriptName], [TaskId])
		VALUES(@ScriptName, @TaskId)
	END
ELSE
	BEGIN
		UPDATE [dbo].[ScriptDeploymentLog]
		SET [TaskId] = @TaskId,
			[AppliedTimes] = [AppliedTimes] + 1,
			[CreatedOnUTC] = GETUTCDATE()
		WHERE [Id] = @ScriptId
	END