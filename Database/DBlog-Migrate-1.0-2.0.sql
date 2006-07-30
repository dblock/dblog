-- Iterate through entries
DECLARE EntryIterator Cursor
FOR SELECT Entry_Id FROM Entry ORDER BY [Created] ASC

OPEN EntryIterator
DECLARE @entry_id int

FETCH NEXT FROM EntryIterator INTO @entry_id
While (@@FETCH_STATUS <> -1)
BEGIN

 -- copy Entry -> Post
 DECLARE @entry_post_id int
 INSERT INTO Post ( Topic_Id, Login_Id, Title, Body, Created, Modified )
 SELECT Topic_Id, Owner_Login_Id, Title, [Text], Created, Modified FROM Entry
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
While (@@FETCH_STATUS <> -1)
BEGIN

 -- copy Gallery -> Post
 DECLARE @gallery_post_id int
 INSERT INTO Post ( Topic_Id, Login_Id, Title, Body, Created, Modified )
 SELECT Topic_Id, Owner_Login_Id, Title, [Text], Created, Modified FROM Gallery
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
