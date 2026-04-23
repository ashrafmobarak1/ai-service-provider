using System.ComponentModel.DataAnnotations;

namespace ServiceMarketplace.Application.Requests.DTOs;

public class NearbyRequestsQuery
{
    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Range(0.1, 500)]
    public double RadiusKm { get; set; } = 10;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
