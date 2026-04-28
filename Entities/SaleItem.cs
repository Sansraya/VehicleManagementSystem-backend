using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementSystem.Entities;

public class SaleItem
{
    public Guid SaleItemId { get; set; }

    [ForeignKey("SaleId")]
    public Guid SaleId { get; set; }
    public required Sale Sale { get; set; }
    
    [ForeignKey("VehicleId")]
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [ForeignKey("PartsId")]
    public Guid? PartsId { get; set; }
    public Parts? Parts { get; set; }

    public string ItemType { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double LineTotal { get; set; }
}