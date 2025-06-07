CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL
);

CREATE TABLE Items (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200),
    Type NVARCHAR(50),      -- "Film" ya da "Kitap"
    Genre NVARCHAR(100)     -- "Bilim Kurgu", "Dram" gibi
);
INSERT INTO Items (Title, Type, Genre) VALUES 
('Inception', 'Film', 'Bilim Kurgu'),
('Interstellar', 'Film', 'Bilim Kurgu'),
('Yüzüklerin Efendisi', 'Film', 'Fantastik'),
('Sefiller', 'Kitap', 'Dram'),
('Dune', 'Kitap', 'Bilim Kurgu');


CREATE TABLE Ratings (
    RatingID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    ItemID INT,
    Score INT CHECK (Score >= 1 AND Score <= 5),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ItemID) REFERENCES Items(ItemID)
);
DROP TABLE Ratings;
CREATE TABLE UserRatings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100),
    ItemId INT,
    Rating INT,
    FOREIGN KEY (ItemId) REFERENCES Items(Id)
);
ALTER TABLE Users ADD Password NVARCHAR(50);