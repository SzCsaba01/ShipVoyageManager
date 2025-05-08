namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
public class FilteredPaginatedUsersDto
{
    public List<UserDto> Users { get; set; }
    public int TotalCount { get; set; }
}
