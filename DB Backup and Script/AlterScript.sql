ALTER TABLE dbo.UserProfiles
ADD CityId INT NULL;

ALTER TABLE dbo.UserProfiles
ADD City NVARCHAR(100) NULL;


ALTER TABLE dbo.UserAddresses
ADD CityId INT NULL;

UPDATE ua
SET ua.CityId = c.CityId
FROM dbo.UserAddresses ua
INNER JOIN dbo.Cities c
    ON ua.City = c.CityName
   AND ua.RegionId = c.RegionId;


   --UPDATE dbo.UserAddresses
   --SET City = NULL


   SELECT *
FROM dbo.UserAddresses
WHERE City IS NOT NULL
AND CityId IS NULL;


ALTER TABLE dbo.UserProfiles
ADD CONSTRAINT FK_UserProfiles_City
FOREIGN KEY (CityId) REFERENCES dbo.Cities(CityId);


ALTER TABLE dbo.UserAddresses
ADD CONSTRAINT FK_UserAddresses_City
FOREIGN KEY (CityId) REFERENCES dbo.Cities(CityId);
