using Moq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

using Contracts;
using Shared.DTOs;
using Entities.Models;
using Shared.RequestFeatures;
using abs_insurance.Controllers;

namespace TestAbs
{
    public class PolicyControllerTests
    {
        private readonly Mock<IServiceManager> _mockService;
        private readonly Mock<IValidator<CreatePolicyDto>> _mockValidator;
        private readonly PolicyController _controller;

        public PolicyControllerTests()
        {
            _mockService = new Mock<IServiceManager>();
            _mockValidator = new Mock<IValidator<CreatePolicyDto>>();
            _controller = new PolicyController(_mockService.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task GetPolicies_ReturnsOkResult_WithPolicies()
        {
            // Arrange
            var parameters = new PolicyParameters();
            var expectedPolicies = new List<ReadPolicyDto> 
            { 
                new ReadPolicyDto(
                    1, 
                    "Test Policy",
                     new List<ReadPolicyComponentDto>
                    {
                        new(1,ComponentName.MarketValuePremium.ToString(),Operation.Add.ToString(), 0.0m, 10.0m)
                    }
                )
            };

            
            _mockService.Setup(s => s.PolicyService.GetAllPolicies(parameters, false))
                .ReturnsAsync(expectedPolicies);

            // Act
            var result = await _controller.GetPolicies(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPolicies = Assert.IsType<List<ReadPolicyDto>>(okResult.Value);
            Assert.Single(returnedPolicies);
            Assert.Equal(expectedPolicies, returnedPolicies);
        }

        [Fact]
        public async Task GetPolicy_ReturnsOkResult_WithPolicy()
        {
            // Arrange
            int policyId = 1;
            var expectedPolicy = new ReadPolicyDto(
                1,
                "Test Policy",
                new List<ReadPolicyComponentDto>
                {
                    new(1, ComponentName.MarketValuePremium.ToString(), Operation.Add.ToString(), 0.0m, 10.0m)
                }
            );
            
            _mockService.Setup(s => s.PolicyService.GetPolicy(policyId, false))
                .ReturnsAsync(expectedPolicy);
        
            // Act
            var result = await _controller.GetPolicy(policyId);
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPolicy = Assert.IsType<ReadPolicyDto>(okResult.Value);
            Assert.Equal(expectedPolicy, returnedPolicy);
        }
        
        [Fact]
        public async Task CreatePolicy_WithValidPolicy_ReturnsCreatedAtRoute()
        {
            // Arrange
            var createPolicy = new CreatePolicyDto(

                "New Policy",
                [
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 20m, 0.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 0.0m, 30.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m)

                ]
            );
            var createdPolicy = new ReadPolicyDto(
                1, 
                "Test Policy",
                new List<ReadPolicyComponentDto>
                {
                    new(1,ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                    new(2, ComponentName.ExtraPerils.ToString(), Operation.Add.ToString(), 20.0m, 0.0m),
                    new(3, ComponentName.MarketValuePremium.ToString(), Operation.Add.ToString(), 0.0m, 30.0m),
                    new(4, ComponentName.PromoDiscount.ToString(), Operation.Add.ToString(), 50.0m, 0.0m),
                });
            
            _mockValidator.Setup(v => v.ValidateAsync(createPolicy, default))
                .ReturnsAsync(new ValidationResult());
            
            _mockService.Setup(s => s.PolicyService.CreatePolicy(createPolicy))
                .ReturnsAsync(createdPolicy);
        
            // Act
            var result = await _controller.CreatePolicy(createPolicy);
        
            // Assert
            var createdAtRoute = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal("GetPolicy", createdAtRoute.RouteName);
            Assert.Equal(1, createdAtRoute.RouteValues["id"]);
        }
        
        [Fact]
        public async Task CreatePolicy_WithInvalidPolicy_ReturnsBadRequest()
        {
            // Arrange
            var createPolicy = new CreatePolicyDto(

                "",
                [
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 20m, 0.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 0.0m, 30.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m)

                ]
            );
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };
            
            _mockValidator.Setup(v => v.ValidateAsync(createPolicy, default))
                .ReturnsAsync(new ValidationResult(validationFailures));
        
            // Act
            var result = await _controller.CreatePolicy(createPolicy);
        
            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
       
        
        [Fact]
        public async Task UpdatePolicy_WithValidPolicy_ReturnsCreatedAtRoute()
        {
            // Arrange
            int policyId = 1;
            var updatePolicy = new CreatePolicyDto(

                "Something",
                [
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 20m, 0.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 0.0m, 30.0m),
                    new(ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m)

                ]
            );
            var updatedPolicy = new ReadPolicyDto(
                1,
                "Something Else",
                [
                    new(1,ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m),
                    new(2,ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 20m, 0.0m),
                    new(3,ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 0.0m, 30.0m),
                    new(4,ComponentName.PremiumBase.ToString(), Operation.Add.ToString(), 50m, 0.0m)

                ]
            );
            
            _mockValidator.Setup(v => v.ValidateAsync(updatePolicy, default))
                .ReturnsAsync(new ValidationResult());
            
            _mockService.Setup(s => s.PolicyService.UpdatePolicy(policyId, updatePolicy, true))
                .ReturnsAsync(updatedPolicy);
        
            // Act
            var result = await _controller.UpdatePolicy(policyId, updatePolicy);
        
            // Assert
            var createdAtRoute = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal("GetPolicy", createdAtRoute.RouteName);
            Assert.Equal(policyId, createdAtRoute.RouteValues["id"]);
        }
        
        
        [Fact]
        public async Task DeletePolicy_ReturnsNoContent()
        {
            // Arrange
            int policyId = 1;
            
            _mockService.Setup(s => s.PolicyService.DeletePolicy(policyId, false))
                .Returns(Task.CompletedTask);
        
            // Act
            var result = await _controller.DeletePolicy(policyId);
        
            // Assert
            Assert.IsType<NoContentResult>(result);
        }
      
        
        [Fact]
        public async Task RequestQuote_ReturnsOkResult_WithBenefits()
        {
            // Arrange
            var quoteDto = new RequestQuoteDto(1,500m);
            var expectedBenefits = new ResponseQuote(1, "New Policy", 1000);
            
            _mockService.Setup(s => s.PolicyService.CalculateBenefits(quoteDto))
                .ReturnsAsync(expectedBenefits);
        
            // Act
            var result = await _controller.RequestQuote(quoteDto);
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBenefits = Assert.IsType<ResponseQuote>(okResult.Value);
            Assert.Equal(expectedBenefits, returnedBenefits);
        }
    }
}