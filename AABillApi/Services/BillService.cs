using AABillApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AABillApi.Services
{
    public class BillService
    {
        private readonly IMongoCollection<Bill> _bills;

        public BillService(IBillsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _bills = database.GetCollection<Bill>(settings.BillsCollectionName);
        }

        async public void DelBillInfo(int roomId, int billInfoId)
        {
            var update = Builders<Bill>.Update.PullFilter(b => b.BillInfo,bi=>bi.BillInfoId == billInfoId);
            await _bills.UpdateOneAsync<Bill>(bill => bill.RoomId == roomId, update);
        }
        async public void CreatBillInfo(int roomId, BillInfo billInfo)
        {
            var bill = await Get(roomId);
            int billInfoId = 0;
            foreach (var item in bill.BillInfo)
            {
                if (item.BillInfoId > billInfoId)
                {
                    billInfoId = item.BillInfoId;
                }
            }
            billInfoId++;
            billInfo.BillInfoId = billInfoId;
            var update = Builders<Bill>.Update.Push(b => b.BillInfo, billInfo);
            await _bills.UpdateOneAsync(b => b.RoomId == roomId, update);
        }

        async public Task<bool> CreatPayer(int roomId, string payerName)
        {
            var bill = await Get(roomId);
            int payerId = 0;
            foreach (var item in bill.PayerInfo)
            {
                if (item.PayerId > payerId)
                {
                    payerId = item.PayerId;
                }
                if (item.PayerName == payerName)
                {
                    return false;
                }
            }
            payerId++;
            var newPayer = new PayerInfo() { PayerId = payerId, PayerName = payerName };
            var update = Builders<Bill>.Update.Push(b => b.PayerInfo, newPayer);
            await _bills.UpdateOneAsync(b => b.RoomId == roomId, update);
            return true;
        }

        async public void EditPayer(int roomId, int payerId, string payerName)
        {
            var filter = Builders<Bill>.Filter.Where(b => b.RoomId == roomId)
            & Builders<Bill>.Filter.Where(p => p.PayerInfo.Any(pd => pd.PayerId == payerId));
            var update = Builders<Bill>.Update.Set(p => p.PayerInfo[-1].PayerName, payerName);
            await _bills.UpdateOneAsync(filter, update);
        }

        async public void DelPayer(int roomId, int payerId)
        {
            var update = Builders<Bill>.Update.PullFilter(bill => bill.PayerInfo, p => p.PayerId == payerId);
            await _bills.UpdateOneAsync<Bill>(bill => bill.RoomId == roomId, update);

            var updateBillInfo = Builders<Bill>.Update.PullFilter(bill => bill.BillInfo, p => p.PayerId == payerId);
            await _bills.UpdateOneAsync<Bill>(bill => bill.RoomId == roomId, updateBillInfo);

            var filter = Builders<Bill>.Filter.Where(b => b.RoomId == roomId)
            & Builders<Bill>.Filter.Where(p => p.BillInfo.Any(pd => pd.PayerIds.Contains(payerId)));
            var updateC = Builders<Bill>.Update.PullFilter(b => b.BillInfo,p=>p.PayerIds.Contains(payerId));
            await _bills.UpdateOneAsync(filter, updateC);
        }
        async public Task<CreatRoomDTO> GetNewRoomId()
        {
            CreatRoomDTO creatRoom = new CreatRoomDTO();
            Random random = new Random();
            int roomId;
            while (true)
            {
                roomId = random.Next(100000, 999999);
                var id = await FindIdbyRoomId(roomId);
                if (string.IsNullOrEmpty(id))
                {
                    creatRoom.RoomId = roomId;
                    creatRoom.RoomPwd = 1234;
                    break;
                }
            }
            return creatRoom;
        }
        async public Task<string> FindIdbyRoomId(int roomId)
        {
            var bill = await _bills.Find(b => b.RoomId == roomId).FirstOrDefaultAsync();
            if (bill == null)
            {
                return "";
            }
            return bill.Id;
        }

        public List<Bill> Get() =>
            _bills.Find(bill => true).ToList();

        async public Task<Bill> Get(string id) =>
            await _bills.Find<Bill>(bill => bill.Id == id).FirstOrDefaultAsync();

        async public Task<Bill> Get(int roomId) =>
            await _bills.Find<Bill>(bill => bill.RoomId == roomId).FirstOrDefaultAsync();

        async public void Create(Bill bill)
        {
            await _bills.InsertOneAsync(bill);
        }

        async public void Update(string id, Bill billIn)
        {
            await _bills.ReplaceOneAsync(bill => bill.Id == id, billIn);
        }

        public void Remove(Bill billIn) =>
            _bills.DeleteOne(bill => bill.Id == billIn.Id);

        public void Remove(string id) =>
            _bills.DeleteOne(bill => bill.Id == id);
    }
}
