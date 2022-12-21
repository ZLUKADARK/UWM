﻿
using UWM.Domain.DTO.Watehouses;
using UWM.Domain.Entity;

namespace UWM.BLL.Interfaces
{
    public interface IWarehouseServices
    {
        Task<int> Create(AddressDto address, WarehouseDto warehouse);
        Task Delete(int id);
        Task<IEnumerable<WarehouseDto>> GetAll();
        Task Update(WarehouseDto item);
    }
}
