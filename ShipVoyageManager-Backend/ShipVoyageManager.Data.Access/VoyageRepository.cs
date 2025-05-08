using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShipVoyageManager.Data.Access.Data;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Voyage;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Access;
public class VoyageRepository : IVoyageRepository
{
    private readonly ShipVoyageManagerContext _context;
    private readonly IMapper _mapper;

    public VoyageRepository(ShipVoyageManagerContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedVoyagesDto> GetVoyagesPaginatedAsync(int page, int pageSize)
    {
        var query = _context.Voyages
            .Include(v => v.DeparturePort)
            .Include(v => v.ArrivalPort)
            .Include(v => v.Ship)
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var voyages = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedVoyages = new PaginatedVoyagesDto
        {
            TotalCount = totalCount,
            Voyages = _mapper.Map<List<VoyageDto>>(voyages)
        };

        return paginatedVoyages;
    }

    public async Task AddVoyageAsync(VoyageEntity voyage)
    {
        await _context.Voyages.AddAsync(voyage);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVoyageAsync(VoyageEntity voyage)
    {
        _context.Voyages.Update(voyage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteVoyageByIdAsync(Guid id)
    {
        var voyage = await _context.Voyages.FindAsync(id);
        if (voyage != null)
        {
            _context.Voyages.Remove(voyage);
            await _context.SaveChangesAsync();
        }

        //ExecuteDeleteAsync is not supported in EF Core in memory
        //await _context.Voyages
        //    .Where(v => v.Id == id)
        //    .ExecuteDeleteAsync();
    }
}
