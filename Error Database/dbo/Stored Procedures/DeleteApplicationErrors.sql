CREATE PROCEDURE [dbo].[DeleteApplicationErrors]
	(
	@ApplicationID int
	)
AS

DELETE Error
WHERE ApplicationID = @ApplicationID

	RETURN