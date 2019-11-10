using System;
using System.Collections.Generic;
using System.Text;

namespace SecretSanta.EmailSender
{
  public interface IEmailSender
  {
    bool SendEmail(string to, string body);
  }
}
