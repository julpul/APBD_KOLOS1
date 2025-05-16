namespace kolos_1.Models.DTOs;

public class PatientsAppointmentDto
{
    public DateTime date { get; set; } 
    public PatientDto patient { get; set; }
    public DoctorDto doctor { get; set; }
    public List<appointmentServicesDto> appointmentServices { get; set; }
}

public class PatientDto
{
    public string firstName { get; set; }= string.Empty;
    public string lastName { get; set; }= string.Empty;
    public DateTime dateOfBirth { get; set; }
}

public class DoctorDto
{
    public int doctorId { get; set; }
    public string pwz { get; set; }= string.Empty;
}

public class appointmentServicesDto
{
    public string name { get; set; }= string.Empty;
    public decimal serviceFee { get; set; }
}