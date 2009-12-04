-- 05/15/2008: add created and updated fields to ReferrerHost
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerHost]') AND name = N'Created')
ALTER TABLE dbo.ReferrerHost ADD [Created] datetime NULL
GO
UPDATE dbo.ReferrerHost SET Created = DATEADD(week, -2, getutcdate()) WHERE Created IS NULL
ALTER TABLE dbo.ReferrerHost ALTER COLUMN [Created] datetime NOT NULL
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ReferrerHost]') AND name = N'Updated')
ALTER TABLE dbo.ReferrerHost ADD [Updated] datetime NULL
GO
UPDATE dbo.ReferrerHost SET Updated = DATEADD(week, -2, getutcdate()) WHERE Updated IS NULL
ALTER TABLE dbo.ReferrerHost ALTER COLUMN [Updated] datetime NOT NULL
GO
-- 09/07/2008: add publish (vs. draft) to posts
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Post]') AND name = N'Publish')
ALTER TABLE dbo.Post ADD [Publish] bit NOT NULL DEFAULT 1
GO
-- 10/25/2008: add display (vs. hide) to posts
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Post]') AND name = N'Display')
ALTER TABLE dbo.Post ADD [Display] bit NOT NULL DEFAULT 1
GO
-- 12/01/2008: delete unattached comments
DELETE Comment
WHERE NOT EXISTS ( SELECT * FROM ImageComment WHERE ImageComment.Comment_Id = Comment.Comment_Id )
AND NOT EXISTS ( SELECT * FROM PostComment where PostComment.Comment_Id = Comment.Comment_Id )
-- 12/29/2008: add sticky to posts
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Post]') AND name = N'Sticky')
ALTER TABLE dbo.Post ADD [Sticky] bit NOT NULL DEFAULT 0
GO
-- 04/19/2009: export to album
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Post]') AND name = N'Export')
ALTER TABLE dbo.Post ADD [Export] bit NOT NULL DEFAULT 0
GO
-- 03/12/2009: posts can have multiple topics
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Post]') AND name = N'Topic_Id')
INSERT INTO PostTopic ( Post_Id, Topic_Id) 
SELECT Post_Id, Topic_Id FROM Post
ALTER TABLE dbo.Post DROP CONSTRAINT FK_Post_Topic
ALTER TABLE dbo.Post DROP COLUMN Topic_Id 
GO
