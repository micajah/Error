
CREATE PROCEDURE [dbo].[UpdateError]
(
	@Date datetime,
	@ApplicationID int,
	@Browser nvarchar(1024),
	@Method nvarchar(255),
	@Name nvarchar(4000),
	@Description nvarchar(4000),
	@URL nvarchar(4000),
	@URLReferrer nvarchar(4000),
	@SourceFile nvarchar(4000),
	@ErrorLineNumber int,
	@QueryString nvarchar(4000),
	@MachineName nvarchar(255),
	@UserIPAddress nvarchar(15),
	@ExceptionType nvarchar(4000),
	@StackTrace ntext,
	@QueryStringDescription ntext,
	@Version ntext,
	@RequestCookies ntext,
	@RequestHeader ntext,
	@Path nvarchar(4000),
	@Original_ErrorID int,
	@Session ntext,
	@CacheSize decimal(18, 2),
	@Application ntext,
	@ServerVariables ntext,
	@ErrorID int
)
AS
	SET NOCOUNT OFF;
UPDATE [dbo].[Error] SET [Date] = @Date, [ApplicationID] = @ApplicationID, [Browser] = @Browser, [Method] = @Method, [Name] = @Name, [Description] = @Description, [URL] = @URL, [URLReferrer] = @URLReferrer, [SourceFile] = @SourceFile, [ErrorLineNumber] = @ErrorLineNumber, [QueryString] = @QueryString, [MachineName] = @MachineName, [UserIPAddress] = @UserIPAddress, [ExceptionType] = @ExceptionType, [StackTrace] = @StackTrace, [QueryStringDescription] = @QueryStringDescription, [Version] = @Version, [RequestCookies] = @RequestCookies, [RequestHeader] = @RequestHeader, [Path] = @Path, [Session] = @Session, [CacheSize] = @CacheSize, [Application] = @Application, [ServerVariables] = @ServerVariables WHERE (([ErrorID] = @Original_ErrorID));
	
SELECT ErrorID, Date, ApplicationID, Browser, Method, Name, Description, URL, URLReferrer, SourceFile, ErrorLineNumber, QueryString, MachineName, UserIPAddress, ExceptionType, StackTrace, QueryStringDescription, Version, RequestCookies, RequestHeader, Path, [Session], [CacheSize], [Application], [ServerVariables] FROM dbo.Error WHERE (ErrorID = @ErrorID)