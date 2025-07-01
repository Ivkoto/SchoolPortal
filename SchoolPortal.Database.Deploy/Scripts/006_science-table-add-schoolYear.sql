ALTER TABLE [Application].[Science]
ADD [SchoolYearId] int null;
GO

ALTER TABLE [Application].[Science]
ADD CONSTRAINT [FK_Science_SchoolYearId] FOREIGN KEY ([SchoolYearId]) REFERENCES [Application].[SchoolYear] ([Id]);
GO