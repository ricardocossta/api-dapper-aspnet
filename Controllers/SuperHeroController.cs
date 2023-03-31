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
                var hero = await FindHeroById(id);
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
            SuperHero insertedSuperHero = await FindHeroById(lastInsertedId);

            return Ok(insertedSuperHero);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] SuperHero superHero)
        {
            superHero.Id = id;
            await _conn.ExecuteAsync("update superheroes set name = @Name, firstname = @FirstName, " +
                "lastname = @LastName, place = @Place where id = @Id", superHero);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _conn.ExecuteAsync("delete from superheroes where id = @Id", new { Id = id });
            return NoContent();
        }

        private async Task<SuperHero> FindHeroById(int id)
        {
            return await _conn.QueryFirstAsync<SuperHero>("select * from superheroes where id = @Id", new { Id = id });
        }

        private async Task<IEnumerable<SuperHero>> FindAllHeroes()
        {
            return await _conn.QueryAsync<SuperHero>("select * from superheroes");
        }

    }
}
