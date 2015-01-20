CREATE PROCEDURE [dbo].[DeleteError]
(
	@Original_ErrorID int
)
AS
	SET NOCOUNT OFF;
DELETE FROM [dbo].[Error] WHERE (([ErrorID] = @Original_ErrorID))