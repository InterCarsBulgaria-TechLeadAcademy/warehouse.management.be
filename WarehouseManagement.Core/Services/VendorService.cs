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

        public async Task<IEnumerable<VendorDto>> GetAllDeletedAsync()
        {
            return await repository
                .AllWithDeletedReadOnly<Vendor>()
                .Where(v => v.IsDeleted)
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

        public async Task<int> AddAsync(VendorFormDto model, string userId)
        {
            if (await ExistByNameAsync(model.Name))
            {
                throw new ArgumentException($"Vendor with name {model.Name} already exist");
            }

            if (await ExistBySystemNumberAsync(model.SystemNumber))
            {
                throw new ArgumentException(
                    $"Vendor with system number {model.SystemNumber} already exist"
                );
            }

            var vendor = new Vendor()
            {
                Name = model.Name,
                SystemNumber = model.SystemNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };

            await repository.AddAsync(vendor);
            await repository.SaveChangesAsync();

            return vendor.Id;
        }

        public async Task EditAsync(int id, VendorFormDto model, string userId)
        {
            var vendor = await repository.GetByIdAsync<Vendor>(id);

            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {id} not found.");
            }

            if (await this.AnotherVendorWithNameExistAsync(id, model.Name))
            {
                throw new InvalidOperationException(
                    $"Another vendor with name {model.Name} already exist"
                );
            }

            if (await this.AnotherVendorWithSystemNumberExistAsync(id, model.SystemNumber))
            {
                throw new InvalidOperationException(
                    $"Another vendor with system number {model.SystemNumber} already exist"
                );
            }

            vendor.Name = model.Name;
            vendor.SystemNumber = model.SystemNumber;
            vendor.LastModifiedAt = DateTime.UtcNow;
            vendor.LastModifiedByUserId = userId;

            await repository.SaveChangesWithLogAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var vendor = await repository
                .All<Vendor>()
                .Where(v => v.Id == id)
                .Include(v => v.Deliveries)
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {id} not found.");
            }

            if (vendor.Deliveries.Any())
            {
                var deliveries = vendor.Deliveries.Select(d => d.SystemNumber).ToList();
                throw new InvalidOperationException(
                    $"Vendor can not be deleted because has existing deliveries with system numbers: {string.Join(", ", deliveries)}"
                );
            }

            vendor.DeletedByUserId = userId;

            repository.SoftDelete(vendor);

            await repository.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var vendor = await repository
                .AllWithDeleted<Vendor>()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {id} not found.");
            }

            if (!vendor.IsDeleted)
            {
                throw new InvalidOperationException($"Vendor {vendor.Name} is not deleted");
            }

            if (await this.AnotherVendorWithNameExistAsync(vendor.Id, vendor.Name))
            {
                throw new InvalidOperationException(
                    $"Another vendor with name {vendor.Name} already exist"
                );
            }

            if (await this.AnotherVendorWithSystemNumberExistAsync(vendor.Id, vendor.SystemNumber))
            {
                throw new InvalidOperationException(
                    $"Another vendor with system number {vendor.SystemNumber} already exist"
                );
            }

            repository.UnDelete(vendor);
            await repository.SaveChangesAsync();
        }

        private async Task<bool> ExistByNameAsync(string name)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .AnyAsync(v => v.Name.ToLower() == name.ToLower());
        }

        private async Task<bool> ExistBySystemNumberAsync(string systemNumber)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .AnyAsync(v => v.SystemNumber.ToLower() == systemNumber.ToLower());
        }

        private async Task<bool> AnotherVendorWithNameExistAsync(int id, string name)
        {
            return await repository
                .AllReadOnly<Vendor>()
                .Where(v => v.Id != id)
                .AnyAsync(v => v.Name.ToLower() == name.ToLower());
        }

        private async Task<bool> AnotherVendorWithSystemNumberExistAsync(
            int id,
            string systemNumber
        )
        {
            return await repository
                .AllReadOnly<Vendor>()
                .Where(v => v.Id != id)
                .AnyAsync(v => v.SystemNumber.ToLower() == systemNumber.ToLower());
        }
    }
}
