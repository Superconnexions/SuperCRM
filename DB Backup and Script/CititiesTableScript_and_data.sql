CREATE TABLE Cities
(
    CityId INT IDENTITY(1,1) PRIMARY KEY,
    CityName NVARCHAR(150) NOT NULL,
    RegionId INT NOT NULL,
    
    CreatedAt DATETIME2(7) NOT NULL,
    UpdatedAt DATETIME2(7) NULL,
    IsActive BIT NOT NULL,

    CONSTRAINT FK_Cities_Region FOREIGN KEY (RegionId)
        REFERENCES Regions(RegionId)
);


CREATE UNIQUE INDEX UX_Cities_Region_CityName
ON Cities (RegionId, CityName);

INSERT INTO Cities (CityName, RegionId, CreatedAt, IsActive)
VALUES
-- Greater London (1)
('London', 1, GETUTCDATE(), 1),

-- South East England (2)
('Reading', 2, GETUTCDATE(), 1),
('Brighton', 2, GETUTCDATE(), 1),
('Oxford', 2, GETUTCDATE(), 1),

-- South West England (3)
('Bristol', 3, GETUTCDATE(), 1),
('Exeter', 3, GETUTCDATE(), 1),
('Plymouth', 3, GETUTCDATE(), 1),

-- West Midlands (4)
('Birmingham', 4, GETUTCDATE(), 1),
('Coventry', 4, GETUTCDATE(), 1),

-- East Midlands (5)
('Nottingham', 5, GETUTCDATE(), 1),
('Leicester', 5, GETUTCDATE(), 1),

-- North West England (6)
('Manchester', 6, GETUTCDATE(), 1),
('Liverpool', 6, GETUTCDATE(), 1),

-- North East England (7)
('Newcastle', 7, GETUTCDATE(), 1),
('Sunderland', 7, GETUTCDATE(), 1),

-- Yorkshire and the Humber (8)
('Leeds', 8, GETUTCDATE(), 1),
('Sheffield', 8, GETUTCDATE(), 1),

-- Scotland (9)
('Edinburgh', 9, GETUTCDATE(), 1),
('Glasgow', 9, GETUTCDATE(), 1),

-- Wales (10)
('Cardiff', 10, GETUTCDATE(), 1),
('Swansea', 10, GETUTCDATE(), 1),

-- Northern Ireland (11)
('Belfast', 11, GETUTCDATE(), 1),
('Derry', 11, GETUTCDATE(), 1);


