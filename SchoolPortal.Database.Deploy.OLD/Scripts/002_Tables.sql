
--CREATE TABLE [Applications].[Sources] -- 2023-2024/Sofia-grad/Profiles-2024.05.10-ri.mon.bg.xlsx
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [Name] NVARCHAR(MAX) NOT NULL,

--    CONSTRAINT [PK__Applications_Sources_Id] PRIMARY KEY CLUSTERED ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[ValidityPeriods] -- from 2021-09-15 00:00:00.0000000 to 2022-09-14 23:59:59.9999999
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [From] DATETIME2 NOT NULL,
--    [To] DATETIME2 NOT NULL,

--    CONSTRAINT [PK__Applications_ValidityPeriods_Id] PRIMARY KEY CLUSTERED ( [Id] ),
--);
--GO;

---- Synonym == Name
---- SynonymId == NameSynonymId == FullNameSynonymId

--CREATE TABLE [Applications].[Synonyms] -- София-град <- София
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [BaseId] INT,
--    [IsBase] AS ( CAST ( CASE WHEN [BaseId] IS NOT NULL AND [BaseId] = [Id] THEN 1 ELSE 0 END AS BIT ) ) PERSISTED NOT NULL,
--    [Exact] NVARCHAR(MAX) NOT NULL,
--    [Simple] NVARCHAR(MAX) NOT NULL,

--    CONSTRAINT [PK__Applications_Synonyms_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--    CONSTRAINT [FK__Applications_Synonyms_BaseId__Applications_Synonyms_Id] FOREIGN KEY ( [BaseId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),

--    [SourceId] INT NOT NULL,
--    CONSTRAINT [FK__Applications_Synonyms_SourceId__Applications_Sources_Id] FOREIGN KEY ( [SourceId] ) REFERENCES [Applications].[Sources] ( [Id] ),
--    [StoredPeriodFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
--    [StoredPeriodTo] DATETIME2 GENERATED ALWAYS AS ROW END,
--    PERIOD FOR SYSTEM_TIME ( [StoredPeriodFrom], [StoredPeriodTo] ),
--)
--WITH ( SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [Applications].[SynonymsHistory] ) );
--GO;

--CREATE TABLE [Applications].[Transliterations] -- Sofia <- Sofiq
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [BaseId] INT,
--    [IsBase] AS ( CAST ( CASE WHEN [BaseId] IS NOT NULL AND [BaseId] = [Id] THEN 1 ELSE 0 END AS BIT ) ) PERSISTED NOT NULL,
--    [Exact] NVARCHAR(MAX) NOT NULL,
--    [Simple] NVARCHAR(MAX) NOT NULL,

--    CONSTRAINT [PK__Applications_Transliterations_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--    CONSTRAINT [FK__Applications_Transliterations_BaseId__Applications_Transliterations_Id] FOREIGN KEY ( [BaseId] ) REFERENCES [Applications].[Transliterations] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[SynonymsTransliterations]
--(
--    [SynonymId] INT NOT NULL,
--    [TransliterationId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_SynonymsTransliterations_SynonymId_TransliterationId] PRIMARY KEY ( [SynonymId], [TransliterationId] ),

--    CONSTRAINT [FK__Applications_SynonymsTransliterations_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--    CONSTRAINT [FK__Applications_SynonymsTransliterations_TransliterationId__Applications_Transliterations_Id] FOREIGN KEY ( [TransliterationId] ) REFERENCES [Applications].[Transliterations] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Locations] -- 42.6954322, 23.3239467, 500
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [Latitude] DECIMAL(9, 7) NOT NULL,
--    [Longitude] DECIMAL(10, 7) NOT NULL,
--    [Altitude] INT NOT NULL,

--    CONSTRAINT [PK__Applications_Locations_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [UQ__Applications_Locations_Latitude_Longitude] UNIQUE ( [Latitude], [Longitude] ),
--);
--GO;

--CREATE TABLE [Applications].[Countries]
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,
--    [PhoneCode] NVARCHAR(3) NOT NULL,

--    CONSTRAINT [PK__Applications_Countries_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Countries_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Areas] -- София-град
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [CountryId] INT NOT NULL,
--    [SynonymId] INT NOT NULL,
--    [Code] NVARCHAR(3) NOT NULL, -- SOF

--    CONSTRAINT [PK__Applications_Areas_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Areas_CountryId__Applications_Countries_Id] FOREIGN KEY ( [CountryId] ) REFERENCES [Applications].[Countries] ( [Id] ),
--	CONSTRAINT [FK__Applications_Areas_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),

--    CONSTRAINT [UQ__Applications_Areas_Code] UNIQUE ( [Code] ),
--);
--GO;

--CREATE TABLE [Applications].[Communities] -- Столична
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [AreaId] INT NOT NULL,
--    [SynonymId] INT NOT NULL,
--    [Code] NVARCHAR(5) NOT NULL, -- SOF46

--    CONSTRAINT [PK__Applications_Communities_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Communities_AreaId__Applications_Areas_Id] FOREIGN KEY ( [AreaId] ) REFERENCES [Applications].[Areas] ( [Id] ),
--	CONSTRAINT [FK__Applications_Communities_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),

--    CONSTRAINT [UQ__Applications_Communities_Code] UNIQUE ( [Code] ),
--);
--GO;

--CREATE TABLE [Applications].[SettlementTypes] -- Град, Село, Манастир (Town, Village, Monastery)
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_SettlementTypes_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_SettlementTypes_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Settlements] -- София
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [Ekatte] NVARCHAR(5) NOT NULL, -- 68134
--    [CommunityId] INT NOT NULL,
--    [SettlementTypeId] INT NOT NULL,
--    [SynonymId] INT NOT NULL, -- <= 28
--	[LocationId] INT NOT NULL,
--    [PostalCode] NVARCHAR(13) NOT NULL,
--    [PhoneCode] NVARCHAR(5) NOT NULL,

--    CONSTRAINT [PK__Applications_Settlements_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Settlements_CommunityId__Applications_Communities_Id] FOREIGN KEY ( [CommunityId] ) REFERENCES [Applications].[Communities] ( [Id] ),
--	CONSTRAINT [FK__Applications_Settlements_SettlementTypeId__Applications_SettlementTypes_Id] FOREIGN KEY ( [SettlementTypeId] ) REFERENCES [Applications].[SettlementTypes] ( [Id] ),
--	CONSTRAINT [FK__Applications_Settlements_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Settlements_LocationId__Applications_Locations_Id] FOREIGN KEY ( [LocationId] ) REFERENCES [Applications].[Locations] ( [Id] ),

--    CONSTRAINT [UQ__Applications_Settlements_Ekatte] UNIQUE ( [Ekatte] ),
--);
--GO;

--CREATE TABLE [Applications].[Regions] -- Лозенец
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SettlementId] INT NOT NULL,
--    [SynonymId] INT NOT NULL,
--    [PostalCode] NVARCHAR(13),

--    CONSTRAINT [PK__Applications_Regions_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Regions_SettlementId__Applications_Settlements_Id] FOREIGN KEY ( [SettlementId] ) REFERENCES [Applications].[Settlements] ( [Id] ),
--	CONSTRAINT [FK__Applications_Regions_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Addresses] -- (квартал "К",) улица "У", номер Н, ...
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SettlementId] INT,
--    [RegionId] INT,
--    [SynonymId] INT NOT NULL,
--	[LocationId] INT,
--    [PostalCode] NVARCHAR(13),

--    CONSTRAINT [PK__Applications_Addresses_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Addresses_SettlementId__Applications_Settlements_Id] FOREIGN KEY ( [SettlementId] ) REFERENCES [Applications].[Settlements] ( [Id] ),
--	CONSTRAINT [FK__Applications_Addresses_RegionId__Applications_Regions_Id] FOREIGN KEY ( [RegionId] ) REFERENCES [Applications].[Regions] ( [Id] ),
--	CONSTRAINT [FK__Applications_Addresses_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Addresses_LocationId__Applications_Locations_Id] FOREIGN KEY ( [LocationId] ) REFERENCES [Applications].[Locations] ( [Id] ),

--    --CONSTRAINT [CC__Applications_Addresses_SettlementId_RegionId__NotNullAtLeastOne] CHECK ( NOT ( [SettlementId] IS NULL AND [RegionId] IS NULL ) ),
--    CONSTRAINT [CC__Applications_Addresses_SettlementId_RegionId__NotNullExactlyOne] CHECK
--        (
--            NOT ( [SettlementId] IS NULL AND    [RegionId] IS NULL ) AND
--                ( [SettlementId] IS NULL OR     [RegionId] IS NULL )
--        ),
--);
--GO;

--CREATE TABLE [Applications].[Websites]
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [Address] NVARCHAR(MAX) NOT NULL,

--    CONSTRAINT [PK__Applications_Websites_Id] PRIMARY KEY CLUSTERED ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Emails]
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [Address] NVARCHAR(MAX) NOT NULL,

--    CONSTRAINT [PK__Applications_Emails_Id] PRIMARY KEY CLUSTERED ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[PhoneNumbers] -- или (София, 1234567, вътрешен > 123), или (България, 888888888, вътрешен > 123 или натисни _ 4)
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [CountryId] INT,
--    [SettlementId] INT,
--    [Number] NVARCHAR(13) NOT NULL,
--    [Additional] NVARCHAR(MAX),

--    CONSTRAINT [PK__Applications_PhoneNumbers_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--    CONSTRAINT [FK__Applications_PhoneNumbers_CountryId__Applications_Countries_Id] FOREIGN KEY ( [CountryId] ) REFERENCES [Applications].[Countries] ( [Id] ),
--    CONSTRAINT [FK__Applications_PhoneNumbers_SettlementId__Applications_Settlements_Id] FOREIGN KEY ( [SettlementId] ) REFERENCES [Applications].[Settlements] ( [Id] ),

--    --CONSTRAINT [CC__Applications_PhoneNumbers_CountryId_SettlementId__NotNullAtLeastOne] CHECK ( NOT ( [CountryId] IS NULL AND [SettlementId] IS NULL ) ),
--    CONSTRAINT [CC__Applications_PhoneNumbers_CountryId_SettlementId__NotNullExactlyOne] CHECK
--        (
--            NOT ( [CountryId] IS NULL AND   [SettlementId] IS NULL ) AND
--                ( [CountryId] IS NULL OR    [SettlementId] IS NULL )
--        ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionStatuses] -- Действаща, Действаща (не провежда учебен процес през текущата година), Заличена, Закрита, Преобразувана, Преобразувана (закрита), Отписана, ...
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionStatuses_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_InstitutionStatuses_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),

--    [SourceId] INT NOT NULL,
--    CONSTRAINT [FK__Applications_InstitutionStatuses_SourceId__Applications_Sources_Id] FOREIGN KEY ( [SourceId] ) REFERENCES [Applications].[Sources] ( [Id] ),
--    [StoredPeriodFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
--    [StoredPeriodTo] DATETIME2 GENERATED ALWAYS AS ROW END,
--    PERIOD FOR SYSTEM_TIME ( [StoredPeriodFrom], [StoredPeriodTo] ),
--)
--WITH ( SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [Applications].[InstitutionStatusesHistory] ) );
--GO;

--CREATE TABLE [Applications].[InstitutionTypesBy24to27Short] -- Училище, Детска градина, ...
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionTypesBy24to27Short_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_InstitutionTypesBy24to27Short_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionTypesBy35to36Ownership] -- Общинско, Държавно, Частно, Духовно, По силата на международен договор
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionTypesBy35to36Ownership_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_InstitutionTypesBy35to36Ownership_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionTypesBy37Preparation] -- Неспециализирано, Специализирано
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionTypesBy37Preparation_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_InstitutionTypesBy37Preparation_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionTypesBy38Detailed] -- Професионална гимназия, Профилирана гимназия, Основно училище, ...
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionTypesBy38Detailed_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_InstitutionTypesBy38Detailed_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionTypesBy39Specialized] -- Духовно, По изкуствата, По културата, Спортно
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionTypesBy39Specialized_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_InstitutionTypesBy39Specialized_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionTypesByFinancing] -- МОН, Община, Частно, ...
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionTypesByFinancing_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_InstitutionTypesByFinancing_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Institutions] --
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SelfId] INT NOT NULL, -- 102010
--    [IsClosed] BIT NOT NULL, -- (Не е действаща)
--    [StatusId] INT NOT NULL,
--    [TypeBy24to27ShortId] INT,
--    [TypeBy35to36OwnershipId] INT,
--    [TypeBy37PreparationId] INT,
--    [TypeBy38DetailedId] INT,
--    [TypeBy39SpecializedId] INT,
--    [TypeByFinancingId] INT,
--    [EIK] NVARCHAR(15), -- 0123456789
--    [SynonymId] INT, -- Софийска математическа гимназия "Паисий Хилендарски"
--    [ShortSynonymId] INT, -- СМГ "П. Хилендарски"
--    [DirectorSynonymId] INT, -- Иван Иванов

--    CONSTRAINT [PK__Applications_Institutions_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Institutions_StatusId__Applications_InstitutionStatuses_Id] FOREIGN KEY ( [StatusId] ) REFERENCES [Applications].[InstitutionStatuses] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_TypeBy24to27ShortId__Applications_InstitutionTypesBy24to27Short_Id] FOREIGN KEY ( [TypeBy24to27ShortId] ) REFERENCES [Applications].[InstitutionTypesBy24to27Short] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_TypeBy35to36OwnershipId__Applications_InstitutionTypesBy35to36Ownership_Id] FOREIGN KEY ( [TypeBy35to36OwnershipId] ) REFERENCES [Applications].[InstitutionTypesBy35to36Ownership] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_TypeBy37PreparationId__Applications_InstitutionTypesBy37Preparation_Id] FOREIGN KEY ( [TypeBy37PreparationId] ) REFERENCES [Applications].[InstitutionTypesBy37Preparation] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_TypeBy38DetailedId__Applications_InstitutionTypesBy38Detailed_Id] FOREIGN KEY ( [TypeBy38DetailedId] ) REFERENCES [Applications].[InstitutionTypesBy38Detailed] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_TypeBy39SpecializedId__Applications_InstitutionTypesBy39Specialized_Id] FOREIGN KEY ( [TypeBy39SpecializedId] ) REFERENCES [Applications].[InstitutionTypesBy39Specialized] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_TypeByFinancingId__Applications_InstitutionTypesByFinancing_Id] FOREIGN KEY ( [TypeByFinancingId] ) REFERENCES [Applications].[InstitutionTypesByFinancing] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_ShortSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [ShortSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Institutions_DirectorSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [DirectorSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),

--    CONSTRAINT [CC__Applications_Institutions_SynonymId_ShortSynonymId__ShouldHasName] CHECK ( NOT ( [SynonymId] IS NULL AND [ShortSynonymId] IS NULL ) ),
--    --CONSTRAINT [CC__Applications_Institutions_IsClosed_HeadquartersAndAddressOfManagementId_MainAddressOfActivityId__ActiveShouldHasAddress] CHECK ( NOT ( [IsClosed] = 0 AND [HeadquartersAndAddressOfManagementId] IS NULL  AND [MainAddressOfActivityId] IS NULL ) ),

--    [ValidityPeriodId] INT NOT NULL,
--    CONSTRAINT [FK__Applications_Institutions_ValidityPeriodId__Applications_ValidityPeriods_Id] FOREIGN KEY ( [ValidityPeriodId] ) REFERENCES [Applications].[ValidityPeriods] ( [Id] ),
--    [SourceId] INT NOT NULL,
--    CONSTRAINT [FK__Applications_Institutions_SourceId__Applications_Sources_Id] FOREIGN KEY ( [SourceId] ) REFERENCES [Applications].[Sources] ( [Id] ),
--    [StoredPeriodFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
--    [StoredPeriodTo] DATETIME2 GENERATED ALWAYS AS ROW END,
--    PERIOD FOR SYSTEM_TIME ( [StoredPeriodFrom], [StoredPeriodTo] ),
--)
--WITH ( SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [Applications].[InstitutionsHistory] ) );
--GO;

--CREATE TABLE [Applications].[InstitutionsAddresses]
--(
--    [InstitutionId] INT NOT NULL,
--    [AddressId] INT NOT NULL,
--    [IsAdministrative] BIT, -- (Седалище и управление)
--    [IsActivity] BIT, -- (Дейност)

--    CONSTRAINT [PK__Applications_InstitutionsAddresses_InstitutionId_AddressId] PRIMARY KEY CLUSTERED ( [InstitutionId], [AddressId] ),

--    CONSTRAINT [FK__Applications_InstitutionsAddresses_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
--    CONSTRAINT [FK__Applications_InstitutionsAddresses_AddressId__Applications_Addresses_Id] FOREIGN KEY ( [AddressId] ) REFERENCES [Applications].[Addresses] ( [Id] ),

--    CONSTRAINT [CC__Applications_InstitutionsAddresses_IsHeadquartersAndManagement_IsActivity__ShouldBeAdministrativeOrActivity] CHECK ( [IsAdministrative] = 1 OR [IsActivity] = 1 ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionsWebsites]
--(
--    [InstitutionId] INT NOT NULL,
--    [WebsiteId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionsWebsites_InstitutionId_WebsiteId] PRIMARY KEY CLUSTERED ( [InstitutionId], [WebsiteId] ),

--    CONSTRAINT [FK__Applications_InstitutionsWebsites_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
--    CONSTRAINT [FK__Applications_InstitutionsWebsites_WebsiteId__Applications_Websites_Id] FOREIGN KEY ( [WebsiteId] ) REFERENCES [Applications].[Websites] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionsEmails]
--(
--    [InstitutionId] INT NOT NULL,
--    [EmailId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionsEmails_InstitutionId_EmailId] PRIMARY KEY CLUSTERED ( [InstitutionId], [EmailId] ),

--    CONSTRAINT [FK__Applications_InstitutionsEmails_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
--    CONSTRAINT [FK__Applications_InstitutionsEmails_EmailId__Applications_Emails_Id] FOREIGN KEY ( [EmailId] ) REFERENCES [Applications].[Emails] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[InstitutionsPhoneNumbers]
--(
--    [InstitutionId] INT NOT NULL,
--    [PhoneNumberId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_InstitutionsPhoneNumbers_InstitutionId_PhoneNumberId] PRIMARY KEY CLUSTERED ( [InstitutionId], [PhoneNumberId] ),

--    CONSTRAINT [FK__Applications_InstitutionsPhoneNumbers_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
--    CONSTRAINT [FK__Applications_InstitutionsPhoneNumbers_PhoneNumberId__Applications_PhoneNumbers_Id] FOREIGN KEY ( [PhoneNumberId] ) REFERENCES [Applications].[PhoneNumbers] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[ProfessionDirectionWrappers] -- Изкуства
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SelfId] INT NOT NULL,
--    [SynonymId] INT NOT NULL,
--    [IsDroppedOut] BIT NOT NULL,
--    [DescriptionSynonymId] INT,

--    CONSTRAINT [PK__Applications_ProfessionDirectionWrappers_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_ProfessionDirectionWrappers_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_ProfessionDirectionWrappers_DescriptionSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [DescriptionSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[ProfessionDirections] -- Приложни изкуства и занаяти
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [ProfessionDirectionWrapperId] INT NOT NULL,
--    [SelfId] INT NOT NULL,
--    [SynonymId] INT NOT NULL,
--    [IsDroppedOut] BIT NOT NULL,
--    [DescriptionSynonymId] INT,

--    CONSTRAINT [PK__Applications_ProfessionDirections_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_ProfessionDirections_ProfessionDirectionWrapperId__Applications_ProfessionDirectionWrappers_Id] FOREIGN KEY ( [ProfessionDirectionWrapperId] ) REFERENCES [Applications].[ProfessionDirectionWrappers] ( [Id] ),
--	CONSTRAINT [FK__Applications_ProfessionDirections_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_ProfessionDirections_DescriptionSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [DescriptionSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Professions] -- Керамик
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [ProfessionDirectionId] INT NOT NULL,
--    [SelfId] INT NOT NULL,
--    [SynonymId] INT NOT NULL,
--    [IsDroppedOut] BIT NOT NULL,
--    [DescriptionSynonymId] INT,

--    CONSTRAINT [PK__Applications_Professions_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Professions_ProfessionDirectionId__Applications_ProfessionDirections_Id] FOREIGN KEY ( [ProfessionDirectionId] ) REFERENCES [Applications].[ProfessionDirections] ( [Id] ),
--	CONSTRAINT [FK__Applications_Professions_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Professions_DescriptionSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [DescriptionSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Specialities] --
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [ProfessionId] INT NOT NULL,
--    [SelfId] INT NOT NULL, -- 2150501
--    [SynonymId] INT NOT NULL, -- Керамика
--    [ProfessionalQualificationDegree] INT NOT NULL, -- 1, 2, 3, 4
--    [IsDroppedOut] BIT NOT NULL,
--    [IsProtected] BIT NOT NULL,
--    [IsWithExpectedShortage] BIT NOT NULL,
--    [DescriptionSynonymId] INT, -- РД 09 – 298/19.02.2009, добавена на 19.02.2009 г.

--    CONSTRAINT [PK__Applications_Specialities_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Specialities_ProfessionId__Applications_Professions_Id] FOREIGN KEY ( [ProfessionId] ) REFERENCES [Applications].[Professions] ( [Id] ),
--	CONSTRAINT [FK__Applications_Specialities_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Specialities_DescriptionSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [DescriptionSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[ProfileTypes] -- Професионална, Профилирана
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_ProfileTypes_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_ProfileTypes_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[ProfileForms] -- Дневна, Дуална == Чрез работа, Вечерна, Индивидуална, Самостоятелна, Задочна, Дистанционна, Комбинирана,
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_ProfileForms_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_ProfileForms_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Profiles] --
--(
--    [Id] INT IDENTITY (1, 1) NOT NULL,
--    [InstitutionId] INT NOT NULL,
--    [SelfId] INT NOT NULL, -- 463
--    [SynonymId] INT NOT NULL, -- Извършване на термални процедури - АЕ
--    [ProfileTypeId] INT NOT NULL,
--    [ProfileFormId] INT NOT NULL,
--    [SpecialityId] INT NOT NULL,
--    [RequesterSynonymId] INT, -- (заявил потребност от паралелката)
--    [Grade] INT NOT NULL, -- в 8 клас
--    [Duration] DECIMAL(2, 1) NOT NULL, -- 5 години
--    [IsNewForInstitution] BIT,
--    [IsNewForCommunity] BIT,
--    [IsNewForArea] BIT,
--    [Count] DECIMAL(2, 1) NOT NULL, -- (брой паралелки в институцията от вида)
--    [IsByQuotas] BIT NOT NULL, -- (по квоти) X мъже + Y жени
--    [TotalSeats] INT NOT NULL, -- X + Y
--    [MenSeats] INT, -- 0 или X
--    [WomenSeats] INT, -- 0 или Y
--    [StemProfessionsSeats] INT,
--    [StemProfilesSeats] INT,
--    [SteamProfessionsSeats] INT,
--    [SteamProfilesSeats] INT,
--    [StreamProfessionsSeats] INT,
--    [StreamProfilesSeats] INT,
--    [IsConfirmed] BIT, -- (потвърдена)
--    [IsApproved] BIT, -- (утвърдена)

--    CONSTRAINT [PK__Applications_Profiles_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Profiles_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
--	CONSTRAINT [FK__Applications_Profiles_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Profiles_ProfileTypeId__Applications_ProfileTypes_Id] FOREIGN KEY ( [ProfileTypeId] ) REFERENCES [Applications].[ProfileTypes] ( [Id] ),
--	CONSTRAINT [FK__Applications_Profiles_ProfileFormId__Applications_ProfileForms_Id] FOREIGN KEY ( [ProfileFormId] ) REFERENCES [Applications].[ProfileForms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Profiles_SpecialityId__Applications_Specialities_Id] FOREIGN KEY ( [SpecialityId] ) REFERENCES [Applications].[Specialities] ( [Id] ),
--	CONSTRAINT [FK__Applications_Profiles_RequesterSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [RequesterSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),

--    CONSTRAINT [CC__Applications_Profiles_IsNewForInstitution_IsNewForCommunity_IsNewForArea__Impossibility] CHECK
--        (
--            NOT
--            (
--                ( [IsNewForInstitution] = 0 AND ( [IsNewForCommunity] = 1 OR    [IsNewForArea] = 1 ) ) OR
--                                                ( [IsNewForCommunity] = 0 AND   [IsNewForArea] = 1 )
--            )
--        ),
--);
--GO;

--CREATE TABLE [Applications].[ProfilesFormulaSynonyms] -- (1*БЕЛ + 1*МАТ + 2*БИО) + (1*БЗО + 1*ХООС) или (1*БЕЛ + 1*М) + (2*БЗО + 2*ХООС)
--(
--    [ProfileId] INT NOT NULL,
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_ProfilesFormulaSynonyms_ProfileId_SynonymId] PRIMARY KEY CLUSTERED ( [ProfileId], [SynonymId] ),

--    CONSTRAINT [FK__Applications_ProfilesFormulaSynonyms_ProfileId__Applications_Profiles_Id] FOREIGN KEY ( [ProfileId] ) REFERENCES [Applications].[Profiles] ( [Id] ),
--    CONSTRAINT [FK__Applications_ProfilesFormulaSynonyms_SynonymId__Applications_Profiles_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[ProfileFormulaScores] -- (Минимален и Максимален бал за успешен прием в паралелка след поредното класиране)
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [Stage] INT NOT NULL, -- 1-во класиране
--    [ProfileId] INT NOT NULL,
--    [MinTotal] INT,
--    [MinMen] INT,
--    [MinWomen] INT,
--    [MaxTotal] INT,
--    [MaxMen] INT,
--    [MaxWomen] INT,

--    CONSTRAINT [PK__Applications_ProfileFormulaScores_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_ProfileFormulaScores_ProfileId__Applications_Profiles_Id] FOREIGN KEY ( [ProfileId] ) REFERENCES [Applications].[Profiles] ( [Id] ),
--);
--GO;

---- ДИППК-п.р  == Държавен изпит за придобиване на професионална квалификация - писмена работа по теория на професията + практика
---- ДИППК-тест == Държавен изпит за придобиване на професионална квалификация - писмен тест по теория на професията + практика
---- ДИППК-Д.Пр == Държавен изпит за придобиване на професионална квалификация - дипломен проект
---- ДИППК-пр.  == Държавен изпит за придобиване на професионална квалификация - практика
--CREATE TABLE [Applications].[Subjects] -- Български, Математика, ДИППК*
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,
--    [ShortSynonymId] INT NOT NULL, -- МАТ, М

--    CONSTRAINT [PK__Applications_Subjects_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Subjects_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_Subjects_ShortSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [ShortSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[StudyingWays] -- Разширено, Интензивно, Не разширено и не интензивно
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_StudyingWays_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_StudyingWays_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[ProfilesSubjectsStudyingWays] -- (Английски разширено + Немски интензивно)
--(
--    [ProfileId] INT NOT NULL,
--    [SubjectId] INT NOT NULL,
--    [StudyingWayId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_ProfilesSubjectsStudyingWays_ProfileId_SubjectId_StudyingWayId] PRIMARY KEY CLUSTERED ( [ProfileId], [SubjectId], [StudyingWayId] ),

--	CONSTRAINT [FK__Applications_ProfilesSubjectsStudyingWays_ProfileId__Applications_Profiles_Id] FOREIGN KEY ( [ProfileId] ) REFERENCES [Applications].[Profiles] ( [Id] ),
--	CONSTRAINT [FK__Applications_ProfilesSubjectsStudyingWays_SubjectId__Applications_Subjects_Id] FOREIGN KEY ( [SubjectId] ) REFERENCES [Applications].[Subjects] ( [Id] ),
--	CONSTRAINT [FK__Applications_ProfilesSubjectsStudyingWays_StudyingWayId__Applications_StudyingWays_Id] FOREIGN KEY ( [StudyingWayId] ) REFERENCES [Applications].[StudyingWays] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[PreparationStyles] -- Общообразователна, Профилирана подготовка
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,
--    [ShortSynonymId] INT NOT NULL, -- ООП, ПП

--    CONSTRAINT [PK__Applications_PreparationStyles_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_PreparationStyles_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_PreparationStyles_ShortSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [ShortSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[PreparationTypes] -- Задължителна, ...
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,
--    [ShortSynonymId] INT NOT NULL, -- З, B1-З, B1.1-З, B2-З

--    CONSTRAINT [PK__Applications_PreparationTypes_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_PreparationTypes_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--	CONSTRAINT [FK__Applications_PreparationTypes_ShortSynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [ShortSynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[Exams] -- Национално външно оценяване, Държавен зрелостен изпит
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [SynonymId] INT NOT NULL,

--    CONSTRAINT [PK__Applications_Exams_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--	CONSTRAINT [FK__Applications_Exams_SynonymId__Applications_Synonyms_Id] FOREIGN KEY ( [SynonymId] ) REFERENCES [Applications].[Synonyms] ( [Id] ),
--);
--GO;

--CREATE TABLE [Applications].[SummaryExamResults] --
--(
--    [Id] INT NOT NULL IDENTITY (1, 1),
--    [ExamId] INT NOT NULL,
--    [Grade] INT NOT NULL, -- 4, 7, 10, 12
--    [InstitutionId] INT NOT NULL,
--    [SubjectId] INT NOT NULL,
--    [PreparationStyleId] INT,
--    [PreparationTypeId] INT,
--    [Takers] INT NOT NULL, -- 100 ученика
--    [AverageSuccess] DECIMAL(5, 2) NOT NULL, -- 79.30

--    CONSTRAINT [PK__Applications_SummaryExamResults_Id] PRIMARY KEY CLUSTERED ( [Id] ),

--    CONSTRAINT [FK__Applications_SummaryExamResults_ExamId__Applications_Exams_Id] FOREIGN KEY ( [ExamId] ) REFERENCES [Applications].[Exams] ( [Id] ),
--    CONSTRAINT [FK__Applications_SummaryExamResults_InstitutionId__Applications_Institutions_Id] FOREIGN KEY ( [InstitutionId] ) REFERENCES [Applications].[Institutions] ( [Id] ),
--    CONSTRAINT [FK__Applications_SummaryExamResults_SubjectId__Applications_Subjects_Id] FOREIGN KEY ( [SubjectId] ) REFERENCES [Applications].[Subjects] ( [Id] ),
--    CONSTRAINT [FK__Applications_SummaryExamResults_PreparationStyleId__Applications_PreparationStyles_Id] FOREIGN KEY ( [PreparationStyleId] ) REFERENCES [Applications].[PreparationStyles] ( [Id] ),
--    CONSTRAINT [FK__Applications_SummaryExamResults_PreparationTypeId__Applications_PreparationTypes_Id] FOREIGN KEY ( [PreparationTypeId] ) REFERENCES [Applications].[PreparationTypes] ( [Id] ),
--);
--GO;
