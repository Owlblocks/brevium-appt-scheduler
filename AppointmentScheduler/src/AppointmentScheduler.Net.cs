using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public partial class AppointmentScheduler
{
  private string? APIKey { get; init; }
  private HttpClient Client { get; init; }

  public AppointmentScheduler()
  {
    ConfigurationBuilder configBuilder = new ConfigurationBuilder();
    IConfiguration config = configBuilder.AddUserSecrets<AppointmentScheduler>().Build();
    APIKey = config["APIKey"];
    if (APIKey == null)
    {
      throw new Exception("Must Set User Secret 'APIKey'!");
    }
    Client = new HttpClient();
    Client.BaseAddress = new Uri("https://scheduling.interviews.brevium.com/api/Scheduling/");
  }

  private async Task Start()
  {
    var resp = await Client.PostAsync($"Start?token={APIKey}", null);
    resp.EnsureSuccessStatusCode();
  }

  private async Task<AppointmentInfo[]> Stop()
  {
    var resp = await Client.PostAsync($"Stop?token={APIKey}", null);
    resp.EnsureSuccessStatusCode();
    AppointmentInfo[] schedule = JsonSerializer.Deserialize<AppointmentInfo[]>(resp.Content.ReadAsStream()) ?? [];
    return schedule;
  }

  private async Task<AppointmentRequest?> NextRequest()
  {
    var resp = await Client.GetAsync($"AppointmentRequest?token={APIKey}");
    resp.EnsureSuccessStatusCode();
    AppointmentRequest? req = JsonSerializer.Deserialize<AppointmentRequest>(resp.Content.ReadAsStream());
    return req;
  }

  private async Task<AppointmentInfo[]> GetInitialSchedule()
  {
    var resp = await Client.GetAsync($"Schedule?token={APIKey}");
    resp.EnsureSuccessStatusCode();
    AppointmentInfo[] schedule = JsonSerializer.Deserialize<AppointmentInfo[]>(resp.Content.ReadAsStream()) ?? [];
    return schedule;
  }

  private async Task ScheduleAppointment(AppointmentInfoRequest appt)
  {
    string? error = null;
    try
    {
      var resp = await Client.PostAsync($"Schedule?token={APIKey}", JsonContent.Create(appt));
      error = await resp.Content.ReadAsStringAsync();
      resp.EnsureSuccessStatusCode();
    } catch (HttpRequestException e)
    {
      Console.WriteLine(error ?? e.Message);
    }    
  }

  
  
}