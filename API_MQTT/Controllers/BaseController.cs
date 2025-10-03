using Common.Utils;
using log4net;
using DomainService.Interfaces;
using DomainService.Interfaces.MQTT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Model.ResponseModel;

namespace API_MQTT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly ILog Log = LogManager.GetLogger(typeof(BaseController));

        protected Guid CurrentUserId;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
        }

        protected OkObjectResult Ok([ActionResultObjectValue] object? value, int totalRecord = 0)
        {
            return base.Ok(Utils.CreateResponseModel(value, totalRecord));
        }

        protected OkObjectResult Ok([ActionResultObjectValue] object? value, object body)
        {
            Utils.WriteLogInfo(Log, CurrentUserId.ToString(), Request, body);
            return base.Ok(value);
        }

        protected OkObjectResult Error(Exception e)
        {
            Utils.WriteLogError(Log, CurrentUserId.ToString(), Request, e);
            return base.Ok(Utils.CreateErrorModel<object>(message: e.Message, exception: e));
        }
    }
}