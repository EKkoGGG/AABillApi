using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AABillApi.Models
{
   
    public class Bill
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("roomId")]
        public int RoomId { get; set; }

        [BsonElement("roomPwd")]
        public int RoomPwd { get; set; }

        [BsonElement("roomTitle")]
        public string RoomTitle { get; set; }

        [BsonElement("billInfo")]
        public List<BillInfo> BillInfo { get; set; }

        [BsonElement("payerInfo")]
        public List<PayerInfo> PayerInfo { get; set; }
    }

    public class PayerInfo
    {
        [BsonElement("payerId")]
        public int PayerId { get; set; }

        [BsonElement("payerName")]
        public string PayerName { get; set; }

    }

    public class BillInfo
    {
        [BsonElement("billInfoId")]
        public int BillInfoId { get; set; }

        [BsonElement("payerId")]
        public int PayerId { get; set; }

        [BsonElement("payNum")]
        public double PayNum { get; set; }

        [BsonElement("payerIds")]
        public List<int> PayerIds { get; set; }
    }
    
}
