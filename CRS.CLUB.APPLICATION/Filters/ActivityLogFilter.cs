﻿using CRS.CLUB.APPLICATION.Library;
using CRS.CLUB.BUSINESS.NotificationManagement;
using CRS.CLUB.SHARED;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CRS.CLUB.APPLICATION.Filters
{
    public class ActivityLogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext httpctx = HttpContext.Current;
            string url = httpctx.Request.Url.AbsoluteUri;
            string pageName = httpctx.Request.RequestContext.RouteData.GetRequiredString("action");
            var browserDetails = ApplicationUtilities.GetBrowserDetails();
            string username = httpctx.Session["UserName"] == null ? "system" : httpctx.Session["UserName"].ToString();
            string ip = ApplicationUtilities.GetIP();
            AddActitivies(pageName, url, browserDetails, ip, "", username);
            //check session
            var getSession = ApplicationUtilities.GetSessionValue("SessionGuid").ToString();
            var getUserId = ApplicationUtilities.GetSessionValue("UserId").ToString().DecryptParameter();
            var getAgentId = ApplicationUtilities.GetSessionValue("AgentId").ToString().DecryptParameter();
            var roleid = ApplicationUtilities.GetSessionValue("RoleId").ToString().DecryptParameter();
            CommonDbResponse resp = CheckSession(getUserId, getSession, roleid);
            if (resp != null)
            {
                if (resp.Code != 0)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                        { "Controller", "Home" },
                        { "Action", "LogOff" }
                        });
                }
                var _notificationBuss = new NotificationManagementBusiness();
                var notifications = _notificationBuss.GetNotification(getAgentId);
                httpctx.Session["Notifications"] = notifications;
            }
            base.OnActionExecuting(filterContext);
        }
        public CommonDbResponse AddActitivies(string page_name, string page_url, string browser, string ip, string logtype, string usernname)
        {
            CommonDbResponse cResponse = new CommonDbResponse();
            //ActivityLogBusiness ab = new ActivityLogBusiness();
            //ActivityLog al = new ActivityLog()
            //{
            //    page_name = page_name,
            //    page_url = page_url,
            //    browser_detail = browser,
            //    ipaddress = ip,
            //    log_type = logtype,
            //    user_name = usernname
            //};
            //cResponse = ab.InsertActivityLog(al);
            return cResponse;
        }
        public CommonDbResponse CheckSession(string UserId, string SessionId, string RoleId)
        {
            CommonDbResponse cResponse = new CommonDbResponse();
            //LoginBusiness buss = new LoginBusiness();
            //CheckSession model = new CheckSession()
            //{
            //    UserId = UserId,
            //    SessionId = SessionId,
            //    RoleId = RoleId
            //};
            //cResponse = buss.CheckUserSession(model);
            return cResponse;
        }
    }
}