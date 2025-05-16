namespace kolos_1.Models.DTOs;

public class CreateAppointmentDto
{
    public int appointmentId { get; set; }
    public int patientId { get; set; }
    public string pwz { get; set; }
    public List<ServicesDto> services { get; set; }
}

public class ServicesDto
{
    public string serviceName { get; set; }
    public decimal serviceFee { get; set; }
}