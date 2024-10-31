-- [Application].[Profile]
CREATE INDEX UIDX_Profile_Grade_SubInstitutionId ON [Application].[Profile] (Grade, SubInstitutionId);

-- [Application].[ProfileDetails]
CREATE INDEX UIDX_ProfileDetails_SchoolYearId ON [Application].[ProfileDetails] (SchoolYearId);
CREATE INDEX UIDX_ProfileDetails_SpecialtyId ON [Application].[ProfileDetails] (SpecialtyId);
CREATE INDEX UIDX_ProfileDetails_ProfileId ON [Application].[ProfileDetails] (ProfileId);

--[Application].[Specialty]
CREATE INDEX UIDX_Specialty_ProfessionId ON [Application].[Specialty] (ProfessionId);
CREATE INDEX UIDX_Specialty_IsProfessional ON [Application].[Specialty] (IsProfessional);

--[Application].[Profession]
CREATE INDEX UIDX_Profession_ProfessionalDirectionId ON [Application].[Profession] (ProfessionalDirectionId);

--[Application].[ProfessionalDirection]
CREATE INDEX UIDX_ProfessionalDirection_ScienceId ON [Application].[ProfessionalDirection] (ScienceId);

--[Application].[SchoolYear]
CREATE INDEX UIDX_SchoolYear_Year ON [Application].[SchoolYear] (Year);

--[Application].[SubInstitution]
CREATE INDEX UIDX_SubInstitution_InstitutionId ON [Application].[SubInstitution] (InstitutionId);
CREATE INDEX UIDX_SubInstitution_AddressOfActivityId ON [Application].[SubInstitution] (AddressOfActivityId);

--[Application].[Address]
CREATE INDEX UIDX_Address_Settlement_Neighbourhood ON [Application].[Address] (Settlement, Neighbourhood);
CREATE INDEX UIDX_Address_Latitude_Longitude ON [Application].[Address] (Latitude, Longitude);
