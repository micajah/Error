CREATE PROCEDURE [dbo].[DeleteSpecErrors]
	(
	@ErrorName		nvarchar(1024),
	@LineNumber		int,
	@ApplicationID	int
	)
AS

	DELETE FROM Error
	WHERE ApplicationID = @ApplicationID
		AND ErrorLineNumber = @LineNumber
		AND [Name] = @ErrorName
		
	RETURN