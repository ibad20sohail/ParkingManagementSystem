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
CREATE TABLE login_logs
(
	login_log_id INT PRIMARY KEY IDENTITY(1,1),
	user_id INT NOT NULL,
	login_at DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
	logout_at DATETIME2 NULL
);
GO
CREATE TABLE reset_password_links
(
	reset_password_links_id INT PRIMARY KEY IDENTITY(1,1),
	user_id INT NOT NULL,
	link VARCHAR(200) NOT NULL,
	identifier UNIQUEIDENTIFIER NOT NULL,
	has_used BIT DEFAULT(0),
	expire_at DATETIME2 NOT NULL,
	created_at DATETIME2 NOT NULL DEFAULT(GETUTCDATE())
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
GO
CREATE TABLE templates
(
	template_id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(30) UNIQUE NOT NULL,
	subject NVARCHAR(200) NOT NULL,
	body NVARCHAR(MAX) NOT NULL,
	is_active BIT DEFAULT 1,
);
GO
CREATE TABLE email_logs
(
    email_log_id        INT IDENTITY(1,1) PRIMARY KEY,
    template_id         INT NULL,
    user_id             INT NULL,
    status              VARCHAR(20) NOT NULL DEFAULT('Pending'),
    send_at             DATETIME2 NULL,
    created_at          DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
    retry_count         INT NOT NULL DEFAULT(0),
    error_message       NVARCHAR(MAX) NULL,
    error_number        INT NULL,
    mailitem_id         INT NULL,
    created_by          SYSNAME NULL,
    server_name         SYSNAME NULL DEFAULT(@@SERVERNAME)
);
--TABLES END--

--FOREIGN KEY START--
GO
ALTER TABLE users ADD CONSTRAINT FK_users_roles FOREIGN KEY (role_id) REFERENCES roles(role_id) ON DELETE NO ACTION;
GO
ALTER TABLE reset_password_links ADD CONSTRAINT FK_reset_password_links_users FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE;
GO
ALTER TABLE login_logs ADD CONSTRAINT FK_login_logs_users FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE;
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
GO
ALTER TABLE email_logs ADD CONSTRAINT FK_email_logs_templates FOREIGN KEY (template_id) REFERENCES templates(template_id) ON DELETE CASCADE;
GO
ALTER TABLE email_logs ADD CONSTRAINT FK_email_logs_users FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE;

--FOREIGN KEY END--