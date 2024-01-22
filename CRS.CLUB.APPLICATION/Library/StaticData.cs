﻿using CRS.CLUB.APPLICATION.CustomHelpers;
using CRS.CLUB.SHARED;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CRS.CLUB.APPLICATION.Library
{
    public static class StaticData
    {
        public static string GetUser()
        {
            var user = ReadSession("UserName", "");
            if (null == user)
            {
                HttpContext.Current.Response.Redirect("/Home");
            }

            if (ReadSession("ForcedPwdChange", "").ToUpper() == "Y")
            {
                HttpContext.Current.Response.Redirect("/ChangeUserPassword");
            }
            return user;
        }

        public static string ReadSession(string key, string defVal)
        {
            try
            {
                var User = HttpContext.Current.Session[key] == null ? defVal : HttpContext.Current.Session[key].ToString();
                return User;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static HtmlHelper GetHtmlHelper(this Controller controller)
        {
            var viewContext = new ViewContext(controller.ControllerContext, new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
            return new HtmlHelper(viewContext, new ViewPage());
        }
        public static string GetActions(string ControlText, string Id, Controller cont, string controlRoleUrl = "", string ControlUrl = "", string ExtraId1 = "", string ExtraId2 = "", string ExtraId3 = "", string ExtraId4 = "", string ExtraId5 = "", string ExtraId6 = "", bool DisableAddEdit = false, bool IsEnable = false, bool IsDistributionStatus = false)
        {
            var link = "";
            link += " <div class=\"btn-group\">";
            ExtraId1 = ExtraId1 ?? "";
            ExtraId2 = ExtraId2 ?? "";
            ExtraId3 = ExtraId3 ?? "";
            ExtraId1 = ExtraId1.Trim();
            var h = GetHtmlHelper(cont);
            switch (ControlText.ToUpper())
            {
                case "CLUBLIST":
                    {
                        link += h.NHyperLink("Detail", new { @class = "btn-sm btn-primary" }, "/ClubManagement/ClubDetails", "/ClubManagement/ClubDetails?AgentId=" + Id, true, "fas fa-eye").ToHtmlString();
                        link += h.NHyperLink("Manage", new { @class = "btn-sm btn-primary" }, "/ClubManagement/ManageClub", "/ClubManagement/ManageClub?AgentId=" + Id, true, "fas fa-edit").ToHtmlString();
                        if (ExtraId1.ToUpper() == "A") link += h.NHyperLink("Block", new { @class = "btn-sm btn-danger", @onclick = "BlockEvent('" + Id + "')" }, "/ClubManagement/BlockClub", "#", true, "fa fa-lock").ToHtmlString();
                        else link += h.NHyperLink("Un-Block", new { @class = "btn-sm btn-success", @onclick = "UnblockEvent('" + Id + "')" }, "/ClubManagement/UnBlockClub", "#", true, "fa fa-unlock-alt").ToHtmlString();
                        link += h.NHyperLink("Reset Password", new { @class = "btn-sm btn-primary", @onclick = "ResetPasswordEvent('" + Id + "')" }, "/ClubManagement/ResetClubPassword?id=" + Id, "#", true, "fas fa-key").ToHtmlString();
                        link += h.NHyperLink("Manage Host", new { @class = "btn-sm btn-primary" }, "/HostManagement/HostList", "/HostManagement/HostList?AgentId=" + Id, true, "fas fa-users").ToHtmlString();
                        break;
                    }
                case "CLUBMANAGEMENT":
                    {
                        link += h.NHyperLink("Edit", new { @class = "btn-sm btn-primary" }, "/ClubManagement/EditClubManagement", "/ClubManagement/EditClubManagement?AgentId=" + Id + "&clubsno=" + ExtraId2, true, "fas fa-edit").ToHtmlString();
                        link += h.NHyperLink("Delete", new { @class = "btn-sm btn-danger" }, "/ClubManagement/DeleteClubManagement", "/ClubManagement/DeleteClubManagement?AgentId=" + Id + "&clubsno=" + ExtraId2, true, "fas fa-users").ToHtmlString();
                        break;
                    }
                case "HOSTMANAGEMENT":
                    {
                        link += h.NHyperLink("Edit", new { @class = "btn-sm btn-primary" }, "/ClubManagement/EditHostManagement", "/ClubManagement/EditHostManagement?AgentId=" + Id + "&HostId=" + ExtraId2, true, "fas fa-edit").ToHtmlString();
                        link += h.NHyperLink("Delete", new { @class = "btn-sm btn-danger" }, "/ClubManagement/DeleteHostManagement", "/ClubManagement/DeleteHostManagement?AgentId=" + Id + "&HostId=" + ExtraId2, true, "fas fa-users").ToHtmlString();
                        break;
                    }
                case "EVENTMANAGEMENT":
                    {
                        link += h.NHyperLink("Edit", new { @class = "btn-sm btn-primary" }, "/ClubManagement/EditEventManagement", "/ClubManagement/EditEventManagement?AgentId=" + Id + "&EventId=" + ExtraId2, true, "fas fa-edit").ToHtmlString();
                        link += h.NHyperLink("Delete", new { @class = "btn-sm btn-danger" }, "/ClubManagement/DeleteEventManagement", "/ClubManagement/DeleteEventManagement?AgentId=" + Id + "&EventId=" + ExtraId2, true, "fas fa-users").ToHtmlString();
                        break;
                    }
                default:
                    break;
            };
            link += "</div>";
            return link;
        }

        private static string SaltKey = "";
        static string salt1 = "&%$%#@#";
        static string salt2 = "@$^#%^";

        public static string Base64Encode(string plainText)
        {
            plainText = plainText + SaltKey;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                base64EncodedData = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                base64EncodedData = base64EncodedData.Replace(SaltKey, "");
            }
            catch (Exception e)
            {
                base64EncodedData = "";
            }

            return base64EncodedData;
        }

        public static string Base64Encode_URL(string plainText)
        {
            var enc = "";
            try
            {
                plainText = salt1 + plainText + salt2;
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                enc = System.Convert.ToBase64String(plainTextBytes);
                enc = enc.Replace("=", "000");
            }
            catch (Exception)
            {
                enc = "";
            }

            return enc;
        }
        public static string Base64Decode_URL(string base64EncodedData)
        {
            if (base64EncodedData == "Index" || string.IsNullOrWhiteSpace(base64EncodedData))
            {
                return "";
            }
            try
            {
                base64EncodedData = base64EncodedData.Replace("000", "=");
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                base64EncodedData = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                base64EncodedData = base64EncodedData.Replace(salt1, "");
                base64EncodedData = base64EncodedData.Replace(salt2, "");
            }
            catch (Exception e)
            {
                base64EncodedData = "";
            }

            return base64EncodedData;
        }
        public static string GetIdFromQuery()
        {
            string getEnc = "";
            if (HttpContext.Current.Request.QueryString.Count > 0)
            {
                getEnc = HttpContext.Current.Request.QueryString[0];
            }

            return StaticData.Base64Decode_URL(getEnc);
        }
        public static CommonDbResponse GetSessionMessage()
        {
            var resp = HttpContext.Current.Session["Msg"] as CRS.CLUB.SHARED.CommonDbResponse;
            HttpContext.Current.Session.Remove("Msg");
            return resp;
        }
        public static string MakeXmlByComa(string value)
        {
            string val = "<root>";
            foreach (var item in value.Split(','))
            {
                val += @"<row id=""" + item + "\" />";
            }
            val += "</root>";
            return val;
        }
        public static void SetMessageInSession(CommonDbResponse resp)
        {
            HttpContext.Current.Session["Msg"] = resp;
        }
    }
    public class FakeView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}