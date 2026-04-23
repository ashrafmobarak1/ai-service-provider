using System.ComponentModel.DataAnnotations;

namespace ServiceMarketplace.Application.Auth.DTOs;

public class RegisterDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Allowed values: "Customer", "Provider"</summary>
    [Required]
    public string Role { get; set; } = "Customer";
}
