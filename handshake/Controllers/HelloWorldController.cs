using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace handshake.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class HelloWorldController : ControllerBase
  {
    [HttpGet]
    public string Get()
    {
      return "Hello World! Hallo Blue! Hallo Max!";
    }
  }
}
