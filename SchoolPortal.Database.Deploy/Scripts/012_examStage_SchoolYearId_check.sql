IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE Name = N'SchoolYearId' 
    AND Object_ID = Object_ID(N'[Application].[ExamStage]')
)
BEGIN
    ALTER TABLE [Application].[ExamStage]
    ADD [SchoolYearId] INT NULL;
    
    ALTER TABLE [Application].[ExamStage]
    ADD CONSTRAINT FK_ExamStage_SchoolYear 
    FOREIGN KEY (SchoolYearId) 
    REFERENCES [Application].[SchoolYear](Id);
END
GO
