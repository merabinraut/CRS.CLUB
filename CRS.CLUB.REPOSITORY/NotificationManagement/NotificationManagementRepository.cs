﻿using CRS.CLUB.SHARED.NotificationManagement;
using System.Collections.Generic;
using System.Linq;

namespace CRS.CLUB.REPOSITORY.NotificationManagement
{
    public class NotificationManagementRepository : INotificationManagementRepository
    {
        private readonly RepositoryDao _dao;
        public NotificationManagementRepository() => _dao = new RepositoryDao();

        public List<NotificationDetailCommon> GetNotification(string AgentId)
        {
            string SQL = "sproc_club_notification_management @Flag='s'";
            SQL += ",@AgentId=" + _dao.FilterString(AgentId);
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<NotificationDetailCommon>(dbResponse).ToList();
            return new List<NotificationDetailCommon>();
        }

        public List<NotificationDetailCommon> GetAllNotification(ManageNotificationCommon Request)
        {
            string SQL = "sproc_club_notification_management @Flag='s'";
            SQL += ",@AgentId=" + _dao.FilterString(Request.AgentId);
            SQL += !string.IsNullOrEmpty(Request.NotificationId) ? ",@NotificationId=" + _dao.FilterString(Request.NotificationId) : "";
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<NotificationDetailCommon>(dbResponse).ToList();
            return new List<NotificationDetailCommon>();
        }
    }
}
