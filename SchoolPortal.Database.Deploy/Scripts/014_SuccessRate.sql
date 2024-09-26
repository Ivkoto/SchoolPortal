
CREATE TABLE [Application].[SuccessRate]
(
    [Id] INT NOT NULL IDENTITY (1, 1),

    [ExamAbbreviationId] INT,
    [Grade] INT,
    [SchoolYearId] INT,
    [Area] NVARCHAR(30),
    [SubjectAbbreviationId] INT,
    [PreparationType] NVARCHAR(50),
    [LevelType] NVARCHAR(50),

    [AreSeparated] BIT DEFAULT 0,

	[FromInclusive] DECIMAL (6, 3),
	[ToInclusive] DECIMAL (6, 3),

    [WorseTotalCount] INT DEFAULT 0,
    [SimilarTotalCount] INT DEFAULT 0,
    [BetterTotalCount] INT DEFAULT 0,

    [WorseMenCount] INT DEFAULT 0,
    [SimilarMenCount] INT DEFAULT 0,
    [BetterMenCount] INT DEFAULT 0,

    [WorseWomenCount] INT DEFAULT 0,
    [SimilarWomenCount] INT DEFAULT 0,
    [BetterWomenCount] INT DEFAULT 0,

	CONSTRAINT [PK_SuccessRate_Id] PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_SuccessRate_ExamAbbreviationId] FOREIGN KEY ([ExamAbbreviationId]) REFERENCES [Application].[ExamAbbreviation] (Id),
    CONSTRAINT [FK_SuccessRate_SchoolYearId] FOREIGN KEY ([SchoolYearId]) REFERENCES [Application].[SchoolYear] ([Id]),
    CONSTRAINT [FK_SuccessRate_SubjectAbbreviationId] FOREIGN KEY ([SubjectAbbreviationId]) REFERENCES [Application].[SubjectAbbreviation] (Id)
);
GO
