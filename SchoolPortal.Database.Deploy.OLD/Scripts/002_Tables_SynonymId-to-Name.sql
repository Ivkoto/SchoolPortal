
CREATE TABLE [Applications].[ValidityPeriods] -- from 2021-09-15 00:00:00.0000000 to 2022-09-14 23:59:59.9999999
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [From] DATETIME2 NOT NULL,
    [To] DATETIME2 NOT NULL,

    CONSTRAINT [PK__Applications_ValidityPeriods_Id] PRIMARY KEY CLUSTERED ( [Id] ),
);
GO;

--Synonym == Name
-- SynonymId == NameSynonymId == FullNameSynonymId

CREATE TABLE [Applications].[Synonyms] -- София - град < -София
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [BaseId] INT,
    [IsBase] AS ( CAST ( CASE WHEN [BaseId] IS NOT NULL AND [BaseId] = [Id] THEN 1 ELSE 0 END AS BIT ) ) PERSISTED NOT NULL,
    [Exact] NVARCHAR(MAX) NOT NULL,
    [Simple] NVARCHAR(MAX) NOT NULL,

    CONSTRAINT [PK__Applications_Synonyms_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Synonyms_BaseId__Applications_Synonyms_Id] FOREIGN KEY ( [BaseId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
)
GO;

CREATE TABLE [Applications].[Transliterations] -- Sofia < -Sofiq
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [BaseId] INT,
    [IsBase] AS ( CAST ( CASE WHEN [BaseId] IS NOT NULL AND [BaseId] = [Id] THEN 1 ELSE 0 END AS BIT ) ) PERSISTED NOT NULL,
    [Exact] NVARCHAR(MAX) NOT NULL,
    [Simple] NVARCHAR(MAX) NOT NULL,
    [SynonymId] INT NOT NULL,

    CONSTRAINT [PK__Applications_Transliterations_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Transliterations_BaseId__Applications_Transliterations_Id] FOREIGN KEY ( [BaseId] ) REFERENCES [Applications].[Transliterations] ( [Id] ),
    CONSTRAINT [FK__Applications_Transliterations_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Countries]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Name] NVARCHAR(56) NOT NULL,
    [PhoneCode] NVARCHAR(4) NOT NULL,

    CONSTRAINT [PK__Applications_Countries_Id] PRIMARY KEY CLUSTERED ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Areas] -- София - град
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [CountryId] INT NOT NULL,
    [Name] NVARCHAR(28) NOT NULL,

    CONSTRAINT [PK__Applications_Areas_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Areas_CountryId__Applications_Countries_Id] FOREIGN KEY ( [CountryId] ) REFERENCES [Applications].[Countries] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Communities] -- Столична
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [AreaId] INT NOT NULL,
    [Name] NVARCHAR(28) NOT NULL,

    CONSTRAINT [PK__Applications_Communities_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Communities_AreaId__Applications_Areas_Id] FOREIGN KEY ( [AreaId] ) REFERENCES [Applications].[Areas] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Locations] -- 42.6954322, 23.3239467, 500
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Latitude] DECIMAL(17, 15) NOT NULL, -- (9, 7)
    [Longitude] DECIMAL(18, 15) NOT NULL, -- (10, 7)

    CONSTRAINT [PK__Applications_Locations_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [UQ__Applications_Locations_Latitude_Longitude] UNIQUE ( [Latitude], [Longitude] ),
);
GO;

CREATE TABLE [Applications].[Settlements] -- София
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Ekatte] NVARCHAR(5) NOT NULL, -- 68134
    [CommunityId] INT NOT NULL,
    [Type] NVARCHAR(9) NOT NULL, -- Град, Село, Манастир (Town, Village, Monastery)
    [Name] NVARCHAR(28) NOT NULL, -- <= 28
    [LocationId] INT NOT NULL,
    [PostalCode] NVARCHAR(13) NOT NULL,
    [PhoneCode] NVARCHAR(5) NOT NULL,

    CONSTRAINT [PK__Applications_Settlements_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Settlements_CommunityId__Applications_Communities_Id] FOREIGN KEY ( [CommunityId] ) REFERENCES [Applications].[Communities] ( [Id] ),
    CONSTRAINT [FK__Applications_Settlements_LocationId__Applications_Locations_Id] FOREIGN KEY ( [LocationId] ) REFERENCES [Applications].[Locations] ( [Id] ),

    CONSTRAINT [UQ__Applications_Settlements_Ekatte] UNIQUE ( [Ekatte] ),
);
GO;

CREATE TABLE [Applications].[Regions] -- Лозенец
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [SettlementId] INT NOT NULL,
    [Name] NVARCHAR(56) NOT NULL,
    [PostalCode] NVARCHAR(13),

    CONSTRAINT [PK__Applications_Regions_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Regions_SettlementId__Applications_Settlements_Id] FOREIGN KEY ( [SettlementId] ) REFERENCES [Applications].[Settlements] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Institutions] --
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [SelfId] INT NOT NULL, -- 102010
    [IsClosed] BIT NOT NULL, -- (Не е действаща)
    [Status] NVARCHAR(99) NOT NULL, -- Действаща, Действаща (не провежда учебен процес през текущата година), Заличена, Закрита, Преобразувана, Преобразувана (закрита), Отписана, ...
    [TypeBy24to27Short] NVARCHAR(99), -- Училище, Детска градина, ...
    [TypeBy35to36Ownership] NVARCHAR(99), -- Общинско, Държавно, Частно, Духовно, По силата на международен договор
    [TypeBy37Preparation] NVARCHAR(99), -- Неспециализирано, Специализирано
    [TypeBy38Detailed] NVARCHAR(99), -- Професионална гимназия, Профилирана гимназия, Основно училище, ...
    [TypeBy39Specialized] NVARCHAR(99), -- Духовно, По изкуствата, По културата, Спортно
    [TypeByFinancing] NVARCHAR(99), -- МОН, Община, Частно, ...
    [EIK] NVARCHAR(15), -- 0123456789
    [Name] NVARCHAR(99), -- Софийска математическа гимназия "Паисий Хилендарски"
    [ShortName] NVARCHAR(99), -- СМГ "П. Хилендарски"
    [Director] NVARCHAR(99), -- Иван Иванов

    CONSTRAINT [PK__Applications_Institutions_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [CC__Applications_Institutions_Name_ShortName__ShouldHasName] CHECK ( NOT ( [Name] IS NULL AND [ShortName] IS NULL ) ),
    --CONSTRAINT [CC__Applications_Institutions_IsClosed_HeadquartersAndAddressOfManagementId_MainAddressOfActivityId__ActiveShouldHasAddress] CHECK ( NOT ( [IsClosed] = 0 AND [HeadquartersAndAddressOfManagementId] IS NULL AND [MainAddressOfActivityId] IS NULL ) ),

    [ValidityPeriodId] INT NOT NULL,
    CONSTRAINT [FK__Applications_Institutions_ValidityPeriodId__Applications_ValidityPeriods_Id] FOREIGN KEY ( [ValidityPeriodId] ) REFERENCES [Applications].[ValidityPeriods] ( [Id] ),
    [StoredPeriodFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [StoredPeriodTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ( [StoredPeriodFrom], [StoredPeriodTo] ),
)
WITH ( SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [Applications].[InstitutionsHistory] ) );
GO;

CREATE TABLE [Applications].[Addresses] -- (квартал "К",) улица "У", номер Н, ...
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [SettlementId] INT,
    [RegionId] INT,
    [Name] NVARCHAR(MAX) NOT NULL,
    [LocationId] INT,
    [PostalCode] NVARCHAR(13),

    CONSTRAINT [PK__Applications_Addresses_Id] PRIMARY KEY CLUSTERED ( [Id] ),

	CONSTRAINT [FK__Applications_Addresses_SettlementId__Applications_Settlements_Id] FOREIGN KEY ( [SettlementId] ) REFERENCES [Applications].[Settlements] ( [Id] ),
	CONSTRAINT [FK__Applications_Addresses_RegionId__Applications_Regions_Id] FOREIGN KEY ( [RegionId] ) REFERENCES [Applications].[Regions] ( [Id] ),
	CONSTRAINT [FK__Applications_Addresses_LocationId__Applications_Locations_Id] FOREIGN KEY ( [LocationId] ) REFERENCES [Applications].[Locations] ( [Id] ),

    --CONSTRAINT [CC__Applications_Addresses_SettlementId_RegionId__NotNullAtLeastOne] CHECK ( NOT ( [SettlementId] IS NULL AND [RegionId] IS NULL ) ),
    CONSTRAINT [CC__Applications_Addresses_SettlementId_RegionId__NotNullExactlyOne] CHECK
        (
            NOT (   [SettlementId] IS NULL AND  [RegionId] IS NULL) AND
                (   [SettlementId] IS NULL OR   [RegionId] IS NULL)
        ),
);
GO;

CREATE TABLE [Applications].[InstitutionsAddresses]
(
    [InstitutionId] INT NOT NULL,
    [AddressId] INT NOT NULL,
    [IsAdministrative] BIT, -- (Седалище и управление)
    [IsActivity] BIT, -- (Дейност)

    CONSTRAINT [PK__Applications_InstitutionsAddresses_InstitutionId_AddressId] PRIMARY KEY CLUSTERED ( [InstitutionId], [AddressId] ),

    CONSTRAINT [FK__Applications_InstitutionsAddresses_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
    CONSTRAINT [FK__Applications_InstitutionsAddresses_AddressId__Applications_Addresses_Id] FOREIGN KEY ( [AddressId] ) REFERENCES [Applications].[Addresses] ( [Id] ),

    CONSTRAINT [CC__Applications_InstitutionsAddresses_IsAdministrative_IsActivity__ShouldBeAdministrativeOrActivity] CHECK ( [IsAdministrative] = 1 OR [IsActivity] = 1 ),
);
GO;

CREATE TABLE [Applications].[Websites]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Address] NVARCHAR(MAX) NOT NULL,
    [InstitutionId] INT NOT NULL,

    CONSTRAINT [PK__Applications_Websites_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Websites_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Emails]
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Address] NVARCHAR(MAX) NOT NULL,
    [InstitutionId] INT NOT NULL,

    CONSTRAINT [PK__Applications_Emails_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Emails_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[PhoneNumbers] -- или (София, 1234567, вътрешен > 123), или(България, 888888888, вътрешен > 123 или натисни _ 4)
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [CountryId] INT,
    [SettlementId] INT,
    [Number] NVARCHAR(13) NOT NULL,
    [Additional] NVARCHAR(MAX),
    [InstitutionId] INT NOT NULL,

    CONSTRAINT [PK__Applications_PhoneNumbers_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_PhoneNumbers_CountryId__Applications_Countries_Id] FOREIGN KEY ( [CountryId] ) REFERENCES [Applications].[Countries] ( [Id] ),
    CONSTRAINT [FK__Applications_PhoneNumbers_SettlementId__Applications_Settlements_Id] FOREIGN KEY ( [SettlementId] ) REFERENCES [Applications].[Settlements] ( [Id] ),
    CONSTRAINT [FK__Applications_PhoneNumbers_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),

    --CONSTRAINT [CC__Applications_PhoneNumbers_CountryId_SettlementId__NotNullAtLeastOne] CHECK ( NOT ( [CountryId] IS NULL AND [SettlementId] IS NULL ) ),
    CONSTRAINT [CC__Applications_PhoneNumbers_CountryId_SettlementId__NotNullExactlyOne] CHECK
        (
            NOT (   [CountryId] IS NULL AND [SettlementId] IS NULL) AND
                (   [CountryId] IS NULL OR  [SettlementId] IS NULL)
        ),
);
GO;

CREATE TABLE [Applications].[ProfessionDirectionWrappers] -- Изкуства
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [SelfId] INT NOT NULL,
    [Name] NVARCHAR(99) NOT NULL,
    [IsDroppedOut] BIT NOT NULL,
    [Description] NVARCHAR(99),

    CONSTRAINT [PK__Applications_ProfessionDirectionWrappers_Id] PRIMARY KEY CLUSTERED ( [Id] ),
);
GO;

CREATE TABLE [Applications].[ProfessionDirections] -- Приложни изкуства и занаяти
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [ProfessionDirectionWrapperId] INT NOT NULL,
    [SelfId] INT NOT NULL,
    [Name] NVARCHAR(99) NOT NULL,
    [IsDroppedOut] BIT NOT NULL,
    [Description] NVARCHAR(99),

    CONSTRAINT [PK__Applications_ProfessionDirections_Id] PRIMARY KEY CLUSTERED ( [Id] ),

	CONSTRAINT [FK__Applications_ProfessionDirections_ProfessionDirectionWrapperId__Applications_ProfessionDirectionWrappers_Id] FOREIGN KEY ( [ProfessionDirectionWrapperId] ) REFERENCES [Applications].[ProfessionDirectionWrappers] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Professions] -- Керамик
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [ProfessionDirectionId] INT NOT NULL,
    [SelfId] INT NOT NULL,
    [Name] NVARCHAR(99) NOT NULL,
    [IsDroppedOut] BIT NOT NULL,
    [Description] NVARCHAR(99),

    CONSTRAINT [PK__Applications_Professions_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Professions_ProfessionDirectionId__Applications_ProfessionDirections_Id] FOREIGN KEY ( [ProfessionDirectionId] ) REFERENCES [Applications].[ProfessionDirections] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Specialities] --
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [ProfessionId] INT NOT NULL,
    [SelfId] INT NOT NULL, -- 2150501
    [Name] NVARCHAR(99) NOT NULL, -- Керамика
    [ProfessionalQualificationDegree] INT NOT NULL, -- 1, 2, 3, 4
    [IsDroppedOut] BIT NOT NULL,
    [IsProtected] BIT NOT NULL,
    [IsWithExpectedShortage] BIT NOT NULL,
    [Description] NVARCHAR(99), -- РД 09 – 298 / 19.02.2009, добавена на 19.02.2009 г.

    CONSTRAINT [PK__Applications_Specialities_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_Specialities_ProfessionId__Applications_Professions_Id] FOREIGN KEY ( [ProfessionId] ) REFERENCES [Applications].[Professions] ( [Id] ),
);
GO;

-- ДИППК-п.р == Държавен изпит за придобиване на професионална квалификация - писмена работа по теория на професията + практика
-- ДИППК-тест == Държавен изпит за придобиване на професионална квалификация - писмен тест по теория на професията + практика
-- ДИППК-Д.Пр == Държавен изпит за придобиване на професионална квалификация - дипломен проект
-- ДИППК-пр.  == Държавен изпит за придобиване на професионална квалификация - практика
CREATE TABLE [Applications].[Subjects] -- Български, Математика, ДИППК*
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Name] NVARCHAR(99) NOT NULL,
    [ShortName] NVARCHAR(99) NOT NULL, -- МАТ, М

    CONSTRAINT [PK__Applications_Subjects_Id] PRIMARY KEY CLUSTERED ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Profiles] --
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [InstitutionId] INT NOT NULL,
    [SelfId] INT NOT NULL, -- 463
    [Name] NVARCHAR(99) NOT NULL, -- Извършване на термални процедури - АЕ
    [Type] NVARCHAR(99) NOT NULL, -- Професионална, Профилирана
    [Form] NVARCHAR(99) NOT NULL, -- Дневна, Дуална == Чрез работа, Вечерна, Индивидуална, Самостоятелна, Задочна, Дистанционна, Комбинирана,
    [SpecialityId] INT NOT NULL,
    [SubjectId] INT NOT NULL, -- Не можем да добавим няколко (Английски разширено + Немски интензивно)
    [StudyingWay] NVARCHAR(99) NOT NULL, -- Разширено, Интензивно, Нито разширено, нито интензивно
    [Requester] NVARCHAR(99) NOT NULL, -- (заявил потребност от паралелката)
    [Grade] INT NOT NULL, -- в 8 клас
    [Duration] DECIMAL(2, 1) NOT NULL, -- 5 години
    [IsNewForInstitution] BIT,
    [IsNewForCommunity] BIT,
    [IsNewForArea] BIT,
    [Count] DECIMAL(2, 1) NOT NULL, -- (брой паралелки в институцията от вида)
    [IsByQuotas] BIT NOT NULL, -- (по квоти) X мъже + Y жени
    [TotalSeats] INT NOT NULL, -- X + Y
    [MenSeats] INT, -- 0 или X
    [WomenSeats] INT, -- 0 или Y
    --[StemProfessionsSeats] INT,
    --[StemProfilesSeats] INT,
    --[SteamProfessionsSeats] INT,
    --[SteamProfilesSeats] INT,
    --[StreamProfessionsSeats] INT,
    --[StreamProfilesSeats] INT,
    --[IsConfirmed] BIT, -- (потвърдена)
    --[IsApproved] BIT, -- (утвърдена)

    CONSTRAINT [PK__Applications_Profiles_Id] PRIMARY KEY CLUSTERED ( [Id] ),

	CONSTRAINT [FK__Applications_Profiles_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
	CONSTRAINT [FK__Applications_Profiles_SpecialityId__Applications_Specialities_Id] FOREIGN KEY ( [SpecialityId] ) REFERENCES [Applications].[Specialities] ( [Id] ),
    CONSTRAINT [FK__Applications_Profiles_SubjectId__Applications_Subjects_Id] FOREIGN KEY ( [SubjectId] ) REFERENCES [Applications].[Subjects] ( [Id] ),

    CONSTRAINT [CC__Applications_Profiles_IsNewForInstitution_IsNewForCommunity_IsNewForArea__Impossibility] CHECK
        (
            NOT
            (
                ( [IsNewForInstitution] = 0 AND ( [IsNewForCommunity] = 1 OR    [IsNewForArea] = 1 ) ) OR
                                                ( [IsNewForCommunity] = 0 AND   [IsNewForArea] = 1 )
            )
        ),
);
GO;

CREATE TABLE[Applications].[ProfileFormulas] -- (1 * БЕЛ + 1 * МАТ + 2 * БИО) + (1 * БЗО + 1 * ХООС) или (1 * БЕЛ + 1 * М) + (2 * БЗО + 2 * ХООС)
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [ProfileId] INT NOT NULL,
    [Formula] NVARCHAR(MAX) NOT NULL,

    CONSTRAINT [PK__Applications_ProfileFormulas_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_ProfileFormulas_ProfileId__Applications_Profiles_Id] FOREIGN KEY ( [ProfileId] ) REFERENCES [Applications].[Profiles] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[ProfileFormulaScores] -- (Минимален и Максимален бал за успешен прием в паралелка след поредното класиране)
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Stage] INT NOT NULL, -- 1-во класиране
    [ProfileId] INT NOT NULL,
    [MinTotal] INT,
    [MinMen] INT,
    [MinWomen] INT,
    [MaxTotal] INT,
    [MaxMen] INT,
    [MaxWomen] INT,

    CONSTRAINT [PK__Applications_ProfileFormulaScores_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_ProfileFormulaScores_ProfileId__Applications_Profiles_Id] FOREIGN KEY ( [ProfileId] ) REFERENCES [Applications].[Profiles] ( [Id] ),
);
GO;

CREATE TABLE [Applications].[Exams] -- Национално външно оценяване, Държавен зрелостен изпит
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [Name] NVARCHAR(99) NOT NULL,

    CONSTRAINT [PK__Applications_Exams_Id] PRIMARY KEY CLUSTERED ( [Id] ),
);
GO;

CREATE TABLE [Applications].[SummaryExamResults] --
(
    [Id] INT NOT NULL IDENTITY (1, 1),
    [ExamId] INT NOT NULL,
    [Grade] INT NOT NULL, -- 4, 7, 10, 12
    [InstitutionId] INT NOT NULL,
    [SubjectId] INT NOT NULL,
    [PreparationStyle] NVARCHAR(99) NOT NULL, -- Общообразователна, Профилирана подготовка
    --[ShortPreparationStyle] NVARCHAR(99) NOT NULL, -- ООП, ПП
    [PreparationType] NVARCHAR(99) NOT NULL, -- Задължителна, ...
    --[ShortPreparationType] NVARCHAR(99) NOT NULL, -- З, B1-З, B1.1-З, B2-З
    [Takers] INT NOT NULL, -- 100 ученици
    [AverageSuccess] DECIMAL(5, 2) NOT NULL, -- 79.30

    CONSTRAINT [PK__Applications_SummaryExamResults_Id] PRIMARY KEY CLUSTERED ( [Id] ),

    CONSTRAINT [FK__Applications_SummaryExamResults_ExamId__Applications_Exams_Id] FOREIGN KEY ( [ExamId] ) REFERENCES [Applications].[Exams] ( [Id] ),
    CONSTRAINT [FK__Applications_SummaryExamResults_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
    CONSTRAINT [FK__Applications_SummaryExamResults_SubjectId__Applications_Subjects_Id] FOREIGN KEY ( [SubjectId] ) REFERENCES [Applications].[Subjects] ( [Id] ),
);
GO;
