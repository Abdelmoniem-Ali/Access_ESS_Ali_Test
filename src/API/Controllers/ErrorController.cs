using ARCRM.Pidgets.MultiUserDiaryAPI.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {
        public BaseResponse<string> HandleError([FromServices] IHostEnvironment hostEnvironment)
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            if (Program.StaticConfig.GetValue<bool>("ShowExceptionDetails"))
            {

                return new BaseResponse<string>() { Response = exceptionHandlerFeature.Error.StackTrace, Error = exceptionHandlerFeature.Error.Message };
            }
            else
            {
                return new BaseResponse<string>() { Response = string.Empty, Error = "Error calling: " + exceptionHandlerFeature.Endpoint };
            }
        }
    }
}
