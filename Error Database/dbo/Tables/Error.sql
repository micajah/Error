﻿CREATE TABLE [dbo].[Error] (
    [ErrorID]                INT             IDENTITY (1, 1) NOT NULL,
    [Date]                   DATETIME        CONSTRAINT [DF_Error_Date] DEFAULT (getdate()) NOT NULL,
    [ApplicationID]          INT             NOT NULL,
    [Browser]                NVARCHAR (1024) NULL,
    [Method]                 NVARCHAR (255)  NULL,
    [Name]                   NVARCHAR (4000) NULL,
    [Description]            NVARCHAR (4000) NULL,
    [URL]                    NVARCHAR (4000) NULL,
    [URLReferrer]            NVARCHAR (4000) NULL,
    [SourceFile]             NVARCHAR (4000) NULL,
    [ErrorLineNumber]        INT             NULL,
    [QueryString]            NVARCHAR (4000) NULL,
    [MachineName]            NVARCHAR (255)  NULL,
    [UserIPAddress]          NVARCHAR (15)   NULL,
    [ExceptionType]          NVARCHAR (4000) NULL,
    [StackTrace]             TEXT            NULL,
    [QueryStringDescription] TEXT            NULL,
    [Version]                TEXT            NULL,
    [RequestCookies]         TEXT            NULL,
    [RequestHeader]          TEXT            NULL,
    [Path]                   NVARCHAR (4000) NULL,
    [IsFixed]                BIT             NULL,
    [Session]                TEXT            NULL,
    [Cache]                  TEXT            NULL,
    [CacheSize]              DECIMAL (18, 2) NULL,
    [Application]            TEXT            NULL,
    [ServerVariables]        TEXT            NULL,
    CONSTRAINT [PK_Error] PRIMARY KEY CLUSTERED ([ApplicationID] ASC, [ErrorID] ASC)
);



