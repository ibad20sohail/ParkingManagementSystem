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

	DECLARE @stored_user_id INT;
	DECLARE @stored_hashed_password VARBINARY(64);
	DECLARE @stored_role_name VARCHAR(20);
	DECLARE @stored_role_id INT;
	DECLARE @stored_email VARCHAR(30);
	DECLARE @today_date DATE = CAST(GETUTCDATE() AS DATE);

	IF NULLIF(@user_name,'') IS NULL OR NULLIF(@password,'') IS NULL
	BEGIN
		;THROW 50001, 'Username and password is required.',1;
	END

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
	
	IF NOT EXISTS (SELECT 1 FROM login_logs 
		WHERE user_id = @stored_user_id AND
		CAST(login_at AS DATE) = @today_date)
	BEGIN
		INSERT INTO login_logs (user_id) VALUES (@stored_user_id);
	END
	SELECT 
		@stored_user_id AS user_id,
		@user_name AS user_name,
		@stored_role_id AS role_id,
		@stored_role_name AS role_name;
END
GO
CREATE OR ALTER PROCEDURE usp_logout_user
@user_id INT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	;WITH LatestSession AS (
        SELECT TOP 1 logout_at
        FROM login_logs
        WHERE user_id = @user_id AND logout_at IS NULL
        ORDER BY login_at DESC
    )
    UPDATE LatestSession 
    SET logout_at = GETUTCDATE();

	SELECT N'Bye 👋' AS message;
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
	
	SELECT u.user_id, u.user_name, u.email, u.contact_no, u.role_id, r.name AS role_name
	FROM users u INNER JOIN roles r ON u.role_id = r.role_id
	WHERE user_name = @user_name;
	
	IF @@ROWCOUNT = 0
	BEGIN
		;THROW 50001, 'Invalid Username.',2;
	END
END
GO
CREATE OR ALTER PROCEDURE usp_get_user_by_id
@user_id INT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF @user_id IS NULL OR @user_id <= 0
	BEGIN
		;THROW 50001, 'User id is required.',1;
	END
	
	SELECT u.user_id, u.user_name, u.email, u.contact_no, u.role_id, r.name AS role_name
	FROM users u INNER JOIN roles r ON u.role_id = r.role_id
	WHERE user_name = @user_id;
	
	IF @@ROWCOUNT = 0
	BEGIN
		;THROW 50001, 'Invalid User id.',2;
	END
END
GO
CREATE OR ALTER PROCEDURE usp_generate_reset_password_link
-- @generator-response OperationResponse
	@user_name VARCHAR(30), 
    @base_url VARCHAR(30) 
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @user_id INT;
    DECLARE @user_email VARCHAR(30);
    DECLARE @identifier UNIQUEIDENTIFIER = NEWID();
    DECLARE @generated_link VARCHAR(200);
    DECLARE @expire_minutes INT = 30;
    DECLARE @expire_at DATETIME2(0) = DATEADD(MINUTE, @expire_minutes, GETUTCDATE());
    DECLARE @email_subject NVARCHAR(200);
    DECLARE @email_body NVARCHAR(MAX);
	DECLARE @template_id INT;

    SELECT @user_id = user_id, @user_email = email FROM users WHERE user_name = @user_name AND is_active = 1;

    IF @user_id IS NULL OR @user_id <= 0
    BEGIN
        ;THROW 50001, 'User account is invalid or does not exist.', 1;
    END

    SET @base_url = CASE WHEN RIGHT(@base_url, 1) = '/' THEN LEFT(@base_url, LEN(@base_url) - 1) ELSE @base_url END;
    SET @generated_link = CONCAT(@base_url, '/Auth/ResetPassword?Identifier=', CAST(@identifier AS VARCHAR(36)));

    BEGIN TRANSACTION;
    BEGIN TRY
        
        INSERT INTO reset_password_links (user_id, expire_at, link, identifier)
        VALUES (@user_id, @expire_at, @generated_link, @identifier);
		
		SELECT
			@template_id = template_id,
            @email_subject = subject, 
            @email_body = body 
        FROM templates 
        WHERE is_active = 1 AND name = 'ResetPassword';

        IF NULLIF(@email_body, '') IS NULL OR NULLIF(@email_subject, '') IS NULL
        BEGIN
            ;THROW 50001, 'Email notification dispatch failed: Missing communication asset template.', 2;
        END

        SET @email_body = REPLACE(REPLACE(REPLACE(@email_body, '{{user_name}}', @user_name), '{{reset_link}}', @generated_link), '{{expire_in}}', CAST(@expire_minutes AS VARCHAR(10)));        

		EXEC usp_send_email @user_email, @user_id, @template_id, @email_subject, @email_body;

        COMMIT TRANSACTION;

		DECLARE @message VARCHAR(100) = CONCAT('An password reset link has been sent to ', @user_email, ' account.');
		SELECT @message AS message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE usp_reset_user_password
@identifier UNIQUEIDENTIFIER, @new_password VARCHAR(500), @confirm_password VARCHAR(300)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @expire_at DATETIME2;
	DECLARE @id INT;
	DECLARE @has_used BIT;
	DECLARE @user_id INT;

	SELECT TOP(1) 
		@expire_at = r.expire_at,
		@id = r.reset_password_links_id,
		@has_used = r.has_used,
		@user_id = r.user_id
	FROM reset_password_links r
	WHERE r.identifier = @identifier ORDER BY reset_password_links_id DESC;
	
	IF NULLIF(@id, '') IS NULL
	BEGIN
		;THROW 50001, 'Invalid link.',1;
	END

	IF (@has_used = 1)
	BEGIN
		;THROW 50001, 'This link has already used.',2;
	END

	IF (@expire_at < GETUTCDATE())
	BEGIN
		;THROW 50001, 'This link has been expired.',3;
	END

	IF NULLIF(@user_id, '') IS NULL
	BEGIN
		;THROW 50001, 'User Id is required.', 1;
	END

	IF(@new_password <> @confirm_password)
	BEGIN
		;THROW 50001, 'Confirm password does not match with new password.', 2; 
	END
	
	UPDATE reset_password_links SET
		has_used = 1
	WHERE reset_password_links_id = @id;

	UPDATE users SET
		password_hash = HASHBYTES('SHA2_512',@new_password)
	WHERE user_id = @user_id;

	SELECT 'Password has been reseted successfully' AS message;
END
GO
CREATE OR ALTER PROCEDURE usp_send_email
-- @generator-response None
@user_email VARCHAR(50), @user_id INT, @template_id INT, @subject NVARCHAR(200), @body NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	BEGIN TRY
		INSERT INTO email_logs (template_id, user_id) VALUES (@template_id, @user_id);

		DECLARE @email_log_id INT = SCOPE_IDENTITY();
		DECLARE @mailitem_id INT;

		EXEC msdb.dbo.sp_send_dbmail  
				@profile_name = 'PMS_Email_Profile',
				@recipients   = @user_email,
				@subject      = @subject,  
				@body         = @body,  
				@body_format  = 'HTML',
				@mailitem_id = @mailitem_id OUTPUT;

		UPDATE email_logs
		SET
			status = 'Sent',
			send_at = GETUTCDATE(),
			mailitem_id = @mailitem_id
		WHERE email_log_id = @email_log_id;
	END TRY
	BEGIN CATCH
		UPDATE email_logs
		SET
			status = 'Failed',
			error_number = ERROR_NUMBER(),
			error_message = ERROR_MESSAGE()
		WHERE email_log_id = @email_log_id;
	END CATCH;
END

