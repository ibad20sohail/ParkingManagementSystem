CREATE DATABASE ParkingManagment;
GO
USE ParkingManagment;
GO
--TABLES START--
GO
CREATE TABLE Users
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	RoleId INT NOT NULL,
	UserName VARCHAR(30) NOT NULL UNIQUE,
	PasswordHash VARBINARY(64) NOT NULL,
	IsActive BIT DEFAULT 1
);
GO
CREATE TABLE Roles
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(20) NOT NULL UNIQUE
);
GO
CREATE TABLE ParkedVehicles
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Ticket VARCHAR(50) NOT NULL UNIQUE,
	LicenseNo VARCHAR(30) NOT NULL,
	DriverName VARCHAR(30) NULL,
	Company VARCHAR(20) NULL,
	Model VARCHAR(30) NULL,
	Entered_At DATETIME2 NULL,
	Exit_AT DATETIME2 NULL,
	CategoryId INT NOT NULL,
	ParkingSpaceId INT NULL,
	UserId INT NOT NULL
);
GO
CREATE TABLE Categories
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(30) NOT NULL UNIQUE,
	Cost DECIMAL(6,2) NOT NULL
);
GO
CREATE TABLE ParkingSpaces
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Floor VARCHAR(20) NULL,
	Code VARCHAR(10) NOT NULL UNIQUE,
	StatusId INT NOT NULL
);
GO
CREATE TABLE ParkingSpaceStatuses
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(20) NOT NULL UNIQUE
);
GO
CREATE TABLE Billings
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	VehicleId INT NOT NULL,
	PaymentMethodId INT NOT NUll,
	CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE())
);
GO
CREATE TABLE PaymentMethods
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	PaymentType VARCHAR(30) NOT NULL UNIQUE,
);
--TABLES END--

--FOREIGN KEY START--
GO
ALTER TABLE Users ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE NO ACTION;
GO
ALTER TABLE Vehicles ADD CONSTRAINT FK_Vehicles_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE NO ACTION;
GO
ALTER TABLE Vehicles ADD CONSTRAINT FK_Vehicles_ParkingSpaces FOREIGN KEY (ParkingSpaceId) REFERENCES ParkingSpaces(Id) ON DELETE SET NULL;
GO
ALTER TABLE Vehicles ADD CONSTRAINT FK_Vehicles_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION;
GO
ALTER TABLE ParkingSpaces ADD CONSTRAINT FK_ParkingSpaces_ParkingSpaceStatuses FOREIGN KEY (StatusId) REFERENCES ParkingSpaceStatuses(Id) ON DELETE NO ACTION;
GO
ALTER TABLE Billings ADD CONSTRAINT FK_Billings_Vehicles FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE NO ACTION;
GO
ALTER TABLE Billings ADD CONSTRAINT FK_Billings_PaymentMethods FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethods(Id) ON DELETE NO ACTION;
--FOREIGN KEY END--

--SP START--
GO
CREATE OR ALTER PROCEDURE SP_AddUser @UserName VARCHAR(30), @Password VARCHAR(30)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF NULLIF(@UserName, '') IS NULL OR NULLIF(@Password, '') IS NULL
		BEGIN
			RAISERROR('Username and password is requird.',16, 1);
		END
		INSERT INTO Users (UserName, PasswordHash) 
		VALUES (@UserName, HASHBYTES('SHA2_512', @Password));
		SELECT 'User has been added successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_EditUser 
@Id INT, @UserName VARCHAR(30) = NULL, @OldPassword VARCHAR(30) = NULL, @NewPassword VARCHAR(30) = NULL, @ConfirmNewPassword VARCHAR(30) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @Id)
		BEGIN
			RAISERROR('User not found.', 16, 1);
		END
		IF NULLIF(@UserName, '') IS NOT NULL
		BEGIN
			IF EXISTS(SELECT 1 FROM Users WHERE UserName = @UserName AND Id <> @Id)
			BEGIN
				RAISERROR('Username already exists.', 16, 2);
			END
			ELSE
			UPDATE Users SET UserName = @UserName WHERE Id = @Id;
		END
		IF NULLIF(@OldPassword,'') IS NOT NULL AND NULLIF(@NewPassword,'') IS NOT NULL AND NULLIF(@ConfirmNewPassword,'') IS NOT NULL
		BEGIN
			IF @NewPassword <> @ConfirmNewPassword
			BEGIN
				RAISERROR('Confirm password does not match with new password.', 16, 1);
			END
			IF NOT EXISTS(SELECT 1 FROM Users WHERE Id = @Id AND PasswordHash = HASHBYTES('SHA2_512', @OldPassword))
			BEGIN
				RAISERROR('Old password is incorrect.', 16, 1)
			END
			UPDATE Users SET PasswordHash = HASHBYTES('SHA2_512', @NewPassword) WHERE Id = @Id;
		END
		SELECT 'User has been updated successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_DeleteUser @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @Id)
		BEGIN
			RAISERROR('User not found.',16,1);
		END
		DELETE FROM Users WHERE Id = @Id;
		SELECT 'User has been deleted successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_AddRole @Name VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF EXISTS(SELECT 1 FROM Roles WHERE Name = @Name)
		BEGIN
			RAISERROR('This Role is already in roles.',16,1);
		END
		INSERT INTO Roles(Name) VALUES (@Name);
		SELECT 'Role has been added successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_EditRole @Id INT, @Name VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF EXISTS(SELECT 1 FROM Roles WHERE Name = @Name AND Id <> @Id)
		BEGIN
			RAISERROR('This role name is already in roles.',16,1);
		END
		UPDATE Roles SET Name = @Name WHERE Id = @Id;
		SELECT 'Role has been updated successfully' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_DeleteRole @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF NOT EXISTS(SELECT 1 FROM Roles WHERE Id = @Id)
		BEGIN
			RAISERROR('Role not found.',16,1);
		END
		DELETE FROM Roles WHERE Id = @Id;
		SELECT 'Role has been deleted successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_AddCategory @Name VARCHAR(30), @Cost DECIMAL(6,2)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF EXISTS(SELECT 1 FROM Categories WHERE Name = @Name)
		BEGIN
			RAISERROR('This category is already in category.',16,1);
		END
		INSERT INTO Categories(Name, Cost) VALUES (@Name, @Cost);
		SELECT 'Category has been added successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_EditCategory @Id INT, @Name VARCHAR(30) = NULL, @Cost DECIMAL(6,2) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF EXISTS(SELECT 1 FROM Categories WHERE Name = @Name AND Id <> @Id)
		BEGIN
			RAISERROR('This category name is already in category.',16,1);
		END
		UPDATE Categories SET Name = @Name, Cost = @Cost WHERE Id = @Id;
		SELECT 'Category has been updated successfully' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_DeleteCategory @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF NOT EXISTS(SELECT 1 FROM Categories WHERE Id = @Id)
		BEGIN
			RAISERROR('Category not found.',16,1);
		END
		DELETE FROM Categories WHERE Id = @Id;
		SELECT 'Category has been deleted successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_AddParkingSpace @Floor VARCHAR(20), @Code VARCHAR(10), @StatusId INT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF ISNULL(@Code, '') IS NULL AND ISNULL(@StatusId, '') IS NULL
		BEGIN
			RAISERROR('Code and status is required', 16, 1);
		END
		IF EXISTS (SELECT 1 FROM ParkingSpaces WHERE Code = @Code)
		BEGIN
			RAISERROR('Code is already exists.',16,1);
		END
		IF NOT EXISTS (SELECT 1 FROM ParkingSpaceStatuses WHERE Id = @StatusId)
		BEGIN
			RAISERROR('Not a valid status.', 16, 1);
		END
		INSERT INTO ParkingSpaces(Floor, Code, StatusId) VALUES (@Floor, @Code, @StatusId);
		SELECT 'Parking space has been added successfully.' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_EditParkingSpace @Id INT, @Floor VARCHAR(20), @Code VARCHAR(10), @StatusId INT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF ISNULL(@Code, '') IS NULL AND ISNULL(@StatusId, '') IS NULL
		BEGIN
			RAISERROR('Code and status is required', 16, 1);
		END
		IF EXISTS (SELECT 1 FROM ParkingSpaces WHERE Code = @Code AND Id <> @Id)
		BEGIN
			RAISERROR('Code is already exists.',16,1);
		END
		IF NOT EXISTS (SELECT 1 FROM ParkingSpaceStatuses WHERE Id = @StatusId)
		BEGIN
			RAISERROR('Not a valid status.', 16, 1);
		END
		UPDATE ParkingSpaces SET Floor = @Floor, Code = @Code, StatusId = @StatusId;
		SELECT 'Parking space has been updated successfully.' AS Message;
		RETURN:
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
GO
CREATE OR ALTER PROCEDURE SP_DeleteParkingSpace @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM ParkingSpaces WHERE Id = @Id)
		BEGIN
			RAISERROR('Parking space not found', 16,1);
		END
		DELETE FROM ParkingSpaces WHERE Id = @Id;
		SELECT 'Parking space has been deleted' AS Message;
		RETURN;
	END TRY
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_MESSAGE() AS ErrorMessage
	END CATCH
END
--SP END--

--FUNCTION START--
GO
CREATE OR ALTER FUNCTION FN_GetUserById(@Id INT, @IsActive BIT = 1)
RETURNS TABLE
AS
RETURN
(
	SELECT * FROM Users WHERE Id = @Id AND IsActive = @IsActive
);
GO
CREATE OR ALTER FUNCTION FN_GetUsers(@IsActive BIT = 1)
RETURNS TABLE
AS
RETURN
(
	SELECT * FROM Users WHERE IsActive = @IsActive
);
GO
CREATE OR ALTER FUNCTION FN_GetRoleById(@Id INT)
RETURNS TABLE
AS
RETURN
(
	SELECT * FROM Roles WHERE Id = @Id
);
GO
CREATE OR ALTER FUNCTION FN_GetRoles()
RETURNS TABLE
AS
RETURN
(
	SELECT * FROM Roles
);
GO
CREATE OR ALTER FUNCTION FN_GetCategoryById(@Id INT)
RETURNS TABLE
AS
RETURN
(
	SELECT * FROM Categories WHERE Id = @Id
);
GO
CREATE OR ALTER FUNCTION FN_GetCategories()
RETURNS TABLE
AS
RETURN
(
	SELECT * FROM Categories
);

--FUNCTION END--
--INSERT START--



--INSERT END--