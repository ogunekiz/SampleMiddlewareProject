using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleMiddlewareProject.Data;
using SampleMiddlewareProject.Models;

namespace SampleMiddlewareProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public PersonController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("GetPersons")]
        public async Task<List<Person>> GetPersons()
        {
            return await _dataContext.Persons.ToListAsync();
        }

        [HttpPost("CreatePerson")]
        public async Task<Person?> CreatePerson(Person person)
        {
            if (person != null)
            {
                _dataContext.Persons.Add(person);
                await _dataContext.SaveChangesAsync();
                return person;
            }
            else
            {
                return null;
            }
        }
    }
}
