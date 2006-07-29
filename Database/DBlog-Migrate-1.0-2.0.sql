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
DECLARE @gallery_path nvarchar

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