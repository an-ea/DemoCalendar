using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Api.Models;
using Calendar.Api.Validation;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Calendar.Api.UnitTests.Validation;

public class NewEventModelValidatorTests
{
    private static readonly DateTime ValidBegin = DateTime.Today;


    [Theory]
    [MemberData(nameof(TestData))]
    public void Validate_DifferentData_DifferentResults(string subject, string description, DateTime begin, DateTime end, 
        bool expectedResult, IReadOnlyCollection<string> errorPropertyNames)
    {
        var model = new NewEventModel(subject, description, begin, end);
        var sut = new NewEventModelValidator();



        var result = sut.TestValidate(model);



        result.IsValid.Should().Be(expectedResult);
        foreach (var propertyName in errorPropertyNames)
            result.ShouldHaveValidationErrorFor(propertyName);
        result.Errors.Count.Should().Be(errorPropertyNames.Count);
    }


    private static IEnumerable<object[]> TestData() =>
        new[]
        {
            GetParameters(expectedResult: true),
            
            GetParameters(end: ValidBegin, errorPropertyNames: new[] { nameof(NewEventModel.End)}),
            GetParameters(begin: default(DateTime), end: default(DateTime), errorPropertyNames: new[] { nameof(NewEventModel.Begin), nameof(NewEventModel.End)}),

            GetParameters(subject: CreateString(1),   expectedResult: true),
            GetParameters(subject: CreateString(99),  expectedResult: true),
            GetParameters(subject: CreateString(100), expectedResult: true),
            GetParameters(subject: CreateString(101), errorPropertyNames: new[] { nameof(NewEventModel.Subject) }),
            GetParameters(subject: CreateString(200), errorPropertyNames: new[] { nameof(NewEventModel.Subject) }),


            GetParameters(description: CreateString(1),   expectedResult: true),
            GetParameters(description: CreateString(499), expectedResult: true),
            GetParameters(description: CreateString(500), expectedResult: true),
            GetParameters(description: CreateString(501), errorPropertyNames: new[] { nameof(NewEventModel.Description) }),
            GetParameters(description: CreateString(600), errorPropertyNames: new[] { nameof(NewEventModel.Description) }),

            GetParameters(subject: "",    description: "",    errorPropertyNames: new[] { nameof(NewEventModel.Subject), nameof(NewEventModel.Description)}),
            GetParameters(subject: null!, description: null!, errorPropertyNames: new[] { nameof(NewEventModel.Subject), nameof(NewEventModel.Description)}),

            GetParameters(subject: null!, description: null!, begin: default(DateTime), end: default(DateTime), errorPropertyNames: new[] { nameof(NewEventModel.Subject), nameof(NewEventModel.Description), nameof(NewEventModel.Begin), nameof(NewEventModel.End)})
        };


    public static object[] GetParameters(string subject = "subject", string description = "description", DateTime? begin = null,
        DateTime? end = null, bool expectedResult = false, IReadOnlyCollection<string>? errorPropertyNames = null) =>
        new object[]
        {
            subject, description, begin.GetValueOrDefault(ValidBegin), end.GetValueOrDefault(DateTime.Today.AddHours(1)), expectedResult, errorPropertyNames ?? Array.Empty<string>()
        };

    private static string CreateString(int length) => new(Enumerable.Repeat('x', length).ToArray());

}