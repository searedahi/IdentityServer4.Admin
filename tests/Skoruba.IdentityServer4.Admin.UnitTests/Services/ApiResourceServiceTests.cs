﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services
{
    public class ApiResourceServiceTests
    {
        public ApiResourceServiceTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<AdminDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
            _operationalStore = new OperationalStoreOptions();
        }

        private readonly DbContextOptions<AdminDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;
        private readonly OperationalStoreOptions _operationalStore;

        [Fact]
        public async Task AddApiResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);
                
                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task GetApiResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task RemoveApiResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Remove api resource
                await apiResourceService.DeleteApiResourceAsync(newApiResourceDto);

                //Try get removed api resource
                var removeApiResource = await context.ApiResources.Where(x => x.Id == apiResource.Id)
                    .SingleOrDefaultAsync();

                //Assert removed api resource
                removeApiResource.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateApiResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(apiResource).State = EntityState.Detached;

                //Generete new api resuorce with added item id
                var updatedApiResource = ApiResourceDtoMock.GenerateRandomApiResource(apiResource.Id);

                //Update api resource
                await apiResourceService.UpdateApiResourceAsync(updatedApiResource);
                
                var updatedApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert updated api resuorce
                updatedApiResource.Should().BeEquivalentTo(updatedApiResourceDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task AddApiScopeAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Generate random new api scope
                var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

                //Add new api scope
                await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

                //Get inserted api scope
                var apiScope = await context.ApiScopes.Where(x => x.Name == apiScopeDtoMock.Name && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var apiScopesDto = apiScope.ToModel();

                //Get new api scope
                var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

                //Assert
                newApiScope.Should().BeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));
            }
        }

        [Fact]
        public async Task GetApiScopeAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Generate random new api scope
                var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

                //Add new api scope
                await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

                //Get inserted api scope
                var apiScope = await context.ApiScopes.Where(x => x.Name == apiScopeDtoMock.Name && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var apiScopesDto = apiScope.ToModel();

                //Get new api scope
                var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

                //Assert
                newApiScope.Should().BeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));
            }
        }

        [Fact]
        public async Task UpdateApiScopeAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Generate random new api scope
                var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

                //Add new api scope
                await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

                //Get inserted api scope
                var apiScope = await context.ApiScopes.Where(x => x.Name == apiScopeDtoMock.Name && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();
                
                //Map entity to model
                var apiScopesDto = apiScope.ToModel();

                //Get new api scope
                var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

                //Assert
                newApiScope.Should().BeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));

                //Detached the added item
                context.Entry(apiScope).State = EntityState.Detached;

                //Update api scope
                var updatedApiScope = ApiResourceDtoMock.GenerateRandomApiScope(apiScopesDto.ApiScopeId, apiScopesDto.ApiResourceId);

                await apiResourceService.UpdateApiScopeAsync(updatedApiScope);
                
                var updatedApiScopeDto = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

                //Assert updated api scope
                updatedApiScope.Should().BeEquivalentTo(updatedApiScopeDto, o => o.Excluding(x => x.ResourceName));
            }
        }

        [Fact]
        public async Task DeleteApiScopeAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Generate random new api scope
                var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

                //Add new api scope
                await apiResourceService.AddApiScopeAsync(apiScopeDtoMock);

                //Get inserted api scope
                var apiScope = await context.ApiScopes.Where(x => x.Name == apiScopeDtoMock.Name && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var apiScopesDto = apiScope.ToModel();

                //Get new api scope
                var newApiScope = await apiResourceService.GetApiScopeAsync(apiScopesDto.ApiResourceId, apiScopesDto.ApiScopeId);

                //Assert
                newApiScope.Should().BeEquivalentTo(apiScopesDto, o => o.Excluding(x => x.ResourceName));

                //Delete it
                await apiResourceService.DeleteApiScopeAsync(newApiScope);

                var deletedApiScope = await context.ApiScopes.Where(x => x.Name == apiScopeDtoMock.Name && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();

                //Assert after deleting
                deletedApiScope.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddApiSecretAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Generate random new api secret
                var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

                //Add new api secret
                await apiResourceService.AddApiSecretAsync(apiSecretsDto);

                //Get inserted api secret
                var apiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDto.Value && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var secretsDto = apiSecret.ToModel();

                //Get new api secret    
                var newApiSecret = await apiResourceService.GetApiSecretAsync(secretsDto.ApiSecretId);

                //Assert
                newApiSecret.Should().BeEquivalentTo(secretsDto, o => o.Excluding(x => x.ApiResourceName));
            }
        }

        [Fact]
        public async Task DeleteApiSecretAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IApiResourceRepository apiResourceRepository = new ApiResourceRepository(context);
                IClientRepository clientRepository = new ClientRepository(context);

                var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
                var localizerApiResource = localizerApiResourceMock.Object;

                var localizerClientResourceMock = new Mock<IClientServiceResources>();
                var localizerClientResource = localizerClientResourceMock.Object;

                IClientService clientService = new ClientService(clientRepository, localizerClientResource);
                IApiResourceService apiResourceService = new ApiResourceService(apiResourceRepository, localizerApiResource, clientService);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

                await apiResourceService.AddApiResourceAsync(apiResourceDto);

                //Get new api resource
                var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

                var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

                //Generate random new api secret
                var apiSecretsDtoMock = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

                //Add new api secret
                await apiResourceService.AddApiSecretAsync(apiSecretsDtoMock);

                //Get inserted api secret
                var apiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var apiSecretsDto = apiSecret.ToModel();

                //Get new api secret    
                var newApiSecret = await apiResourceService.GetApiSecretAsync(apiSecretsDto.ApiSecretId);

                //Assert
                newApiSecret.Should().BeEquivalentTo(apiSecretsDto, o => o.Excluding(x => x.ApiResourceName));

                //Delete it
                await apiResourceService.DeleteApiSecretAsync(newApiSecret);

                var deletedApiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id)
                    .SingleOrDefaultAsync();

                //Assert after deleting
                deletedApiSecret.Should().BeNull();
            }
        }
    }
}