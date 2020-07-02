using AABillApi.Models;
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

        public Bills Get(string id) =>
            _billss.Find<Bills>(bills => bills.Id == id).FirstOrDefault();

        async public void Create(Bills bills)
        {
           await  _billss.InsertOneAsync(bills);
        }

        async public void Update(string id, Bills billsIn)
        {
           await  _billss.ReplaceOneAsync(bills => bills.Id == id, billsIn);
        }

        public void Remove(Bills billsIn) =>
            _billss.DeleteOne(bills => bills.Id == billsIn.Id);

        public void Remove(string id) =>
            _billss.DeleteOne(bills => bills.Id == id);
    }
}
