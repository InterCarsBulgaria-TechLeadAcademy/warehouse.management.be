using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Vendor;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.Services
{
    public class VendorService : IVendorService
    {
        private readonly IRepository repository;

        public VendorService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<VendorDto?> GetByIdAsync(int id)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .Where(v => v.Id == id)
                .Select(v => new VendorDto()
                {
                    Id = v.Id,
                    Name = v.Name,
                    SystemNumber = v.SystemNumber,
                    Markers = v
                        .VendorsMarkers.Select(vm => new VendorMarkerDto()
                        {
                            MarkerId = vm.MarkerId,
                            MarkerName = vm.Marker.Name,
                        })
                        .ToList(),
                    Zones = v
                        .VendorsZones.Select(vz => new VendorZoneDto()
                        {
                            ZoneId = vz.ZoneId,
                            ZoneName = vz.Zone.Name,
                            IsFinal = vz.Zone.IsFinal,
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<VendorDto>> GetAllAsync()
        {
            return await repository
                .AllReadOnly<Vendor>()
                .Select(v => new VendorDto()
                {
                    Id = v.Id,
                    Name = v.Name,
                    SystemNumber = v.SystemNumber,
                    Markers = v
                        .VendorsMarkers.Select(vm => new VendorMarkerDto()
                        {
                            MarkerId = vm.MarkerId,
                            MarkerName = vm.Marker.Name,
                        })
                        .ToList(),
                    Zones = v
                        .VendorsZones.Select(vz => new VendorZoneDto()
                        {
                            ZoneId = vz.ZoneId,
                            ZoneName = vz.Zone.Name,
                            IsFinal = vz.Zone.IsFinal,
                        })
                        .ToList(),
                })
                .ToListAsync();
        }
    }
}
