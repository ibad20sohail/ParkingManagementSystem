CREATE DATABASE parking_management_system;
GO
USE parking_management_system;
GO
--TABLES START--
GO
CREATE TABLE users
(
	user_id INT PRIMARY KEY IDENTITY(1,1),
	role_id INT NOT NULL,
	user_name VARCHAR(30) NOT NULL UNIQUE,
	password_hash VARBINARY(64) NOT NULL,
	email VARCHAR(30) UNIQUE NOT NULL,
	contact_no VARCHAR(20) NULL,
	is_active BIT DEFAULT 1
);
GO
CREATE TABLE roles
(
	role_id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(20) NOT NULL UNIQUE
);
GO
CREATE TABLE tickets
(
	ticket_id INT PRIMARY KEY IDENTITY(1,1),
	license_no VARCHAR(30) NOT NULL,
	driver_name VARCHAR(30) NULL,
	company VARCHAR(20) NULL,
	model_no VARCHAR(30) NULL,
	issued_at DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
	expires_at DATETIME2 NULL,
	is_used BIT DEFAULT 0,

	category_id INT NOT NULL,
	parking_space_Id INT NULL,
	user_id INT NOT NULL
);
GO
CREATE TABLE categories
(
	category_id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(30) NOT NULL UNIQUE
);
GO
CREATE TABLE parking_spaces
(
	parking_space_id INT PRIMARY KEY IDENTITY(1,1),
	floor VARCHAR(20) NULL,
	code VARCHAR(10) NOT NULL UNIQUE,
	parking_space_status_id INT NOT NULL
);
GO
CREATE TABLE parking_space_statuses
(
	parking_space_status_id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(20) NOT NULL UNIQUE
);
GO
CREATE TABLE billings
(
	billing_id INT PRIMARY KEY IDENTITY(1,1),
	ticket_id INT NOT NULL,
	payment_method_id INT NOT NUll,
	amount DECIMAL(10,2) NULL,
	created_at DATETIME2 NOT NULL DEFAULT(GETUTCDATE())
);
GO
CREATE TABLE payment_methods
(
	payment_method_id INT PRIMARY KEY IDENTITY(1,1),
	payment_type VARCHAR(30) NOT NULL UNIQUE,
);
--TABLES END--

--FOREIGN KEY START--
GO
ALTER TABLE users ADD CONSTRAINT FK_users_roles FOREIGN KEY (role_id) REFERENCES roles(role_id) ON DELETE NO ACTION;
GO
ALTER TABLE tickets ADD CONSTRAINT FK_tickets_categories FOREIGN KEY (category_id) REFERENCES categories(category_id) ON DELETE NO ACTION;
GO
ALTER TABLE tickets ADD CONSTRAINT FK_tickets_parking_spaces FOREIGN KEY (parking_space_id) REFERENCES parking_spaces(parking_space_id) ON DELETE SET NULL;
GO
ALTER TABLE tickets ADD CONSTRAINT FK_tickets_users FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE NO ACTION;
GO
ALTER TABLE parking_spaces ADD CONSTRAINT FK_parking_spaces_parking_space_statuses FOREIGN KEY (parking_space_status_id) REFERENCES parking_space_statuses(parking_space_status_Id) ON DELETE NO ACTION;
GO
ALTER TABLE billings ADD CONSTRAINT FK_billings_tickets FOREIGN KEY (ticket_id) REFERENCES tickets(ticket_id) ON DELETE NO ACTION;
GO
ALTER TABLE billings ADD CONSTRAINT FK_billings_payment_methods FOREIGN KEY (payment_method_id) REFERENCES payment_methods(payment_method_id) ON DELETE NO ACTION;
--FOREIGN KEY END--

--SP START--
GO
CREATE OR ALTER PROCEDURE usp_add_user
@user_name VARCHAR(30), @password VARCHAR(30), @role_id INT, @email VARCHAR(30), @contact_no VARCHAR(20) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	
	IF NULLIF(TRIM(@user_name), '') IS NULL
	BEGIN
		;THROW 50001, 'Username is requird.', 1;
	END
	IF  NULLIF(TRIM(@password), '') IS NULL
	BEGIN
		;THROW 50001, 'Password is requird.', 2;
	END
	IF  NULLIF(TRIM(@email), '') IS NULL
	BEGIN
		;THROW 50001, 'Email is requird.', 3;
	END
	INSERT INTO users (user_name, password_hash,role_id, email, contact_no) 
	VALUES (@user_name, HASHBYTES('SHA2_512', @password),@role_id, @email, @contact_no);
	SELECT 'User has been added successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_edit_user
@user_id INT, @user_name VARCHAR(30) = NULL, @old_password VARCHAR(30) = NULL, @new_password VARCHAR(30) = NULL, @confirm_new_password VARCHAR(30) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = @user_id)
	BEGIN
		;THROW 50001, 'User not found', 1;
	END
	IF NULLIF(@user_name, '') IS NOT NULL
	BEGIN
		IF EXISTS(SELECT 1 FROM users WHERE user_name = @user_name AND user_id <> @user_id)
		BEGIN
			;THROW 50001, 'Use name already exists.', 2;
		END
		UPDATE users SET user_name = @user_name WHERE user_id = @user_id;
	END
	IF NULLIF(@old_password,'') IS NOT NULL AND NULLIF(@new_password,'') IS NOT NULL AND NULLIF(@confirm_new_password,'') IS NOT NULL
	BEGIN
		IF @new_password <> @confirm_new_password
		BEGIN
			;THROW 50001, 'Confirm password does not match with new password.', 3;
		END
		IF NOT EXISTS(SELECT 1 FROM users WHERE user_id = @user_id AND password_hash = HASHBYTES('SHA2_512', @old_password))
		BEGIN
			;THROW 50001, 'Old password is incorrect.', 4;
		END
		UPDATE users SET password_hash = HASHBYTES('SHA2_512', @new_password) WHERE user_id = @user_id;
	END
	SELECT 'User has been updated successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_delete_user 
@user_id INT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = @user_id)
	BEGIN
		;THROW 50001, 'User not found', 1;
	END
	DELETE FROM users WHERE user_id = @user_id;
	SELECT 'User has been deleted successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_add_role 
@name VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT 1 FROM roles WHERE Name = @name)
	BEGIN
		;THROW 50001, 'This Role is already in roles.', 1;
	END
	INSERT INTO roles(name) VALUES (@name);
	SELECT 'Role has been added successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_edit_role 
@role_id INT, @name VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT 1 FROM roles WHERE name = @name AND role_id <> @role_id)
	BEGIN
		;THROW 50001, 'This role name is already in roles.', 1;
	END
	UPDATE roles SET Name = @name WHERE role_id = @role_id;
	SELECT 'Role has been updated successfully' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_delete_role 
@role_id INT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS(SELECT 1 FROM roles WHERE role_id = @role_id)
	BEGIN
		;THROW 50001, 'Role not found.', 1;
	END
	DELETE FROM roles WHERE role_id = @role_id;
	SELECT 'Role has been deleted successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_add_category 
@name VARCHAR(30)
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT 1 FROM categories WHERE name = @name)
	BEGIN
		;THROW 50001, 'This category is already in category.', 1;
	END
	INSERT INTO Categories(name) VALUES (@name);
	SELECT 'Category has been added successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_edit_category 
@category_id INT, @name VARCHAR(30) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT 1 FROM categories WHERE Name = @name AND category_id <> @category_id)
	BEGIN
		;THROW 50001, 'This category is already in category.', 1;
	END
	UPDATE categories SET Name = @name WHERE category_id = @category_id;
	SELECT 'Category has been updated successfully' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_delete_category 
@category_id INT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS(SELECT 1 FROM categories WHERE category_id = @category_id)
	BEGIN
		;THROW 50001, 'Category not found.', 1;
	END
	DELETE FROM categories WHERE category_id = @category_id;
	SELECT 'Category has been deleted successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_add_parking_space 
@floor VARCHAR(20), @code VARCHAR(10), @parking_space_status_id INT
AS
BEGIN
	SET NOCOUNT ON;
	IF ISNULL(@code, '') IS NULL AND ISNULL(@parking_space_status_id, '') IS NULL
	BEGIN
		;THROW 50001, 'Code and status is required', 1;
	END
	IF EXISTS (SELECT 1 FROM parking_spaces WHERE code = @code)
	BEGIN
		;THROW 50001, 'Code is already exists.', 2;
	END
	IF NOT EXISTS (SELECT 1 FROM parking_space_statuses WHERE parking_space_status_id = @parking_space_status_id)
	BEGIN
		;THROW 50001, 'Not a valid status.', 3;
	END
	INSERT INTO parking_spaces(floor, code, parking_space_status_id) VALUES (@floor, @code, @parking_space_status_id);
	SELECT 'Parking space has been added successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_edit_parking_space 
@parking_space_id INT, @floor VARCHAR(20), @code VARCHAR(10), @parking_space_status_id INT
AS
BEGIN
	SET NOCOUNT ON;
	IF ISNULL(@code, '') IS NULL AND ISNULL(@parking_space_status_id, '') IS NULL
	BEGIN
		;THROW 50001, 'Code and status is required', 1;
	END
	IF EXISTS (SELECT 1 FROM parking_spaces WHERE code = @code AND parking_space_id <> @parking_space_id)
	BEGIN
		;THROW 50001, 'Code is already exists.', 2;
	END
	IF NOT EXISTS (SELECT 1 FROM parking_space_statuses WHERE parking_space_status_id = @parking_space_status_id)
	BEGIN
		;THROW 50001, 'Not a valid status.', 3;
	END
	UPDATE parking_spaces SET floor = @floor, code = @code, parking_space_status_id = @parking_space_status_id WHERE parking_space_id = @parking_space_id;
	SELECT 'Parking space has been updated successfully.' AS Message;
	RETURN;
END
GO
CREATE OR ALTER PROCEDURE usp_delete_parking_space 
@parking_space_id INT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT 1 FROM parking_spaces WHERE parking_space_id = @parking_space_id)
	BEGIN
		;THROW 50001, 'Parking space not found', 1;
	END
	DELETE FROM parking_spaces WHERE parking_space_id = @parking_space_id;
	SELECT 'Parking space has been deleted' AS Message;
	RETURN;
END
---------AUTH SP'S--------------
GO
CREATE OR ALTER PROCEDURE usp_login_user
@user_name VARCHAR(30), @password VARCHAR(500)
AS
BEGIN 
	SET NOCOUNT ON;
	SET XACT_ABORT ON; 

	IF NULLIF(@user_name,'') IS NULL OR NULLIF(@password,'') IS NULL
	BEGIN
		;THROW 50001, 'Username and password is required.',1;
	END

	DECLARE @stored_user_id INT;
	DECLARE @stored_hashed_password VARBINARY(64);
	DECLARE @stored_role_name VARCHAR(20);
	DECLARE @stored_role_id INT;
	DECLARE @stored_email VARCHAR(30);

	SELECT 
		@stored_user_id = u.user_id,
		@stored_hashed_password = u.password_hash,
		@stored_role_name = r.name,
		@stored_role_id = u.role_id,
		@stored_email = u.email
	FROM users u INNER JOIN roles r ON u.role_id = r.role_id
	WHERE u.user_name = @user_name AND u.is_active = 1;

	IF @stored_user_id IS NULL OR @stored_hashed_password <> HASHBYTES('SHA2_512',@password)
	BEGIN
		;THROW 50001, 'Invalid credentials',2;
	END

	SELECT 
		@stored_user_id AS user_id,
		@user_name AS user_name,
		@stored_role_id AS role_id,
		@stored_role_name AS role_name;
END
GO
CREATE OR ALTER PROCEDURE usp_get_user_by_username
@user_name VARCHAR(30)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF NULLIF(TRIM(@user_name), '') IS NULL
	BEGIN
		;THROW 50001, 'Username is required.',1;
	END

	DECLARE @stored_email VARCHAR(30);
	
	SELECT @stored_email = email FROM users WHERE user_name = @user_name;
	IF @@ROWCOUNT = 0
	BEGIN
		;THROW 50001, 'Invalid credentials.',2;
	END
	IF NULLIF(TRIM(@stored_email),'') IS NULL
	BEGIN
		DECLARE @error_message VARCHAR(30) = FORMATMESSAGE('Email of "%s" not found', @user_name)
		;THROW 50001, @error_message,3;
	END
	SELECT
		@stored_email AS email;
END
--SP END--

--FUNCTION START--
--GO
--CREATE OR ALTER FUNCTION FN_GetUserById(@Id INT, @IsActive BIT = 1)
--RETURNS TABLE
--AS
--RETURN
--(
--	SELECT * FROM Users WHERE Id = @Id AND IsActive = @IsActive
--);
--GO
--CREATE OR ALTER FUNCTION FN_GetUsers(@IsActive BIT = 1)
--RETURNS TABLE
--AS
--RETURN
--(
--	SELECT * FROM Users WHERE IsActive = @IsActive
--);
--GO
--CREATE OR ALTER FUNCTION FN_GetRoleById(@Id INT)
--RETURNS TABLE
--AS
--RETURN
--(
--	SELECT * FROM Roles WHERE Id = @Id
--);
--GO
--CREATE OR ALTER FUNCTION FN_GetRoles()
--RETURNS TABLE
--AS
--RETURN
--(
--	SELECT * FROM Roles
--);
--GO
--CREATE OR ALTER FUNCTION FN_GetCategoryById(@Id INT)
--RETURNS TABLE
--AS
--RETURN
--(
--	SELECT * FROM Categories WHERE Id = @Id
--);
--GO
--CREATE OR ALTER FUNCTION FN_GetCategories()
--RETURNS TABLE
--AS
--RETURN
--(
--	SELECT * FROM Categories
--);

--FUNCTION END--
--INSERT START--
GO
EXEC usp_add_role 'admin';
EXEC usp_add_role 'operator';

EXEC usp_add_user 'operator','Operator@123', 2,'operator@yopmail.com';
--INSERT END--
