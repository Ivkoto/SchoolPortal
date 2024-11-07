ALTER TABLE [Application].[ProfileDetails]
ADD [IsClosed] BIT NOT NULL DEFAULT 0;
GO

ALTER TABLE [Application].[ExamStage]
ADD [IsClosed] BIT NOT NULL DEFAULT 0;
GO