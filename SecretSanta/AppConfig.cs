using System;
using System.Collections.Generic;
using System.Text;

namespace SecretSanta
{
  public class AppConfigSmtpSettings
  {
    public string Host { get; set; }
    public bool UseAuthentication { get; set; }
    public string UserName { get; set; }
    public int Port { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string Subject { get; set; }
    public string TestEmail { get; set; }
  }


  public class Person
  {
    public string Name { get; set; }
    public string Email { get; set; }
  }

  public class AppConfig
  {
    public AppConfigSmtpSettings Smtp { get; set; }
    public Person[] People { get; set; }
  }
}
