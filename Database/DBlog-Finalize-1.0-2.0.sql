-- Delete duplicate browser entries

DELETE BrowserCounter WHERE Browser_Id IN 
(
 SELECT b2.Browser_Id from Browser b1, Browser b2 
 WHERE b1.Name = b2.Name 
 AND b1.Platform = b2.Platform 
 AND b1.Version = b2.Version
 AND b1.Browser_Id != b2.Browser_Id
)

DELETE Browser WHERE Browser_Id IN 
(
 SELECT b2.Browser_Id from Browser b1, Browser b2 
 WHERE b1.Name = b2.Name 
 AND b1.Platform = b2.Platform 
 AND b1.Version = b2.Version
 AND b1.Browser_Id != b2.Browser_Id
)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Browser]') AND name = N'UK_Browser')
CREATE UNIQUE NONCLUSTERED INDEX [UK_Browser] ON [dbo].[Browser] 
(
	[Name] ASC,
	[Platform] ASC,
	[Version] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
