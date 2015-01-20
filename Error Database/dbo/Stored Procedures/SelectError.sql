CREATE PROCEDURE [dbo].[SelectError]
AS
	SET NOCOUNT ON;
SELECT     dbo.Error.*
FROM         dbo.Error