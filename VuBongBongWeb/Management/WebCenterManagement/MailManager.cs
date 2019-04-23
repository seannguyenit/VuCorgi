using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public static class MailManager
    {
        #region Private system
        private const string SystempEmail = "seannguyen.arthurideaco@gmail.com";
        private const string user = "seannguyen.arthurideaco@gmail.com";
        private const string pass = "Thinh_250892";

        #endregion

        /// <summary>
        /// function send mail
        /// </summary>
        /// <param name="to">to email</param>
        /// <param name="subject">title of mail</param>
        /// <param name="body">contain</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool SendEmail(string to, string subject, string body, out string error)
        {
            error = string.Empty;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(SystempEmail);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(user, pass);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public static bool SendRegisterEmail(User newOne, string codehref, out string error)
        {
            error = string.Empty;
            try
            {
                string footerAutoEmail = string.Format("{0}\n{1}\n{2}", "Lưu ý: Đây là email tự động gửi của hệ thống.", "Chúng tôi không bao giờ cung cấp hay yêu cầu mật khẩu của bạn.", "Đây là email tự động, vui lòng không trả lời mail này. Cảm ơn bạn đã tương tác.");
                string link = codehref.Replace("%3F", "?");
                string title = "Xác nhận đăng ký thành viên";
                string body = "Bạn hoặc ai đó đã sử dụng địa chỉ email này để đăng ký thành viên website ytuongvagiaiphap.com với username là " + newOne.UserName + ".";
                body = string.Format("{0}\n", body);
                body = string.Format("{0}\n{1}", body, "Vui lòng nhấp vào link dưới đây để hoàn tất việc đăng ký. Lưu ý email chỉ có hiệu lực trong 24h !");
                body = string.Format("{0}\n", body);
                body = string.Format("{0}\n{1}", body, link);
                body = string.Format("{0}\n{1}", body, footerAutoEmail);
                return SendEmail(newOne.Email, title, body, out error);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }
    }
}