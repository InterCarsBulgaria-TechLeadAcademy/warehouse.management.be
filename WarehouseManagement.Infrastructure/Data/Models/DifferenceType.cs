using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.ValidationConstants.DifferenceTypeConstants;

namespace WarehouseManagement.Infrastructure.Data.Models;

public class DifferenceType : BaseClass
{
    [MaxLength(NameMaxLenght)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Difference> Differences { get; set; } = new HashSet<Difference>();
}
