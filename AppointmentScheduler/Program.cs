using  System.Net.Http;
using Microsoft.Extensions.Configuration;
 
var sched = new AppointmentScheduler();

public class AppointmentScheduler
{
  private string? APIKey { get; init; }

  public AppointmentScheduler()
  {
    ConfigurationBuilder configBuilder = new ConfigurationBuilder();
    IConfiguration config = configBuilder.AddUserSecrets<AppointmentScheduler>().Build();
    APIKey = config["APIKey"];
    if (APIKey == null)
    {
      throw new Exception("Must Set User Secret 'APIKey'!");
    }
  }
  
}