using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Vendor;
using WarehouseManagement.Core.Extensions;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.VendorMessageKeys;

namespace WarehouseManagement.Core.Services
{
    public class VendorService : IVendorService
    {
        private readonly IRepository repository;

        public VendorService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<VendorDetailsDto> GetByIdAsync(int id)
        {
            if (!await ExistByIdAsync(id))
            {
                throw new KeyNotFoundException($"{VendorWithIdNotFound} {id}");
            }
            
            var vendor = await repository
                .AllReadOnly<Vendor>()
                .Include(v => v.VendorsMarkers)
                .FirstAsync(v => v.Id == id);

            return new VendorDetailsDto()
            {
                Id = vendor.Id,
                Name = vendor.Name,
                SystemNumber = vendor.SystemNumber,
                CreatedAt = vendor.CreatedAt,
                Markers = vendor
                    .VendorsMarkers.Select(vm => new VendorMarkerDto()
                    {
                        MarkerId = vm.MarkerId,
                        MarkerName = vm.Marker.Name,
                    })
                    .ToList(),
            };
        }

        public async Task<IEnumerable<VendorDto>> GetAllAsync(PaginationParameters paginationParams)
        {
            Expression<Func<Vendor, bool>> filter = v =>
                EF.Functions.Like(v.Name, $"%{paginationParams.SearchQuery}%")
                || EF.Functions.Like(v.SystemNumber, $"%{paginationParams.SearchQuery}%");

            return await repository
                .AllReadOnly<Vendor>()
                .Paginate(paginationParams, filter)
                .Select(v => new VendorDto()
                {
                    Id = v.Id,
                    Name = v.Name,
                    SystemNumber = v.SystemNumber,
                    CreatedAt = v.CreatedAt,
                    Markers = v
                        .VendorsMarkers.Select(vm => new VendorMarkerDto()
                        {
                            MarkerId = vm.MarkerId,
                            MarkerName = vm.Marker.Name,
                        })
                        .ToList()
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
                    CreatedAt = v.CreatedAt,
                    Markers = v
                        .VendorsMarkers.Select(vm => new VendorMarkerDto()
                        {
                            MarkerId = vm.MarkerId,
                            MarkerName = vm.Marker.Name,
                        })
                        .ToList(),
                })
                .ToListAsync();
        }

        public async Task<int> AddAsync(VendorFormDto model, string userId)
        {
            if (await ExistByNameAsync(model.Name))
            {
                throw new ArgumentException($"{VendorWithNameExist} {model.Name}");
            }

            if (await ExistBySystemNumberAsync(model.SystemNumber))
            {
                throw new ArgumentException($"{VendorWithSystemNumberExist} {model.SystemNumber}");
            }

            var vendor = new Vendor()
            {
                Name = model.Name,
                SystemNumber = model.SystemNumber,
                CreatedByUserId = userId,
            };

            await repository.AddAsync(vendor);

            var vendorMarkers = await repository
                .All<Marker>()
                .Where(m => model.MarkerIds.Contains(m.Id))
                .Select(m => new VendorMarker { Vendor = vendor, Marker = m })
                .ToListAsync();

            vendor.VendorsMarkers = vendorMarkers;

            await repository.SaveChangesAsync();

            return vendor.Id;
        }

        public async Task EditAsync(int id, VendorFormDto model, string userId)
        {
            var vendor = await repository
                .All<Vendor>()
                .Where(v => v.Id == id)
                .Include(v => v.VendorsMarkers)
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                throw new KeyNotFoundException($"{VendorWithIdNotFound} {id}");
            }

            if (await this.AnotherVendorWithNameExistAsync(id, model.Name))
            {
                throw new InvalidOperationException($"{VendorWithNameExist} {model.Name}");
            }

            if (await this.AnotherVendorWithSystemNumberExistAsync(id, model.SystemNumber))
            {
                throw new InvalidOperationException(
                    $"{VendorWithSystemNumberExist} {model.SystemNumber}"
                );
            }

            vendor.Name = model.Name;
            vendor.SystemNumber = model.SystemNumber;
            vendor.LastModifiedAt = DateTime.UtcNow;
            vendor.LastModifiedByUserId = userId;

            repository.DeleteRange(vendor.VendorsMarkers);

            foreach (var markerId in model.MarkerIds)
            {
                var newVendorMarker = new VendorMarker() { MarkerId = markerId, VendorId = id, };

                await repository.AddAsync(newVendorMarker);
            }

            await repository.SaveChangesWithLogAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var vendor = await repository
                .All<Vendor>()
                .Where(v => v.Id == id)
                .Include(v => v.Deliveries)
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                throw new KeyNotFoundException($"{VendorWithIdNotFound} {id}");
            }

            if (vendor.Deliveries.Any())
            {
                var deliveries = vendor.Deliveries.Select(d => d.SystemNumber).ToList();
                throw new InvalidOperationException(
                    $"{VendorHasDeliveries} {string.Join(",", deliveries)}"
                );
            }

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
                throw new KeyNotFoundException($"{VendorWithIdNotFound} {id}");
            }

            if (!vendor.IsDeleted)
            {
                throw new InvalidOperationException($"{VendorNotDeleted} {id}");
            }

            if (await AnotherVendorWithNameExistAsync(vendor.Id, vendor.Name))
            {
                throw new InvalidOperationException($"{VendorWithNameExist} {vendor.Name}");
            }

            if (await AnotherVendorWithSystemNumberExistAsync(vendor.Id, vendor.SystemNumber))
            {
                throw new InvalidOperationException(
                    $"{VendorWithSystemNumberExist} {vendor.SystemNumber}"
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

        public async Task<bool> ExistByIdAsync(int id)
        {
            return await repository.AllReadOnly<Vendor>().AnyAsync(v => v.Id == id);
        }
    }
}
