-- Move the ExternalId column to the ProfileDetails table, as the ExternalId may vary each year.
ALTER TABLE [Application].[ProfileDetails]
ADD ExternalId int;
GO

ALTER TABLE [Application].[Profile]
DROP CONSTRAINT [UQ_Profile_ExternalId];
GO

ALTER TABLE [Application].[Profile]
DROP COLUMN [ExternalId]
GO

--Mode Admission by quotas in profile details table
ALTER TABLE [Application].[ProfileDetails]
ADD Quotas_Total int, Quotas_Male int, Quotas_Female int;
GO

ALTER TABLE [Application].[ProfileDetails]
DROP CONSTRAINT [FK_ProfileDetails_AdmissionByQuotasId];
GO

ALTER TABLE [Application].[ProfileDetails]
DROP COLUMN [AdmissionByQuotasId];
GO

DROP TABLE [Application].[AdmissionByQuotas];
GO