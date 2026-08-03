--Terminal command to run this script
--1. Open terminal in your script directory.
--2. $env:GMAIL_APP_PASS = "xxxx xxxx xxxx xxxx" 
--3. sqlcmd -S {server} -E -C -i .\PMS-Configurations.sql -v SMTP_PASSWORD=`"$env:GMAIL_APP_PASS`" 
--Replace placeholder with valid credentials like {server} becomes localhost\SQLEXPRESS

USE msdb;
GO

-- Safety check: Ensure the password variable is being supplied by the terminal
IF '$(SMTP_PASSWORD)' = '$' + '(SMTP_PASSWORD)' OR '$(SMTP_PASSWORD)' = ''
BEGIN
    ;THROW 51000, 'Deployment Failed: You must supply a valid SMTP_PASSWORD variable via SQLCMD.', 1;
END

-- 1. Enable Database Mail feature on the server instance
IF (SELECT value_in_use FROM sys.configurations WHERE name = 'Database Mail XPs') = 0
BEGIN
    EXEC sp_configure 'show advanced options', 1;
    RECONFIGURE;
    EXEC sp_configure 'Database Mail XPs', 1;
    RECONFIGURE;
END

-- 2. Create the Database Mail Account using your exact JSON settings
IF NOT EXISTS (SELECT 1 FROM msdb.dbo.sysmail_account WHERE name = 'PMS_Gmail_Account')
BEGIN
    EXEC msdb.dbo.sysmail_add_account_sp
        @account_name = 'PMS_Gmail_Account',
        @description = 'Gmail SMTP account for Parking Management System notifications.',
        @email_address = 'ibad20sohail@gmail.com',         -- Your Username
        @display_name = 'Parking Management System',       -- Your FromName
        @replyto_address = 'pms.dev@yopmail.com',          -- Your FromEmail
        @mailserver_name = 'smtp.gmail.com',               -- Your Host
        @port = 587,                                       -- Your Port
        @enable_ssl = 1,                                   -- Your EnableSsl (True)
        @username = 'ibad20sohail@gmail.com',              -- Your Username
        @password = '$(SMTP_PASSWORD)';                    -- Secure runtime variable injection
END

-- 3. Create the Mail Profile
IF NOT EXISTS (SELECT 1 FROM msdb.dbo.sysmail_profile WHERE name = 'PMS_Email_Profile')
BEGIN
    EXEC msdb.dbo.sysmail_add_profile_sp
        @profile_name = 'PMS_Email_Profile',
        @description = 'Main production mail profile for PMS background jobs.';
END

-- 4. Associate Account to the Profile
IF NOT EXISTS (
    SELECT 1 FROM msdb.dbo.sysmail_profileaccount pa
    JOIN msdb.dbo.sysmail_profile p ON pa.profile_id = p.profile_id
    JOIN msdb.dbo.sysmail_account a ON pa.account_id = a.account_id
    WHERE p.name = 'PMS_Email_Profile' AND a.name = 'PMS_Gmail_Account'
)
BEGIN
    EXEC msdb.dbo.sysmail_add_profileaccount_sp
        @profile_name = 'PMS_Email_Profile',
        @account_name = 'PMS_Gmail_Account',
        @sequence_number = 1;
END

-- 5. Grant global public permissions to use this profile
IF NOT EXISTS (
    SELECT 1 FROM msdb.dbo.sysmail_principalprofile pp
    JOIN msdb.dbo.sysmail_profile p ON pp.profile_id = p.profile_id
    WHERE p.name = 'PMS_Email_Profile' AND pp.is_default = 1
)
BEGIN
    EXEC msdb.dbo.sysmail_add_principalprofile_sp
        @profile_name = 'PMS_Email_Profile',
        @principal_name = 'public',
        @is_default = 1;
END
GO