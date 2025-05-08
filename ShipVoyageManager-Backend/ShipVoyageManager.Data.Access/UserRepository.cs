using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShipVoyageManager.Data.Access.Data;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Access;
public class UserRepository : IUserRepository
{
    private readonly ShipVoyageManagerContext _context;
    private readonly IMapper _mapper;

    public UserRepository(ShipVoyageManagerContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserEntity?> GetUserByRegistrationTokenAsync(string registrationToken)
    {
        return await _context.Users
            .Where(u => u.RegistrationToken == registrationToken)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<UserEntity?> GetUserByResetPasswordTokenAsync(string resetPasswordToken)
    {
        return await _context.Users
            .Where(u => u.ResetPasswordToken == resetPasswordToken)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<UserEntity?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Where(u => u.Username == username)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<UserEntity?> GetUserByUsernameOrEmailAndPasswordAsync(string userCredential, string password)
    {
        return await _context.Users
            .Where(u => (u.Username == userCredential || u.Email == userCredential) && u.Password == password)
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<FilteredPaginatedUsersDto> GetFilteredUsersPaginatedAsync(int page, int pageSize, string search)
    {
        var userQuery = _context.Users
            .Where(u => u.Role.RoleName != AppConstants.ADMIN_ROLE)
            .Include(u => u.Role)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            userQuery = userQuery.Where(u => u.Username.Contains(search) || u.Email.Contains(search));
        }

        var totalUsersCount = await userQuery.CountAsync();
        var users = await userQuery
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var filteredUsersResult = new FilteredPaginatedUsersDto
        {
            TotalCount = totalUsersCount,
            Users = _mapper.Map<List<UserDto>>(users)
        };

        return filteredUsersResult;
    }

    public async Task AddUserAsync(UserEntity user)
    {
        var userRole = await _context.Roles
            .Where(r => r.RoleName == AppConstants.USER_ROLE)
            .FirstOrDefaultAsync();

        user.Role = userRole;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(UserEntity user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserByUsernameAsync(string username)
    {
        var user = await _context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        //ExecuteDeleteAsync is not supported in EF Core in memory
        //await _context.Users
        //    .Where(u => u.Username == username)
        //    .ExecuteDeleteAsync();
    }
}
