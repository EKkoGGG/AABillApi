using AABillApi.Models;
using AABillApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace AABillApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly BillsService _billsService;
        private readonly IAuthenticateService _authService;

        public BillController(BillsService billsService, IAuthenticateService authService)
        {
            this._billsService = billsService;
            this._authService = authService;
        }

        [HttpPatch, Route("{roomId}/PayerInfo/{payerId}")]
        public void EditPayer(int roomId, int payerId, [FromQuery] string payerName)
        {
            _billsService.EditPayer(roomId, payerId, payerName);
        }

        [HttpDelete, Route("{roomId}/PayerInfo/{payerId}")]
        public void DelPayer(int roomId, int payerId)
        {
            _billsService.DelPayer(roomId, payerId);
        }

        [HttpGet, Route("{roomId}")]
        async public Task<Bills> GetBill(int roomId)
        {
            var id = await _billsService.FindIdbyRoomId(roomId);
            return await _billsService.Get(id);
        }

        [AllowAnonymous]
        [HttpPost, Route("NewRoom")]
        async public Task<Bills> PostNewRoom(CreatRoomDTO request)
        {
            Bills bills = new Bills();
            bills.Id = await _billsService.FindIdbyRoomId(request.RoomId);
            bills.RoomId = request.RoomId;
            bills.RoomPwd = request.RoomPwd;
            bills.RoomTitle = "new room";
            bills.BillInfo = new List<BillInfo>();
            _billsService.Update(bills.Id, bills);
            return bills;
        }

        [AllowAnonymous]
        [HttpGet, Route("NewRoom")]
        async public Task<CreatRoomDTO> GetNewRoom()
        {
            var cr = await _billsService.GetNewRoomId();
            Bills bills = new Bills();
            bills.RoomId = cr.RoomId;
            bills.RoomPwd = cr.RoomPwd;
            _billsService.Create(bills);
            return cr;
        }
    }
}
