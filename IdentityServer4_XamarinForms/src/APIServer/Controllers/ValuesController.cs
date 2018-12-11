
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIServer.Models;
namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<object> Get()
        {
            return new [] {
                new {  Name =  "Value Item 1", Description = "Some Random Desc"},
                new {  Name =  "Value Item 2", Description = "Some Random Desc"},
                new {  Name =  "Value Item 3", Description = "Some Random Desc"},
                new {  Name =  "Value Item 4", Description = "Some Random Desc"}
            };
        }
    }
}
