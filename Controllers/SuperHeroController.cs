using Dapper;
using DapperCrud.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace DapperCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IDbConnection _conn;

        public SuperHeroController(MySqlConnection conn)
        {
            _conn = conn;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> FindAll()
        {
            var heroes = await _conn.QueryAsync<SuperHero>("select * from superheroes");

            return Ok(heroes);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHero>> FindById(int id)
        {
            try
            {
                var hero = await _conn.QueryFirstAsync<SuperHero>($"select * from superheroes where id = @Id", new { Id = id });
                return Ok(hero);
            }
            catch(InvalidOperationException)
            {
                return NotFound();
            }
        }

    }
}
