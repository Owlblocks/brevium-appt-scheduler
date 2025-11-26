public record AppointmentRequest(
  int requestId,
  int personId,
  string[] preferredDays,
  int[] preferredDocs,
  bool isNew
)
{
  
}