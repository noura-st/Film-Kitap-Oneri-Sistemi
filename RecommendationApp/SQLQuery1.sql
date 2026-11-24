-- RecommendationApp schema (SQL Server)
--
-- This file replaces the original SQLQuery1.sql, which was a scratchpad:
-- it created Users without a Password column, then ALTERed it in, created and
-- dropped a Ratings table, and mixed ItemID/Id naming so none of the C# queries
-- would run against it. Everything below is idempotent and matches the code.

IF OBJECT_ID('dbo.UserRatings', 'U') IS NOT NULL DROP TABLE dbo.UserRatings;
IF OBJECT_ID('dbo.Ratings', 'U') IS NOT NULL DROP TABLE dbo.Ratings;
IF OBJECT_ID('dbo.Items', 'U') IS NOT NULL DROP TABLE dbo.Items;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;

CREATE TABLE dbo.Users (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    -- PBKDF2 output: "pbkdf2$iterations$salt$hash", so it needs more room than
    -- the raw password would have.
    PasswordHash NVARCHAR(200) NOT NULL
);

CREATE TABLE dbo.Items (
    ItemID INT IDENTITY(1,1) PRIMARY KEY,
    Title  NVARCHAR(200) NOT NULL,
    Type   NVARCHAR(50)  NOT NULL,   -- 'Film' or 'Kitap'
    Genre  NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.Ratings (
    RatingID INT IDENTITY(1,1) PRIMARY KEY,
    UserID   INT NOT NULL,
    ItemID   INT NOT NULL,
    Score    INT NOT NULL CHECK (Score BETWEEN 1 AND 5),
    CONSTRAINT FK_Ratings_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID),
    CONSTRAINT FK_Ratings_Items FOREIGN KEY (ItemID) REFERENCES dbo.Items(ItemID),
    -- One score per user per item: the app inserts in a loop, so without this a
    -- double click silently duplicates rows and skews the recommendation.
    CONSTRAINT UQ_Ratings_User_Item UNIQUE (UserID, ItemID)
);

CREATE INDEX IX_Ratings_UserID ON dbo.Ratings(UserID);

IF NOT EXISTS (SELECT 1 FROM dbo.Items)
BEGIN
    INSERT INTO dbo.Items (Title, Type, Genre) VALUES
        (N'Inception',              N'Film',  N'Bilim Kurgu'),
        (N'Interstellar',           N'Film',  N'Bilim Kurgu'),
        (N'Yüzüklerin Efendisi',    N'Film',  N'Fantastik'),
        (N'Sefiller',               N'Kitap', N'Dram'),
        (N'Dune',                   N'Kitap', N'Bilim Kurgu');
END

-- No default users: accounts are created through the register form, which
-- hashes the password. Seeding a user here would mean seeding a hash.
