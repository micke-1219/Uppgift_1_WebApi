using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Uppgift_1_WebApi.Data;
using Uppgift_1_WebApi.Models;

namespace Uppgift_1_WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceWorkersController : ControllerBase
    {
        private readonly SqlDbContext _context;
        public IConfiguration Configuration { get; }

        public ServiceWorkersController(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        // POST: api/ServiceWorkers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] ServiceWorkerRegisterModel model)
        {
            if (_context.ServiceWorkers.Any(serviceWorker => serviceWorker.Email == model.Email))
                return Conflict();

            try
            {
                var serviceWorker = new ServiceWorker()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };
                serviceWorker.CreatePasswordHash(model.Password);
                _context.ServiceWorkers.Add(serviceWorker);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return new BadRequestResult();
            }
            return new OkResult();
        }

        /*public async Task<ActionResult<ServiceWorker>> PostServiceWorker(ServiceWorker serviceWorker)
        {
            _context.ServiceWorkers.Add(serviceWorker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceWorker", new { id = serviceWorker.Id }, serviceWorker);
        }*/

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] ServiceWorkerLoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return new BadRequestObjectResult("Invalid email or password!");

            try
            {
                var serviceWorker = await _context.ServiceWorkers.SingleOrDefaultAsync(serviceWorker => serviceWorker.Email == model.Email);
                if (serviceWorker == null)
                    return new BadRequestObjectResult("User not found!");

                if (!serviceWorker.ValidatePasswordHash(model.Password))
                    return new BadRequestObjectResult("Invalid email or password!");

                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = Encoding.UTF8.GetBytes(Configuration.GetSection("SecretKey").Value);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, serviceWorker.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(secretKey), 
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new OkObjectResult(new
                {
                    Id = serviceWorker.Id,
                    Email = serviceWorker.Email,
                    Token = tokenString
                });
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/ServiceWorkers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceWorker(int id)
        {
            var serviceWorker = await _context.ServiceWorkers.FindAsync(id);
            if (serviceWorker == null)
            {
                return NotFound();
            }

            _context.ServiceWorkers.Remove(serviceWorker);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceWorkerExists(int id)
        {
            return _context.ServiceWorkers.Any(e => e.Id == id);
        }
    }
}
