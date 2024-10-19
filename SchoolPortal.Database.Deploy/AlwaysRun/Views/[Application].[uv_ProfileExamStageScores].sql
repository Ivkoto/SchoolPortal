CREATE OR ALTER VIEW [Application].[uv_ProfileExamStageScores] AS
SELECT 
    stage.[Id]			AS StageId,
    stage.[StageNumber],
    stage.[FreePositionsTotal],
    stage.[FreePositionsMen],
    stage.[FreePositionsWomen],
    stage.[IsAggregatedScore],
    stage.[ProfileId],
	prof.[Name]			AS ProfileName,
    y.[Year]			AS SchoolYear,
    
    minScores.[Total]	AS MinTotalScore,
    minScores.[Male]	AS MinMaleScore,
    minScores.[Female]	AS MinFemaleScore,
    
    maxScores.[Total]	AS MaxTotalScore,
    maxScores.[Male]	AS MaxMaleScore,
    maxScores.[Female]	AS MaxFemaleScore
    
FROM 
    [Application].[ExamStage]	AS stage
JOIN 
    [Application].[ExamScores]	AS minScores ON stage.MinScoresId = minScores.Id
JOIN 
    [Application].[ExamScores]	AS maxScores ON stage.MaxScoresId = maxScores.Id
JOIN
	[Application].[SchoolYear]	AS y ON stage.SchoolYearId = y.Id
JOIN
	[Application].[Profile]		AS prof ON stage.ProfileId = prof.Id;
GO
