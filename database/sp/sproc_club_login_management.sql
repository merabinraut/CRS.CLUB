USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_club_login_management]    Script Date: 11/26/2023 3:10:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_club_login_management] @Flag VARCHAR(10)
	,@LoginId VARCHAR(200) = NULL
	,@Password VARCHAR(16) = NULL
	,@ConfirmPassword VARCHAR(16) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(10) = NULL
	,@AgentId BIGINT =NULL
AS
DECLARE @MaxFailedLoginAttempt INT = 5
	,@Session VARCHAR(500)
	
BEGIN
	IF ISNULL(@Flag, '') = 'Login'
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_club_details a WITH (NOLOCK)
				INNER JOIN dbo.tbl_users b ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') = 'A'
				INNER JOIN dbo.tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId
					AND c.RoleName = 'Club'
				WHERE b.LoginId = @LoginId
				)
		BEGIN
			SELECT 1 Code
				,'Club user is inactive' Message;

			RETURN;
		END;

		SELECT @AgentId = a.AgentId
		FROM dbo.tbl_club_details a WITH (NOLOCK)
		INNER JOIN dbo.tbl_users b ON b.AgentId = a.AgentId
			AND ISNULL(b.STATUS, '') = 'A'
		INNER JOIN dbo.tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId
			AND c.RoleName = 'Club'
		WHERE b.LoginId = @LoginId;

		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_club_details a WITH (NOLOCK)
				INNER JOIN dbo.tbl_users b ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') = 'A'
				INNER JOIN dbo.tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId
					AND c.RoleName = 'Club'
				WHERE b.LoginId = @LoginId
					AND a.AgentId = @AgentId
					AND PWDCOMPARE(@Password, b.Password) = 1
				)
		BEGIN
			IF EXISTS (
					SELECT b.FailedLoginAttempt
					FROM dbo.tbl_club_details a WITH (NOLOCK)
					INNER JOIN dbo.tbl_users b ON b.AgentId = a.AgentId
						AND ISNULL(b.STATUS, '') = 'A'
					INNER JOIN dbo.tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId
						AND c.RoleName = 'Club'
					WHERE b.LoginId = @LoginId
						AND a.AgentId = @AgentId
						AND ISNULL(b.FailedLoginAttempt, 0) = @MaxFailedLoginAttempt
					)
			BEGIN
				UPDATE dbo.tbl_users
				SET STATUS = 'B'
					,FailedLoginAttempt = 0
					,Session = NULL
					,ActionUser = @LoginId
					,ActionIP = @ActionIP
					,ActionPlatform = @ActionPlatform
					,ActionDate = GETDATE()
				WHERE LoginId = @LoginId
					AND AgentId = @AgentId
					AND [Status] = 'A';

				SELECT 1 Code
					,'Invalid credentials. Club user is blocked' Message;

				RETURN;
			END;
		END;

		IF EXISTS (
				SELECT 'X'
				FROM dbo.tbl_club_details a WITH (NOLOCK)
				INNER JOIN dbo.tbl_users b ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') = 'A'
				INNER JOIN dbo.tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId
					AND c.RoleName = 'Club'
				WHERE b.LoginId = @LoginId
					AND a.AgentId = @AgentId
					AND PWDCOMPARE(@Password, b.Password) = 1
				)
		BEGIN
			SELECT @Session = NEWID();

			UPDATE dbo.tbl_users
			SET Session = @Session
				,FailedLoginAttempt = 0
				,ActionUser = @LoginId
				,ActionIP = @ActionIP
				,ActionPlatform = @ActionPlatform
				,ActionDate = GETDATE()
			WHERE LoginId = @LoginId
				AND AgentId = @AgentId
				AND ISNULL([Status], '') = 'A';

			SELECT 0 Code
				,'Success' Message
				,a.AgentId
				,b.UserId
				,a.ClubName1 AS ClubName
				,a.Email AS EmailAddress
				,a.Logo
				,@Session AS SessionId
				,b.LoginId AS Username
				,b.RoleId
				,ISNULL(b.IsPasswordForceful, 'Y') IsPasswordForceful
			FROM dbo.tbl_club_details a WITH (NOLOCK)
			INNER JOIN dbo.tbl_users b ON b.AgentId = a.AgentId
				AND ISNULL(b.STATUS, '') = 'A'
			INNER JOIN dbo.tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId
				AND c.RoleName = 'Club'
			WHERE b.LoginId = @LoginId
				AND a.AgentId = @AgentId
				AND PWDCOMPARE(@Password, b.Password) = 1;
		END;
		ELSE IF (
				SELECT b.FailedLoginAttempt
				FROM dbo.tbl_club_details a WITH (NOLOCK)
				INNER JOIN dbo.tbl_users b ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') = 'A'
				INNER JOIN dbo.tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId
					AND c.RoleName = 'Club'
				WHERE b.LoginId = @LoginId
					AND a.AgentId = @AgentId
				) = (@MaxFailedLoginAttempt - 1)
		BEGIN
			UPDATE dbo.tbl_users
			SET FailedLoginAttempt = ISNULL(FailedLoginAttempt, 0) + 1
				,ActionUser = @LoginId
				,ActionIP = @ActionIP
				,ActionPlatform = @ActionPlatform
				,ActionDate = GETDATE()
			WHERE LoginId = @LoginId
				AND AgentId = @AgentId;

			SELECT 1 Code
				,'Invalid credentials! <br/> Last attempt remaning.' Message;

			RETURN;
		END;
		ELSE
		BEGIN
			UPDATE dbo.tbl_users
			SET FailedLoginAttempt = ISNULL(FailedLoginAttempt, 0) + 1
				,ActionUser = @LoginId
				,ActionIP = @ActionIP
				,ActionPlatform = @ActionPlatform
				,ActionDate = GETDATE()
			WHERE LoginId = @LoginId;

			SELECT 1 Code
				,'Invalid credentials!' Message;

			RETURN;
		END;
	END;

	IF ISNULL(@Flag, '') = 'setpass'
	BEGIN
		IF @AgentId IS NULL
		BEGIN
			SELECT 1 Code
				,'Unable to set new password' Message
				,N'新しいパスワードを設定できません' Message2;

			RETURN;
		END

		IF NOT EXISTS (
				SELECT 'x'
				FROM tbl_users WITH (NOLOCK)
				WHERE AgentId = @AgentId
					AND ISNULL(STATUS, '') = 'A'
					AND ISNULL(IsPasswordForceful,'')='y'
					AND RoleId = 5
				)
		BEGIN
			SELECT 1 Code
				,'Unable to set new password' Message
				,N'新しいパスワードを設定できません' Message2;
			RETURN;
		END

		UPDATE tbl_users
		SET Password = PWDENCRYPT(@Password)
			,LastPasswordChangedDate = GETDATE()
			,LastLoginDate = GETDATE()
			,IsPasswordForceful = 'N'
		WHERE AgentId = @AgentId
		AND IsPasswordForceful='Y'
		AND ISNULL(STATUS, '') = 'A'
		AND RoleId = 5

		SELECT '0' code
			,'New password set successfully' message
			,N'新しいパスワードが正常に設定されました' Message2;
		RETURN
	END
END;
