namespace Core.DTOs.Currency
{
    public class CreateCurrencyDTO
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public int DecimalPlaces { get; set; }
    }
}
