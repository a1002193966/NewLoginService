using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Login.Data;
using Login.Data.Interface;
using Login.Integration.Interface.Commands;
using Login.Services.CommandHandlers;
using Login.Services.UtilityServices.PasswordService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Login.Tests.HandlerTests
{
    [TestClass]
    public class RegisterHandlerTests
    {
        private ILoginDbContext _loginDbContext;
        private Mock<ICryptoService> _cryptoService;
        private RegisterHandler _registerHandler;

        [TestInitialize]
        public void Setup()
        {
            _loginDbContext = InitializeInMemoryDatabase();

            _cryptoService = new();
            _cryptoService.Setup(m => m.EncryptAes(It.IsAny<string>()))
                .ReturnsAsync(new Secret());

            _registerHandler = new(_loginDbContext, _cryptoService.Object);
        }

        [TestMethod]
        public async Task ShouldInvokeEncryptAesAsync()
        {
            RegisterCommand request = new()
            {
                Email = "test@test.com",
                Password = "Qwerty123!",
                ConfirmPassword = "Qwerty123!"
            };

            await _registerHandler.Handle(request, It.IsAny<CancellationToken>());

            _cryptoService.Verify(m => m.EncryptAes(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task ShouldReturnCorrectResponse()
        {
            RegisterCommand request = new()
            {
                Email = "test@test.com",
                Password = "Qwerty123!",
                ConfirmPassword = "Qwerty123!"
            };

            var response = await _registerHandler.Handle(request, It.IsAny<CancellationToken>());

            response.Id.Should().NotBe(Guid.Empty);
            response.Status.Should().BeTrue();
        }

        [TestMethod]
        public async Task ShouldSaveToDatabase()
        {
            RegisterCommand request = new()
            {
                Email = "test@test.com",
                Password = "Qwerty123!",
                ConfirmPassword = "Qwerty123!"
            };

            await _registerHandler.Handle(request, It.IsAny<CancellationToken>());

            var account = await _loginDbContext.Account.FirstOrDefaultAsync(x => x.Email == request.Email);

            account.Should().NotBeNull();
            account.Email.Should().BeEquivalentTo(request.Email);
            account.IsDeleted.Should().BeFalse();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (_loginDbContext is LoginDbContext databaseFacade)
            {
                await databaseFacade.Database.EnsureDeletedAsync();
                await databaseFacade.DisposeAsync();
            }
        }

        private ILoginDbContext InitializeInMemoryDatabase()
        {
            var options = new DbContextOptionsBuilder<LoginDbContext>()
                .UseInMemoryDatabase(databaseName: "LoginDatabase")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _loginDbContext = new LoginDbContext(options);
            return _loginDbContext;
        }
    }
}
