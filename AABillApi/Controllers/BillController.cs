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
        private readonly BillService _billService;
        private readonly IAuthenticateService _authService;

        public BillController(BillService billService, IAuthenticateService authService)
        {
            this._billService = billService;
            this._authService = authService;
        }

        [HttpPost,Route("{roomId}/PayerInfo")]
        async public Task<ActionResult> CreatPayer(int roomId, [FromQuery] string payerName)
        {
            var res = await _billService.CreatPayer(roomId, payerName);
            if (res == false)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPatch, Route("{roomId}/PayerInfo/{payerId}")]
        public void EditPayer(int roomId, int payerId, [FromQuery] string payerName)
        {
            _billService.EditPayer(roomId, payerId, payerName);
        }

        [HttpDelete, Route("{roomId}/PayerInfo/{payerId}")]
        public void DelPayer(int roomId, int payerId)
        {
            _billService.DelPayer(roomId, payerId);
        }

        [HttpGet, Route("{roomId}")]
        async public Task<Bill> GetBill(int roomId)
        {
            var id = await _billService.FindIdbyRoomId(roomId);
            return await _billService.Get(id);
        }

        [AllowAnonymous]
        [HttpPost, Route("NewRoom")]
        async public Task<Bill> PostNewRoom(CreatRoomDTO request)
        {
            Bill bill = new Bill();
            bill.Id = await _billService.FindIdbyRoomId(request.RoomId);
            bill.RoomId = request.RoomId;
            bill.RoomPwd = request.RoomPwd;
            bill.RoomTitle = "new room";
            bill.BillInfo = new List<BillInfo>();
            _billService.Update(bill.Id, bill);
            return bill;
        }

        [AllowAnonymous]
        [HttpGet, Route("NewRoom")]
        async public Task<CreatRoomDTO> GetNewRoom()
        {
            var cr = await _billService.GetNewRoomId();
            Bill bill = new Bill();
            bill.RoomId = cr.RoomId;
            bill.RoomPwd = cr.RoomPwd;
            _billService.Create(bill);
            return cr;
        }
    }
}
