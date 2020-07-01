using AABillApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;


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

        public string FindIdbyRoomId(int roomId)
        {
            return _billss.Find<Bills>(bills => bills.RoomId == roomId).First().Id;
        }

        public List<Bills> Get() =>
            _billss.Find(bills => true).ToList();

        public Bills Get(string id) =>
            _billss.Find<Bills>(bills => bills.Id == id).FirstOrDefault();

        public Bills Create(Bills bills)
        {
            _billss.InsertOne(bills);
            return bills;
        }

        public void Update(string id, Bills billsIn) =>
            _billss.ReplaceOne(bills => bills.Id == id, billsIn);

        public void Remove(Bills billsIn) =>
            _billss.DeleteOne(bills => bills.Id == billsIn.Id);

        public void Remove(string id) =>
            _billss.DeleteOne(bills => bills.Id == id);
    }
}
