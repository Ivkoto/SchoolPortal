
--INSERT INTO [Applications].[Sources]
--	(	[Name]							)	VALUES
--	(	N'Site-1.com'					),	-- 1
--	(	N'DirectoryTree-2/File-2.xlsx'	),	-- 2
--	(	N'DirectoryTree-3/File-3.xlsx'	);	-- 3
--GO;

--INSERT INTO [Applications].[ValidityPeriods]
--	(	[From],							[To]							)	VALUES	-- 9999-12-31 23:59:59.9999999
--	(	N'2021-09-15 00:00:00.0000000',	N'2022-09-14 23:59:59.9999999'	);	-- 1
--GO;

--INSERT INTO [Applications].[Synonyms]
--	(	[Exact],		[Simple],		[SourceId]	)	VALUES
--	(	N'Действаща',	N'действаща',	1			),	-- 1
--	(	N'Закрита',		N'закрита',		2			),	-- 2
--	(	N'СМГ',			N'смг',			2			);	-- 3
--GO;

--INSERT INTO [Applications].[InstitutionStatuses]
--	(	[SynonymId],	[SourceId]	)	VALUES
--	(	1,				1			),	-- 1
--	(	2,				1			);	-- 2
--GO;

--INSERT INTO [Applications].[Institutions]
--	(	[SelfId],	[IsClosed],	[StatusId],	[ShortSynonymId],	[ValidityPeriodId],	[SourceId]	)	VALUES
--	(	1234567,	0,			1,			3,					1,					1			);	-- 1
--GO;

--INSERT INTO [Applications].[Synonyms]
--	(	[Exact],	[Simple],	[SourceId]	)	VALUES
--	(	N'СеМеГе',	N'семеге',	1			);	-- 4
--GO;

--DECLARE @InsertedSynonymId INT = SCOPE_IDENTITY();

--UPDATE [Applications].[Synonyms]
--SET
--	[BaseId] = @InsertedSynonymId
--WHERE
--	Id = 3 OR
--	Id = @InsertedSynonymId
--GO;

--UPDATE [Applications].[Institutions]
--SET
--	[IsClosed] = 1,
--	[StatusId] = 2,
--	[ShortSynonymId] = 4,
--	[SourceId] = 3
--WHERE
--	Id = 1;
--GO;
