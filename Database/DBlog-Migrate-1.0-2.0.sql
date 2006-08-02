-- Iterate through entries
DECLARE EntryIterator Cursor
FOR SELECT Entry_Id FROM Entry ORDER BY [Created] ASC

OPEN EntryIterator
DECLARE @entry_id int

FETCH NEXT FROM EntryIterator INTO @entry_id
WHILE (@@FETCH_STATUS <> -1)
BEGIN

 -- copy Entry -> Post
 DECLARE @entry_post_id int
 INSERT INTO Post ( Topic_Id, Login_Id, Title, Body, Created, Modified )
 SELECT Topic_Id, Owner_Login_Id, Title, [Text]
	, DATEADD(hh, DATEDIFF(hh, getdate(), getutcdate()), Created)
	, DATEADD(hh, DATEDIFF(hh, getdate(), getutcdate()), Modified)
 FROM Entry
 WHERE Entry_Id = @entry_id
 SET @entry_post_id = SCOPE_IDENTITY()

 -- insert Permalink
 INSERT INTO Permalink ( Source_Id, Target_Id, SourceType, TargetType)
 VALUES ( @entry_id, @entry_post_id, 'Entry', 'Post' )

 -- copy EntryComment -> PostComment
 INSERT INTO PostComment ( Post_Id, Comment_Id )
 SELECT @entry_post_id, Comment_Id FROM EntryComment WHERE Entry_Id = @entry_id

 -- copy EntryCounter -> PostCounter
 INSERT INTO PostCounter ( Post_Id, Counter_Id )
 SELECT @entry_post_id, Counter_Id FROM EntryCounter WHERE Entry_Id = @entry_id

 -- copy EntryImage -> PostImage
 INSERT INTO PostImage ( Post_Id, Image_Id )
 SELECT @entry_post_id, Image_Id FROM EntryImage WHERE Entry_Id = @entry_id

 FETCH NEXT FROM EntryIterator INTO @entry_id
END

CLOSE EntryIterator
DEALLOCATE EntryIterator

-- Drop entry tables
DROP TABLE EntryImage
DROP TABLE EntryCounter
DROP TABLE EntryComment
DROP TABLE Entry

-- Iterate through galleries
DECLARE GalleryIterator Cursor
FOR SELECT Gallery_Id, [Path] FROM Gallery ORDER BY [Created] ASC

OPEN GalleryIterator
DECLARE @gallery_id int
DECLARE @gallery_path varchar(128)

FETCH NEXT FROM GalleryIterator INTO @gallery_id, @gallery_path
WHILE (@@FETCH_STATUS <> -1)
BEGIN

 -- copy Gallery -> Post
 DECLARE @gallery_post_id int
 INSERT INTO Post ( Topic_Id, Login_Id, Title, Body, Created, Modified )
 SELECT Topic_Id, Owner_Login_Id, Title, [Text]
	, DATEADD(hh, DATEDIFF(hh, getdate(), getutcdate()), Created)
	, DATEADD(hh, DATEDIFF(hh, getdate(), getutcdate()), Modified) 
 FROM Gallery
 WHERE Gallery_Id = @gallery_id
 SET @gallery_post_id = SCOPE_IDENTITY()

 -- insert Permalink
 INSERT INTO Permalink ( Source_Id, Target_Id, SourceType, TargetType)
 VALUES ( @gallery_id, @gallery_post_id, 'Gallery', 'Post' )

 -- copy GalleryComment -> PostComment
 INSERT INTO PostComment ( Post_Id, Comment_Id )
 SELECT @gallery_post_id, Comment_Id FROM GalleryComment WHERE Gallery_Id = @gallery_id

 -- copy GalleryCounter -> PostCounter
 INSERT INTO PostCounter ( Post_Id, Counter_Id )
 SELECT @gallery_post_id, Counter_Id FROM GalleryCounter WHERE Gallery_Id = @gallery_id

 -- copy GalleryImage -> PostImage
 INSERT INTO PostImage ( Post_Id, Image_Id )
 SELECT @gallery_post_id, Image_Id FROM GalleryImage WHERE Gallery_Id = @gallery_id

 -- copy GalleryLogin -> PostLogin
 INSERT INTO PostLogin ( Post_Id, Login_Id )
 SELECT @gallery_post_id, Login_Id FROM GalleryLogin WHERE Gallery_Id = @gallery_id

 -- update relative image paths
 UPDATE [Image] SET [Path] = @gallery_path WHERE 
 Image_Id IN ( SELECT Image_Id FROM GalleryImage WHERE Gallery_Id = @gallery_id )

 FETCH NEXT FROM GalleryIterator INTO @gallery_id, @gallery_path
END

CLOSE GalleryIterator
DEALLOCATE GalleryIterator

-- Drop Gallery tables
DROP TABLE GalleryImage
DROP TABLE GalleryCounter
DROP TABLE GalleryLogin
DROP TABLE GalleryComment
DROP TABLE Gallery

-- Drop the Blog view
DROP VIEW Blog

-- Alter Comment to allow NULL in Owner_Login_Id
ALTER TABLE Comment ALTER COLUMN Owner_Login_Id int NULL

-- Modify Counters view
DROP VIEW Counters
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[Counters] AS
(SELECT ''Post'' AS Type, dbo.Counter.Counter_Id, dbo.Counter.Resource_Id, dbo.Counter.Count, dbo.Counter.Created
FROM dbo.Counter INNER JOIN
 dbo.PostCounter ON dbo.PostCounter.Counter_Id = dbo.Counter.Counter_Id
UNION ALL
 SELECT ''Image'' AS Type, Counter_1.Counter_Id, Counter_1.Resource_Id, Counter_1.Count, Counter_1.Created
 FROM dbo.Counter AS Counter_1 INNER JOIN
 dbo.ImageCounter ON dbo.ImageCounter.Counter_Id = Counter_1.Counter_Id
)'
GO

-- Modify the Comments view
DROP VIEW Comments
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[Comments] AS
(SELECT
 dbo.Comment.Comment_Id, dbo.Comment.Text, dbo.Comment.IpAddress, dbo.Comment.Modified, dbo.Comment.Created, 
 dbo.Comment.Owner_Login_Id, ''Post'' AS Type, dbo.PostComment.Post_Id AS Parent_Id
FROM dbo.Comment INNER JOIN
 dbo.PostComment ON dbo.Comment.Comment_Id = dbo.PostComment.Comment_Id
UNION ALL
SELECT Comment_1.Comment_Id, Comment_1.Text, Comment_1.IpAddress, Comment_1.Modified, Comment_1.Created, Comment_1.Owner_Login_Id, 
 ''Image'' AS Type, dbo.ImageComment.Image_Id AS Parent_Id
FROM dbo.Comment AS Comment_1 INNER JOIN
 dbo.ImageComment ON Comment_1.Comment_Id = dbo.ImageComment.Comment_Id
)' 
GO

-- Drop the counter Resource column
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Counter]') AND [name] = 'Resource_Id')
ALTER TABLE Counter DROP COLUMN Resource_Id

-- Drop sp_counter procedures
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_counter_increment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [sp_counter_increment]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounter_increment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [sp_hourlycounter_increment]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_namedcounter_increment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [sp_namedcounter_increment]

-- Update feeds with valid save/update date
UPDATE Feed SET Saved = getutcdate() WHERE Saved IS NULL
UPDATE Feed SET Updated = getutcdate() WHERE Updated IS NULL

-- Merge counters
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_daily]') AND type in (N'P', N'PC'))
BEGIN
 CREATE TABLE #tmp_daily ( [Id] int, [c] bigint, [ts] DateTime )
 insert into #tmp_daily exec sp_hourlycounters_daily
 insert into DailyCounter select c, ts from #tmp_daily
 DROP PROCEDURE [sp_hourlycounters_daily]
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_weekly]') AND type in (N'P', N'PC'))
BEGIN
 CREATE TABLE #tmp_weekly ( [Id] int, [c] bigint, [ts] DateTime )
 insert into #tmp_weekly exec sp_hourlycounters_weekly
 insert into WeeklyCounter select c, ts from #tmp_weekly
 DROP PROCEDURE [sp_hourlycounters_weekly]
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_monthly]') AND type in (N'P', N'PC'))
BEGIN
 CREATE TABLE #tmp_monthly ( [Id] int, [c] bigint, [ts] DateTime )
 insert into #tmp_monthly exec sp_hourlycounters_monthly
 insert into MonthlyCounter select c, ts from #tmp_monthly
 DROP PROCEDURE [sp_hourlycounters_monthly]
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_yearly]') AND type in (N'P', N'PC'))
BEGIN
 CREATE TABLE #tmp_yearly ( [Id] int, [c] bigint, [ts] DateTime )
 insert into #tmp_yearly exec sp_hourlycounters_yearly
 insert into YearlyCounter select c, ts from #tmp_yearly
 DROP PROCEDURE [sp_hourlycounters_yearly]
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_hourlycounters_hourly]') AND type in (N'P', N'PC'))
BEGIN
 DROP PROCEDURE [sp_hourlycounters_hourly]
END

-- Drop named counters
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_namedconter_increment]') AND type in (N'P', N'PC'))
BEGIN
 DROP PROCEDURE [sp_namedconter_increment]
END

-- Update browser table and counter
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_rollup_browserversionplatform]') AND type in (N'P', N'PC'))
BEGIN

EXEC [sp_rollup_browserversionplatform]

DROP PROCEDURE [sp_rollup_browserversionplatform]
DROP PROCEDURE [sp_browser_create]
DROP PROCEDURE [sp_browserplatform_create]
DROP PROCEDURE [sp_browserversion_create]
DROP PROCEDURE [sp_browserversionplatform_create]
DROP PROCEDURE [sp_platform_create]
DROP PROCEDURE [sp_request_create]

CREATE TABLE #tmp_Browser ( 
   [Id] int
 , [Name] nvarchar(128) 
 , [Platform] nvarchar(128)
 , [Version] nvarchar(12)
 , [RequestCount] bigint
)

INSERT INTO #tmp_Browser ( 
   [Id]
 , [Name]
 , [Platform]
 , [Version]
 , [RequestCount] )
SELECT 
   BrowserVersionPlatform.BrowserVersionPlatform_Id
 , Browser.[Name]
 , Platform.[Name]
 , RTRIM(LTRIM(STR(BrowserVersion.Major))) + '.' + RTRIM(LTRIM(STR(BrowserVersion.Minor)))
 , RollupBrowserVersionPlatform.RequestCount
FROM 
   Browser
 , BrowserPlatform
 , Platform
 , BrowserVersion
 , BrowserVersionPlatform
 , RollupBrowserVersionPlatform
WHERE BrowserPlatform.Browser_Id = Browser.Browser_Id
AND Platform.Platform_Id = BrowserPlatform.Platform_Id
AND BrowserVersion.Browser_Id = Browser.Browser_Id
AND BrowserVersionPlatform.BrowserPlatform_Id = BrowserPlatform.BrowserPlatform_Id
AND BrowserVersionPlatform.BrowserVersion_Id = BrowserVersion.BrowserVersion_Id
AND RollupBrowserVersionPlatform.BrowserVersionPlatform_Id = BrowserVersionPlatform.BrowserVersionPlatform_Id

DROP TABLE Request
DROP TABLE RollupBrowserVersionPlatform
DROP TABLE BrowserVersionPlatform
DROP TABLE BrowserVersion
DROP TABLE BrowserPlatform
DROP TABLE Platform
DELETE Browser

ALTER TABLE [Browser] DROP CONSTRAINT DF_Browser_Crawler
ALTER TABLE [Browser] DROP COLUMN [Crawler]
ALTER TABLE [Browser] ADD [Platform] nvarchar(128) NOT NULL
ALTER TABLE [Browser] ADD [Version] nvarchar(12) NOT NULL

DECLARE tmp_BrowserIterator Cursor
FOR SELECT Id, [Name], Platform, Version, RequestCount FROM #tmp_Browser
OPEN tmp_BrowserIterator
DECLARE @browser_name nvarchar(128)
DECLARE @browser_platform nvarchar(128)
DECLARE @browser_version nvarchar(12)
DECLARE @browser_rc bigint
DECLARE @browser_id int
DECLARE @counter_id int

DECLARE @counter_created_oldest DATETIME
SELECT @counter_created_oldest = MIN(Created) FROM Counter

FETCH NEXT FROM tmp_BrowserIterator INTO @browser_id, @browser_name, @browser_platform, @browser_version, @browser_rc
WHILE (@@FETCH_STATUS <> -1)
BEGIN
 INSERT INTO Browser ( [Name], [Platform], [Version] )
 VALUES ( @browser_name, @browser_platform, @browser_version )
 
 SET @browser_id = SCOPE_IDENTITY()

 INSERT INTO Counter ( [Count], [Created] ) VALUES ( @browser_rc, @counter_created_oldest )
 SET @counter_id = SCOPE_IDENTITY()

 INSERT INTO BrowserCounter ( [Browser_Id], [Counter_Id] ) VALUES ( @browser_id, @counter_id )
 
 FETCH NEXT FROM tmp_BrowserIterator INTO @browser_id, @browser_name, @browser_platform, @browser_version, @browser_rc
END
DEALLOCATE tmp_BrowserIterator
