using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YMNNetCoreFrameWork.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {


        [HttpGet("Test1")]
        [Authorize]
        public async Task<object> Test1() {
            return true;
        }

       
        [HttpGet("Test2")]
        public async Task<object> Test2()
        {
            return true;
        }
    }
}