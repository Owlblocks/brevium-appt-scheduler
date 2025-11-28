public partial class AppointmentScheduler
{
  private readonly int[] Doctors = { 1, 2, 3};
  public async Task Run()
  {
    await Start();

    AppointmentInfo[] initialSchedule = await GetInitialSchedule();
    Schedule = [.. initialSchedule];

    while (true)
    {
      await Task.Delay(100);
      AppointmentRequest? request = await NextRequest();
      if (request == null)
      {
        break;
      }

      Console.WriteLine(request + "\n");

      await HandleRequest(request);
    }

    await Stop();
  }

  private async Task HandleRequest(AppointmentRequest request)
  {
    var a = GetAvailableTimes(request.preferredDays.Select(DateTime.Parse), request.preferredDocs);
    foreach (var b in a)
    {
      Console.WriteLine(b.Item1.ToUniversalTime() + "\n");
    }
  }

  private (DateTime, int)[] GetAvailableTimes(IEnumerable<DateTime> dates, int[] doctors)
  {
    var taken = GetTakenTimes();
    return dates
      .SelectMany(GetAvailableTimes)
      .SelectMany(d => GetAvailableDoctorTimes(d, doctors))
      .Where((d) => !taken.Contains(d))
      .ToArray();
  }

  private IEnumerable<DateTime> GetAvailableTimes(DateTime date)
  {
    for (int i = 8; i <= 16; i++)
    {
      yield return date.AddHours(i);
    }
  }

  private IEnumerable<(DateTime, int)> GetAvailableDoctorTimes(DateTime date, int[] doctors)
  {
    foreach (int doctor in doctors)
    {
      yield return (date, doctor);
    }
  }

  private IEnumerable<(DateTime, int)> GetTakenTimes()
  {
    return Schedule.Select(app => (DateTime.Parse(app.appointmentTime), app.doctorId));
  }
}