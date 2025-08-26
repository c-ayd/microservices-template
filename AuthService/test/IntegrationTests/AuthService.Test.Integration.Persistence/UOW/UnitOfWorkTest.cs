using Cayd.Test.Generators;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Domain.SeedWork;
using AuthService.Persistence.DbContexts;
using AuthService.Persistence.UOW;
using AuthService.Test.Integration.Persistence.Collections;
using AuthService.Test.Utility.Extensions.EFCore;
using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.UOW
{
    [Collection(nameof(AppDbContextCollection))]
    public class UnitOfWorkTest
    {
        private readonly AppDbContext _appDbContext;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTest(AppDbContextFixture appDbContextFixture)
        {
            _appDbContext = appDbContextFixture.DbContext;

            _unitOfWork = new UnitOfWork(_appDbContext);
        }

        public enum ETransactionResult
        {
            Commit      =   0,
            Rollback    =   1
        }

        [Theory]
        [InlineData(ETransactionResult.Commit)]
        [InlineData(ETransactionResult.Rollback)]
        public async Task Transaction_WhenTransactionMethodsAreCalled_ShouldUseTransactionalOperation(ETransactionResult transactionResult)
        {
            // Arrange
            var email = EmailGenerator.Generate();
            var user = new User()
            {
                Email = email
            };

            // Act
            using (var transactionScope = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _appDbContext.Users.AddAsync(user);
                    await _appDbContext.SaveChangesAsync();

                    switch (transactionResult)
                    {
                        case ETransactionResult.Commit:
                            await _unitOfWork.CommitTransactionAsync();
                            break;
                        case ETransactionResult.Rollback:
                            throw new Exception();
                        default:
                            throw new ArgumentException();
                    }
                }
                catch (ArgumentException ex)
                {
                    Assert.Fail("The enumeration value is wrong.");
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            // Assert
            if (transactionResult == ETransactionResult.Commit)
            {
                _appDbContext.UntrackEntity(user);
                var result = await _appDbContext.Users
                    .Where(u => u.Email == email)
                    .FirstOrDefaultAsync();

                Assert.NotNull(result);
            }
            else
            {
                var result = await _appDbContext.Users
                    .Where(u => u.Email == email)
                    .FirstOrDefaultAsync();

                Assert.Null(result);
            }
        }

        [Fact]
        public void GetRepository_WhenRepositoryIsRequested_ShouldReturnRepository()
        {
            // Arrange
            var properties = typeof(UnitOfWork).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var repositories = properties.Where(p => typeof(IRepositoryBase).IsAssignableFrom(p.PropertyType));

            // Act
            foreach (var repository in repositories)
            {
                var value = repository.GetValue(_unitOfWork);
                if (value == null)
                    Assert.Fail($"Repository is null for {repository.Name}");
            }
        }
    }
}
