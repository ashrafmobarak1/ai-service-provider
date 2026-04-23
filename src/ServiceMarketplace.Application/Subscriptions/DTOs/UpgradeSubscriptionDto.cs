using System.ComponentModel.DataAnnotations;

namespace ServiceMarketplace.Application.Subscriptions.DTOs;

public class UpgradeSubscriptionDto
{
    /// <summary>Allowed values: "free", "paid"</summary>
    [Required]
    public string Tier { get; set; } = "paid";
}
