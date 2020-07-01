namespace AABillApi.Models
{
    public class BillsDatabaseSettings : IBillsDatabaseSettings
    {
        public string BillsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBillsDatabaseSettings
    {
        string BillsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
