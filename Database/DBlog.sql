IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'blog')
EXEC sys.sp_executesql N'CREATE SCHEMA [blog] AUTHORIZATION [blog]'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Template]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Template](
	[Template_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Source] [ntext] NOT NULL,
	[Type] [char](64) NOT NULL,
 CONSTRAINT [PK_Template] PRIMARY KEY CLUSTERED 
(
	[Template_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Template] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Type] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Topic]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Topic](
	[Topic_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Type] [varchar](64) NULL,
 CONSTRAINT [PK_Topic] PRIMARY KEY CLUSTERED 
(
	[Topic_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Topic] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Type] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_counter_increment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_counter_increment]
(
      @object sysname
    , @object_id int
)
AS
BEGIN
    BEGIN TRANSACTION CounterIncrement
    EXECUTE
    (''
            DECLARE @CounterId int
	    SELECT @CounterId = [Counter_Id]
            FROM ['' + @object + ''Counter]
	    WHERE ['' + @object + ''_Id] = '' + @object_id + ''
	    IF @CounterId IS NULL
	    BEGIN
	    	INSERT INTO [Counter]
                (
                    [Resource_Id],
                    [Count],
                    [Created]
                )
                VALUES
                (
                    '' + @object_id + ''
                  , 0
                  , getdate()
                )
                SET @CounterId = SCOPE_IDENTITY()
	    	INSERT INTO ['' + @object + ''Counter]
                (
                    ['' + @object + ''_Id],
                    [Counter_Id]
                )
                VALUES
                (
                    '' + @object_id + ''
                  , @CounterId
                )
	    END
            UPDATE [Counter]
            SET [Count] = [Count] + 1
            WHERE [Counter_Id] = @CounterId
            SELECT [Count] 
            FROM [Counter]
            WHERE [Counter_Id] = @CounterId
    '')
    COMMIT TRANSACTION CounterIncrement
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerHostRollup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ReferrerHostRollup](
	[ReferrerHostRollup_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Rollup] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_ReferrerHostRollup] PRIMARY KEY CLUSTERED 
(
	[ReferrerHostRollup_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_ReferrerHostRollup] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Browser]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Browser](
	[Browser_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Crawler] [bit] NOT NULL CONSTRAINT [DF_Browser_Crawler]  DEFAULT (0),
 CONSTRAINT [PK_Browser] PRIMARY KEY CLUSTERED 
(
	[Browser_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Counter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Counter](
	[Counter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Resource_Id] [int] NOT NULL,
	[Count] [bigint] NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Counter] PRIMARY KEY CLUSTERED 
(
	[Counter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Feed]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Feed](
	[Feed_Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [varchar](256) NOT NULL,
	[Updated] [datetime] NULL,
	[Interval] [int] NOT NULL CONSTRAINT [DF_Feed_Interval]  DEFAULT (60),
	[Name] [nvarchar](64) NULL,
	[Description] [nvarchar](256) NULL,
	[Exception] [ntext] NULL,
	[Xsl] [ntext] NULL,
	[Username] [varchar](64) NULL,
	[Password] [varchar](64) NULL,
	[Type] [varchar](64) NOT NULL,
	[Saved] [datetime] NULL,
 CONSTRAINT [PK_Feed] PRIMARY KEY CLUSTERED 
(
	[Feed_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HourlyCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HourlyCounter](
	[HourlyCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestCount] [int] NOT NULL CONSTRAINT [DF_HourlyCounter_RequestCount]  DEFAULT (0),
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_HourlyCounter_DateTime]  DEFAULT (getdate()),
 CONSTRAINT [PK_HourlyCounter] PRIMARY KEY CLUSTERED 
(
	[HourlyCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[HourlyCounter]') AND name = N'IX_HourlyCounter')
CREATE NONCLUSTERED INDEX [IX_HourlyCounter] ON [dbo].[HourlyCounter] 
(
	[DateTime] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Image]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Image](
	[Image_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Data] [image] NULL,
	[Modified] [datetime] NOT NULL,
	[Path] [nvarchar](260) NULL,
	[Thumbnail] [image] NULL,
	[Preferred] [bit] NULL CONSTRAINT [DF_Image_Preferred]  DEFAULT (0),
	[Description] [nvarchar](256) NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[Image_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Login]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Login](
	[Login_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NULL,
	[Email] [varchar](32) NULL,
	[Role] [varchar](64) NOT NULL,
	[Username] [varchar](64) NULL,
	[Password] [varchar](32) NULL,
	[Website] [varchar](128) NULL,
 CONSTRAINT [PK_Login] PRIMARY KEY CLUSTERED 
(
	[Login_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Login] UNIQUE NONCLUSTERED 
(
	[Email] ASC,
	[Username] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Platform]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Platform](
	[Platform_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_Platform] PRIMARY KEY CLUSTERED 
(
	[Platform_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Platform] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reference]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Reference](
	[Reference_Id] [int] IDENTITY(1,1) NOT NULL,
	[Word] [nvarchar](256) NOT NULL,
	[Url] [nvarchar](256) NOT NULL,
	[Result] [nvarchar](256) NULL,
 CONSTRAINT [PK_Reference] PRIMARY KEY CLUSTERED 
(
	[Reference_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Referrer]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Referrer](
	[Referrer_Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](1024) NOT NULL,
	[Source] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_Referrer] PRIMARY KEY CLUSTERED 
(
	[Referrer_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_mondayofweek]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [dbo].[udf_mondayofweek]
(
    @week int
  , @year varchar(4)
)
RETURNS DATETIME
AS
BEGIN
	DECLARE @date varchar(10)
	DECLARE @firstdayofweek datetime
	SET @date=''01/01/'' + @year
	set @date = CONVERT(varchar(10), CONVERT(datetime, @date) - (datepart(dw, @date) - 1), 101)
	RETURN dateadd(ww, @week-1, @date)
END
' 
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerHost]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ReferrerHost](
	[ReferrerHost_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[LastUrl] [nvarchar](1024) NOT NULL,
	[LastSource] [nvarchar](1024) NOT NULL,
	[RequestCount] [bigint] NULL CONSTRAINT [DF_ReferrerHost_RequestCount]  DEFAULT (0),
 CONSTRAINT [PK_ReferrerHost] PRIMARY KEY CLUSTERED 
(
	[ReferrerHost_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_ReferrerHost] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerSearchQuery]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ReferrerSearchQuery](
	[ReferrerSearchQuery_Id] [int] IDENTITY(1,1) NOT NULL,
	[SearchQuery] [nvarchar](128) NOT NULL,
	[RequestCount] [bigint] NOT NULL CONSTRAINT [DF_ReferrerSearchQuery_RequestCount]  DEFAULT (0),
 CONSTRAINT [PK_ReferrerSearchQuery] PRIMARY KEY CLUSTERED 
(
	[ReferrerSearchQuery_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_ReferrerSearchQuery] UNIQUE NONCLUSTERED 
(
	[SearchQuery] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Entry]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Entry](
	[Entry_Id] [int] IDENTITY(1,1) NOT NULL,
	[Owner_Login_Id] [int] NOT NULL,
	[Title] [nvarchar](128) NOT NULL,
	[Text] [ntext] NOT NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_Entry_Created]  DEFAULT (getdate()),
	[Modified] [datetime] NOT NULL,
	[Template_Id] [int] NOT NULL,
	[IpAddress] [varchar](24) NOT NULL,
	[Topic_Id] [int] NOT NULL,
 CONSTRAINT [PK_Entry] PRIMARY KEY CLUSTERED 
(
	[Entry_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Entry]') AND name = N'IX_Entry')
CREATE NONCLUSTERED INDEX [IX_Entry] ON [dbo].[Entry] 
(
	[Modified] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Comment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Comment](
	[Comment_Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [ntext] NOT NULL,
	[IpAddress] [varchar](24) NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Owner_Login_Id] [int] NOT NULL,
	[Template_Id] [int] NOT NULL,
 CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED 
(
	[Comment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Gallery]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Gallery](
	[Gallery_Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](128) NOT NULL,
	[Text] [ntext] NULL,
	[Owner_Login_Id] [int] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Topic_Id] [int] NOT NULL,
	[Path] [varchar](128) NOT NULL,
	[Template_Id] [int] NOT NULL,
 CONSTRAINT [PK_Gallery] PRIMARY KEY CLUSTERED 
(
	[Gallery_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Gallery] UNIQUE NONCLUSTERED 
(
	[Title] ASC,
	[Path] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Gallery]') AND name = N'IX_Gallery')
CREATE NONCLUSTERED INDEX [IX_Gallery] ON [dbo].[Gallery] 
(
	[Modified] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BrowserVersionPlatform]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BrowserVersionPlatform](
	[BrowserVersionPlatform_Id] [int] IDENTITY(1,1) NOT NULL,
	[BrowserPlatform_Id] [int] NOT NULL,
	[BrowserVersion_Id] [int] NOT NULL,
 CONSTRAINT [PK_BrowserVersionPlatform] PRIMARY KEY CLUSTERED 
(
	[BrowserVersionPlatform_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BrowserVersionPlatform]') AND name = N'UK_BrowserVersionPlatform')
CREATE NONCLUSTERED INDEX [UK_BrowserVersionPlatform] ON [dbo].[BrowserVersionPlatform] 
(
	[BrowserPlatform_Id] ASC,
	[BrowserVersion_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EntryComment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EntryComment](
	[EntryComment_Id] [int] IDENTITY(1,1) NOT NULL,
	[Entry_Id] [int] NOT NULL,
	[Comment_Id] [int] NOT NULL,
 CONSTRAINT [PK_EntryComment] PRIMARY KEY CLUSTERED 
(
	[EntryComment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_EntryComment] UNIQUE NONCLUSTERED 
(
	[Entry_Id] ASC,
	[Comment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GalleryComment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GalleryComment](
	[GalleryComment_Id] [int] IDENTITY(1,1) NOT NULL,
	[Gallery_Id] [int] NOT NULL,
	[Comment_Id] [int] NOT NULL,
 CONSTRAINT [PK_GalleryComment] PRIMARY KEY CLUSTERED 
(
	[GalleryComment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_GalleryComment] UNIQUE NONCLUSTERED 
(
	[Gallery_Id] ASC,
	[Comment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Thread]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Thread](
	[Thread_Id] [int] IDENTITY(1,1) NOT NULL,
	[Comment_Id] [int] NOT NULL,
	[ParentComment_Id] [int] NOT NULL,
 CONSTRAINT [PK_CommentThread] PRIMARY KEY CLUSTERED 
(
	[Thread_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Thread] UNIQUE NONCLUSTERED 
(
	[Comment_Id] ASC,
	[ParentComment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ImageComment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ImageComment](
	[ImageComment_Id] [int] IDENTITY(1,1) NOT NULL,
	[Image_Id] [int] NOT NULL,
	[Comment_Id] [int] NOT NULL,
 CONSTRAINT [PK_ImageComment] PRIMARY KEY CLUSTERED 
(
	[ImageComment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_ImageComment] UNIQUE NONCLUSTERED 
(
	[Image_Id] ASC,
	[Comment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EntryImage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EntryImage](
	[EntryImage_Id] [int] IDENTITY(1,1) NOT NULL,
	[Entry_Id] [int] NOT NULL,
	[Image_Id] [int] NOT NULL,
 CONSTRAINT [PK_EntryImage] PRIMARY KEY CLUSTERED 
(
	[EntryImage_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_EntryImage] UNIQUE NONCLUSTERED 
(
	[Entry_Id] ASC,
	[Image_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[EntryImage]') AND name = N'IX_EntryImage')
CREATE NONCLUSTERED INDEX [IX_EntryImage] ON [dbo].[EntryImage] 
(
	[Entry_Id] DESC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EntryCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EntryCounter](
	[EntryCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Entry_Id] [int] NULL,
	[Counter_Id] [int] NULL,
 CONSTRAINT [PK_EntryCounter] PRIMARY KEY CLUSTERED 
(
	[EntryCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_EntryCounter] UNIQUE NONCLUSTERED 
(
	[Counter_Id] ASC,
	[Entry_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GalleryCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GalleryCounter](
	[GalleryCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Gallery_Id] [int] NOT NULL,
	[Counter_Id] [int] NOT NULL,
 CONSTRAINT [PK_GalleryCounter] PRIMARY KEY CLUSTERED 
(
	[GalleryCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_GalleryCounter] UNIQUE NONCLUSTERED 
(
	[Gallery_Id] ASC,
	[Counter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GalleryImage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GalleryImage](
	[GalleryImage_Id] [int] IDENTITY(1,1) NOT NULL,
	[Gallery_Id] [int] NOT NULL,
	[Image_Id] [int] NOT NULL,
 CONSTRAINT [PK_GalleryImage] PRIMARY KEY CLUSTERED 
(
	[GalleryImage_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_GalleryImage] UNIQUE NONCLUSTERED 
(
	[Gallery_Id] ASC,
	[Image_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GalleryImage]') AND name = N'IX_GalleryImage')
CREATE NONCLUSTERED INDEX [IX_GalleryImage] ON [dbo].[GalleryImage] 
(
	[Gallery_Id] DESC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GalleryLogin]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GalleryLogin](
	[GalleryLogin_Id] [int] IDENTITY(1,1) NOT NULL,
	[Gallery_Id] [int] NOT NULL,
	[Login_Id] [int] NOT NULL,
 CONSTRAINT [PK_GalleryLogin] PRIMARY KEY CLUSTERED 
(
	[GalleryLogin_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_GalleryLogin] UNIQUE NONCLUSTERED 
(
	[Gallery_Id] ASC,
	[Login_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Request]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Request](
	[Request_Id] [int] IDENTITY(1,1) NOT NULL,
	[IpAddress] [varchar](24) NOT NULL,
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_Request_DateTime]  DEFAULT (getdate()),
	[BrowserVersionPlatform_Id] [int] NOT NULL,
 CONSTRAINT [PK_Request] PRIMARY KEY CLUSTERED 
(
	[Request_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RollupBrowserVersionPlatform]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RollupBrowserVersionPlatform](
	[RollupBrowserVersionPlatform_Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestCount] [bigint] NOT NULL CONSTRAINT [DF_RollupBrowserVersionPlatform_RequestCount]  DEFAULT (0),
	[BrowserVersionPlatform_Id] [int] NOT NULL,
 CONSTRAINT [PK_RollupBrowserVersionPlatform] PRIMARY KEY CLUSTERED 
(
	[RollupBrowserVersionPlatform_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_RollupBrowserVersionPlatform] UNIQUE NONCLUSTERED 
(
	[BrowserVersionPlatform_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BrowserVersion]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BrowserVersion](
	[BrowserVersion_Id] [int] IDENTITY(1,1) NOT NULL,
	[Major] [int] NULL,
	[Minor] [int] NULL,
	[Browser_Id] [int] NOT NULL,
 CONSTRAINT [PK_BrowserVersion] PRIMARY KEY CLUSTERED 
(
	[BrowserVersion_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_BrowserVersion] UNIQUE NONCLUSTERED 
(
	[Major] ASC,
	[Minor] ASC,
	[Browser_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BrowserPlatform]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BrowserPlatform](
	[BrowserPlatform_Id] [int] IDENTITY(1,1) NOT NULL,
	[Browser_Id] [int] NOT NULL,
	[Platform_Id] [int] NOT NULL,
 CONSTRAINT [PK_BrowserPlatform] PRIMARY KEY CLUSTERED 
(
	[BrowserPlatform_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_BrowserPlatform] UNIQUE NONCLUSTERED 
(
	[Browser_Id] ASC,
	[Platform_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NamedCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NamedCounter](
	[NamedCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Counter_Id] [int] NOT NULL,
 CONSTRAINT [PK_NamedCounter] PRIMARY KEY CLUSTERED 
(
	[NamedCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_NamedCounter] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Counter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ImageCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ImageCounter](
	[ImageCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Image_Id] [int] NULL,
	[Counter_Id] [int] NULL,
 CONSTRAINT [PK_ImageCounter] PRIMARY KEY CLUSTERED 
(
	[ImageCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_ImageCounter] UNIQUE NONCLUSTERED 
(
	[Image_Id] ASC,
	[Counter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoginCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LoginCounter](
	[LoginCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Login_Id] [int] NOT NULL,
	[Counter_Id] [int] NOT NULL,
 CONSTRAINT [PK_LoginCounter] PRIMARY KEY CLUSTERED 
(
	[LoginCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_LoginCounter] UNIQUE NONCLUSTERED 
(
	[Login_Id] ASC,
	[Counter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FeedItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FeedItem](
	[FeedItem_Id] [int] IDENTITY(1,1) NOT NULL,
	[Feed_Id] [int] NOT NULL,
	[Title] [nvarchar](256) NULL,
	[Description] [ntext] NULL,
	[Link] [varchar](256) NULL,
 CONSTRAINT [PK_FeedItem] PRIMARY KEY CLUSTERED 
(
	[FeedItem_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FeedItem]') AND name = N'IX_FeedItem')
CREATE NONCLUSTERED INDEX [IX_FeedItem] ON [dbo].[FeedItem] 
(
	[FeedItem_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Highlight]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Highlight](
	[Highlight_Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](64) NOT NULL,
	[Image_Id] [int] NOT NULL,
	[Description] [nvarchar](256) NULL,
	[Url] [varchar](256) NOT NULL,
 CONSTRAINT [PK_Highlight] PRIMARY KEY CLUSTERED 
(
	[Highlight_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Highlight] UNIQUE NONCLUSTERED 
(
	[Title] ASC,
	[Url] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Platforms]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[Platforms] AS
(
  SELECT
      0 AS ''Platforms_Id''
    , Platform.[Name] AS ''Name''
    , SUM(RollupBrowserVersionPlatform.RequestCount) AS ''RequestCount'' 
  FROM 
    RollupBrowserVersionPlatform
  INNER JOIN BrowserPlatform ON RollupBrowserVersionPlatform.BrowserVersionPlatform_Id = BrowserPlatform.BrowserPlatform_Id
  INNER JOIN Platform ON BrowserPlatform.Platform_Id = Platform.Platform_Id
  GROUP BY 
    Platform.[Name]
)
' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Browsers]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[Browsers] AS
(
	SELECT 
	   Browser.Browser_Id AS ''Browser_Id''
	 , Browser.[Name] AS ''BrowserName''
	 , Browser.Crawler As ''BrowserCrawler''
	 , LTRIM(STR(BrowserVersion.Major)) + ''.'' + LTRIM(STR(BrowserVersion.Minor)) AS ''BrowserVersion''
	 , Platform.[Name] AS ''BrowserPlatform''
         , RequestCount AS ''RequestCount''
	FROM Browser
	 INNER JOIN BrowserVersion ON Browser.Browser_Id = BrowserVersion.Browser_Id
	 INNER JOIN BrowserPlatform ON Browser.Browser_Id = BrowserPlatform.Browser_Id
	 INNER JOIN Platform ON BrowserPlatform.Platform_Id = Platform.Platform_Id
         INNER JOIN BrowserVersionPlatform ON (
               BrowserVersion.BrowserVersion_Id = BrowserVersionPlatform.BrowserVersion_Id
           AND BrowserPlatform.BrowserPlatform_Id = BrowserVersionPlatform.BrowserPlatform_Id
         )
         INNER JOIN RollupBrowserVersionPlatform ON RollupBrowserVersionPlatform.BrowserVersionPlatform_Id = BrowserVersionPlatform.BrowserVersionPlatform_Id
)
' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_browserplatform_create]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_browserplatform_create]
(
        @browser_id int
      , @platform_id int
)
AS
BEGIN
    DECLARE @BrowserPlatformId int
    BEGIN TRANSACTION BrowserPlatformCreate
    
	    SELECT @BrowserPlatformId = (
	        SELECT
	            [BrowserPlatform_Id]
	        FROM
	            [BrowserPlatform]
	        WHERE
	            BrowserPlatform.[Platform_Id] = @platform_id
	            AND BrowserPlatform.[Browser_Id] = @browser_id
	    )
	    
	    IF @BrowserPlatformId IS NULL
	    BEGIN   
	
	        INSERT INTO 
	            [BrowserPlatform]
	        (
	              [Browser_Id]
                    , [Platform_Id]
	        )
	        VALUES
	        ( 
	               @browser_id
                     , @platform_id
	        )
	        
	        SET @BrowserPlatformId = SCOPE_IDENTITY()
	    END
    COMMIT TRANSACTION BrowserPlatformCreate
    RETURN @BrowserPlatformId
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_browserversion_create]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_browserversion_create]
(
        @browser_id int
      , @major int = NULL
      , @minor int = NULL
)
AS
BEGIN
    DECLARE @BrowserVersionId int    
    BEGIN TRANSACTION BrowserVersionCreate
    
    SELECT @BrowserVersionId = (
        SELECT
            [BrowserVersion_Id]
        FROM
            [BrowserVersion]
        WHERE
            BrowserVersion.[Browser_Id] = @browser_id
            AND BrowserVersion.[Major] = @major
            AND BrowserVersion.[Minor] = @minor
    )
    
    IF @BrowserVersionId IS NULL
    BEGIN   
        INSERT INTO 
            dbo.[BrowserVersion]
        (
              [Browser_Id]
            , [Major]
            , [Minor]
        )
        VALUES
        ( 
              @browser_id
            , @major
            , @minor
        )
        
        SET @BrowserVersionId = SCOPE_IDENTITY()
    END
    COMMIT TRANSACTION BrowserVersionCreate
    RETURN @BrowserVersionId
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Comments]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[Comments]
AS
SELECT     dbo.Comment.Comment_Id, dbo.Comment.Text, dbo.Comment.IpAddress, dbo.Comment.Modified, dbo.Comment.Created, 
                      dbo.Comment.Owner_Login_Id, ''Entry'' AS Type, dbo.EntryComment.Entry_Id AS Parent_Id
FROM         dbo.Comment INNER JOIN
                      dbo.EntryComment ON dbo.Comment.Comment_Id = dbo.EntryComment.Comment_Id
UNION ALL
SELECT     Comment_2.Comment_Id, Comment_2.Text, Comment_2.IpAddress, Comment_2.Modified, Comment_2.Created, Comment_2.Owner_Login_Id, 
                      ''Gallery'' AS Type, dbo.GalleryComment.Gallery_Id AS Parent_Id
FROM         dbo.Comment AS Comment_2 INNER JOIN
                      dbo.GalleryComment ON Comment_2.Comment_Id = dbo.GalleryComment.Comment_Id
UNION ALL
SELECT     Comment_1.Comment_Id, Comment_1.Text, Comment_1.IpAddress, Comment_1.Modified, Comment_1.Created, Comment_1.Owner_Login_Id, 
                      ''Image'' AS Type, dbo.ImageComment.Image_Id AS Parent_Id
FROM         dbo.Comment AS Comment_1 INNER JOIN
                      dbo.ImageComment ON Comment_1.Comment_Id = dbo.ImageComment.Comment_Id
' 
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[30] 2[40] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 3
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 5
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'Comments'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'Comments'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Blog]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[Blog]
AS
SELECT     Gallery_Id AS Blog_Id, ''Gallery'' AS Type, Title, Text, Owner_Login_Id, Modified, Created, Topic_Id
FROM         dbo.Gallery
UNION ALL
SELECT     Entry_Id AS Blog_Id, ''Entry'' AS Type, Title, Text, Owner_Login_Id, Modified, Created, Topic_Id
FROM         dbo.Entry
' 
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[30] 2[40] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 3
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 5
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'Blog'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'Blog'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Counters]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[Counters] AS
(
SELECT ''Entry'' AS [Type], Counter.* FROM Counter
INNER JOIN EntryCounter ON EntryCounter.Counter_Id = Counter.Counter_Id 
UNION ALL
SELECT ''Gallery'' AS [Type], Counter.* FROM Counter
INNER JOIN GalleryCounter ON GalleryCounter.Counter_Id = Counter.Counter_Id 
UNION ALL
SELECT ''Image'' AS [Type], Counter.* FROM Counter
INNER JOIN ImageCounter ON ImageCounter.Counter_Id = Counter.Counter_Id 
)
' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_namedcounter_increment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_namedcounter_increment]
(
      @name varchar(64)
)
AS
BEGIN
    BEGIN TRANSACTION CounterIncrement
    DECLARE @CounterId int
    SELECT @CounterId = [Counter_Id]
    FROM [NamedCounter]
    WHERE [Name] = @name
    IF @CounterId IS NULL
    BEGIN
        INSERT INTO [Counter]
        (
            [Resource_Id],
            [Count],
            [Created]
        )
        VALUES
        (
            0
          , 0
          , getdate()
        )
        SET @CounterId = SCOPE_IDENTITY()
        INSERT INTO [NamedCounter]
        (
            [Name],
            [Counter_Id]
        )
        VALUES
        (
            @name
          , @CounterId
        )
    END
    UPDATE [Counter]
    SET [Count] = [Count] + 1
    WHERE [Counter_Id] = @CounterId
    SELECT [Count] 
    FROM [Counter]
    WHERE [Counter_Id] = @CounterId
    COMMIT TRANSACTION CounterIncrement
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_rollup_browserversionplatform]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_rollup_browserversionplatform]
AS
BEGIN
  BEGIN TRANSACTION RollupBrowserVersionPlatformT
  INSERT INTO [RollupBrowserVersionPlatform]
  (
      [BrowserVersionPlatform_Id]
    , [RequestCount]
  )
  SELECT 
      BrowserVersionPlatform.BrowserVersionPlatform_Id
    , 0
  FROM
      BrowserVersionPlatform
  WHERE NOT EXISTS
  (
   SELECT BrowserVersionPlatform_Id 
   FROM 
    [RollupBrowserVersionPlatform]
   WHERE 
      RollupBrowserVersionPlatform.BrowserVersionPlatform_Id = BrowserVersionPlatform.BrowserVersionPlatform_Id
  )
  UPDATE 
      [RollupBrowserVersionPlatform]
  SET
      [RequestCount] = [RequestCount] + 
      (
        SELECT 
          COUNT(*) 
        FROM 
          [Request] 
        WHERE 
          Request.BrowserVersionPlatform_Id = RollupBrowserVersionPlatform.BrowserVersionPlatform_Id
      )
  TRUNCATE TABLE [Request]
  COMMIT Transaction RollupBrowserVersionPlatformT
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_browserversionplatform_create]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_browserversionplatform_create]
(
        @browserversion_id int
      , @browserplatform_id int
)
AS
BEGIN
    DECLARE @BrowserVersionPlatformId int    
    BEGIN TRANSACTION BrowserVersionPlatformCreate
    
    SELECT @BrowserVersionPlatformId = (
        SELECT
            [BrowserVersionPlatform_Id]
        FROM
            [BrowserVersionPlatform]
        WHERE
            BrowserVersionPlatform.[BrowserVersion_Id] = @browserversion_id
            AND BrowserVersionPlatform.[BrowserPlatform_Id] = @browserplatform_id
    )
    
    IF @BrowserVersionPlatformId IS NULL
    BEGIN   
        INSERT INTO 
            dbo.[BrowserVersionPlatform]
        (
              [BrowserVersion_Id]
            , [BrowserPlatform_Id]
        )
        VALUES
        ( 
              @browserversion_id
            , @browserplatform_id
        )
        
        SET @BrowserVersionPlatformId = SCOPE_IDENTITY()
    END
    COMMIT TRANSACTION BrowserVersionPlatformCreate
    RETURN @BrowserVersionPlatformId
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_create_referrerhostrollup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

CREATE PROCEDURE [dbo].[sp_create_referrerhostrollup]
(
    @name nvarchar(128)
  , @rollup nvarchar(128)
)
AS
BEGIN

 INSERT INTO [ReferrerHostRollup] 
 (
  [Name],
  [Rollup]
 )
 SELECT 
  [Name], 
  @rollup
 FROM ReferrerHost 
 WHERE 
  [Name] LIKE @name
 AND NOT EXISTS 
 ( 
   SELECT 
    [ReferrerHostRollup_Id] 
   FROM 
    [ReferrerHostRollup] 
   WHERE 
    ReferrerHostRollup.[Name] = ReferrerHost.[Name] 
 )

END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_browser_create]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_browser_create]
(
	  @name nvarchar(128)
      	, @crawler bit = 0
)
AS
BEGIN
    DECLARE @BrowserId int
    BEGIN TRANSACTION BrowserCreate
    
    SELECT @BrowserId = (
        SELECT
            [Browser_Id]
        FROM
            [Browser]
        WHERE
            Browser.[Name] = @name
            AND Browser.[Crawler] = @crawler
    )
    
    IF @BrowserId IS NULL
    BEGIN   
        INSERT INTO 
            [Browser]
        (
              [Name]
            , [Crawler]
        )
        VALUES
        ( 
              @name
            , @crawler
        )
        
        SET @BrowserId = SCOPE_IDENTITY()
    END
    COMMIT TRANSACTION BrowserCreate
    RETURN @BrowserId
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_monthly]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_hourlycounters_monthly]
(
    @hhdelta int = 0
  , @hhperiod int = 0
)
AS
BEGIN
  SELECT
    0 AS ''HourlyCounter_Id'', 
    SUM(RequestCount) AS ''RequestCount'',
    CONVERT(DATETIME, LTRIM(STR(DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))) + ''-'' +
    LTRIM(STR(DATEPART("month", DATEADD("hh", @hhdelta, [DateTime])))) + ''-01'')
    AS ''DateTime''
    FROM HourlyCounter
  WHERE
    [DateTime] >= DATEADD("hh", - @hhperiod, GETDATE())
    OR @hhperiod = 0
  GROUP BY
    CONVERT(DATETIME, LTRIM(STR(DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))) + ''-'' +
    LTRIM(STR(DATEPART("month", DATEADD("hh", @hhdelta, [DateTime])))) + ''-01'')
  ORDER BY
    [DateTime] DESC
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_hourly]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_hourlycounters_hourly]
(
    @hhdelta int = 0
  , @hhperiod int = 0
)
AS
BEGIN
  SELECT  
    0 AS ''HourlyCounter_Id'',   
    RequestCount AS ''RequestCount'',  
    DATEADD("hh", @hhdelta, [DateTime]) 
    AS ''DateTime''  
  FROM 
    HourlyCounter
  WHERE
    [DateTime] >= DATEADD("hh", - @hhperiod, GETDATE())
    OR @hhperiod = 0
  ORDER BY
    [DateTime] DESC
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_weekly]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_hourlycounters_weekly]
(
    @hhdelta int = 0
  , @hhperiod int = 0
)
AS
BEGIN
  SELECT 
    0 AS ''HourlyCounter_Id'',
    SUM(RequestCount) AS ''RequestCount'',
    dbo.udf_mondayofweek(DATEPART("wk", DATEADD("hh", @hhdelta, [DateTime])), DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))
    AS ''DateTime''
  FROM 
    HourlyCounter
  WHERE
    [DateTime] >= DATEADD("hh", - @hhperiod, GETDATE())
    OR @hhperiod = 0
  GROUP BY
    dbo.udf_mondayofweek(DATEPART("wk", DATEADD("hh", @hhdelta, [DateTime])), DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))
  ORDER BY
    [DateTime] DESC
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_yearly]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_hourlycounters_yearly]
(
    @hhdelta int = 0
  , @hhperiod int = 0
)
AS
BEGIN
  SELECT
    0 AS ''HourlyCounter_Id'', 
    SUM(RequestCount) AS ''RequestCount'',
    CONVERT(DATETIME, LTRIM(STR(DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))) + ''-01-01'')
    AS ''DateTime''
    FROM HourlyCounter
  WHERE
    [DateTime] >= DATEADD("hh", - @hhperiod, GETDATE())
    OR @hhperiod = 0
  GROUP BY
    CONVERT(DATETIME, LTRIM(STR(DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))) + ''-01-01'')
  ORDER BY
    [DateTime] DESC
 
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_daily]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_hourlycounters_daily]
(
    @hhdelta int = 0
  , @hhperiod int = 0
)
AS
BEGIN
  SELECT  
    0 AS ''HourlyCounter_Id'',   
    SUM(RequestCount) AS ''RequestCount'',  
    CONVERT(DATETIME, LTRIM(STR(DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))) + ''-'' +  
    LTRIM(STR(DATEPART("month", DATEADD("hh", @hhdelta, [DateTime])))) + ''-'' +  
    LTRIM(STR(DATEPART("day", DATEADD("hh", @hhdelta, [DateTime])))))  
    AS ''DateTime''  
  FROM 
    HourlyCounter
  WHERE
    [DateTime] >= DATEADD("hh", - @hhperiod, GETDATE())
    OR @hhperiod = 0
  GROUP BY  
    CONVERT(DATETIME, LTRIM(STR(DATEPART("year", DATEADD("hh", @hhdelta, [DateTime])))) + ''-'' +  
    LTRIM(STR(DATEPART("month", DATEADD("hh", @hhdelta, [DateTime])))) + ''-'' +  
    LTRIM(STR(DATEPART("day", DATEADD("hh", @hhdelta, [DateTime])))))
  ORDER BY
    [DateTime] DESC
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounter_increment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_hourlycounter_increment]
AS
BEGIN
    BEGIN TRANSACTION HourlyCounterIncrement
    DECLARE @HourlyCounterId int
    DECLARE @now DATETIME 
    SET @now = getdate()
    SET @now = DATEADD(ss, - DATEPART(ss, @now), @now)
    SET @now = DATEADD(ms, - DATEPART(ms, @now), @now)
    SET @now = DATEADD(mi, - DATEPART(mi, @now), @now)
    SELECT 
      @HourlyCounterId = [HourlyCounter_Id]
    FROM 
      [HourlyCounter]
    WHERE 
      [DateTime] = @now
    IF @HourlyCounterId IS NULL
    BEGIN
        INSERT INTO [HourlyCounter]
        (
              [RequestCount]
            , [DateTime]
        )
        VALUES
        (
            1
          , @now
        )
    END
    ELSE
    BEGIN
        UPDATE 
            [HourlyCounter]
        SET 
            [RequestCount] = [RequestCount] + 1
        WHERE 
            [HourlyCounter_Id] = @HourlyCounterId
    END
    COMMIT TRANSACTION HourlyCounterIncrement
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_platform_create]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_platform_create]
(
      @name nvarchar(128)
)
AS
BEGIN
    DECLARE @PlatformId int    
    BEGIN TRANSACTION PlatformCreate
    
    SELECT @PlatformId = (
        SELECT
            [Platform_Id]
        FROM
            [Platform]
        WHERE
            Platform.[Name] = @name
    )
    
    IF @PlatformId IS NULL
    BEGIN   
        INSERT INTO 
            dbo.[Platform]
        (
              [Name]
        )
        VALUES
        ( 
              @name
        )
        
        SET @PlatformId = SCOPE_IDENTITY()
    END
    COMMIT TRANSACTION PlatformCreate
    RETURN @PlatformId
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_rollup_referrerhost]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_rollup_referrerhost]
(
    @name nvarchar(128)
  , @lasturl nvarchar(1024)
  , @lastsource nvarchar(1024)
)
AS
BEGIN
    DECLARE @ReferrerHostId int    
    BEGIN TRANSACTION ReferrerHostCreate
    
    SELECT @ReferrerHostId = (
        SELECT
            [ReferrerHost_Id]
        FROM
            [ReferrerHost]
        WHERE
            ReferrerHost.[Name] = @name
    )
    
    IF @ReferrerHostId IS NULL
    BEGIN   
        INSERT INTO 
            dbo.[ReferrerHost]
        (
                [Name]
              , [LastUrl]
              , [LastSource]
              , [RequestCount]
        )
        VALUES
        ( 
                @name
              , @lasturl
              , @lastsource
              , 1
        )
        
        SET @ReferrerHostId = SCOPE_IDENTITY()
    END
    ELSE
    BEGIN
	    UPDATE
	        [ReferrerHost]
	    SET
	          [LastUrl] = @lasturl
	        , [LastSource] = @lastsource
	        , [RequestCount] = [RequestCount] + 1
	    WHERE
	        [ReferrerHost_Id] = @ReferrerHostId
    END
	
    COMMIT TRANSACTION ReferrerHostCreate
    RETURN @ReferrerHostId
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_rollup_referrersearchquery]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_rollup_referrersearchquery]
(
    @searchquery nvarchar(128)
)
AS
BEGIN
    DECLARE @ReferrerSearchQueryId int    
    BEGIN TRANSACTION ReferrerSearchQueryCreate
    
    SELECT @ReferrerSearchQueryId = (
        SELECT
            [ReferrerSearchQuery_Id]
        FROM
            [ReferrerSearchQuery]
        WHERE
            ReferrerSearchQuery.[SearchQuery] = @searchquery
    )
    
    IF @ReferrerSearchQueryId IS NULL
    BEGIN   
        INSERT INTO 
            dbo.[ReferrerSearchQuery]
        (
                [SearchQuery]
              , [RequestCount]
        )
        VALUES
        ( 
                @searchquery
              , 1
        )
        
        SET @ReferrerSearchQueryId = SCOPE_IDENTITY()
    END
    ELSE
    BEGIN
	    UPDATE
	        [ReferrerSearchQuery]
	    SET
	        [RequestCount] = [RequestCount] + 1
	    WHERE
	        [ReferrerSearchQuery_Id] = @ReferrerSearchQueryId
    END
    COMMIT TRANSACTION ReferrerSearchQueryCreate
    RETURN @ReferrerSearchQueryId
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[BrowsersByName]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[BrowsersByName] AS
(
  SELECT
      0 AS ''BrowsersByName_Id''
    , BrowserName AS ''Name''
    , SUM(RequestCount) AS ''RequestCount'' 
  FROM 
    Browsers
  GROUP BY 
    BrowserName
)
' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_request_create]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_request_create]
(
          @ipaddress nvarchar(24)
	, @browser_name nvarchar(128)
	, @browser_platform nvarchar(128)
      	, @browser_crawler bit = 0
	, @browser_majorversion int = 0
	, @browser_minorversion int = 0
)
AS
BEGIN
    DECLARE @BrowserId int
    DECLARE @BrowserPlatformId int
    DECLARE @BrowserVersionId int
    DECLARE @PlatformId int
    DECLARE @BrowserVersionPlatformId int
    EXEC @BrowserId = sp_browser_create
          @name = @browser_name
        , @crawler = @browser_crawler
    EXEC @PlatformId = sp_platform_create
          @name = @browser_platform
    EXEC @BrowserPlatformId = sp_browserplatform_create
          @browser_id = @BrowserId
        , @platform_id = @PlatformId
    EXEC @BrowserVersionId = sp_browserversion_create
          @browser_id = @BrowserId
        , @major = @browser_majorversion
        , @minor = @browser_minorversion
    EXEC @BrowserVersionPlatformId = sp_browserversionplatform_create
          @browserversion_id = @BrowserVersionId
        , @browserplatform_id = @BrowserPlatformId
    INSERT INTO [Request]
    (
	  [IpAddress]
	, [DateTime]
        , [BrowserVersionPlatform_Id]
    )
    VALUES
    (
	  @ipaddress
	, DEFAULT
        , @BrowserVersionPlatformId
    )
END
' 
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Entry_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[Entry]'))
ALTER TABLE [dbo].[Entry]  WITH CHECK ADD  CONSTRAINT [FK_Entry_Login] FOREIGN KEY([Owner_Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Entry_Template]') AND parent_object_id = OBJECT_ID(N'[dbo].[Entry]'))
ALTER TABLE [dbo].[Entry]  WITH CHECK ADD  CONSTRAINT [FK_Entry_Template] FOREIGN KEY([Template_Id])
REFERENCES [dbo].[Template] ([Template_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Entry_Topic]') AND parent_object_id = OBJECT_ID(N'[dbo].[Entry]'))
ALTER TABLE [dbo].[Entry]  WITH CHECK ADD  CONSTRAINT [FK_Entry_Topic] FOREIGN KEY([Topic_Id])
REFERENCES [dbo].[Topic] ([Topic_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Comment_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[Comment]'))
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_Login] FOREIGN KEY([Owner_Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Comment_Template]') AND parent_object_id = OBJECT_ID(N'[dbo].[Comment]'))
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_Template] FOREIGN KEY([Template_Id])
REFERENCES [dbo].[Template] ([Template_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Gallery_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[Gallery]'))
ALTER TABLE [dbo].[Gallery]  WITH CHECK ADD  CONSTRAINT [FK_Gallery_Login] FOREIGN KEY([Owner_Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Gallery_Template]') AND parent_object_id = OBJECT_ID(N'[dbo].[Gallery]'))
ALTER TABLE [dbo].[Gallery]  WITH CHECK ADD  CONSTRAINT [FK_Gallery_Template] FOREIGN KEY([Template_Id])
REFERENCES [dbo].[Template] ([Template_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Gallery_Topic]') AND parent_object_id = OBJECT_ID(N'[dbo].[Gallery]'))
ALTER TABLE [dbo].[Gallery]  WITH CHECK ADD  CONSTRAINT [FK_Gallery_Topic] FOREIGN KEY([Topic_Id])
REFERENCES [dbo].[Topic] ([Topic_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BrowserVersionPlatform_BrowserPlatform]') AND parent_object_id = OBJECT_ID(N'[dbo].[BrowserVersionPlatform]'))
ALTER TABLE [dbo].[BrowserVersionPlatform]  WITH CHECK ADD  CONSTRAINT [FK_BrowserVersionPlatform_BrowserPlatform] FOREIGN KEY([BrowserPlatform_Id])
REFERENCES [dbo].[BrowserPlatform] ([BrowserPlatform_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BrowserVersionPlatform_BrowserVersion]') AND parent_object_id = OBJECT_ID(N'[dbo].[BrowserVersionPlatform]'))
ALTER TABLE [dbo].[BrowserVersionPlatform]  WITH CHECK ADD  CONSTRAINT [FK_BrowserVersionPlatform_BrowserVersion] FOREIGN KEY([BrowserVersion_Id])
REFERENCES [dbo].[BrowserVersion] ([BrowserVersion_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EntryComment_Comment]') AND parent_object_id = OBJECT_ID(N'[dbo].[EntryComment]'))
ALTER TABLE [dbo].[EntryComment]  WITH CHECK ADD  CONSTRAINT [FK_EntryComment_Comment] FOREIGN KEY([Comment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EntryComment_Entry]') AND parent_object_id = OBJECT_ID(N'[dbo].[EntryComment]'))
ALTER TABLE [dbo].[EntryComment]  WITH CHECK ADD  CONSTRAINT [FK_EntryComment_Entry] FOREIGN KEY([Entry_Id])
REFERENCES [dbo].[Entry] ([Entry_Id])
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryComment_Comment]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryComment]'))
ALTER TABLE [dbo].[GalleryComment]  WITH CHECK ADD  CONSTRAINT [FK_GalleryComment_Comment] FOREIGN KEY([Comment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryComment_Gallery]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryComment]'))
ALTER TABLE [dbo].[GalleryComment]  WITH CHECK ADD  CONSTRAINT [FK_GalleryComment_Gallery] FOREIGN KEY([Gallery_Id])
REFERENCES [dbo].[Gallery] ([Gallery_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Thread_Comment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Thread]'))
ALTER TABLE [dbo].[Thread]  WITH CHECK ADD  CONSTRAINT [FK_Thread_Comment] FOREIGN KEY([Comment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Thread_Comment_Parent]') AND parent_object_id = OBJECT_ID(N'[dbo].[Thread]'))
ALTER TABLE [dbo].[Thread]  WITH CHECK ADD  CONSTRAINT [FK_Thread_Comment_Parent] FOREIGN KEY([ParentComment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ImageComment_Comment]') AND parent_object_id = OBJECT_ID(N'[dbo].[ImageComment]'))
ALTER TABLE [dbo].[ImageComment]  WITH CHECK ADD  CONSTRAINT [FK_ImageComment_Comment] FOREIGN KEY([Comment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ImageComment_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[ImageComment]'))
ALTER TABLE [dbo].[ImageComment]  WITH CHECK ADD  CONSTRAINT [FK_ImageComment_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EntryImage_Entry]') AND parent_object_id = OBJECT_ID(N'[dbo].[EntryImage]'))
ALTER TABLE [dbo].[EntryImage]  WITH CHECK ADD  CONSTRAINT [FK_EntryImage_Entry] FOREIGN KEY([Entry_Id])
REFERENCES [dbo].[Entry] ([Entry_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EntryImage_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[EntryImage]'))
ALTER TABLE [dbo].[EntryImage]  WITH CHECK ADD  CONSTRAINT [FK_EntryImage_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EntryCounter_Counter]') AND parent_object_id = OBJECT_ID(N'[dbo].[EntryCounter]'))
ALTER TABLE [dbo].[EntryCounter]  WITH CHECK ADD  CONSTRAINT [FK_EntryCounter_Counter] FOREIGN KEY([Counter_Id])
REFERENCES [dbo].[Counter] ([Counter_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EntryCounter_Entry]') AND parent_object_id = OBJECT_ID(N'[dbo].[EntryCounter]'))
ALTER TABLE [dbo].[EntryCounter]  WITH CHECK ADD  CONSTRAINT [FK_EntryCounter_Entry] FOREIGN KEY([Entry_Id])
REFERENCES [dbo].[Entry] ([Entry_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryCounter_Counter]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryCounter]'))
ALTER TABLE [dbo].[GalleryCounter]  WITH CHECK ADD  CONSTRAINT [FK_GalleryCounter_Counter] FOREIGN KEY([Counter_Id])
REFERENCES [dbo].[Counter] ([Counter_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryCounter_Gallery]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryCounter]'))
ALTER TABLE [dbo].[GalleryCounter]  WITH CHECK ADD  CONSTRAINT [FK_GalleryCounter_Gallery] FOREIGN KEY([Gallery_Id])
REFERENCES [dbo].[Gallery] ([Gallery_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryImage_Gallery]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryImage]'))
ALTER TABLE [dbo].[GalleryImage]  WITH CHECK ADD  CONSTRAINT [FK_GalleryImage_Gallery] FOREIGN KEY([Gallery_Id])
REFERENCES [dbo].[Gallery] ([Gallery_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryImage_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryImage]'))
ALTER TABLE [dbo].[GalleryImage]  WITH CHECK ADD  CONSTRAINT [FK_GalleryImage_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryLogin_Gallery]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryLogin]'))
ALTER TABLE [dbo].[GalleryLogin]  WITH CHECK ADD  CONSTRAINT [FK_GalleryLogin_Gallery] FOREIGN KEY([Gallery_Id])
REFERENCES [dbo].[Gallery] ([Gallery_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GalleryLogin_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[GalleryLogin]'))
ALTER TABLE [dbo].[GalleryLogin]  WITH CHECK ADD  CONSTRAINT [FK_GalleryLogin_Login] FOREIGN KEY([Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Request_BrowserVersionPlatform]') AND parent_object_id = OBJECT_ID(N'[dbo].[Request]'))
ALTER TABLE [dbo].[Request]  WITH CHECK ADD  CONSTRAINT [FK_Request_BrowserVersionPlatform] FOREIGN KEY([BrowserVersionPlatform_Id])
REFERENCES [dbo].[BrowserVersionPlatform] ([BrowserVersionPlatform_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RollupBrowserVersionPlatform_BrowserVersionPlatform]') AND parent_object_id = OBJECT_ID(N'[dbo].[RollupBrowserVersionPlatform]'))
ALTER TABLE [dbo].[RollupBrowserVersionPlatform]  WITH CHECK ADD  CONSTRAINT [FK_RollupBrowserVersionPlatform_BrowserVersionPlatform] FOREIGN KEY([BrowserVersionPlatform_Id])
REFERENCES [dbo].[BrowserVersionPlatform] ([BrowserVersionPlatform_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BrowserVersion_Browser]') AND parent_object_id = OBJECT_ID(N'[dbo].[BrowserVersion]'))
ALTER TABLE [dbo].[BrowserVersion]  WITH CHECK ADD  CONSTRAINT [FK_BrowserVersion_Browser] FOREIGN KEY([Browser_Id])
REFERENCES [dbo].[Browser] ([Browser_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BrowserPlatform_Browser]') AND parent_object_id = OBJECT_ID(N'[dbo].[BrowserPlatform]'))
ALTER TABLE [dbo].[BrowserPlatform]  WITH CHECK ADD  CONSTRAINT [FK_BrowserPlatform_Browser] FOREIGN KEY([Browser_Id])
REFERENCES [dbo].[Browser] ([Browser_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BrowserPlatform_Platform]') AND parent_object_id = OBJECT_ID(N'[dbo].[BrowserPlatform]'))
ALTER TABLE [dbo].[BrowserPlatform]  WITH CHECK ADD  CONSTRAINT [FK_BrowserPlatform_Platform] FOREIGN KEY([Platform_Id])
REFERENCES [dbo].[Platform] ([Platform_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NamedCounter_Counter]') AND parent_object_id = OBJECT_ID(N'[dbo].[NamedCounter]'))
ALTER TABLE [dbo].[NamedCounter]  WITH CHECK ADD  CONSTRAINT [FK_NamedCounter_Counter] FOREIGN KEY([Counter_Id])
REFERENCES [dbo].[Counter] ([Counter_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ImageCounter_Counter]') AND parent_object_id = OBJECT_ID(N'[dbo].[ImageCounter]'))
ALTER TABLE [dbo].[ImageCounter]  WITH CHECK ADD  CONSTRAINT [FK_ImageCounter_Counter] FOREIGN KEY([Counter_Id])
REFERENCES [dbo].[Counter] ([Counter_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ImageCounter_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[ImageCounter]'))
ALTER TABLE [dbo].[ImageCounter]  WITH CHECK ADD  CONSTRAINT [FK_ImageCounter_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LoginCounter_Counter]') AND parent_object_id = OBJECT_ID(N'[dbo].[LoginCounter]'))
ALTER TABLE [dbo].[LoginCounter]  WITH CHECK ADD  CONSTRAINT [FK_LoginCounter_Counter] FOREIGN KEY([Counter_Id])
REFERENCES [dbo].[Counter] ([Counter_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LoginCounter_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[LoginCounter]'))
ALTER TABLE [dbo].[LoginCounter]  WITH CHECK ADD  CONSTRAINT [FK_LoginCounter_Login] FOREIGN KEY([Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FeedItem_Feed]') AND parent_object_id = OBJECT_ID(N'[dbo].[FeedItem]'))
ALTER TABLE [dbo].[FeedItem]  WITH CHECK ADD  CONSTRAINT [FK_FeedItem_Feed] FOREIGN KEY([Feed_Id])
REFERENCES [dbo].[Feed] ([Feed_Id])
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Highlight_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[Highlight]'))
ALTER TABLE [dbo].[Highlight]  WITH CHECK ADD  CONSTRAINT [FK_Highlight_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
