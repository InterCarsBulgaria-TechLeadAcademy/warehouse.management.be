using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.ValidationConstants.EntryConstants;

namespace WarehouseManagement.Core.DTOs.Entry;

public class EntryFormDto
{
    [Range(PalletsMinValue, PalletsMaxValue)]
    public int Pallets { get; set; }

    [Range(PackagesMinValue, PackagesMaxValue)]
    public int Packages { get; set; }

    [Range(PiecesMinValue, PiecesMaxValue)]
    public int Pieces { get; set; }
}
