CREATE PROCEDURE [dbo].[spGetScriptDeploymentLog]
	@ScriptName VARCHAR(250),
	@AppliedTimes INT OUTPUT

AS
	SELECT TOP 1 @AppliedTimes = ISNULL([AppliedTimes], 0) FROM [dbo].[ScriptDeploymentLog] WHERE [ScriptName] = @ScriptName
