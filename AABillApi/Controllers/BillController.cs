using AABillApi.Models;
using AABillApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AABillApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User")]
    public class BillController : ControllerBase
    {
        private readonly BillService _billService;
        private readonly IAuthenticateService _authService;

        public BillController(BillService billService, IAuthenticateService authService)
        {
            this._billService = billService;
            this._authService = authService;
        }

        [HttpDelete, Route("{roomId}/BillInfo/{billInfoId}")]
        public void DelBillInfo(int roomId,int billInfoId)
        {
            _billService.DelBillInfo(roomId, billInfoId);
        }

        [HttpPost,Route("{roomId}/BillInfo")]
        public void CreatBillInfo(int roomId,BillInfo billInfo)
        {
            _billService.CreatBillInfo(roomId,billInfo);
        }

        [HttpPost,Route("{roomId}/PayerInfo")]
        async public Task<ActionResult<BillResult>> CreatPayer(int roomId, [FromQuery] string payerName)
        {
            var res = await _billService.CreatPayer(roomId, payerName);
            if (res == false)
            {
                return NotFound(new BillResult(StatusCodes.Code404));
            }
            return Ok(new BillResult(StatusCodes.Code200));
        }

        [HttpPatch, Route("{roomId}/BillInfo/{billInfoId}")]
        public void EditBillInfo(int roomId, int billInfoId, BillInfo billInfo)
        {
            _billService.EditBillInfo(roomId, billInfoId, billInfo);
        }

        [HttpPatch, Route("{roomId}/PayerInfo/{payerId}")]
        public void EditPayer(int roomId, int payerId, [FromQuery] string payerName)
        {
            _billService.EditPayer(roomId, payerId, payerName);
        }

        [HttpPatch, Route("{roomId}")]
        public void EditRoomTitle(int roomId,[FromQuery] string roomTitle)
        {
            _billService.EditRoomTitle(roomId, roomTitle);
        }

        [HttpDelete, Route("{roomId}/PayerInfo/{payerId}")]
        public void DelPayer(int roomId, int payerId)
        {
            _billService.DelPayer(roomId, payerId);
        }

        [HttpGet, Route("{roomId}")]
        async public Task<ActionResult<BillResult>> GetBill(int roomId)
        {
            var id = await _billService.FindIdbyRoomId(roomId);
            var bill = await _billService.Get(id);
            return Ok(new BillResult(StatusCodes.Code200, bill));
        }

        [AllowAnonymous]
        [HttpPost, Route("{roomId}")]
        async public Task<ActionResult<BillResult>> LoginBillRoom(int roomId,CreatRoomDTO request)
        {
            string token;
            var flag = await _billService.LoginBillRoom(roomId, request);
            if (flag)
            {
                _authService.IsAuthenticated(request, out token);
                var bill = await _billService.Get(roomId);
                bill.Token = token;
                return Ok(new BillResult(StatusCodes.Code200, bill));
            }
            else
            {
                return NotFound(StatusCodes.Code404);
            }
        }

        [HttpPost, Route("NewRoom")]
        async public Task<ActionResult<BillResult>> PostNewRoom(CreatRoomDTO request)
        {
            Bill bill = new Bill();
            bill.Id = await _billService.FindIdbyRoomId(request.RoomId);
            bill.RoomId = request.RoomId;
            bill.RoomPwd = request.RoomPwd;
            bill.RoomTitle = "new room";
            bill.BillInfo = new List<BillInfo>();
            bill.PayerInfo = new List<PayerInfo>();
            _billService.Update(bill.Id, bill);
            return Ok(new BillResult(StatusCodes.Code200, bill));
        }

        [AllowAnonymous]
        [HttpGet, Route("NewRoom")]
        async public Task<ActionResult<BillResult>> GetNewRoom()
        {
            string token;
            var cr = await _billService.GetNewRoomId();
            Bill bill = new Bill();
            bill.RoomId = cr.RoomId;
            bill.RoomPwd = cr.RoomPwd;
            _billService.Create(bill);
            _authService.IsAuthenticated(cr, out token);
            cr.Token = token;
            return Ok(new BillResult(StatusCodes.Code200,cr));
        }
    }
}
