public partial class AppointmentScheduler
{
  public async Task Run()
  {
    await Start();

    Console.WriteLine(await GetInitialSchedule());
    await ScheduleAppointment(new AppointmentInfoRequest(0, 0, "2025-11-26T21:00:37.620Z", true, 0));

    await Stop();
  }
}