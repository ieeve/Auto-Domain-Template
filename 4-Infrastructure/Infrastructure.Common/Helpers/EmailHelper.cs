using System.Net;
using System.Net.Mail;
using System.Text;


namespace Infrastructure.Common.Helpers
{
    /// <summary>
    /// Email操作类
    /// </summary>
    public class EmailHelper
    {
        #region [ 属性(发送Email相关) ]   

        /// <summary>  
        /// smtp 服务器   
        /// </summary>  
        public string SmtpHost { get; set; }
        /// <summary>  
        /// smtp 服务器端口  默认为25  
        /// </summary>  
        public int SmtpPort { get; set; }
        /// <summary>  
        /// 发送者 Eamil 地址  
        /// </summary>  
        public string FromEmailAddress { get; set; }

        /// <summary>  
        /// 发送者 Eamil 密码  
        /// </summary>  
        public string FormEmailPassword { get; set; }
        #endregion

        #region [ 属性(邮件相关) ]  
        /// <summary>  
        /// 收件人 Email 列表，多个邮件地址之间用 半角逗号 分开  
        /// </summary>  
        public string ToList { get; set; }
        /// <summary>  
        /// 邮件的抄送者，支持群发，多个邮件地址之间用 半角逗号 分开  
        /// </summary>  
        public string CcList { get; set; }
        /// <summary>  
        /// 邮件的密送者，支持群发，多个邮件地址之间用 半角逗号 分开  
        /// </summary>  
        public string BccList { get; set; }
        /// <summary>  
        /// 邮件标题  
        /// </summary>  
        public string Subject { get; set; }
        /// <summary>  
        /// 邮件正文  
        /// </summary>  
        public string Body { get; set; }

        /// <summary>  
        /// 邮件正文是否为Html格式  
        /// </summary>   
        public bool IsBodyHtml { get; set; } = true;

        /// <summary>  
        /// 附件列表  
        /// </summary>  
        public List<Attachment> AttachmentList { get; set; }
        #endregion

        #region [ 发送邮件 ]  
        /// <summary>  
        /// 发送邮件  
        /// </summary>  
        /// <returns></returns>  
        public void Send()
        {
            //实例化一个SmtpClient
            SmtpClient smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,//将smtp的出站方式设为 Network  
                EnableSsl = false, //smtp服务器是否启用SSL加密  
                Host = SmtpHost,//指定 smtp 服务器地址 
                Port = SmtpPort, //指定 smtp 服务器的端口，默认是25，如果采用默认端口，可省去  
                UseDefaultCredentials = true,//如果你的SMTP服务器不需要身份认证，则使用下面的方式，不过，目前基本没有不需要认证的了 
                Credentials = new NetworkCredential(FromEmailAddress, FormEmailPassword)//如果需要认证，则用下面的方式  
            };






            //实例化一个邮件类  
            MailMessage mm = new MailMessage
            {
                Priority = MailPriority.Normal,
                From = new MailAddress(FromEmailAddress, "系统自发邮件", Encoding.GetEncoding(936))
            };
            //邮件的优先级，分为 Low, Normal, High，通常用 Normal即可  

            //收件人  
            if (!string.IsNullOrEmpty(ToList))
                mm.To.Add(ToList);
            //抄送人  
            if (!string.IsNullOrEmpty(CcList))
                mm.CC.Add(CcList);
            //密送人  
            if (!string.IsNullOrEmpty(BccList))
                mm.Bcc.Add(BccList);

            mm.Subject = Subject;                      //邮件标题  
            mm.SubjectEncoding = Encoding.GetEncoding(936); //这里非常重要，如果你的邮件标题包含中文，这里一定要指定，否则对方收到的极有可能是乱码。  
            mm.IsBodyHtml = IsBodyHtml;                //邮件正文是否是HTML格式  
            mm.BodyEncoding = Encoding.GetEncoding(936);    //邮件正文的编码， 设置不正确， 接收者会收到乱码  
            mm.Body = Body;                            //邮件正文  
            //邮件附件  
            if (AttachmentList != null && AttachmentList.Count > 0)
            {
                foreach (Attachment attachment in AttachmentList)
                {
                    mm.Attachments.Add(attachment);
                }
            }
            //发送邮件，如果不返回异常， 则大功告成了。  
            smtp.Send(mm);
        }
        #endregion
    }
}
