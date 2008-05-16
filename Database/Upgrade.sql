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
