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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permalink]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Permalink](
	[Permalink_Id] [int] IDENTITY(1,1) NOT NULL,
	[Source_Id] [int] NOT NULL,
	[Target_Id] [int] NOT NULL,
	[SourceType] [nvarchar](64) NOT NULL,
	[TargetType] [nvarchar](64) NOT NULL,
 CONSTRAINT [PK_Permalink] PRIMARY KEY CLUSTERED 
(
	[Permalink_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Permalink]') AND name = N'IX_Permalink_Source')
CREATE NONCLUSTERED INDEX [IX_Permalink_Source] ON [dbo].[Permalink] 
(
	[SourceType] ASC,
	[Source_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Permalink]') AND name = N'IX_Permalink_Target')
CREATE NONCLUSTERED INDEX [IX_Permalink_Target] ON [dbo].[Permalink] 
(
	[Target_Id] ASC,
	[TargetType] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Permalink]') AND name = N'UK_Permalink')
CREATE UNIQUE NONCLUSTERED INDEX [UK_Permalink] ON [dbo].[Permalink] 
(
	[Source_Id] ASC,
	[Target_Id] ASC,
	[SourceType] ASC,
	[TargetType] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
	[Platform] [nvarchar](128) NOT NULL,
	[Version] [nvarchar](12) NOT NULL,
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Counter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Counter](
	[Counter_Id] [int] IDENTITY(1,1) NOT NULL,
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

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[HourlyCounter]') AND name = N'UK_HourlyCounter')
CREATE UNIQUE NONCLUSTERED INDEX [UK_HourlyCounter] ON [dbo].[HourlyCounter] 
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DailyCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DailyCounter](
	[DailyCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestCount] [int] NOT NULL,
	[DateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_DailyCounter] PRIMARY KEY CLUSTERED 
(
	[DailyCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DailyCounter]') AND name = N'UK_DailyCounter')
CREATE UNIQUE NONCLUSTERED INDEX [UK_DailyCounter] ON [dbo].[DailyCounter] 
(
	[DateTime] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WeeklyCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WeeklyCounter](
	[WeeklyCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestCount] [bigint] NOT NULL,
	[DateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_WeeklyCounter] PRIMARY KEY CLUSTERED 
(
	[WeeklyCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WeeklyCounter]') AND name = N'UK_WeeklyCounter')
CREATE UNIQUE NONCLUSTERED INDEX [UK_WeeklyCounter] ON [dbo].[WeeklyCounter] 
(
	[DateTime] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MonthlyCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MonthlyCounter](
	[MonthlyCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestCount] [bigint] NOT NULL,
	[DateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_MonthlyCounter] PRIMARY KEY CLUSTERED 
(
	[MonthlyCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MonthlyCounter]') AND name = N'UK_MonthlyCounter')
CREATE UNIQUE NONCLUSTERED INDEX [UK_MonthlyCounter] ON [dbo].[MonthlyCounter] 
(
	[DateTime] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[YearlyCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[YearlyCounter](
	[YearlyCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestCount] [bigint] NOT NULL,
	[DateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_YearlyCounter] PRIMARY KEY CLUSTERED 
(
	[YearlyCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[YearlyCounter]') AND name = N'UK_YearlyCounter')
CREATE UNIQUE NONCLUSTERED INDEX [UK_YearlyCounter] ON [dbo].[YearlyCounter] 
(
	[DateTime] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Post]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Post](
	[Post_Id] [int] IDENTITY(1,1) NOT NULL,
	[Topic_Id] [int] NOT NULL,
	[Login_Id] [int] NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[Body] [ntext] NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED 
(
	[Post_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Post]') AND name = N'IX_Post_Created')
CREATE NONCLUSTERED INDEX [IX_Post_Created] ON [dbo].[Post] 
(
	[Created] DESC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PostComment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PostComment](
	[PostComment_Id] [int] IDENTITY(1,1) NOT NULL,
	[Post_Id] [int] NOT NULL,
	[Comment_Id] [int] NOT NULL,
 CONSTRAINT [PK_PostComment] PRIMARY KEY CLUSTERED 
(
	[PostComment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PostComment]') AND name = N'UK_PostComment')
CREATE UNIQUE NONCLUSTERED INDEX [UK_PostComment] ON [dbo].[PostComment] 
(
	[Comment_Id] ASC,
	[Post_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PostCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PostCounter](
	[PostCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Post_Id] [int] NOT NULL,
	[Counter_Id] [int] NOT NULL,
 CONSTRAINT [PK_PostCounter] PRIMARY KEY CLUSTERED 
(
	[PostCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PostCounter]') AND name = N'UK_PostCounter')
CREATE UNIQUE NONCLUSTERED INDEX [UK_PostCounter] ON [dbo].[PostCounter] 
(
	[Counter_Id] ASC,
	[Post_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PostLogin]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PostLogin](
	[PostLogin_Id] [int] IDENTITY(1,1) NOT NULL,
	[Post_Id] [int] NOT NULL,
	[Login_Id] [int] NOT NULL,
 CONSTRAINT [PK_PostLogin] PRIMARY KEY CLUSTERED 
(
	[PostLogin_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PostImage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PostImage](
	[PostImage_Id] [int] IDENTITY(1,1) NOT NULL,
	[Post_Id] [int] NOT NULL,
	[Image_Id] [int] NOT NULL,
 CONSTRAINT [PK_PostImage] PRIMARY KEY CLUSTERED 
(
	[PostImage_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PostImage]') AND name = N'UK_PostImage')
CREATE UNIQUE NONCLUSTERED INDEX [UK_PostImage] ON [dbo].[PostImage] 
(
	[Post_Id] ASC,
	[Image_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BrowserCounter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BrowserCounter](
	[BrowserCounter_Id] [int] IDENTITY(1,1) NOT NULL,
	[Browser_Id] [int] NOT NULL,
	[Counter_Id] [int] NOT NULL,
 CONSTRAINT [PK_BrowserCounter] PRIMARY KEY CLUSTERED 
(
	[BrowserCounter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BrowserCounter]') AND name = N'UK_BrowserCounter')
CREATE UNIQUE NONCLUSTERED INDEX [UK_BrowserCounter] ON [dbo].[BrowserCounter] 
(
	[Browser_Id] ASC,
	[Counter_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
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
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Comment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Comment](
	[Comment_Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [ntext] NOT NULL,
	[IpAddress] [varchar](24) NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Owner_Login_Id] [int] NULL,
 CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED 
(
	[Comment_Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Post_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[Post]'))
ALTER TABLE [dbo].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_Login] FOREIGN KEY([Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Post_Topic]') AND parent_object_id = OBJECT_ID(N'[dbo].[Post]'))
ALTER TABLE [dbo].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_Topic] FOREIGN KEY([Topic_Id])
REFERENCES [dbo].[Topic] ([Topic_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ImageComment_Comment]') AND parent_object_id = OBJECT_ID(N'[dbo].[ImageComment]'))
ALTER TABLE [dbo].[ImageComment]  WITH CHECK ADD  CONSTRAINT [FK_ImageComment_Comment] FOREIGN KEY([Comment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ImageComment_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[ImageComment]'))
ALTER TABLE [dbo].[ImageComment]  WITH CHECK ADD  CONSTRAINT [FK_ImageComment_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostComment_Comment]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostComment]'))
ALTER TABLE [dbo].[PostComment]  WITH CHECK ADD  CONSTRAINT [FK_PostComment_Comment] FOREIGN KEY([Comment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostComment_Post]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostComment]'))
ALTER TABLE [dbo].[PostComment]  WITH CHECK ADD  CONSTRAINT [FK_PostComment_Post] FOREIGN KEY([Post_Id])
REFERENCES [dbo].[Post] ([Post_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Thread_Comment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Thread]'))
ALTER TABLE [dbo].[Thread]  WITH CHECK ADD  CONSTRAINT [FK_Thread_Comment] FOREIGN KEY([Comment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Thread_Comment_Parent]') AND parent_object_id = OBJECT_ID(N'[dbo].[Thread]'))
ALTER TABLE [dbo].[Thread]  WITH CHECK ADD  CONSTRAINT [FK_Thread_Comment_Parent] FOREIGN KEY([ParentComment_Id])
REFERENCES [dbo].[Comment] ([Comment_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostCounter_Post]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostCounter]'))
ALTER TABLE [dbo].[PostCounter]  WITH CHECK ADD  CONSTRAINT [FK_PostCounter_Post] FOREIGN KEY([Post_Id])
REFERENCES [dbo].[Post] ([Post_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostCounter_PostCounter]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostCounter]'))
ALTER TABLE [dbo].[PostCounter]  WITH CHECK ADD  CONSTRAINT [FK_PostCounter_PostCounter] FOREIGN KEY([Counter_Id])
REFERENCES [dbo].[Counter] ([Counter_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostLogin_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostLogin]'))
ALTER TABLE [dbo].[PostLogin]  WITH CHECK ADD  CONSTRAINT [FK_PostLogin_Login] FOREIGN KEY([Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostLogin_Post]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostLogin]'))
ALTER TABLE [dbo].[PostLogin]  WITH CHECK ADD  CONSTRAINT [FK_PostLogin_Post] FOREIGN KEY([Post_Id])
REFERENCES [dbo].[Post] ([Post_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostImage_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostImage]'))
ALTER TABLE [dbo].[PostImage]  WITH CHECK ADD  CONSTRAINT [FK_PostImage_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostImage_Post]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostImage]'))
ALTER TABLE [dbo].[PostImage]  WITH CHECK ADD  CONSTRAINT [FK_PostImage_Post] FOREIGN KEY([Post_Id])
REFERENCES [dbo].[Post] ([Post_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BrowserCounter_Browser]') AND parent_object_id = OBJECT_ID(N'[dbo].[BrowserCounter]'))
ALTER TABLE [dbo].[BrowserCounter]  WITH CHECK ADD  CONSTRAINT [FK_BrowserCounter_Browser] FOREIGN KEY([Browser_Id])
REFERENCES [dbo].[Browser] ([Browser_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BrowserCounter_Counter]') AND parent_object_id = OBJECT_ID(N'[dbo].[BrowserCounter]'))
ALTER TABLE [dbo].[BrowserCounter]  WITH CHECK ADD  CONSTRAINT [FK_BrowserCounter_Counter] FOREIGN KEY([Counter_Id])
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
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NamedCounter_Counter]') AND parent_object_id = OBJECT_ID(N'[dbo].[NamedCounter]'))
ALTER TABLE [dbo].[NamedCounter]  WITH CHECK ADD  CONSTRAINT [FK_NamedCounter_Counter] FOREIGN KEY([Counter_Id])
REFERENCES [dbo].[Counter] ([Counter_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FeedItem_Feed]') AND parent_object_id = OBJECT_ID(N'[dbo].[FeedItem]'))
ALTER TABLE [dbo].[FeedItem]  WITH CHECK ADD  CONSTRAINT [FK_FeedItem_Feed] FOREIGN KEY([Feed_Id])
REFERENCES [dbo].[Feed] ([Feed_Id])
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Highlight_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[Highlight]'))
ALTER TABLE [dbo].[Highlight]  WITH CHECK ADD  CONSTRAINT [FK_Highlight_Image] FOREIGN KEY([Image_Id])
REFERENCES [dbo].[Image] ([Image_Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Comment_Login]') AND parent_object_id = OBJECT_ID(N'[dbo].[Comment]'))
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_Login] FOREIGN KEY([Owner_Login_Id])
REFERENCES [dbo].[Login] ([Login_Id])
