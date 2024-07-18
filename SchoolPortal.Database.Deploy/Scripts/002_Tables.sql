CREATE TABLE [Application].[SchoolYear]
(
    --[Id] INT IDENTITY PRIMARY KEY CLUSTERED,
	[Id] INT NOT NULL IDENTITY (1, 1),
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NOT NULL,

	CONSTRAINT [PK_Applications_SchoolYear_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[Address] --(квартал "К",) улица "У", номер Н, ...
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Area] NVARCHAR(100),
    [Municipality] NVARCHAR(100),
    [Settlement] NVARCHAR(100),
    [Region] NVARCHAR(100), --Банкя,Витоша,Възраждане,..
    [Address] NVARCHAR(1000), -- (Седалище и управление)
    [PostalCode] INT, --(Дейност)
	[Latitude] DECIMAL(17,15),
	[Longitude] DECIMAL(18,15),

	CONSTRAINT [PK_Address_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[InstitutionPreparationType]
(
    [Id] INT NOT NULL IDENTITY(1, 1),
    [PreparationType] NVARCHAR(300) NOT NULL, -- Вид по чл. 37 (общ, според вида на подготовката) "неспециализирано","общообразователно","професионално"..?

	CONSTRAINT [PK_InstitutionPreparationType_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[SubjectAbbreviation]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[FullName] NVARCHAR(200),
	[ShortName] NVARCHAR(10),

	CONSTRAINT [PK_SubjectAbbreviation_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[AdmissionByQuotas]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [TotalBasis] INT,
	[Male] INT,
	[Female] INT,

	CONSTRAINT [PK_AdmissionByQuotas_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[Science]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[ExternalId] INT NOT NULL,
    [Name] NVARCHAR(300), --Изкуства, Хуманитарни науки, Информатика...

	CONSTRAINT [PK_Science_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_Science_ExternalId] UNIQUE ([ExternalId])
);
GO

CREATE TABLE [Application].[ExamScores]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[Total] DECIMAL(5,2),
	[Male] DECIMAL(5,2),
	[Female] DECIMAL (5,2),

	CONSTRAINT [PK_ExamScores_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[ExamAbbreviation]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[FullName] NVARCHAR(300),
	[Abbreviation] NVARCHAR(100),

	CONSTRAINT [PK_ExamAbbreviation_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[InstitutionFinancingType]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[FinancingType] NVARCHAR(300),

	CONSTRAINT [PK_InstitutionFinancingType_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[InstitutionOwnership]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[Ownership] NVARCHAR(300),

	CONSTRAINT [PK_InstitutionOwnership_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[InstitutionStatus]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[Status] NVARCHAR(300),

	CONSTRAINT [PK_InstitutionStatus_Id] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [Application].[Institution]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [ExternalId] INT NOT NULL,
    [IsActive] BIT DEFAULT 1, --true/false
    [EIK] NVARCHAR (15), -- 000012465 / BG000012465
    [FullName] NVARCHAR(300), -- Софийска математическа гимназия "Паисий Хилендарски"
    [ShortName] NVARCHAR(300), -- СМГ "П. Хилендарски"

    [FinancingTypeId] INT,
	[OwnershipId] INT,
	[StatusId] INT,

	CONSTRAINT [PK_Institution_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_Institution_ExternalId] UNIQUE ([ExternalId]),
	CONSTRAINT [FK_Institution_FinancingTypeId] FOREIGN KEY ([FinancingTypeId]) REFERENCES [Application].[InstitutionFinancingType] (Id),
	CONSTRAINT [FK_Institution_OwnershipId] FOREIGN KEY ([OwnershipId]) REFERENCES [Application].[InstitutionOwnership] (Id),
	CONSTRAINT [FK_Institution_InstitutionStatusId] FOREIGN KEY ([StatusId]) REFERENCES [Application].[InstitutionStatus] (Id)

	--[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
	--[ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,

    --PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)
--WITH ( SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [Application].[InstitutionsHistory] ));
GO

CREATE TABLE [Application].[SubInstitution]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Kind] NVARCHAR(300), -- (вид по законова уредба?) Вид по чл. 38 (детайлен) "основно","професионална гимназия","средно", "основно (І - VІІІ клас)",...
    [Director] NVARCHAR(300), -- Елена Владимирова Гюмова
    [Websites] NVARCHAR(1000), --keep them as a string with some separator between them
    [Emails] NVARCHAR(1000), --keep them as a string with some separator between them
    [PhoneNumbers] NVARCHAR(1000),  --keep them as a string with some separator between them

    [InstitutionId] INT NOT NULL,
    [AddressOfActivityId] INT,
    [PreparationTypeId] INT,

	CONSTRAINT [PK_SubInstitution_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_SubInstitution_InstitutionId] FOREIGN KEY ([InstitutionId]) REFERENCES [Application].[Institution] (Id),
    CONSTRAINT [FK_SubInstitution_AddressOfActivityId] FOREIGN KEY ([AddressOfActivityId]) REFERENCES [Application].[Address] ([Id]),
    CONSTRAINT [FK_SubInstitution_InstitutionPreparationTypeId] FOREIGN KEY ([PreparationTypeId]) REFERENCES [Application].[InstitutionPreparationType] ([Id])
)
GO

CREATE TABLE [Application].[Profile]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[ExternalId] INT NOT NULL,
	[Name] NVARCHAR(300),
	[Type] NVARCHAR(100), -- професионална, профилирана, ...
	[Grade] INT, -- Отнася се за кандидатстване за клас: 5/8/12
	[StudyPeriod] INT,

	[SubInstitutionId] INT NOT NULL,

	CONSTRAINT [PK_Profile_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_Profile_ExternalId] UNIQUE ([ExternalId]),
	CONSTRAINT [FK_Profile_SubInstitutionId] FOREIGN KEY ([SubInstitutionId]) REFERENCES [Application].[SubInstitution] ([Id])
);
GO

CREATE TABLE [Application].[ProfessionalDirection]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[ExternalId] INT NOT NULL,
    [Name] NVARCHAR(300), --Изящни изкуства, Музикални и сценични изкуства, ...

	[ScienceId] INT,

	CONSTRAINT [PK_ProfessionalDirection_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_ProfessionalDirection_ExternalId] UNIQUE ([ExternalId]),
	CONSTRAINT [FK_ProfessionalDirection_ScienceId] FOREIGN KEY ([ScienceId]) REFERENCES [Application].[Science] (Id),
);
GO

CREATE TABLE [Application].[Profession]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[ExternalId] INT NOT NULL,
    [Name] NVARCHAR(300), --Художник – изящни изкуства, Музикант- инструменталист

	[ProfessionalDirectionId] INT,

	CONSTRAINT [PK_Profession_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_Profession_ExternalId] UNIQUE ([ExternalId]),
	CONSTRAINT [FK_Profession_ProfessionalDirection] FOREIGN KEY ([ProfessionalDirectionId]) REFERENCES [Application].[ProfessionalDirection] ([Id])
);
GO

CREATE TABLE [Application].[Specialty]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[ExternalId] INT NOT NULL,
    [Name] NVARCHAR(300), --Живопис, Стенопис, Графика, Скулптура
	[ProfessionalQualificationLevel] INT, --I/1/първа, II/2/втора, III/3/трета, IV/4
	[Description] NVARCHAR(300),

    [ProfessionId] INT

	CONSTRAINT [PK_Specialty_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_Specialty_ExternalId] UNIQUE ([ExternalId]),
	CONSTRAINT [FK_Specialty_ProfessionId] FOREIGN KEY ([ProfessionId]) REFERENCES [Application].[Profession] ([Id])
);
GO

CREATE TABLE [Application].[ProfileDetails]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[GradingFormulas] NVARCHAR(1000), --(2*БЕЛ+2*МАТ)+(1*БЗО+1*ХООС)
	[StudyMethod] NVARCHAR(100), --разширено/интензивно/нито едно от двете
	[EducatingType] NVARCHAR(100), --дневна, дуална, ...
	[ClassesCount] DECIMAL(4, 2),
	[FirstForeignLanguage] NVARCHAR(100),

	[ProfileId] INT NOT NULL,
	[SchoolYearId] INT,
	[AdmissionByQuotasId] INT,
	[SpecialtyId] INT,

	CONSTRAINT [PK_ProfileDetails_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_ProfileDetails_ProfileId_SchoolYearId] UNIQUE ([ProfileId], [SchoolYearId]),
	CONSTRAINT [FK_ProfileDetails_ProfileId] FOREIGN KEY ([ProfileId]) REFERENCES [Application].[Profile] ([Id]),
	CONSTRAINT [FK_ProfileDetails_SchoolYearId] FOREIGN KEY ([SchoolYearId]) REFERENCES [Application].[SchoolYear] ([Id]),
	CONSTRAINT [FK_ProfileDetails_AdmissionByQuotasId] FOREIGN KEY ([AdmissionByQuotasId]) REFERENCES [Application].[AdmissionByQuotas] ([Id]),
	CONSTRAINT [FK_ProfileDetails_SpecialtyId] FOREIGN KEY ([SpecialtyId]) REFERENCES [Application].[Specialty] ([Id])
);
GO

CREATE TABLE [Application].[ExamStage]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[StageNumber] INT NOT NULL,

	[ProfileId] INT,
	[SchoolYearId] INT,
	[MinScoresId] INT,
	[MaxScoresId] INT,

	CONSTRAINT [PK_ExamStage_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_ExamStage_ProfileId] FOREIGN KEY ([ProfileId]) REFERENCES [Application].[Profile] ([Id]),
	CONSTRAINT [FK_ExamStage_SchoolYearId] FOREIGN KEY ([SchoolYearId]) REFERENCES [Application].[SchoolYear] ([Id]),
	CONSTRAINT [FK_ExamStage_MinScoresId] FOREIGN KEY ([MinScoresId]) REFERENCES [Application].[ExamScores] ([Id]),
	CONSTRAINT [FK_ExamStage_MaxScoresId] FOREIGN KEY ([MaxScoresId]) REFERENCES [Application].[ExamScores] ([Id])
);
GO

CREATE TABLE [Application].[ExamResult]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
	[PreparationType] NVARCHAR(100), -- З(задълж.), B1-З, B2-З, B1.1-З
	[CandidateCount] INT,
	[AverageSuccess] DECIMAL(5, 2),

	[SubjectAbbreviationId] INT,
	[SubInstitutionId] INT,
	[SchoolYearId] INT,
	[ExamAbbreviationId] INT,

	CONSTRAINT [PK_ExamResult_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_ExamResult_SubjectAbbreviationId] FOREIGN KEY ( [SubjectAbbreviationId] ) REFERENCES [Application].[SubjectAbbreviation] (Id),
	CONSTRAINT [FK_ExamResult_SubInstitutionId] FOREIGN KEY ( [SubInstitutionId] ) REFERENCES [Application].[SubInstitution] (Id),
	CONSTRAINT [FK_ExamResult_SchoolYearId] FOREIGN KEY ( [SchoolYearId] ) REFERENCES [Application].[SchoolYear] (Id),
	CONSTRAINT [FK_ExamResult_ExamAbbreviationId] FOREIGN KEY ( [ExamAbbreviationId] ) REFERENCES [Application].[ExamAbbreviation] (Id)
);
GO

------------------------------------------------------------------------

CREATE TABLE [Application].[Student]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Name] NVARCHAR(300),
    [Gender] NVARCHAR(6) CHECK ([Gender] IN ('male', 'female')),

	CONSTRAINT [PK_Student_Id] PRIMARY KEY CLUSTERED ([Id])
)
GO

CREATE TABLE [Application].[StudentPreparationType]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [PreparationType] NVARCHAR(50),

	CONSTRAINT [PK_StudentPreparationType_Id] PRIMARY KEY CLUSTERED ([Id])
)
GO

CREATE TABLE [Application].[StudentPersonalSubject]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Name] NVARCHAR(300),
    [StudentPreparationTypeId] INT NOT NULL,
	[StudentId] INT NOT NULL,

	CONSTRAINT [PK_StudentPersonalSubject_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_StudentPersonalSubject_StudentPreparationType] FOREIGN KEY ([StudentPreparationTypeId]) REFERENCES [Application].[StudentPreparationType] (Id),
	CONSTRAINT [FK_StudentPersonalSubject_Student] FOREIGN KEY ([StudentId]) REFERENCES [Application].[Student] (Id),
)
GO

CREATE TABLE [Application].[StudentSubjectResult]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Result] DECIMAL(1,1),
	[Grade] INT CHECK ([Grade] BETWEEN 1 AND 2),
	[PersonalSubjectId] INT NOT NULL,

	CONSTRAINT [PK_StudentSubjectResult_Id] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UQ_StudentSubjectResult_PersonalSubjectId] UNIQUE ([PersonalSubjectId]),
	CONSTRAINT [FK_StudentSubjectResult_PersonalSubjectId] FOREIGN KEY ([PersonalSubjectId]) REFERENCES [Application].[StudentPersonalSubject] (Id)
)
GO
