using ARCRM.Pidgets.MultiUserDiaryAPI.Models;
using ARCRM.Pidgets.MultiUserDiaryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class MultiUserDiaryController : ControllerBase
    {
        [HttpGet]
        public BaseResponse<List<string>> GetRegistrySettings()
        {
            return new BaseResponse<List<string>>() { Response = GetService().GetRegistrySettings() };
        }

        [HttpGet]
        public BaseResponse<SharedDiaryInfo[]> GetOwners()
        {

           return new BaseResponse<SharedDiaryInfo[]>() { Response = GetService().GetOwners() };
        }

        [HttpPost]
        public BaseResponse<DiaryEventInfo[]> GetDiaryEvents(DiaryEventRequest pRequest)
        {
            return new BaseResponse<DiaryEventInfo[]>() { Response = GetService().GetDiaryEvents(pRequest.StartDate, pRequest.EndDate, pRequest.Owners) };
        }

        [HttpGet]
        public BaseResponse<TaskInfo[]> GetTasks()
        {
            return new BaseResponse<TaskInfo[]>() { Response = GetService().GetTasks() };
        }

        [HttpGet]
        public BaseResponse<TaskInfo> GetTaskInfo(int pTaskId)
        {
            return new BaseResponse<TaskInfo>() { Response = GetService().GetTaskById(pTaskId) };
        }
        private MultiUserDiaryService GetService()
        {
            ClaimsIdentity identity = User.Identity as ClaimsIdentity;
            string email = identity.Claims.FirstOrDefault(o => o.Type == "name")?.Value.ToLower();

            return new MultiUserDiaryService(Request.Headers["Softwarekey"], Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization].ToString().Replace("Bearer", string.Empty).Trim(), email);
        }

        
    }
}
