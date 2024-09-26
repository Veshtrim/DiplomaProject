namespace TemaEDiplomesUBT.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Stock> Stocks { get; set; }

    }
}
