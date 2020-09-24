using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAuthenticationExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JWTAuthenticationExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {/// <summary>
    /// not in use
    /// </summary>
    /// <param name="db"></param>
        public UserController(AppDb db)
        {
            Db = db;
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Policy =Policies.User)]
        public IActionResult GetUserData()
        {
            return Ok("This is a response from user method");
        }


        [HttpGet]
        [Route("GetAdminData")]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult GetAdminData()
        {
            return Ok("This is a response from Admin method");
        }


        // GET api/blog
        [HttpGet]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> GetLatest()
        {
            await Db.Connection.OpenAsync();
            var query = new Operations(Db);
            var result = await query.LatestPostsAsync();
            return new OkObjectResult(result);
        }

        // GET api/blog/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Operations(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        // POST api/blog
        [HttpPost]
        public IActionResult Post([FromBody]Operations body)
        {
           Db.Connection.Open();
            body.Db = Db;
             body.InsertAsync();
            Db.Connection.Close();
            return new OkObjectResult(body);
        }

        // PUT api/blog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOne(int id, [FromBody]Operations body)
        {
            await Db.Connection.OpenAsync();
            var query = new Operations(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.Title = body.Title;
            result.Content = body.Content;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE api/blog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Operations(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkResult();
        }

        // DELETE api/blog
        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await Db.Connection.OpenAsync();
            var query = new Operations(Db);
            await query.DeleteAllAsync();
            return new OkResult();
        }

        public AppDb Db { get; }
    }
}

