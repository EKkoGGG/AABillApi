using AABillApi.Models;
using AABillApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MongoDB.Bson;

namespace AABillApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly BillsService _billsService;
        private readonly IAuthenticateService _authService;

        public TestController(BillsService billsService, IAuthenticateService authService)
        {
            this._billsService = billsService;
            this._authService = authService;
        }

        [AllowAnonymous]
        [HttpPost,Route("T")]
        public ActionResult GetTest([FromForm] CreatRoomDTO request)
        {
            Bills bills = new Bills();
            bills.Id = _billsService.FindIdbyRoomId(request.RoomId);
            bills.RoomId = request.RoomId;
            bills.RoomPwd = request.RoomPwd;
            bills.RoomTitle = "new room1";
            bills.BillInfo = new List<BillInfo>();
            _billsService.Update(bills.Id, bills);
            return Ok();
        }
    }
}
