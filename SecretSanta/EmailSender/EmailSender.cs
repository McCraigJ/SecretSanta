using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security;
using System.Text;

namespace SecretSanta.EmailSender
{
  public class EmailSender : IEmailSender
  {
    private readonly AppConfig appConfig;
    private SmtpClient smtpClient;

    public EmailSender(AppConfig appConfig)
    {
      this.appConfig = appConfig;
    }

    public bool SendEmail(string to, string body)
    {
      if (string.IsNullOrEmpty(appConfig.Smtp.Password) && appConfig.Smtp.UseAuthentication)
      {
        Console.WriteLine("Please enter your email password");
        appConfig.Smtp.Password = GetPassword();
      }
      MailMessage mail = new MailMessage(appConfig.Smtp.From, to, appConfig.Smtp.Subject, body);

      var client = GetSmtpClient();

      try
      {
        client.Send(mail);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred sending an email: {ex.Message}");
        appConfig.Smtp.Password = "";
        smtpClient = null;

        return false;
      }

      return true;
    }

    private SmtpClient GetSmtpClient()
    {
      if (smtpClient == null)
      {
        smtpClient = new SmtpClient(appConfig.Smtp.Host);
        smtpClient.Port = appConfig.Smtp.Port;
        smtpClient.EnableSsl = true;
        
        if (appConfig.Smtp.UseAuthentication)
        {
          smtpClient.Credentials =
            new System.Net.NetworkCredential(appConfig.Smtp.UserName, appConfig.Smtp.Password);
        }

      }
      return smtpClient;
    }

    public string GetPassword()
    {
      var pwd = "";
      while (true)
      {
        ConsoleKeyInfo i = Console.ReadKey(true);
        if (i.Key == ConsoleKey.Enter)
        {
          break;
        }
        else if (i.Key == ConsoleKey.Backspace)
        {
          if (pwd.Length > 0)
          {
            pwd.Substring(0, pwd.Length -1);
            Console.Write("\b \b");
          }
        }
        else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
        {
          pwd += (i.KeyChar);
          Console.Write("*");
        }
      }
      return pwd;
    }
  }
}
