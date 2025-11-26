public record AppointmentInfoRequest(
  int doctorId,
  int personId,
  string appointmentTime,
  bool isNewPatientAppointment,
  int requestId
)
{
  
}