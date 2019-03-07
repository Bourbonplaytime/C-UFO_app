CREATE TABLE [dbo].[Sightings]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SightingDate] DATETIME NULL, 
    [City] NCHAR(10) NULL, 
    [State] NCHAR(10) NULL, 
    [Country] NCHAR(10) NULL, 
    [Shape] NCHAR(10) NULL, 
    [Duration] NCHAR(10) NULL, 
    [Comments] NVARCHAR(50) NULL, 
    [DatePosted] NCHAR(10) NULL, 
    [Latitude] FLOAT NULL, 
    [Longitude] FLOAT NULL
)
