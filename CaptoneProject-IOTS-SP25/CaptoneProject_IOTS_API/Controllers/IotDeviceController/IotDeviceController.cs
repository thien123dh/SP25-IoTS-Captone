using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CaptoneProject_IOTS_API.Controllers.MyBaseController;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_API.Controllers.IotDeviceController
{
    [Route("api/iot-device")]
    [ApiController]

    public class IotDeviceController : MyBaseController.MyBaseController
    {
        private readonly IIotDevicesService iotDevicesService;

        public IotDeviceController(IIotDevicesService iotDevicesService)
        {
            this.iotDevicesService = iotDevicesService;
        }

        [HttpGet("get-iot-device-details-by-id/{id}")]
        public async Task<IActionResult> GetIotDeviceDetailsById(int id)
        {
            var res = await iotDevicesService.GetIotDeviceById(id);

            return GetActionResult(res);
        }

        [HttpPost("create-iot-device")]
        public async Task<IActionResult> CreateIotDevice([FromBody] CreateUpdateIotDeviceDTO payload)
        {
            var res = await iotDevicesService.CreateOrUpdateIotDevice(null, payload);

            return GetActionResult(res);
        }

        [HttpPut("update-iot-device/{id}")]
        public async Task<IActionResult> UpdateIotDevice(int id, [FromBody] CreateUpdateIotDeviceDTO payload)
        {
            var res = await iotDevicesService.CreateOrUpdateIotDevice(id, payload);

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
        public async Task<IActionResult> ActivateIotDevice(int id)
        {
            var res = await iotDevicesService.UpdateIotDeviceStatus(id, 1);

            return GetActionResult(res);
        }

        [HttpPut("deactivate-iot-device/{id}")]
        public async Task<IActionResult> DeactivateIotDevice(int id)
        {
            var res = await iotDevicesService.UpdateIotDeviceStatus(id, 0);

            return GetActionResult(res);
        }
    }
}
