using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using ShipVoyageManager.Data.Access;
using ShipVoyageManager.Data.Access.Data;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Tests;
public class RepositoryTests
{
    private readonly DbContextOptions<ShipVoyageManagerContext> _contextOptions;
    private readonly Mock<IMapper> _mapperMock;

    private readonly PortRepository _portRepository;
    private readonly ShipRepository _shipRepository;
    private readonly VoyageRepository _voyageRepository;
    private readonly UserRepository _userRepository;
    private readonly VisitedCountryRepository _visitedCountryRepository;

    public RepositoryTests()
    {
        _contextOptions = new DbContextOptionsBuilder<ShipVoyageManagerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _mapperMock = new Mock<IMapper>();

        _portRepository = new PortRepository(new ShipVoyageManagerContext(_contextOptions), _mapperMock.Object);
        _shipRepository = new ShipRepository(new ShipVoyageManagerContext(_contextOptions), _mapperMock.Object);
        _voyageRepository = new VoyageRepository(new ShipVoyageManagerContext(_contextOptions), _mapperMock.Object);
        _userRepository = new UserRepository(new ShipVoyageManagerContext(_contextOptions), _mapperMock.Object);
        _visitedCountryRepository = new VisitedCountryRepository(new ShipVoyageManagerContext(_contextOptions));
    }

    //PortRepository Tests

    [Fact]
    public async Task GetFilteredPortsPaginatedAsync_ReturnsFilteredPagedData()
    {
        // Arrange
        var ports = new List<PortEntity>
        {
            CreatePort("Alpha"),
            CreatePort("Beta"),
            CreatePort("Alpha Harbor")
        };

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ports.AddRange(ports);
            await context.SaveChangesAsync();
        }

        _mapperMock.Setup(m => m.Map<List<PortDto>>(It.IsAny<List<PortEntity>>()))
            .Returns((List<PortEntity> source) => source.Select(p => new PortDto { Id = p.Id, Name = p.Name, CountryName = p.CountryName }).ToList());

        // Act
        var result = await _portRepository.GetFilteredPortsPaginatedAsync(0, 2, "Alpha");

        //Assert
        Assert.Equal(2, result.Ports.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPortByNameAsync_ReturnsCorrectPort()
    {
        // Arrange
        var port = CreatePort("UniquePortName");
        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ports.Add(port);
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _portRepository.GetPortByNameAsync("UniquePortName");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UniquePortName", result?.Name);
    }

    [Fact]
    public async Task GetAllPortsAsync_ReturnsAllPorts()
    {
        //Arrange
        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ports.Add(CreatePort("Port1"));
            context.Ports.Add(CreatePort("Port2"));
            await context.SaveChangesAsync();
        }

        //Act
        var result = await _portRepository.GetAllPortsAsync();

        //Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task AddPortAsync_AddsPort()
    {
        //Arrange
        var port = CreatePort();

        //Act
        await _portRepository.AddPortAsync(port);

        using var context = new ShipVoyageManagerContext(_contextOptions);

        var result = await context.Ports.FirstOrDefaultAsync(p => p.Id == port.Id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(port.Name, result.Name);
    }

    [Fact]
    public async Task UpdatePortAsync_UpdatesPort()
    {
        //Arrange
        var port = CreatePort("OldName");
        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ports.Add(port);
            await context.SaveChangesAsync();
        }

        port.Name = "NewName";

        //Act
        await _portRepository.UpdatePortAsync(port);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var updated = await verifyContext.Ports.FirstOrDefaultAsync(p => p.Id == port.Id);

        //Assert
        Assert.Equal("NewName", updated?.Name);
    }

    [Fact]
    public async Task DeletePortByIdAsync_DeletesPort()
    {
        //Arrange
        var port = CreatePort();
        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ports.Add(port);
            await context.SaveChangesAsync();
        }

        //Act
        await _portRepository.DeletePortByIdAsync(port.Id);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var deleted = await verifyContext.Ports.FindAsync(port.Id);

        //Assert
        Assert.Null(deleted);
    }

    //ShipRepository Tests

    [Fact]
    public async Task GetShipsOutOfDateRangeAsync_ReturnsCorrectShips()
    {
        //Arrange
        var inRangePort = CreatePort("PortA");
        var ship1 = CreateShip("InRangeShip");
        var ship2 = CreateShip("OutOfRangeShip");

        var voyage = new VoyageEntity
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.Today,
            EndTime = DateTime.Today.AddHours(5),
            ShipId = ship1.Id,
            Ship = ship1,
            DeparturePort = inRangePort,
            ArrivalPort = inRangePort
        };
        ship1.Voyages.Add(voyage);

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ports.Add(inRangePort);
            context.Ships.AddRange(ship1, ship2);
            context.Voyages.Add(voyage);
            await context.SaveChangesAsync();
        }

        //Act
        var result = await _shipRepository.GetShipsOutOfDateRangeAsync(DateTime.Today.AddHours(1), DateTime.Today.AddHours(6));

        //Assert
        Assert.Single(result);
        Assert.Equal("OutOfRangeShip", result.First().Name);
    }

    [Fact]
    public async Task GetFilteredShipsPaginatedAsync_ReturnsFilteredPagedData()
    {
        //Arrange
        var ships = new List<ShipEntity>
        {
            CreateShip("Explorer"),
            CreateShip("Navigator"),
            CreateShip("Explorer Supreme")
        };

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.AddRange(ships);
            await context.SaveChangesAsync();
        }

        _mapperMock.Setup(m => m.Map<List<ShipDto>>(It.IsAny<List<ShipEntity>>()))
            .Returns((List<ShipEntity> source) => source.Select(s => new ShipDto { Id = s.Id, Name = s.Name, MaxSpeed = s.MaxSpeed }).ToList());

        //Act
        var result = await _shipRepository.GetFilteredShipsPaginatedAsync(0, 2, "Explorer");

        //Assert
        Assert.Equal(2, result.Ships.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetShipByNameAsync_ReturnsCorrectShip()
    {
        //Arrange
        var ship = CreateShip("UniqueShipName");
        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.Add(ship);
            await context.SaveChangesAsync();
        }

        //Act
        var result = await _shipRepository.GetShipByNameAsync("UniqueShipName");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("UniqueShipName", result?.Name);
    }

    [Fact]
    public async Task AddShipAsync_AddsShip()
    {
        //Arrange
        var ship = CreateShip();

        //Act
        await _shipRepository.AddShipAsync(ship);

        using var context = new ShipVoyageManagerContext(_contextOptions);
        var result = await context.Ships.FirstOrDefaultAsync(s => s.Id == ship.Id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(ship.Name, result.Name);
    }

    [Fact]
    public async Task UpdateShipAsync_UpdatesShip()
    {
        //Arrange
        var ship = CreateShip("OldName");
        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.Add(ship);
            await context.SaveChangesAsync();
        }

        ship.Name = "NewName";

        //Act
        await _shipRepository.UpdateShipAsync(ship);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var updated = await verifyContext.Ships.FirstOrDefaultAsync(s => s.Id == ship.Id);

        //Assert
        Assert.Equal("NewName", updated?.Name);
    }

    [Fact]
    public async Task DeleteShipByIdAsync_DeletesShip()
    {
        //Arrange
        var ship = CreateShip();
        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.Add(ship);
            await context.SaveChangesAsync();
        }

        //Act
        await _shipRepository.DeleteShipByIdAsync(ship.Id);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var deleted = await verifyContext.Ships.FindAsync(ship.Id);

        //Assert
        Assert.Null(deleted);
    }

    //VoyageRepository Tests

    [Fact]
    public async Task GetVoyagesPaginatedAsync_ReturnsPaginatedVoyages()
    {
        //Arrange
        var port = CreatePort();
        var ship = CreateShip();
        var voyage1 = CreateVoyage(ship, port, port);
        var voyage2 = CreateVoyage(ship, port, port);

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.Add(ship);
            context.Ports.Add(port);
            context.Voyages.AddRange(voyage1, voyage2);
            await context.SaveChangesAsync();
        }

        _mapperMock.Setup(m => m.Map<List<VoyageDto>>(It.IsAny<List<VoyageEntity>>()))
            .Returns((List<VoyageEntity> source) => source.Select(v => new VoyageDto
            {
                Id = v.Id,
                DeparturePortName = v.DeparturePort?.Name ?? "",
                ArrivalPortName = v.ArrivalPort?.Name ?? "",
                ShipName = v.Ship?.Name ?? ""
            }).ToList());

        //Act
        var result = await _voyageRepository.GetVoyagesPaginatedAsync(0, 2);

        //Assert
        Assert.Equal(2, result.Voyages.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task AddVoyageAsync_AddsVoyage()
    {
        //Arrange
        var port = CreatePort();
        var ship = CreateShip();
        var voyage = CreateVoyage(ship, port, port);

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.Add(ship);
            context.Ports.Add(port);
            await context.SaveChangesAsync();
        }

        //Act
        await _voyageRepository.AddVoyageAsync(voyage);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var result = await verifyContext.Voyages.FindAsync(voyage.Id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(ship.Id, result.ShipId);
    }

    [Fact]
    public async Task UpdateVoyageAsync_UpdatesVoyage()
    {
        //Arrange
        var port = CreatePort();
        var ship = CreateShip();
        var voyage = CreateVoyage(ship, port, port);

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.Add(ship);
            context.Ports.Add(port);
            context.Voyages.Add(voyage);
            await context.SaveChangesAsync();
        }

        voyage.EndTime = voyage.EndTime.AddHours(2);

        //Act
        await _voyageRepository.UpdateVoyageAsync(voyage);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var updated = await verifyContext.Voyages.FindAsync(voyage.Id);

        //Assert
        Assert.Equal(voyage.EndTime, updated.EndTime);
    }

    [Fact]
    public async Task DeleteVoyageByIdAsync_DeletesVoyage()
    {
        //Arrange
        var port = CreatePort();
        var ship = CreateShip();
        var voyage = CreateVoyage(ship, port, port);

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ships.Add(ship);
            context.Ports.Add(port);
            context.Voyages.Add(voyage);
            await context.SaveChangesAsync();
        }

        //Act
        await _voyageRepository.DeleteVoyageByIdAsync(voyage.Id);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var deleted = await verifyContext.Voyages.FindAsync(voyage.Id);

        //Assert
        Assert.Null(deleted);
    }

    //VisitedCountryRepository Tests

    [Fact]
    public async Task GetAllVisitedCountriesAsync_ReturnsAllOrderedByDate()
    {
        //Arrange
        var visited1 = CreateVisitedCountry("Country1", DateTime.UtcNow.AddDays(-10));
        var visited2 = CreateVisitedCountry("Country2", DateTime.UtcNow.AddDays(-5));

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.VisitedCountries.AddRange(visited1, visited2);
            await context.SaveChangesAsync();
        }

        //Act
        var result = await _visitedCountryRepository.GetAllVisitedCountriesAsync();

        //Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Country1", result.First().CountryName);
    }

    [Fact]
    public async Task GetVisitedCountriesLastYearAsync_ReturnsLatestVisitPerCountry()
    {
        //Arrange
        var port1 = CreatePort("Port1", "Country1");
        var port2 = CreatePort("Port2", "Country2");
        var ship = CreateShip("Ship1");

        var voyage1 = new VoyageEntity
        {
            Id = Guid.NewGuid(),
            Ship = ship,
            ShipId = ship.Id,
            DeparturePort = port1,
            DeparturePortId = port1.Id,
            ArrivalPort = port2,
            ArrivalPortId = port2.Id,
            StartTime = DateTime.UtcNow.AddDays(-200),
            EndTime = DateTime.UtcNow.AddDays(-190),
            VoyageDate = DateTime.UtcNow.AddDays(-200)
        };

        var voyage2 = new VoyageEntity
        {
            Id = Guid.NewGuid(),
            Ship = ship,
            ShipId = ship.Id,
            DeparturePort = port2,
            DeparturePortId = port2.Id,
            ArrivalPort = port1,
            ArrivalPortId = port1.Id,
            StartTime = DateTime.UtcNow.AddDays(-100),
            EndTime = DateTime.UtcNow.AddDays(-90),
            VoyageDate = DateTime.UtcNow.AddDays(-100)
        };

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Ports.AddRange(port1, port2);
            context.Ships.Add(ship);
            context.Voyages.AddRange(voyage1, voyage2);
            await context.SaveChangesAsync();
        }

        //Act
        var result = await _visitedCountryRepository.GetVisitedCountriesLastYearAsync();

        //Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.CountryName == "Country1");
        Assert.Contains(result, r => r.CountryName == "Country2");

    }

    [Fact]
    public async Task AddVisitedCountriesAsync_AddsEntities()
    {
        //Arrange
        var newVisits = new List<VisitedCountryEntity>
            {
                CreateVisitedCountry("AddTest1", DateTime.UtcNow),
                CreateVisitedCountry("AddTest2", DateTime.UtcNow.AddDays(-2))
            };

        //Act
        await _visitedCountryRepository.AddVisitedCountriesAsync(newVisits);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);

        //Assert
        Assert.Equal(2, verifyContext.VisitedCountries.Count());
    }

    [Fact]
    public async Task DeleteVisitedCountriesAsync_DeletesEntities()
    {
        //Arrange
        var visits = new List<VisitedCountryEntity>
            {
                CreateVisitedCountry("DeleteTest1", DateTime.UtcNow),
                CreateVisitedCountry("DeleteTest2", DateTime.UtcNow.AddDays(-1))
            };

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.VisitedCountries.AddRange(visits);
            await context.SaveChangesAsync();
        }

        //Act
        await _visitedCountryRepository.DeleteVisitedCountriesAsync(visits);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
      
        //Assert
        Assert.Empty(verifyContext.VisitedCountries);
    }

    //UserRepository Tests

    [Fact]
    public async Task GetUserByRegistrationTokenAsync_ReturnsUser()
    {
        //Arrange
        var user = CreateUser("regtokenuser", "reguser@example.com");
        user.RegistrationToken = "REG123";

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        await _userRepository.AddUserAsync(user);

        //Act
        var result = await _userRepository.GetUserByRegistrationTokenAsync("REG123");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("regtokenuser", result.Username);
    }

    [Fact]
    public async Task GetUserByResetPasswordTokenAsync_ReturnsUser()
    {
        //Arrange
        var user = CreateUser("resettokenuser", "reset@example.com");
        user.ResetPasswordToken = "RESET456";

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        await _userRepository.AddUserAsync(user);

        //Act
        var result = await _userRepository.GetUserByResetPasswordTokenAsync("RESET456");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("resettokenuser", result.Username);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ReturnsUser()
    {
        //Arrange
        var user = CreateUser("testuser", "testuser@example.com");

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        await _userRepository.AddUserAsync(user);

        //Act
        var result = await _userRepository.GetUserByUsernameAsync("testuser");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("testuser@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByUsernameOrEmailAndPasswordAsync_ReturnsUser()
    {
        //Arrange
        var user = CreateUser("testuser", "testuser@example.com", "password123");

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        await _userRepository.AddUserAsync(user);

        //Act
        var result = await _userRepository.GetUserByUsernameOrEmailAndPasswordAsync("testuser", "password123");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetFilteredUsersPaginatedAsync_ReturnsPaginatedUsers()
    {
        //Arrange
        var user1 = CreateUser("user1", "user1@example.com");
        var user2 = CreateUser("user2", "user2@example.com");

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        await _userRepository.AddUserAsync(user1);
        await _userRepository.AddUserAsync(user2);

        _mapperMock
            .Setup(m => m.Map<List<UserDto>>(It.IsAny<List<UserEntity>>()))
            .Returns((List<UserEntity> source) => source.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsEmailConfirmed = u.IsEmailConfirmed,
                RegistrationDate = u.RegistrationDate
            }).ToList());

        //Act
        var result = await _userRepository.GetFilteredUsersPaginatedAsync(0, 10, "user");

        //Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Users, u => u.Username == "user1");
        Assert.Contains(result.Users, u => u.Username == "user2");
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser()
    {
        //Arrange
        var user = CreateUser("emailuser", "emailuser@example.com");

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        await _userRepository.AddUserAsync(user);

        //Act
        var result = await _userRepository.GetUserByEmailAsync("emailuser@example.com");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("emailuser@example.com", result.Email);
    }

    [Fact]
    public async Task AddUserAsync_AddsUserToDatabase()
    {
        //Arrange
        var user = CreateUser("newuser", "newuser@example.com");

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        //Act
        await _userRepository.AddUserAsync(user);

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);

        var result = await verifyContext.Users.FirstOrDefaultAsync(u => u.Username == "newuser");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("newuser@example.com", result.Email);
    }

    [Fact]
    public async Task DeleteUserByUsernameAsync_DeletesUserFromDatabase()
    {
        //Arrange
        var user = CreateUser("deleteuser", "deleteuser@example.com");

        using (var context = new ShipVoyageManagerContext(_contextOptions))
        {
            context.Roles.Add(new RoleEntity { RoleName = AppConstants.USER_ROLE });
            await context.SaveChangesAsync();
        }

        await _userRepository.AddUserAsync(user);

        //Act
        await _userRepository.DeleteUserByUsernameAsync("deleteuser");

        using var verifyContext = new ShipVoyageManagerContext(_contextOptions);
        var result = await verifyContext.Users.FirstOrDefaultAsync(u => u.Username == "deleteuser");
        //Assert
        Assert.Null(result);
    }

    private PortEntity CreatePort(string name = "Test Port", string country = "Test Country")
    {
        return new PortEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            CountryName = country,
            DepartingVoyages = new List<VoyageEntity>(),
            ArrivingVoyages = new List<VoyageEntity>()
        };
    }

    private ShipEntity CreateShip(string name = "Test Ship", double maxSpeed = 20.0)
    {
        return new ShipEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            MaxSpeed = maxSpeed,
            Voyages = new List<VoyageEntity>()
        };
    }

    private VoyageEntity CreateVoyage(ShipEntity ship, PortEntity dep, PortEntity arr)
    {
        return new VoyageEntity
        {
            Id = Guid.NewGuid(),
            VoyageDate = DateTime.Today,
            StartTime = DateTime.Today.AddHours(1),
            EndTime = DateTime.Today.AddHours(5),
            ShipId = ship.Id,
            DeparturePortId = dep.Id,
            ArrivalPortId = arr.Id,
        };
    }

    private UserEntity CreateUser(string username, string email, string password = "defaultPassword", string role = AppConstants.USER_ROLE)
    {
        return new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            Password = password,
            RegistrationToken = Guid.NewGuid().ToString(),
            ResetPasswordToken = null,
        };
    }

    private VisitedCountryEntity CreateVisitedCountry(string countryName = "Test Country", DateTime visitedDate = default)
    {
        return new VisitedCountryEntity
        {
            Id = Guid.NewGuid(),
            CountryName = countryName,
            VisitedDate = visitedDate == default ? DateTime.UtcNow : visitedDate
        };
    }
}
