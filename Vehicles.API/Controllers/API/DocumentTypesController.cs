using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Vehicles.API.Data;
using Vehicles.API.Data.Entities;

namespace Vehicles.API.Controllers.API
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DocumentTypesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public DocumentTypesController(DataContext dataContext)
        {
           _dataContext = dataContext;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentType>> GetDocumentType(int id)
        {
            DocumentType documentType = await _dataContext.DocumentTypes.FindAsync(id);

            if (documentType == null)
            {
                return NotFound();
            }

            return documentType;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocumentType(int id, DocumentType documentType)
        {
            if (id != documentType.Id)
            {
                return BadRequest();
            }

            _dataContext.Entry(documentType).State = EntityState.Modified;

            try
            {
                await _dataContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe tipo de documento.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DocumentType>> PostDocumentType(DocumentType documentType)
        {
            _dataContext.DocumentTypes.Add(documentType);

            try
            {
                await _dataContext.SaveChangesAsync();
                return CreatedAtAction("GetDocumentType", new { id = documentType.Id }, documentType);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe tipo de documento.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            DocumentType documentType = await _dataContext.DocumentTypes.FindAsync(id);
            if (documentType == null)
            {
                return NotFound();
            }

            _dataContext.DocumentTypes.Remove(documentType);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
