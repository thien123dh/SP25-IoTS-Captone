using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CaptoneProject_IOTS_API.Controllers.MyBaseController;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using Microsoft.AspNetCore.Authorization;

namespace CaptoneProject_IOTS_API.Controllers.IotDeviceController
{
    [Route("api/iot-device")]
    [ApiController]

    public class IotDeviceController : MyBaseController.MyBaseController
    {
        private readonly IIotDevicesService iotDevicesService;
        private readonly IActivityLogService activityLogService;
        public IotDeviceController(IIotDevicesService iotDevicesService, IActivityLogService activityLogService)
        {
            this.iotDevicesService = iotDevicesService;
            this.activityLogService = activityLogService;
        }

        [HttpGet("get-iot-device-details-by-id/{id}")]
        public async Task<IActionResult> GetIotDeviceDetailsById(int id)
        {
            var res = await iotDevicesService.GetIotDeviceById(id);

            return GetActionResult(res);
        }

        [HttpPost("create-iot-device")]
        [Authorize]
        public async Task<IActionResult> CreateIotDevice([FromBody] CreateUpdateIotDeviceDTO payload)
        {
            var res = await iotDevicesService.CreateOrUpdateIotDevice(null, payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Created new iot device with ID {res?.Data?.Id}");
            }

            return GetActionResult(res);
        }

        [HttpPut("update-iot-device/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateIotDevice(int id, [FromBody] CreateUpdateIotDeviceDTO payload)
        {
            var res = await iotDevicesService.CreateOrUpdateIotDevice(id, payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated iot device with ID {id}");
            }

            return GetActionResult(res);
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetPaginationIotDevices([FromBody] PaginationRequest payload,
            [FromQuery] int? storeFilterId, 
            [FromQuery] int? categoryFilterId, 
            [FromQuery] IotDeviceTypeEnum? deviceTypeFilter)
        {
            var res = await iotDevicesService.GetPagination(storeFilterId, categoryFilterId, deviceTypeFilter, payload);

            return GetActionResult(res);
        }

        [HttpPut("activate-iot-device/{id}")]
        [Authorize]
        public async Task<IActionResult> ActivateIotDevice(int id)
        {
            var res = await iotDevicesService.UpdateIotDeviceStatus(id, 1);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Activated iot device with ID {id}");
            }

            return GetActionResult(res);
        }

        [HttpPut("deactivate-iot-device/{id}")]
        [Authorize]
        public async Task<IActionResult> DeactivateIotDevice(int id)
        {
            var res = await iotDevicesService.UpdateIotDeviceStatus(id, 0);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Deactivated iot device with ID {id}");
            }

            return GetActionResult(res);
        }
    }
}
