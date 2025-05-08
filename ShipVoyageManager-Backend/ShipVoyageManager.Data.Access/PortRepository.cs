using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShipVoyageManager.Data.Access.Data;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Access;
public class PortRepository : IPortRepository
{
    private readonly ShipVoyageManagerContext _context;
    private readonly IMapper _mapper;

    public PortRepository(ShipVoyageManagerContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FilteredPaginatedPortsDto> GetFilteredPortsPaginatedAsync(int page, int pageSize, string search)
    {
        var query = _context.Ports
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var ports = await query
            .Skip(page* pageSize)
            .Take(pageSize)
            .Include(p => p.ArrivingVoyages)
                .ThenInclude(v => v.Ship)
            .Include(p => p.ArrivingVoyages)
                .ThenInclude(v => v.DeparturePort)
            .Include(p => p.DepartingVoyages)
                .ThenInclude(v => v.Ship)
            .Include(p => p.DepartingVoyages)
                .ThenInclude(v => v.ArrivalPort)
            .ToListAsync();

        return new FilteredPaginatedPortsDto
        {
            Ports = _mapper.Map<List<PortDto>>(ports),
            TotalCount = totalCount
        };
    }

    public async Task<PortEntity?> GetPortByNameAsync(string name)
    {
        return await _context.Ports
            .Where(p => p.Name == name)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<PortEntity>> GetAllPortsAsync()
    {
        return await _context.Ports
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddPortAsync(PortEntity port)
    {
        await _context.Ports.AddAsync(port);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePortAsync(PortEntity port)
    {
        _context.Ports.Update(port);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePortByIdAsync(Guid id)
    {
        var port = await _context.Ports.FindAsync(id);
        if (port != null)
        {
            _context.Ports.Remove(port);
            await _context.SaveChangesAsync();
        }

        //ExecuteDeleteAsync is not supported in EF Core in memory
        //await _context.Ports
        //    .Where(p => p.Id == id)
        //    .ExecuteDeleteAsync();
    }
}
