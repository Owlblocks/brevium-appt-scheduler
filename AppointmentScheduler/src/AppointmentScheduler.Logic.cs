public partial class AppointmentScheduler
{
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
    var available = GetAvailableTimes(request.preferredDays.Select(d => DateTime.Parse(d).ToUniversalTime()), request.preferredDocs);
    // foreach (var b in available)
    // {
    //   Console.WriteLine(b.Item1 + "\n");
    // }

    try
    {
      var chosen = available
        .Where(d => !request.isNew || d.time.Hour == 15 || d.time.Hour == 16)
        .Where(d => d.time.Year == 2021 && (d.time.Month == 11 || d.time.Month == 12))
        .Where(d => d.time.DayOfWeek != DayOfWeek.Sunday && d.time.DayOfWeek != DayOfWeek.Saturday)
        .Where(d => IsFarApart(request.personId, d.time))
        .First();

      AppointmentInfoRequest appointment = new AppointmentInfoRequest
        (chosen.doctor, request.personId, chosen.time.ToString("yyyy-MM-ddTHH:mm:ss.000Z"), request.isNew, request.requestId);
      
      Console.WriteLine(appointment + "\n");
      
      if (await ScheduleAppointment(appointment))
      {
        Schedule.Add(appointment.AppointmentInfo);
        Console.WriteLine("SUCCESS!");
      }
    }
    catch (InvalidOperationException e)
    {
      Console.WriteLine("Couldn't find slot?");
    }
    
  }

  private (DateTime time, int doctor)[] GetAvailableTimes(IEnumerable<DateTime> dates, int[] doctors)
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
    return Schedule.Select(app => (app.AppointmentDateTime, app.doctorId));
  }

  private bool IsFarApart(int personId, DateTime time)
  {
    var appointments = Schedule.Where(app => app.personId == personId);
    foreach (var appt in appointments)
    {
      if (time.Subtract(appt.AppointmentDateTime).Duration().Days < 7)
      {
        // Console.WriteLine(time.Subtract(appt.AppointmentDateTime).Duration().Days);
        return false;
      }
    }
    return true;
  }
}