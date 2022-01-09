/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

BEGIN TRANSACTION
BEGIN TRY

:r .\Post-Deployment\00001_InitializeDb.sql

COMMIT TRANSACTION
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

	DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE(),
            @ErrorNumber int = ERROR_NUMBER(),
            @ErrorSeverity int = ERROR_SEVERITY(),
            @ErrorState int = ERROR_STATE(),
            @ErrorLine int = ERROR_LINE(),
            @ErrorProcedure nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');
    SELECT @ErrorMessage = N'Error %d, Level %d, State %d, Procedure %s, Line %d, ' + 'Message: ' + @ErrorMessage;
    RAISERROR (@ErrorMessage, @ErrorSeverity, 1, @ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorProcedure, @ErrorLine);
END CATCH;