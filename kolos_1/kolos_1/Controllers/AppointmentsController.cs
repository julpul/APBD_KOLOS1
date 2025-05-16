using kolos_1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace kolos_1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController:ControllerBase
{
    readonly private IDbService _dbService;

    public AppointmentsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [Route("{id}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        try
        {
            var res = await _dbService.GetServiecesForPatientById(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointmentRequest([FromBody]CreateAppointmentDto appointmentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            await _dbService.addAppointment(appointmentDto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}