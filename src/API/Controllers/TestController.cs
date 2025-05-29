using Microsoft.AspNetCore.Mvc;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        public object Test()
        {
            return new
            {
                Test = DateTime.Now
            };
        }
    }
}
