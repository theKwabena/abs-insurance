using Moq;
using AutoMapper;
using System.Text.Json;
using Castle.Core.Logging;
using Xunit.Abstractions;

using Contracts;
using Services;
using Shared.DTOs;
using Entities.Models;
using Entities.Exceptions;
using Shared.RequestFeatures;

namespace TestAbs;

public class PolicyServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly PolicyService _policyService;
    private readonly ITestOutputHelper _output;
    private readonly ILogger _logger;

    public PolicyServiceTests(ITestOutputHelper output)
    {
        _mockRepo = new Mock<IRepositoryManager>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Policy, ReadPolicyDto>().ForMember(dest => dest.Components,
                opts => 
                    opts.MapFrom(src => src.Components));

            cfg.CreateMap<CreatePolicyDto, Policy>().ForMember(dest => dest.Components,
                opts =>
                    opts.MapFrom(src => src.Components));

            cfg.CreateMap<CreatePolicyComponentDto, PolicyComponent>().ForMember(dest =>
                dest.Name, opts =>
                opts.MapFrom(src => Enum.Parse<ComponentName>(src.Name.Replace(" ", ""), true)));
        
            cfg.CreateMap<PolicyComponent, ReadPolicyComponentDto>().ForMember(dest => dest.Name,opts =>
                opts.MapFrom(src => string.Join(" ", System.Text.RegularExpressions.Regex.Matches(
                    src.Name.ToString(),"[A-Z][a-z]*").Select(m => m.Value))));
        });
        var mapper = mapperConfig.CreateMapper();
        _policyService = new PolicyService(_mockRepo.Object, mapper);
        _output = output;
    }

    [Fact]
    public async Task GetAllPolicies_ReturnsAllPolicies()
    {
        // Arrange
        var parameters = new PolicyParameters();
        var policies = new List<Policy> 
        { 
            new Policy 
            { 
                Id = 1, 
                Name = "Test Policy",
                Components = new List<PolicyComponent>
                {
                    new() { Name = ComponentName.MarketValuePremium, Operation = Operation.Add, FlatValue = 0.0m, PercentageValue = 10.0m, Sequence = 1 }
                }
            }
        };

        var expectedDtos = new List<ReadPolicyDto> 
        { 
            new ReadPolicyDto(
                1, 
                "Test Policy",
                new List<ReadPolicyComponentDto> 
                { 
                    new(1, ComponentName.MarketValuePremium.ToString(), Operation.Add.ToString(), 0.0m, 10.0m)
                })
        };
        _mockRepo.Setup(r => r.Policy.GetAllPolicies(parameters, false))
            .ReturnsAsync(policies);


        // Act
        var result = await _policyService.GetAllPolicies(parameters, false);

        // Assert
        Assert.Equivalent(expectedDtos, result);
    }

    [Fact]
    public async Task GetPolicy_WithValidId_ReturnsPolicy()
    {
        // Arrange
        var policy = new Policy 
        { 
            Id = 1, 
            Name = "Test Policy",
            Components = new List<PolicyComponent>
            {
                new() {Sequence = 1,Name = ComponentName.PremiumBase, Operation = Operation.Add, FlatValue = 50m, PercentageValue = 0.0m},
                new() {Sequence = 2, Name = ComponentName.ExtraPerils, Operation = Operation.Add, FlatValue = 20.0m, PercentageValue = 0.0m,},
                new() {Sequence = 3,Name = ComponentName.MarketValuePremium, Operation = Operation.Add, FlatValue = 0.0m, PercentageValue = 30.0m },
                new() {Sequence =4, Name = ComponentName.PromoDiscount, Operation = Operation.Add, FlatValue = 30m, PercentageValue = 0.0m},
            }
        };

        var expectedDto = new ReadPolicyDto(
            1, 
            "Test Policy",
            new List<ReadPolicyComponentDto>
            {
                new(1,ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                new(2, ComponentName.ExtraPerils.ToString(), Operation.Add.ToString(), 20.0m, 0.0m),
                new(3, ComponentName.MarketValuePremium.ToString(), Operation.Add.ToString(), 0.0m, 30.0m),
                new(4, ComponentName.PromoDiscount.ToString(), Operation.Add.ToString(), 30.0m, 0.0m),
            });


        _mockRepo.Setup(r => r.Policy.GetPolicy(1, false))
            .ReturnsAsync(policy);
        

        // Act
        var result = await _policyService.GetPolicy(1, false);

        // Assert
        Assert.Equivalent(expectedDto, result);
        
    }

    [Fact]
    public async Task GetPolicy_WithInvalidId_ThrowsPolicyNotFoundException()
    {
        // Arrange
        _mockRepo.Setup(r => r.Policy.GetPolicy(1, false)).ReturnsAsync((Policy)null);

        // Act & Assert
        await Assert.ThrowsAsync<PolicyNotFoundException>(() => _policyService.GetPolicy(1, false));
        _mockRepo.Verify(r => r.Policy.GetPolicy(1, false), Times.Once);
    }
    

    [Fact]
    public async Task CreatePolicy_WithExistingName_ThrowsPolicyAlreadyExistsException()
    {
        // Arrange
        var createPolicy = new CreatePolicyDto(

            "Existing Policy",
            [
                new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m)

            ]);
        var existingPolicy = new Policy 
        { 
            Id = 1, 
            Name = "Existing Policy",
            Components = new List<PolicyComponent>
            {
                new() { Name = ComponentName.PremiumBase, Operation = Operation.Add, FlatValue = 50m, PercentageValue = 0.0m},
                new() { Name = ComponentName.ExtraPerils, Operation = Operation.Add, FlatValue = 20m, PercentageValue = 0.0m,},
                new() { Name = ComponentName.MarketValuePremium, Operation = Operation.Add, FlatValue = 0.0m, PercentageValue = 30.0m },
                new() { Name = ComponentName.PromoDiscount, Operation = Operation.Add, FlatValue = 30m, PercentageValue = 0.0m},
            }
        };

        _mockRepo.Setup(r => r.Policy.GetPolicyByName("Existing Policy", false))
            .ReturnsAsync(existingPolicy);

        // Act & Assert
        await Assert.ThrowsAsync<PolicyAlreadyExistsException>(() =>
            _policyService.CreatePolicy(createPolicy));
    }
    

    [Fact]
    public async Task DeletePolicy_WithValidId_DeletesPolicy()
    {
        // Arrange
        var policy = new Policy { Id = 1, Name = "Test Policy" };
        _mockRepo.Setup(r => r.Policy.GetPolicy(1, false))
            .ReturnsAsync(policy);

        // Act
        await _policyService.DeletePolicy(1, false);

        // Assert
        _mockRepo.Verify(r => r.Policy.DeletePolicy(policy), Times.Once);
        _mockRepo.Verify(r => r.Save(), Times.Once);
    }
    
    [Fact]
    public async Task DeletePolicy_PolicyDoesNotExist_ThrowsPolicyNotFoundException()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.Policy.GetPolicy(1, false))
            .ReturnsAsync((Policy)null);

        // Act & Assert
        await Assert.ThrowsAsync<PolicyNotFoundException>(() => _policyService.DeletePolicy(1, false));
        _mockRepo.Verify(r => r.Policy.GetPolicy(1, false), Times.Once);
    }
    
    [Fact]
    public async Task CalculateBenefits_WithValidPolicy_ReturnsCorrectPremium()
    {
        // Arrange
        var requestQuote = new RequestQuoteDto(1, 1000m);
        var policy = new Policy 
        { 
            Id = 1, 
            Name = "Test Policy",
            Components = new List<PolicyComponent>
            {
                new() { Name = ComponentName.PremiumBase, Operation = Operation.Add, FlatValue = 50m, PercentageValue = 0.0m},
                new() { Name = ComponentName.ExtraPerils, Operation = Operation.Add, FlatValue = 20m, PercentageValue = 0.0m,},
                new() { Name = ComponentName.MarketValuePremium, Operation = Operation.Add, FlatValue = 0.0m, PercentageValue = 30.0m },
                new() { Name = ComponentName.PromoDiscount, Operation = Operation.Subtract, FlatValue = 30m, PercentageValue = 0.0m},
            }
        };

        _mockRepo.Setup(r => r.Policy.GetPolicy(1, false))
            .ReturnsAsync(policy);

        // Act
        var result = await _policyService.CalculateBenefits(requestQuote);

        // Assert
        // Base Premium: 50
        // Extra Perils: +20
        // Market Value Premium: (1000 * 0.30) = +300
        // Promo Discount: +30
        // Total Expected: 50 + 20 + 300 - 30 = 340
        Assert.Equal(340m, result.Premium);
        Assert.Equal(policy.Id, result.PolicyId);
        Assert.Equal(policy.Name, result.PolicyName);
    }
}