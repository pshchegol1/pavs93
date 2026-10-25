/* Apply once to an existing VC3EYE SQL Server database before deploying the new monitor-type UI. */
IF COL_LENGTH('dbo.Services', 'MonitorType') IS NULL
    ALTER TABLE dbo.Services ADD MonitorType int NOT NULL CONSTRAINT DF_Services_MonitorType DEFAULT (0);

IF COL_LENGTH('dbo.Services', 'ExpectedStatusCode') IS NULL
    ALTER TABLE dbo.Services ADD ExpectedStatusCode int NULL;

IF COL_LENGTH('dbo.Services', 'ApiHttpMethod') IS NULL
    ALTER TABLE dbo.Services ADD ApiHttpMethod nvarchar(10) NOT NULL CONSTRAINT DF_Services_ApiHttpMethod DEFAULT (N'GET');

IF COL_LENGTH('dbo.Services', 'ApiExpectedContent') IS NULL
    ALTER TABLE dbo.Services ADD ApiExpectedContent nvarchar(200) NULL;

IF COL_LENGTH('dbo.Services', 'ApiRequestBody') IS NULL
    ALTER TABLE dbo.Services ADD ApiRequestBody nvarchar(1000) NULL;

IF COL_LENGTH('dbo.Services', 'SslExpiryWarningDays') IS NULL
    ALTER TABLE dbo.Services ADD SslExpiryWarningDays int NOT NULL CONSTRAINT DF_Services_SslExpiryWarningDays DEFAULT (14);
GO

/* Preserve the prior TCP fallback for legacy IP-only services that had a configured port. */
UPDATE dbo.Services SET MonitorType = 1
WHERE MonitorType = 0 AND (URL IS NULL OR LTRIM(RTRIM(URL)) = N'')
  AND IPAddress IS NOT NULL AND LTRIM(RTRIM(IPAddress)) <> N'' AND Port IS NOT NULL;
GO

