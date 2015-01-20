
CREATE PROCEDURE [dbo].[InsertError]
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
	@Session ntext,
	@CacheSize decimal(18, 2)
)
AS
	SET NOCOUNT OFF;
INSERT INTO [dbo].[Error] ([Date], [ApplicationID], [Browser], [Method], [Name], [Description], [URL], [URLReferrer], [SourceFile], [ErrorLineNumber], [QueryString], [MachineName], [UserIPAddress], [ExceptionType], [StackTrace], [QueryStringDescription], [Version], [RequestCookies], [RequestHeader], [Path], [Session], [CacheSize]) VALUES (@Date, @ApplicationID, @Browser, @Method, @Name, @Description, @URL, @URLReferrer, @SourceFile, @ErrorLineNumber, @QueryString, @MachineName, @UserIPAddress, @ExceptionType, @StackTrace, @QueryStringDescription, @Version, @RequestCookies, @RequestHeader, @Path, @Session, @CacheSize);
	
SELECT ErrorID, Date, ApplicationID, Browser, Method, Name, Description, URL, URLReferrer, SourceFile, ErrorLineNumber, QueryString, MachineName, UserIPAddress, ExceptionType, StackTrace, QueryStringDescription, Version, RequestCookies, RequestHeader, Path, [Session], [CacheSize] FROM dbo.Error WHERE (ErrorID = SCOPE_IDENTITY())