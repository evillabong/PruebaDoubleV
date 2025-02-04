--CREATE DATABASE PruebaDoubleV;
--GO

USE PruebaDoubleV;
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'User') AND TYPE IN (N'U')) DROP TABLE [User]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Person') AND TYPE IN (N'U')) DROP TABLE Person
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'PersonManager') AND TYPE IN (N'P')) DROP PROCEDURE PersonManager

CREATE TABLE Person (
    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    [Firstname] NVARCHAR(100) NOT NULL,
    [Lastname] NVARCHAR(100) NOT NULL,
    Identification NVARCHAR(20) NOT NULL UNIQUE,
    Email NVARCHAR(100),
    IdentificationType VARCHAR(50) NOT NULL,

    CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    
);
GO

CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
    PersonId INT,
    CONSTRAINT FK_User FOREIGN KEY (PersonId) REFERENCES Person(Id)
);
GO
 CREATE PROCEDURE PersonManager
	@Operation			VARCHAR(10),
	@ResultCode			INT		 OUTPUT,
	@Message		VARCHAR(150) OUTPUT
AS
BEGIN
	
	IF @Operation = 'C'
    BEGIN
		SELECT 
			P.Id AS Id,
			CONCAT(P.Firstname, ' ' ,P.Lastname) as [Name],
			CONCAT(P.IdentificationType,'-',P.Identification) as Identification,
			P.Email,
			P.CreatedAt,
			ISNULL(U.Username, 'Sin Usuario') as Username
		FROM Person P
		LEFT JOIN [User] U ON P.Id = U.[PersonId];
	END
	ELSE
	BEGIN
		SELECT @ResultCode= 99 ,@Message = 'Operation Fail';
	END
	SELECT @ResultCode= 0 ,@Message = 'Ok';
	RETURN 0;
END;
GO

INSERT INTO Person (Firstname, Lastname, Identification, IdentificationType, Email)
VALUES ('Administrador', 'Plus', '00000000000','Cedula', 'admin@example.com'),
 ('José', 'Villarreal', '0000000001','Ruc', 'evillabong@hotmail.com');

DECLARE @PersonaId INT;
SET @PersonaId = SCOPE_IDENTITY();

INSERT INTO [User] (Username, [Password], PersonId)
VALUES ('admin', 'daa6dc24b3cd5c00e1c14c243e720b54aa192c52a7f0245ebf415df887e6670ee24a7381355bbccd5e8a851931fbb45aee87c1460085680d6f9535df9a54da8a', @PersonaId);
--90451594Javo@