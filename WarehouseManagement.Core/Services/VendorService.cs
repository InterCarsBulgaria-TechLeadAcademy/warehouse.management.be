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

        public async Task AddAsync(VendorFormDto model, string userId)
        {
            var vendor = new Vendor()
            {
                Name = model.Name,
                SystemNumber = model.SystemNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };

            await repository.AddAsync(vendor);
            await repository.SaveChangesAsync();
        }

        public async Task EditAsync(int id, VendorFormDto model, string userId)
        {
            var vendor = await repository.GetByIdAsync<Vendor>(id);
            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {id} not found.");
            }

            vendor.Name = model.Name;
            vendor.SystemNumber = model.SystemNumber;
            vendor.LastModifiedAt = DateTime.UtcNow;
            vendor.LastModifiedByUserId = userId;

            await repository.SaveChangesWithLogAsync();
        }

        public async Task<bool> ExistByNameAsync(string name)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .AnyAsync(v => v.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistBySystemNumberAsync(string systemNumber)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .AnyAsync(v => v.SystemNumber.ToLower() == systemNumber.ToLower());
        }

        public async Task<bool> AnotherVendorWithNameExistIdAsync(int id, string name)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .Where(v => v.Id != id)
                .AnyAsync(v => v.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> AnotherVendorWithSystemNumberExistAsync(int id, string systemNumber)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .Where(v => v.Id != id)
                .AnyAsync(v => v.SystemNumber.ToLower() == systemNumber.ToLower());
        }

        public async Task<bool> ExistByIdAsync(int id)
        {
            return await repository.AllReadOnly<Vendor>().AnyAsync(v => v.Id == id);
        }
    }
}
