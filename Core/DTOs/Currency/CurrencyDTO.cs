namespace Core.DTOs.Currency
{
    public class CurrencyDTO
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public int DecimalPlaces { get; set; }
    }
}
