CREATE OR ALTER PROC [Application].[usp_GetExamStageScoresByProfileId]
	@ProfileId	INT,
	@SchoolYear INT
AS
BEGIN
    SET NOCOUNT ON;

	SELECT
		[StageId],
		[StageNumber],
		[FreePositionsTotal],
		[FreePositionsMen],
		[FreePositionsWomen],
		[IsAggregatedScore],
		[ProfileId],
		[ProfileName],
		[SchoolYear],
		[MinTotalScore],
		[MinMaleScore],
		[MinFemaleScore],
		[MaxTotalScore],
		[MaxMaleScore],
		[MaxFemaleScore]
	FROM
		[Application].[uv_ProfileExamStagesScores]
	WHERE
		[ProfileId] = @ProfileId AND [SchoolYear] = @SchoolYear
END;
GO
