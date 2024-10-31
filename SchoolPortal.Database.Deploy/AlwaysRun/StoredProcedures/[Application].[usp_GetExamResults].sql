
CREATE OR ALTER PROC [Application].[usp_GetExamResults]
    @InstitutionId	INT,
    @SchoolYear		INT,
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
		[SchoolYear] = @SchoolYear AND
		[Grade] = @Grade
END;
GO
