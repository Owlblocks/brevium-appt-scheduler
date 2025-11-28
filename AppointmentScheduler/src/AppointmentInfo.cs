public record AppointmentInfo(
  int doctorId,
  int personId,
  string appointmentTime,
  bool isNewPatientAppointment
)
{
  public DateTime AppointmentDateTime => DateTime.Parse(appointmentTime);
}