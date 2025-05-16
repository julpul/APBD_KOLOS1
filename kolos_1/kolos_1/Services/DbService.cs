
using System.Data.Common;
using System.Security.Cryptography.Pkcs;
using kolos_1.Models.DTOs;
using Microsoft.Data.SqlClient;

public class DbService : IDbService
{
    readonly private string _connectionString;

    public DbService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default");
    }

   //Microsoft.Data.


   public async Task<PatientsAppointmentDto> GetServiecesForPatientById(int id)
   {
       var query = @"SELECT A.appoitment_id, A.date,P.first_name,P.last_name,P.date_of_birth,D.doctor_id,D.PWZ,A_S.service_fee,S.name
FROM Appointment A 
JOIN Patient P ON A.patient_id = P.patient_id
JOIN Doctor D ON A.doctor_id = D.doctor_id
JOIN Appointment_Service  A_S ON A.appoitment_id = A_S.appoitment_id
JOIN Service S ON A_S.service_id = S.service_id
WHERE A.appoitment_id = @appointment_id;";
       
       await using SqlConnection conn = new SqlConnection(_connectionString);
       await using SqlCommand cmd = new SqlCommand();
       
       cmd.Connection = conn;
       cmd.CommandText = query;
       
       await conn.OpenAsync();
       cmd.Parameters.AddWithValue("@appointment_id", id);
       PatientsAppointmentDto? dto = null;
       var reader = await cmd.ExecuteReaderAsync();
       while (reader.Read())
       {
           if (dto is null)
           {
               dto = new PatientsAppointmentDto()
               {
                   date = reader.GetDateTime(1),
                   patient = new PatientDto()
                   {
                       firstName = reader.GetString(2),
                       lastName = reader.GetString(3),
                       dateOfBirth = reader.GetDateTime(4),
                   },
                   doctor = new DoctorDto()
                   {
                       doctorId = reader.GetInt32(5),
                       pwz = reader.GetString(6),
                   },
                   appointmentServices = new List<appointmentServicesDto>()
               };
           }
           var appointmentId = reader.GetInt32(0);
           dto.appointmentServices.Add(new appointmentServicesDto()
           {
               name = reader.GetString(8),
               serviceFee = reader.GetDecimal(7),
           });
       }

       if (dto is null)
       {
           throw new Exception("No appointment found");
       }
       return dto;
   }

   public async Task addAppointment(CreateAppointmentDto appointmentDto)
   {
       await using SqlConnection conn = new SqlConnection(_connectionString);
       await using SqlCommand cmd = new SqlCommand(); 
       cmd.Connection = conn;
       await conn.OpenAsync();
       
       DbTransaction transaction = conn.BeginTransaction();
       cmd.Transaction = transaction as SqlTransaction;
       try
       {
           cmd.Parameters.Clear();
           var queue = @"SELECT 1 from Patient where patient_id = @patient_id;";
           cmd.CommandText = queue;
           cmd.Parameters.AddWithValue("@patient_id", appointmentDto.patientId);
           var reader = await cmd.ExecuteScalarAsync();
           if (reader is null)
           {
               throw new Exception("No patient details found");
           }

           cmd.Parameters.Clear();
           cmd.CommandText = @"SELECT doctor_id from Doctor where PWZ = @pwz;";
           cmd.Parameters.AddWithValue("@pwz", appointmentDto.pwz);
           var result = await cmd.ExecuteScalarAsync();
           if (result is null)
           {
               throw new Exception("No Doctor with this pwz");
           }
           int doctor_id = (int)result;
           cmd.Parameters.Clear();
           cmd.CommandText = @"INSERT INTO Appointment VALUES (@appoitment_id,@patient_id,@doctor_id,@date);";
           cmd.Parameters.AddWithValue("@appoitment_id", appointmentDto.appointmentId);
           cmd.Parameters.AddWithValue("@patient_id", appointmentDto.patientId);
           cmd.Parameters.AddWithValue("@doctor_id", doctor_id);
           cmd.Parameters.AddWithValue("@date", DateTime.Now);
           try
           {
               await cmd.ExecuteNonQueryAsync();
           }
           catch (Exception e)
           {
               throw new Exception("Could not insert appointment becaouse of the same Id");
           }

           foreach (var service in appointmentDto.services)
           {
               cmd.Parameters.Clear();
               cmd.CommandText = @"SELECT service_id FROM Service WHERE name = @name;";
               cmd.Parameters.AddWithValue("@name", service.serviceName);
               var res_service = await cmd.ExecuteScalarAsync();
               if (res_service is null)
               {
                   throw new Exception("No service found");
               }
               var service_id = (int)res_service;
               cmd.Parameters.Clear();
               
               cmd.CommandText = @"INSERT INTO Appointment_Service VALUES(@appoitment_id,@service_id,@service_fee);";
               cmd.Parameters.AddWithValue("@appoitment_id", appointmentDto.appointmentId);
               cmd.Parameters.AddWithValue("@service_id", service_id);
               cmd.Parameters.AddWithValue("@service_fee",service.serviceFee);
               
               await cmd.ExecuteNonQueryAsync();

           }
           transaction.Commit();
       }catch (Exception e)
       {
           await cmd.Transaction.RollbackAsync();
           throw;
       }
   }
}