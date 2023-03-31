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
            IEnumerable<SuperHero> heroes = await FindAllHeroes();

            return Ok(heroes);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHero>> FindById(int id)
        {
            try
            {
                var hero = await _conn.QueryFirstAsync<SuperHero>("select * from superheroes where id = @Id", new { Id = id });
                return Ok(hero);
            }
            catch(InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<SuperHero>> Insert([FromBody] SuperHero superHero)
        {
            await _conn.ExecuteAsync("insert into superheroes (name, firstname, lastname, place) values " +
                "(@Name, @FirstName, @LastName, @Place)", superHero);

            var lastInsertedId = await _conn.ExecuteScalarAsync<int>("select MAX(id) from superheroes");

            var insertedSuperHero = await _conn.QueryFirstAsync<SuperHero>("select * from superheroes where id = @Id", new { Id = lastInsertedId });

            return Ok(insertedSuperHero) ;
        }


        private async Task<IEnumerable<SuperHero>> FindAllHeroes()
        {
            return await _conn.QueryAsync<SuperHero>("select * from superheroes");
        }

    }
}
