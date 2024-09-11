DECLARE @constraintName NVARCHAR(MAX);

SELECT @constraintName = [dC].[name]
FROM [sys].[default_constraints] AS [dC]
    INNER JOIN [sys].[columns] AS [c]
    ON
        [dC].[parent_object_id] = [c].[object_id] AND
        [dC].[parent_column_id] = [c].[column_id]
WHERE
    [c].[object_id] = OBJECT_ID('[Application].[ProfileDetails]') AND
    [c].[name] = 'IsPaperOnly';

IF (@constraintName IS NOT NULL)
BEGIN
    EXEC('ALTER TABLE [Application].[ProfileDetails] DROP CONSTRAINT ' + @constraintName);
END

ALTER TABLE [Application].[ProfileDetails]
ADD CONSTRAINT ProfileDetails_IsPaperOnly_Default DEFAULT 0 for [IsPaperOnly]

ALTER TABLE [Application].[ProfileDetails]
ALTER COLUMN [IsPaperOnly] BIT NOT NULL;
GO
