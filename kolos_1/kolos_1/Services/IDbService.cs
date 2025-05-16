

using kolos_1.Models.DTOs;

public interface IDbService
{
    public Task<PatientsAppointmentDto> GetServiecesForPatientById(int id);
    public Task addAppointment(CreateAppointmentDto appointmentDto);

}