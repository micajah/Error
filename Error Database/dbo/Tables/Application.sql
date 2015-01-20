CREATE TABLE [dbo].[Application] (
    [ApplicationID]  INT             IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (255)  NOT NULL,
    [Description]    NVARCHAR (1024) NULL,
    [SendEmail]      BIT             NULL,
    [MailFrom]       NVARCHAR (50)   NULL,
    [MailTo]         NVARCHAR (1050) NULL,
    [MailBWD]        NVARCHAR (1050) NULL,
    [MailAdmin]      NVARCHAR (50)   NULL,
    [SMTPServer]     NVARCHAR (250)  NULL,
    [CacheItemsSize] BIT             NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([ApplicationID] ASC)
);

