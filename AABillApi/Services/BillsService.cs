using AABillApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AABillApi.Services
{
    public class BillsService
    {
        private readonly IMongoCollection<Bills> _billss;

        public BillsService(IBillsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _billss = database.GetCollection<Bills>(settings.BillsCollectionName);
        }

        async public void EditPayer(int roomId, int payerId,string payerName)
        {
            var filter = Builders<Bills>.Filter.Where(b => b.RoomId == roomId) 
            & Builders<Bills>.Filter.Where(p => p.PayerInfo.Any(pd => pd.PayerId == payerId));
            var update = Builders<Bills>.Update.Set(p => p.PayerInfo[-1].PayerName, payerName);
            await _billss.UpdateOneAsync(filter, update);
        }

        async public void DelPayer(int roomId, int payerId)
        {
            var update = Builders<Bills>.Update.PullFilter(bills => bills.PayerInfo, p => p.PayerId == payerId);
            await _billss.UpdateOneAsync<Bills>(bills => bills.RoomId == roomId, update);
        }
        async public Task<CreatRoomDTO> GetNewRoomId()
        {
            CreatRoomDTO creatRoom = new CreatRoomDTO();
            Random random = new Random();
            int roomId = 0;
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
            var bill = await _billss.Find(bills => bills.RoomId == roomId).FirstOrDefaultAsync();
            if (bill == null)
            {
                return "";
            }
            return bill.Id;
        }

        public List<Bills> Get() =>
            _billss.Find(bills => true).ToList();

        async public Task<Bills> Get(string id) =>
            await _billss.Find<Bills>(bills => bills.Id == id).FirstOrDefaultAsync();

        async public void Create(Bills bills)
        {
            await _billss.InsertOneAsync(bills);
        }

        async public void Update(string id, Bills billsIn)
        {
            await _billss.ReplaceOneAsync(bills => bills.Id == id, billsIn);
        }

        public void Remove(Bills billsIn) =>
            _billss.DeleteOne(bills => bills.Id == billsIn.Id);

        public void Remove(string id) =>
            _billss.DeleteOne(bills => bills.Id == id);
    }
}
