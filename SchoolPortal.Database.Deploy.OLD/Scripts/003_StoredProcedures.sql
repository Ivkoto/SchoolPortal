
-- must be edited because [Synonym] was modified
--CREATE OR ALTER PROC [Applications].[AddSynonym]
--(
--    @Name NVARCHAR(255),
--    @BaseId INT = NULL
--)
--AS
--BEGIN
--    BEGIN TRY
--    BEGIN TRAN

--        SET NOCOUNT ON;

--		IF ( @BaseId IS NOT NULL )
--		BEGIN
--			DECLARE @match INT = -1;

--			SELECT TOP 1 @match = COUNT(*)
--			FROM [[Applications]].[Synonyms] AS s
--			WHERE s.[Id] = @BaseId

--			IF ( @match < 1 )
--			BEGIN
--				DECLARE @Error NVARCHAR(MAX) = N'Error: No such baseId = ' + CONVERT(NVARCHAR(MAX), @BaseId);
--				; THROW	50001, @Error, 1 ;
--			END
--		END

--        INSERT INTO [[Applications]].[Synonyms] ( [Name] ) VALUES ( @Name );

--		DECLARE @id INT;
--		SELECT @id = @@IDENTITY;

--		UPDATE s
--		SET s.[BaseId] = CASE WHEN @BaseId IS NULL THEN @id ELSE @BaseId END
--		FROM
--		(
--			SELECT TOP 1 *
--			FROM [[Applications]].[Synonyms] AS s
--			WHERE s.[Id] = @id
--		) AS s

--    COMMIT TRAN
--    END TRY
--    BEGIN CATCH
--        IF ( @@TRANCOUNT > 0 )
--		BEGIN
--            ROLLBACK TRAN;
--		END

--        ; THROW ;
--    END CATCH
--END
--GO

--CREATE OR ALTER PROCEDURE [Applications].[DeleteSynonym]
--(
--	@Id INT
--)
--AS
--BEGIN

--	DELETE [d]
--	FROM [Applications].[Synonyms] AS [d]
--	WHERE [Id] = @Id

--END
--GO
