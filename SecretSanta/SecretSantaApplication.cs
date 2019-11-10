using SecretSanta.EmailSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSanta
{
  public enum AppPhase
  {
    DrawNotStarted = 1,
    DrawCompleted = 2,
    DrawCompletedWithConflicts = 3,
    EmailSending = 4,
    Complete = 5,
    ConfigurationError = 6
  }

  public class SecretSantaApplication
  {
    private readonly AppConfig appConfig;
    private readonly IEmailSender emailSender;
    private AppPhase phase;

    public SecretSantaApplication(AppConfig appConfig, IEmailSender emailSender)
    {
      this.appConfig = appConfig;
      this.emailSender = emailSender;
    }

    private void InitialiseApplication()
    {
      phase = AppPhase.DrawNotStarted;
    }

    public void Run()
    {

      InitialiseApplication();
      

      Console.WriteLine("Welcome to Secret Santa. Would you like to draw?");

      while (phase != AppPhase.Complete && phase != AppPhase.ConfigurationError)
      {
        switch (phase)
        {
          case AppPhase.DrawNotStarted:
            Console.WriteLine("Press 'd' to draw, 'b' to test the assignments or 't' to send a test email");
            var input = GetKeyInput(new char[] { 'd', 't', 'b' });
            switch(input)
            {
              case 'd':
                DoDraw(false);
                break;

              case 'b':
                DoDraw(true);
                break;

              case 't':
                Console.WriteLine("Sending test email");
                var sent = emailSender.SendEmail(appConfig.Smtp.TestEmail, "This is a test email from Secret Santa");
                if (sent)
                {
                  Console.WriteLine("Test Email sent");
                } else { 
                  phase = AppPhase.ConfigurationError;
                }
                break;
            }
            
            break;
        }
      }
      
    }

    private void DoDraw(bool debug)
    {
      List<DrawResult> drawResults = new List<DrawResult>();
      var remainingInPool = new List<string>();
      foreach (var p in appConfig.People)
      {
        remainingInPool.Add(p.Name);
        drawResults.Add(new DrawResult
        {
          Person = p
        });
      }

      foreach (var res in drawResults)
      {
        var random = new Random().Next(0, remainingInPool.Count);
        res.AssignedTo = remainingInPool[random];
        remainingInPool.RemoveAt(random);
      }
      
      foreach (var res in drawResults)
      {
        if (debug)
        {
          Console.WriteLine($"{res.Person.Name} has picked {res.AssignedTo}");

        }
      }

    }

    private char GetKeyInput(char[] allowedCharacters)
    {
      char key = '#';
      bool keyAccepted = false;
      while (!keyAccepted)
      {
        key = Console.ReadKey().KeyChar;
        Console.WriteLine("");
        if (allowedCharacters.Contains(key))
        {
          keyAccepted = true;
        } else { 
          Console.WriteLine("Character invalid. Please try again");
        }
      }
      return key;
    }
  }
}
