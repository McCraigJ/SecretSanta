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
    EmailSending = 2,
    Complete = 3,
    ConfigurationError = 4
  }

  public class SecretSantaApplication
  {
    private readonly AppConfig appConfig;
    private readonly IEmailSender emailSender;
    private AppPhase phase;

    private List<DrawResult> drawResults;

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
            switch (input)
            {
              case 'd':
              case 'b':
                var debug = input == 'b';
                var success = false;
                var attempts = 0;
                var maxAttempts = 10;
                while (!success && attempts < maxAttempts)
                {
                  success = DoDraw(debug);
                  attempts++;
                }
                if (success)
                {
                  phase = debug ? AppPhase.DrawNotStarted : AppPhase.EmailSending;
                }
                else
                {
                  Console.WriteLine("Failed to pick Secret Santa recipients");
                }

                break;

              case 't':
                Console.WriteLine("Sending test email");
                var sent = emailSender.SendEmail(appConfig.Smtp.TestEmail, "This is a test email from\n\r Secret Santa");
                if (sent)
                {
                  Console.WriteLine("Test Email sent");
                }
                else
                {
                  phase = AppPhase.ConfigurationError;
                }
                break;
            }

            break;

          case AppPhase.EmailSending:
            foreach (var result in drawResults)
            {
              var sent = emailSender.SendEmail(result.Person.Email,
                $"Hi, {result.Person.Name},\n\rYou have been assigned {result.AssignedTo} in the Secret Santa draw.");

              if (!sent)
              {
                phase = AppPhase.ConfigurationError;
                break;
              }
            }
            if (phase != AppPhase.ConfigurationError)
            {
              phase = AppPhase.Complete;
            }
            break;
        }
      }

      Console.WriteLine("Complete: Press any key to quit");
      Console.ReadKey();

    }

    private bool DoDraw(bool debug)
    {
      drawResults = new List<DrawResult>();
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

        var picked = false;
        var attempts = 0;
        var maxAttempts = appConfig.People.Count();
        while (!picked && attempts < maxAttempts)
        {
          var pickIndex = new Random().Next(0, remainingInPool.Count);
          var pick = remainingInPool[pickIndex];
          if (pick != res.Person.Name)
          {
            picked = true;
            res.AssignedTo = pick;
            remainingInPool.RemoveAt(pickIndex);
          }
          attempts++;
        }

        if (picked == false)
        {
          //if (debug)
          //{
          //  Console.WriteLine("Failed to pick results");
          //  DebugPrintResults(drawResults);
          //}
          return false;
        }

      }

      if (debug)
      {
        Console.WriteLine("Successfully picked results");
        DebugPrintResults(drawResults);
      }

      return true;

    }

    private void DebugPrintResults(List<DrawResult> drawResults)
    {
      foreach (var res in drawResults)
      {
        Console.WriteLine($"{res.Person.Name} has picked {res.AssignedTo}");
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
        }
        else
        {
          Console.WriteLine("Character invalid. Please try again");
        }
      }
      return key;
    }
  }
}
