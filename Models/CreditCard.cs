namespace RentACar.API.Models;

public class CreditCard
{
    public int     Id             { get; set; }
    public string  CardName       { get; set; } = string.Empty;
    public string  CardNumber     { get; set; } = string.Empty;
    public string  CardCvc        { get; set; } = string.Empty;
    public string  CardExpiration { get; set; } = string.Empty;
    public decimal MoneyInTheCard { get; set; } = 10000m;
}
