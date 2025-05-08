using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Authentication;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.VisitedCountry;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Voyage;
using ShipVoyageManager.Data.Object;
using ShipVoyageManager.Service.Buisness;
using ShipVoyageManager.Service.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ShipVoyageManager.Tests;
public class ServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IShipRepository> _shipRepositoryMock;
    private readonly Mock<IPortRepository> _portRepositoryMock;
    private readonly Mock<IVoyageRepository> _voyageRepositoryMock;
    private readonly Mock<IVisitedCountryRepository> _visitedCountryRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IEncryptionService> _encryptionServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IConfiguration> _configurationMock;

    private readonly AuthenticationService _authenticationService;
    private readonly EncryptionService _encryptionService;
    private readonly EmailService _emailService;
    private readonly PortService _portService;
    private readonly ShipService _shipService;
    private readonly VoyageService _voyageService;
    private readonly UserService _userService;
    private readonly VisitedCountryService _visitedCountryService;

    public ServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _shipRepositoryMock = new Mock<IShipRepository>();
        _portRepositoryMock = new Mock<IPortRepository>();
        _voyageRepositoryMock = new Mock<IVoyageRepository>();
        _visitedCountryRepositoryMock = new Mock<IVisitedCountryRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();

        _authenticationService = new AuthenticationService(
            _userRepositoryMock.Object,
            _encryptionServiceMock.Object,
            _tokenServiceMock.Object
        );

        _encryptionService = new EncryptionService();

        _emailService = new EmailService(
            _userRepositoryMock.Object,
            _configurationMock.Object
        );

        _portService = new PortService(
            _portRepositoryMock.Object,
            _mapperMock.Object
        );

        _shipService = new ShipService(
            _shipRepositoryMock.Object,
            _mapperMock.Object
        );

        _voyageService = new VoyageService(
            _voyageRepositoryMock.Object,
            _mapperMock.Object
        );

        _userService = new UserService(
            _userRepositoryMock.Object,
            _emailServiceMock.Object,
            _tokenServiceMock.Object,
            _encryptionServiceMock.Object,
            _mapperMock.Object
        );

        _visitedCountryService = new VisitedCountryService(
            _visitedCountryRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    //AuthenticationService Tests
    [Fact]
    public async Task AuthenticationService_LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        var request = new AuthenticationRequestDto { UserCredential = "test", Password = "password" };
        var user = CreateUser("test", "test@gmail.com", "hashedPassword");

        _encryptionServiceMock.Setup(x => x.GenerateHashedPassword(request.Password)).Returns("hashedpassword");
        _userRepositoryMock.Setup(x => x.GetUserByUsernameOrEmailAndPasswordAsync(request.UserCredential, "hashedpassword")).ReturnsAsync(user);
        _tokenServiceMock.Setup(x => x.GetAuthentificationJwtAsync(user)).ReturnsAsync("token");

        var result = await _authenticationService.LoginAsync(request);

        Assert.NotNull(result);
        Assert.Equal("token", result.Token);
        Assert.Equal(AppConstants.USER_ROLE, result.Role);
    }

    //EncryptionService Test
    [Fact]
    public void EncryptionService_GenerateHashedPassword_ReturnsHashedPassword()
    {
        var password = "password";

        var result = _encryptionService.GenerateHashedPassword(password);

        Assert.NotNull(result);
    }

    //PortService Tests
    [Fact]
    public async Task PortService_GetFilteredPortsPaginatedAsync_ReturnsFilteredPaginatedPorts()
    {
        var page = 0;
        var pageSize = 10;
        var search = "Test";

        var ports = new List<PortDto>
        {
            CreatePortDto(name: "TestPort1"),
            CreatePortDto(name: "TestPort2")
        };

        _portRepositoryMock.Setup(x => x.GetFilteredPortsPaginatedAsync(page, pageSize, search)).ReturnsAsync(new FilteredPaginatedPortsDto
        {
            Ports = ports,
            TotalCount = ports.Count
        });

        var result = await _portService.GetFilteredPortsPaginatedAsync(page, pageSize, search);

        Assert.NotNull(result);
        Assert.Equal(ports.Count, result.TotalCount);
    }

    [Fact]
    public async Task GetAllPortsAsync_ReturnsAllPorts()
    {
        // Arrange
        var ports = new List<PortEntity>
            {
                CreatePort(name: "Port1"),
                CreatePort(name: "Port2")
            };

        _portRepositoryMock.Setup(repo => repo.GetAllPortsAsync()).ReturnsAsync(ports);
        _mapperMock.Setup(m => m.Map<List<PortDto>>(It.IsAny<List<PortEntity>>()))
            .Returns((List<PortEntity> source) => new List<PortDto>
            {
                    new PortDto { Id = source[0].Id, Name = source[0].Name, CountryName = source[0].CountryName },
                    new PortDto { Id = source[1].Id, Name = source[1].Name, CountryName = source[1].CountryName }
            });

        // Act
        var result = await _portService.GetAllPortsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Port1");
    }

    [Fact]
    public async Task AddPortAsync_ThrowsValidationException_WhenPortAlreadyExists()
    {
        // Arrange
        var portDto = new PortDto { Name = "ExistingPort", CountryName = "Country" };
        _portRepositoryMock.Setup(repo => repo.GetPortByNameAsync("ExistingPort")).ReturnsAsync(CreatePort(name: "ExistingPort"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _portService.AddPortAsync(portDto));
        Assert.Equal("Port with this name already exists. Please choose a different name.", exception.Message);
    }

    [Fact]
    public async Task AddPortAsync_AddsPort_WhenPortDoesNotExist()
    {
        // Arrange
        var portDto = new PortDto { Name = "NewPort", CountryName = "Country" };
        _portRepositoryMock.Setup(repo => repo.GetPortByNameAsync("NewPort")).ReturnsAsync((PortEntity)null); // Simulate no existing port
        _portRepositoryMock.Setup(repo => repo.AddPortAsync(It.IsAny<PortEntity>())).Returns(Task.CompletedTask);

        // Act
        await _portService.AddPortAsync(portDto);

        // Assert
        _portRepositoryMock.Verify(repo => repo.AddPortAsync(It.IsAny<PortEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePortAsync_UpdatesPort()
    {
        // Arrange
        var portDto = CreatePortDto();
        var portEntity = new PortEntity { Id = portDto.Id.Value, Name = portDto.Name, CountryName = portDto.CountryName };

        _mapperMock.Setup(m => m.Map<PortEntity>(portDto)).Returns(portEntity);
        _portRepositoryMock.Setup(repo => repo.UpdatePortAsync(portEntity)).Returns(Task.CompletedTask);

        // Act
        await _portService.UpdatePortAsync(portDto);

        // Assert
        _portRepositoryMock.Verify(repo => repo.UpdatePortAsync(portEntity), Times.Once);
    }

    [Fact]
    public async Task DeletePortByIdAsync_DeletesPort()
    {
        // Arrange
        var portId = Guid.NewGuid();
        _portRepositoryMock.Setup(repo => repo.DeletePortByIdAsync(portId)).Returns(Task.CompletedTask);

        // Act
        await _portService.DeletePortByIdAsync(portId);

        // Assert
        _portRepositoryMock.Verify(repo => repo.DeletePortByIdAsync(portId), Times.Once);
    }

    //ShipService Tests
    [Fact]
    public async Task GetFilteredShipsPaginatedAsync_ReturnsPaginatedShips()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var search = "Test";

        var ships = new List<ShipDto>
            {
               CreateShipDto(name: "TestShip1"),
               CreateShipDto(name: "TestShip2")
            };

        _shipRepositoryMock.Setup(repo => repo.GetFilteredShipsPaginatedAsync(page, pageSize, search)).ReturnsAsync(new FilteredPaginatedShipsDto
        {
            Ships = ships,
            TotalCount = ships.Count
        });

        // Act
        var result = await _shipService.GetFilteredShipsPaginatedAsync(page, pageSize, search);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ships.Count, result.Ships.Count);
    }

    [Fact]
    public async Task GetShipsOutOfDateRangeAsync_ReturnsShipsOutOfDateRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var ships = new List<ShipDto>
            {
                CreateShipDto(name: "Ship1"),
                CreateShipDto(name: "Ship2")
            };

        _shipRepositoryMock.Setup(repo => repo.GetShipsOutOfDateRangeAsync(startDate, endDate)).ReturnsAsync(new List<ShipEntity>
            {
                new ShipEntity { Id = ships[0].Id.Value, Name = ships[0].Name, MaxSpeed = ships[0].MaxSpeed },
                new ShipEntity { Id = ships[1].Id.Value, Name = ships[1].Name, MaxSpeed = ships[1].MaxSpeed }
            });

        _mapperMock.Setup(m => m.Map<List<ShipDto>>(It.IsAny<List<ShipEntity>>())).Returns(ships);

        // Act
        var result = await _shipService.GetShipsOutOfDateRangeAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ships.Count, result.Count);
    }

    [Fact]
    public async Task AddShipAsync_ThrowsValidationException_WhenShipNameAlreadyExists()
    {
        // Arrange
        var shipDto = CreateShipDto(name: "TestShip");
        _shipRepositoryMock.Setup(repo => repo.GetShipByNameAsync("TestShip")).ReturnsAsync(CreateShip());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _shipService.AddShipAsync(shipDto));
        Assert.Equal("Ship with this name already exists. Please choose a different name.", exception.Message);
    }

    [Fact]
    public async Task AddShipAsync_AddsShip_WhenShipNameDoesNotExist()
    {
        // Arrange
        var shipDto = CreateShipDto(name: "NewShip");
        _shipRepositoryMock.Setup(repo => repo.GetShipByNameAsync("NewShip")).ReturnsAsync((ShipEntity)null); // No existing ship with this name
        _shipRepositoryMock.Setup(repo => repo.AddShipAsync(It.IsAny<ShipEntity>())).Returns(Task.CompletedTask);

        // Act
        await _shipService.AddShipAsync(shipDto);

        // Assert
        _shipRepositoryMock.Verify(repo => repo.AddShipAsync(It.IsAny<ShipEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateShipAsync_UpdatesShip()
    {
        // Arrange
        var shipDto = CreateShipDto();
        var shipEntity = new ShipEntity { Id = shipDto.Id.Value, Name = shipDto.Name, MaxSpeed = shipDto.MaxSpeed };

        _mapperMock.Setup(m => m.Map<ShipEntity>(shipDto)).Returns(shipEntity);
        _shipRepositoryMock.Setup(repo => repo.UpdateShipAsync(shipEntity)).Returns(Task.CompletedTask);

        // Act
        await _shipService.UpdateShipAsync(shipDto);

        // Assert
        _shipRepositoryMock.Verify(repo => repo.UpdateShipAsync(shipEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteShipByIdAsync_DeletesShip()
    {
        // Arrange
        var shipId = Guid.NewGuid();
        _shipRepositoryMock.Setup(repo => repo.DeleteShipByIdAsync(shipId)).Returns(Task.CompletedTask);

        // Act
        await _shipService.DeleteShipByIdAsync(shipId);

        // Assert
        _shipRepositoryMock.Verify(repo => repo.DeleteShipByIdAsync(shipId), Times.Once);
    }

    //VoyageService Tests
    [Fact]
    public async Task GetVoyagesPaginatedAsync_ReturnsPaginatedVoyages()
    {
        // Arrange
        var page = 0;
        var pageSize = 10;
        var voyages = new List<VoyageDto>
            {
                CreateVoyageDto(),
                CreateVoyageDto()
            };

        _voyageRepositoryMock.Setup(x => x.GetVoyagesPaginatedAsync(page, pageSize))
            .ReturnsAsync(new PaginatedVoyagesDto
            {
                Voyages = voyages,
                TotalCount = voyages.Count
            });

        // Act
        var result = await _voyageService.GetVoyagesPaginatedAsync(page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(voyages.Count, result.TotalCount);
    }

    [Fact]
    public async Task AddVoyageAsync_AddsVoyage()
    {
        // Arrange
        var voyageDto = CreateVoyageDto();
        var voyageEntity = new VoyageEntity
        {
            Id = voyageDto.Id.Value,
            StartTime = voyageDto.StartTime,
            EndTime = voyageDto.EndTime,
            ShipId = voyageDto.ShipId,
            DeparturePortId = voyageDto.DeparturePortId,
            ArrivalPortId = voyageDto.ArrivalPortId
        };

        _mapperMock.Setup(m => m.Map<VoyageEntity>(voyageDto)).Returns(voyageEntity);
        _voyageRepositoryMock.Setup(repo => repo.AddVoyageAsync(voyageEntity)).Returns(Task.CompletedTask);

        // Act
        await _voyageService.AddVoyageAsync(voyageDto);

        // Assert
        _voyageRepositoryMock.Verify(repo => repo.AddVoyageAsync(voyageEntity), Times.Once);
    }

    [Fact]
    public async Task UpdateVoyageAsync_UpdatesVoyage()
    {
        // Arrange
        var voyageDto = CreateVoyageDto();
        var voyageEntity = new VoyageEntity
        {
            Id = voyageDto.Id.Value,
            StartTime = voyageDto.StartTime,
            EndTime = voyageDto.EndTime,
            ShipId = voyageDto.ShipId,
            DeparturePortId = voyageDto.DeparturePortId,
            ArrivalPortId = voyageDto.ArrivalPortId
        };

        _mapperMock.Setup(m => m.Map<VoyageEntity>(voyageDto)).Returns(voyageEntity);
        _voyageRepositoryMock.Setup(repo => repo.UpdateVoyageAsync(voyageEntity)).Returns(Task.CompletedTask);

        // Act
        await _voyageService.UpdateVoyageAsync(voyageDto);

        // Assert
        _voyageRepositoryMock.Verify(repo => repo.UpdateVoyageAsync(voyageEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteVoyageAsync_DeletesVoyage()
    {
        // Arrange
        var voyageId = Guid.NewGuid();
        _voyageRepositoryMock.Setup(repo => repo.DeleteVoyageByIdAsync(voyageId)).Returns(Task.CompletedTask);

        // Act
        await _voyageService.DeleteVoyageAsync(voyageId);

        // Assert
        _voyageRepositoryMock.Verify(repo => repo.DeleteVoyageByIdAsync(voyageId), Times.Once);
    }

    //VisitedCountryService Tests
    [Fact]
    public async Task GetAllVisitedCountriesAsync_ReturnsMappedVisitedCountries()
    {
        // Arrange
        var visitedCountries = new List<VisitedCountryEntity>
            {
                CreateVisitedCountry("Country1", DateTime.UtcNow.AddDays(-5)),
                CreateVisitedCountry("Country2", DateTime.UtcNow.AddDays(-10))
            };

        var visitedCountryDtos = new List<VisitedCountryDto>
            {
                CreateVisitedCountryDto("Country1", DateTime.UtcNow.AddDays(-5)),
                CreateVisitedCountryDto("Country2", DateTime.UtcNow.AddDays(-10))
            };

        _visitedCountryRepositoryMock.Setup(repo => repo.GetAllVisitedCountriesAsync())
            .ReturnsAsync(visitedCountries);

        _mapperMock.Setup(m => m.Map<List<VisitedCountryDto>>(visitedCountries))
            .Returns(visitedCountryDtos);

        // Act
        var result = await _visitedCountryService.GetAllVisitedCountriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(visitedCountryDtos.Count, result.Count);
        _mapperMock.Verify(m => m.Map<List<VisitedCountryDto>>(visitedCountries), Times.Once);
    }

    [Fact]
    public async Task UpdateVisitedCountriesAsync_AddsNewVisitedCountries()
    {
        // Arrange
        var DateNow = DateTime.UtcNow;
        var DateYesterday = DateNow.AddDays(-1);
        var currentVisitedCountries = new List<VisitedCountryEntity>
            {
                CreateVisitedCountry("Country1", DateNow),
            };

        var visitedCountriesLastYear = new List<VisitedCountryEntity>
            {
                CreateVisitedCountry("Country1", DateNow),
                CreateVisitedCountry("Country2", DateYesterday)
            };

        var visitedCountriesToAdd = new List<VisitedCountryEntity>
            {
                visitedCountriesLastYear.Last()
            };

        _visitedCountryRepositoryMock.Setup(repo => repo.GetAllVisitedCountriesAsync())
            .ReturnsAsync(currentVisitedCountries);
        _visitedCountryRepositoryMock.Setup(repo => repo.GetVisitedCountriesLastYearAsync())
            .ReturnsAsync(visitedCountriesLastYear);
        _visitedCountryRepositoryMock.Setup(repo => repo.AddVisitedCountriesAsync(visitedCountriesToAdd))
            .Returns(Task.CompletedTask);

        // Act
        await _visitedCountryService.UpdateVisitedCountriesAsync();

        // Assert
        _visitedCountryRepositoryMock.Verify(repo => repo.AddVisitedCountriesAsync(visitedCountriesToAdd), Times.Once);
    }

    [Fact]
    public async Task UpdateVisitedCountriesAsync_DeletesOldVisitedCountries()
    {
        // Arrange
        var DateNow = DateTime.UtcNow;
        var DateYesterday = DateNow.AddDays(-1);
        var currentVisitedCountries = new List<VisitedCountryEntity>
            {
                CreateVisitedCountry("Country1", DateNow),
                CreateVisitedCountry("Country2", DateYesterday)
            };

        var visitedCountriesLastYear = new List<VisitedCountryEntity>
            {
                CreateVisitedCountry("Country1", DateNow),
            };

        var visitedCountriesToDelete = new List<VisitedCountryEntity>
            {
                currentVisitedCountries.Last()
            };

        _visitedCountryRepositoryMock.Setup(repo => repo.GetAllVisitedCountriesAsync())
            .ReturnsAsync(currentVisitedCountries);
        _visitedCountryRepositoryMock.Setup(repo => repo.GetVisitedCountriesLastYearAsync())
            .ReturnsAsync(visitedCountriesLastYear);
        _visitedCountryRepositoryMock.Setup(repo => repo.DeleteVisitedCountriesAsync(visitedCountriesToDelete))
            .Returns(Task.CompletedTask);

        // Act
        await _visitedCountryService.UpdateVisitedCountriesAsync();

        // Assert
        _visitedCountryRepositoryMock.Verify(repo => repo.DeleteVisitedCountriesAsync(visitedCountriesToDelete), Times.Once);
    }

    //UserService Tests

    [Fact]
    public async Task UserService_GetFilteredUsersPaginatedAsync_ReturnsFilteredUsers()
    {
        //Arrange
        var page = 0;
        var pageSize = 10;
        var search = "Test";

        var users = new List<UserDto>
        {
            CreateUserDto(username: "TestUser1"),
            CreateUserDto(username: "TestUser2")
        };

        _userRepositoryMock.Setup(x => x.GetFilteredUsersPaginatedAsync(page, pageSize, search)).ReturnsAsync(new FilteredPaginatedUsersDto
        {
            Users = users,
            TotalCount = users.Count
        });

        //Act
        var result = await _userService.GetFilteredUsersPaginatedAsync(page, pageSize, search);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(users.Count, result.TotalCount);
    }


    [Fact]
    public async Task UserService_RegisterUserAsync_RegistersUser()
    {
        //Arrange
        var userRegistrationDto = new UserRegistrationDto
        {
            Username = "testuser",
            Email = "testUser@gamil.com",
            Password = "password",
            RepeatPassword = "password"
        };

        _mapperMock.Setup(x => x.Map<UserEntity>(userRegistrationDto)).Returns(CreateUser(userRegistrationDto.Username, userRegistrationDto.Email, "hashedPassword"));
        _encryptionServiceMock.Setup(x => x.GenerateHashedPassword("password")).Returns("hashedpassword");
        _tokenServiceMock.Setup(x => x.GenerateRandomTokenAsync()).ReturnsAsync("token");

        //Act
        await _userService.RegisterUserAsync(userRegistrationDto);

        //Assert
        _userRepositoryMock.Verify(x => x.AddUserAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task UserService_VerifyIfResetPasswordTokenExistsAsync_ReturnsTrue()
    {
        //Arrange
        var token = "token";
        var user = CreateUser("testUser", "testUser@gmail.com");

        _userRepositoryMock.Setup(x => x.GetUserByResetPasswordTokenAsync(token)).ReturnsAsync(user);

        //Act
        var result = await _userService.VerifyIfResetPasswordTokenExistsAsync(token);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UserService_VerifyEmailByRegistrationTokenAsync_VerifiesEmail()
    {
        //Arrange
        var token = "token";
        var user = CreateUser("testUser", "testUser@gmail.com");

        _userRepositoryMock.Setup(x => x.GetUserByRegistrationTokenAsync(token)).ReturnsAsync(user);

        //Act
        await _userService.VerifyEmailByRegistrationTokenAsync(token);

        //Assert
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task UserService_SendResetPasswordTokenByEmailAsync_SendsEmail()
    {
        //Arrange
        var email = "test@example.com";
        var user = CreateUser("testUser", email);

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(email)).ReturnsAsync(user);
        _tokenServiceMock.Setup(x => x.GenerateRandomTokenAsync()).ReturnsAsync("token");

        //Act
        await _userService.SendResetPasswordTokenByEmailAsync(email);

        //Assert
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task UserService_ChangePasswordAsync_ChangesPassword()
    {
        //Arrange
        var changePasswordDto = new UserChangePasswordDto
        {
            NewPassword = "newpassword",
            RepeatNewPassword = "newpassword",
            ResetPasswordToken = "token"
        };

        var user = CreateUser("test", "test@gmail.com");

        _userRepositoryMock.Setup(x => x.GetUserByResetPasswordTokenAsync(changePasswordDto.ResetPasswordToken)).ReturnsAsync(user);
        _encryptionServiceMock.Setup(x => x.GenerateHashedPassword("newpassword")).Returns("hashedpassword");

        //Act
        await _userService.ChangePasswordAsync(changePasswordDto);

        //Assert
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task UserService_DeleteUserByUsernameAsync_DeletesUser()
    {
        //Arrange
        var username = "testuser";
        var user = CreateUser(username, "testuser@gmail.com");

        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync(username)).ReturnsAsync(user);

        //Act
        await _userService.DeleteUserByUsernameAsync(username);

        //Assert
        _userRepositoryMock.Verify(x => x.DeleteUserByUsernameAsync(username), Times.Once);
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
            IsEmailConfirmed = true,
            RegistrationToken = Guid.NewGuid().ToString(),
            ResetPasswordToken = null,
            Role = new RoleEntity { Id = new Guid(), RoleName = AppConstants.USER_ROLE }
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

    private VoyageDto CreateVoyageDto(Guid? id = null, DateTime? startTime = null, DateTime? endTime = null,
                                                Guid? shipId = null, string shipName = null, Guid? departurePortId = null,
                                                string departurePortName = null, Guid? arrivalPortId = null, string arrivalPortName = null)
    {
        return new VoyageDto
        {
            Id = id ?? Guid.NewGuid(),
            StartTime = startTime ?? DateTime.UtcNow,
            EndTime = endTime ?? DateTime.UtcNow.AddHours(1),
            ShipId = shipId ?? Guid.NewGuid(),
            ShipName = shipName ?? "Default Ship",
            DeparturePortId = departurePortId ?? Guid.NewGuid(),
            DeparturePortName = departurePortName ?? "Default Departure Port",
            ArrivalPortId = arrivalPortId ?? Guid.NewGuid(),
            ArrivalPortName = arrivalPortName ?? "Default Arrival Port"
        };
    }

    private VisitedCountryDto CreateVisitedCountryDto(string countryName = "Default Country", DateTime? visitedDate = null)
    {
        return new VisitedCountryDto
        {
            CountryName = countryName,
            VisitedDate = visitedDate ?? DateTime.UtcNow
        };
    }

    private UserDto CreateUserDto(Guid? id = null, string username = "DefaultUser", string email = "user@example.com",
                                         bool isEmailConfirmed = false, DateTime? registrationDate = null)
    {
        return new UserDto
        {
            Id = id ?? Guid.NewGuid(),
            Username = username,
            Email = email,
            IsEmailConfirmed = isEmailConfirmed,
            RegistrationDate = registrationDate ?? DateTime.UtcNow
        };
    }

    private UserChangePasswordDto CreateUserChangePasswordDto(string resetPasswordToken = "ResetToken123", string newPassword = "NewPassword123",string repeatNewPassword = "NewPassword123")
    {
        return new UserChangePasswordDto
        {
            ResetPasswordToken = resetPasswordToken,
            NewPassword = newPassword,
            RepeatNewPassword = repeatNewPassword
        };
    }

    private PortDto CreatePortDto(Guid? id = null, string name = "Default Port", string countryName = "Default Country",
                                        ICollection<VoyageDto>? departingVoyages = null, ICollection<VoyageDto>? arrivingVoyages = null)
    {
        return new PortDto
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            CountryName = countryName,
            DepartingVoyages = departingVoyages ?? new List<VoyageDto>(),
            ArrivingVoyages = arrivingVoyages ?? new List<VoyageDto>()
        };
    }

    private ShipDto CreateShipDto(Guid? id = null, string name = "Default Ship", double maxSpeed = 20.0,
                                        ICollection<VoyageDto>? voyages = null)
    {
        return new ShipDto
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            MaxSpeed = maxSpeed,
            Voyages = voyages ?? new List<VoyageDto>()
        };
    }
}
