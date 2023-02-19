CREATE DATABASE VotingDb
GO

USE VotingDb
GO

CREATE TABLE [User]
(
    Id UNIQUEIDENTIFIER NOT NULL,
    FirstNames NVARCHAR(50),
    Surname NVARCHAR(50),
    PhoneNumber NVARCHAR(50),
    EmailAddress NVARCHAR(50),
    PostCode NVARCHAR(20),
    Address NVARCHAR(100),
    NationalIdentifier NVARCHAR(50),
    Nationality NVARCHAR(100),
    Password NVARCHAR(255),
    CONSTRAINT PK_User PRIMARY KEY (Id)
)
GO

CREATE TABLE [Election]
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(255),
    Nation NVARCHAR(255),
    Type INT,
    StartTime DATETIME,
    EndTime DATETIME,
    CONSTRAINT PK_Election PRIMARY KEY (Id)
);

CREATE TABLE [ElectionAdmin] (
    [ElectionId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY ([ElectionId]) REFERENCES [Election]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
)
GO

CREATE TABLE [ElectionCandidate]
(
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [ElectionId] UNIQUEIDENTIFIER NOT NULL,
    [Votes] int,
    FOREIGN KEY ([ElectionId]) REFERENCES [Election]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
)
GO

CREATE TABLE [ElectionInvite]
(
    [ElectionId] UNIQUEIDENTIFIER NOT NULL,
    [UserEmail] NVARCHAR(255),
    [StatusId] INT,
    FOREIGN KEY ([ElectionId]) REFERENCES [Election]([Id])
)
GO

CREATE TABLE [Vote] (
    ElectionId UNIQUEIDENTIFIER NOT NULL,
    CandidateId UNIQUEIDENTIFIER NOT NULL,
    VoterId UNIQUEIDENTIFIER NOT NULL,
    Timestamp DATETIME NOT NULL,
    FOREIGN KEY ([ElectionId]) REFERENCES [Election]([Id]),
    FOREIGN KEY ([CandidateId]) REFERENCES [User]([Id]),
    FOREIGN KEY ([VoterId]) REFERENCES [User]([Id])
)
GO
