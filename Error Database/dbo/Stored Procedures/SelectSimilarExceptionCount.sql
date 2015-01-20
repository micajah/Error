CREATE PROCEDURE [dbo].[SelectSimilarExceptionCount]
(
	@AppID int,
	@Path nvarchar(4000),
	@Line int
)
AS
	SET NOCOUNT ON;
SELECT     COUNT(ErrorID) AS ErrorCount
FROM         dbo.Error
WHERE     (ApplicationID = @AppID) AND (Path = @Path) AND (ErrorLineNumber = @Line)