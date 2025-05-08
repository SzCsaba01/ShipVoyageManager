using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShipVoyageManager.Data.Access.Data;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Access;
public class ShipRepository : IShipRepository
{
    private readonly ShipVoyageManagerContext _context;
    private readonly IMapper _mapper;

    public ShipRepository(ShipVoyageManagerContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FilteredPaginatedShipsDto> GetFilteredShipsPaginatedAsync(int page, int pageSize, string search)
    {
        var query = _context.Ships
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s => s.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var ships = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Include(s => s.Voyages)
                .ThenInclude(v => v.DeparturePort)
            .Include(s => s.Voyages)
                .ThenInclude(v => v.ArrivalPort)
            .ToListAsync();

        return new FilteredPaginatedShipsDto
        {
            Ships = _mapper.Map<List<ShipDto>>(ships),
            TotalCount = totalCount
        };
    }

    public async Task<ShipEntity?> GetShipByNameAsync(string name)
    {
        return await _context.Ships
            .Where(s => s.Name == name)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<ShipEntity>> GetShipsOutOfDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Ships
            .Where(s => !s.Voyages.Any(v => 
                v.StartTime >= startDate && v.StartTime <= endDate || 
                v.EndTime >= startDate && v.EndTime <= endDate))
            .Include(s => s.Voyages)
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task AddShipAsync(ShipEntity ship)
    {
        await _context.Ships.AddAsync(ship);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateShipAsync(ShipEntity ship)
    {
        _context.Ships.Update(ship);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteShipByIdAsync(Guid id)
    {
        var ship = await _context.Ships.FindAsync(id);
        if (ship != null)
        {
            _context.Ships.Remove(ship);
            await _context.SaveChangesAsync();
        }


        //ExecuteDeleteAsync is not supported in EF Core in memory
        //await _context.Ships
        //    .Where(s => s.Id == id)
        //    .ExecuteDeleteAsync();
    }
}
