USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_customer_notification_management]    Script Date: 11/4/2023 6:58:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sproc_club_notification_management] (
	@Flag CHAR(5) = NULL
	,@notificationId VARCHAR(50) = NULL
	,@AgentId NVARCHAR(200) = NULL
	,@NotificationSubject NVARCHAR(512) = NULL
	,@NotificationBody VARCHAR(512) = NULL
	,@NotificationImageURL VARCHAR(512) = NULL
	,@NotificationStatus CHAR(1) = 'Y'
	,@NotificationReadStatus CHAR(1) = 'N'
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIp VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @ErrorDesc VARCHAR(MAX);

		--select all notification for specific club
		IF @FLAG = 's'
		BEGIN
			SELECT notificationId
				,ToAgentId
				,NotificationType
				,NotificationSubject Subject
				,NotificationBody Body
				,NotificationImageURL ImageUrl
				,CASE 
					WHEN DATEDIFF(SECOND, CreatedDate, GETDATE()) < 60
						THEN CAST(DATEDIFF(SECOND, CreatedDate, GETDATE()) AS NVARCHAR) + ' seconds ago'
					WHEN DATEDIFF(MINUTE, CreatedDate, GETDATE()) < 60
						THEN CAST(DATEDIFF(MINUTE, CreatedDate, GETDATE()) AS NVARCHAR) + ' minutes ago'
					WHEN DATEDIFF(HOUR, CreatedDate, GETDATE()) < 24
						THEN CAST(DATEDIFF(HOUR, CreatedDate, GETDATE()) AS NVARCHAR) + ' hours ago'
					ELSE CAST(DATEDIFF(DAY, CreatedDate, GETDATE()) AS NVARCHAR) + ' days ago'
					END AS TIME
			FROM tbl_club_notification WITH (NOLOCK)
			WHERE notificationReadStatus <> 'Y'
				AND toAgentId = @AgentId
		END

		--insert notification details
		IF @Flag = 'i'
		BEGIN
			INSERT INTO tbl_club_notification (
				ToAgentId
				,NotificationSubject
				,NotificationBody
				,NotificationImageURL
				,NotificationStatus
				,NotificationReadStatus
				,CreatedBy
				,CreatedDate
				)
			VALUES (
				@AgentId
				,@NotificationSubject
				,@NotificationBody
				,@NotificationImageURL
				,@NotificationStatus
				,@NotificationReadStatus
				,@ActionUser
				,GETDATE()
				)

			SET @notificationId = SCOPE_IDENTITY();

			UPDATE tbl_club_notification
			SET notificationId = @notificationId
			WHERE Sno = @notificationId
				AND ToAgentId = ISNULL(@agentId, ToAgentId);

			SELECT '0' code
				,'Notification inserted successfully' message

			RETURN;
		END

		--update notification details
		IF @Flag = 'u'
		BEGIN
			UPDATE tbl_club_notification
			SET notificationStatus = ISNULL(@NotificationStatus, notificationStatus)
				,notificationReadStatus = ISNULL(@NotificationReadStatus, notificationReadStatus)
				,UpdatedBy = ISNULL(@ActionUser, UpdatedBy)
				,UpdatedDate = GETDATE()
			WHERE ToAgentId = @AgentId
		END
	END TRY

	BEGIN CATCH
		SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

		INSERT INTO tbl_error_log (
			ErrorDesc
			,ErrorScript
			,QueryString
			,ErrorCategory
			,ErrorSource
			,ActionDate
			)
		VALUES (
			@ErrorDesc
			,'sproc_club_notification_management(Flag: ' + ISNULL(@Flag, '') + ')'
			,'SQL'
			,'SQL'
			,'sproc_club_notification_management'
			,GETDATE()
			);

		SELECT 1 Code
			,'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;

		RETURN;
	END CATCH
END
