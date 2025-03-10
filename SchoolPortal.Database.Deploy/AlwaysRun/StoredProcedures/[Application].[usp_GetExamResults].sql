
IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = 'SchoolYearsList' AND schema_id = SCHEMA_ID('Application'))
BEGIN
    CREATE TYPE [Application].[SchoolYearsList] AS TABLE
    (
        [Year] INT NOT NULL
    );
END
GO


CREATE OR ALTER PROC [Application].[usp_GetExamResults]
    @InstitutionId	INT,
    @SchoolYears	[Application].[SchoolYearsList] READONLY,
    @Grade			INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		[SchoolYear],
		[InstitutionId],
		[ExamAbbreviation],
		[SubjectName],
		[SubjectAbbreviation],
		[PreparationType],
		[CandidateCount],
		[AverageScore],
		[Grade],
		[LevelType]
	FROM
		[Application].[uv_ExamResults]
	WHERE
		[InstitutionId] = @InstitutionId AND
		[SchoolYear] IN (SELECT [Year] FROM @SchoolYears) AND
		[Grade] = @Grade
END;
GO
