CREATE PROCEDURE [dbo].[SelectErrors]
AS
	SET NOCOUNT ON;
SELECT     dbo.Error.*
FROM         dbo.Error