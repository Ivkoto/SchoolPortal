CREATE OR ALTER FUNCTION [Application].[fn_CalculateDistanceHaversine] 
(
    @Latitude       DECIMAL(17, 15),  -- (input)
    @Longitude      DECIMAL(18, 15),  -- (input)
    @GeoLatitude    DECIMAL(17, 15),  -- (table)
    @GeoLongitude   DECIMAL(18, 15)   -- (table)
)
RETURNS DECIMAL(18, 6)
AS
BEGIN
    DECLARE @EarthRadiusKm DECIMAL = 6371.0;
    DECLARE @dLat DECIMAL(18, 15), @dLon DECIMAL(18, 15), @a DECIMAL(18, 15), @c DECIMAL(18, 15);

    SET @Latitude       = RADIANS(CAST(@Latitude AS FLOAT)); 
    SET @Longitude      = RADIANS(CAST(@Longitude AS FLOAT));
    SET @GeoLatitude    = RADIANS(CAST(@GeoLatitude AS FLOAT));
    SET @GeoLongitude   = RADIANS(CAST(@GeoLongitude AS FLOAT));

    SET @dLat = @GeoLatitude - @Latitude;
    SET @dLon = @GeoLongitude - @Longitude;

    SET @a = POWER(SIN(@dLat / 2), 2) + COS(@Latitude) * COS(@GeoLatitude) * POWER(SIN(@dLon / 2), 2);
    SET @c = 2 * ATN2(SQRT(@a), SQRT(1 - @a));

    -- distance in kilometers
    RETURN @EarthRadiusKm * @c;
END;