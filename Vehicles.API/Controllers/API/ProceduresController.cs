using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.API.Data;
using Vehicles.API.Data.Entities;

namespace Vehicles.API.Controllers.API
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ProceduresController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ProceduresController(DataContext dataContext){
            _dataContext = dataContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Procedure>>> GetProcedures()
        {
            return await _dataContext.Procedures.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Procedure>> GetProcedure(int id)
        {
            Procedure procedure = await _dataContext.Procedures.FindAsync(id);

            if (procedure == null)
            {
                return NotFound();
            }

            return procedure;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcedure(int id, Procedure procedure)
        {
            if (id != procedure.Id)
            {
                return BadRequest();
            }

            _dataContext.Entry(procedure).State = EntityState.Modified;

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcedureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Procedure>> PostProcedure(Procedure procedure)
        {
            _dataContext.Procedures.Add(procedure);
            await _dataContext.SaveChangesAsync();

            return CreatedAtAction("GetProcedure", new { id = procedure.Id }, procedure);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcedure(int id)
        {
            Procedure procedure = await _dataContext.Procedures.FindAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }

            _dataContext.Procedures.Remove(procedure);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ProcedureExists(int id)
        {
            return _dataContext.Procedures.Any(e => e.Id == id);
        }
    }
}
