public record AppointmentInfoRequest(
  int doctorId,
  int personId,
  string appointmentTime,
  bool isNewPatientAppointment,
  int requestId
)
{
  public AppointmentInfo AppointmentInfo => new AppointmentInfo(doctorId, personId, appointmentTime, isNewPatientAppointment);
}