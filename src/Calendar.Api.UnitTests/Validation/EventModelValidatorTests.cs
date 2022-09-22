using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Api.Models;
using Calendar.Api.Validation;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Calendar.Api.UnitTests.Validation;

public class EventModelValidatorTests
{
    private static readonly DateTime ValidBegin = DateTime.Today;


    [Theory]
    [MemberData(nameof(TestData))]
    public void Validate_DifferentData_DifferentResults(int id, string subject, string description, DateTime begin, DateTime end,
        bool expectedResult, IReadOnlyCollection<string> errorPropertyNames)
    {
        var model = new EventModel(id, subject, description, begin, end);
        var sut = new EventModelValidator();



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

            GetParameters(id: int.MinValue, errorPropertyNames: new[] { nameof(EventModel.Id)}),
            GetParameters(id: -1,           errorPropertyNames: new[] { nameof(EventModel.Id)}),
            GetParameters(id: 0,            errorPropertyNames: new[] { nameof(EventModel.Id)}),
            GetParameters(id: 1,            expectedResult: true),
            GetParameters(id: int.MaxValue, expectedResult: true),

            GetParameters(end: ValidBegin, errorPropertyNames: new[] { nameof(EventModel.End)}),
            GetParameters(begin: default(DateTime), end: default(DateTime), errorPropertyNames: new[] { nameof(EventModel.Begin), nameof(EventModel.End)}),

            GetParameters(subject: CreateString(1),   expectedResult: true),
            GetParameters(subject: CreateString(99),  expectedResult: true),
            GetParameters(subject: CreateString(100), expectedResult: true),
            GetParameters(subject: CreateString(101), errorPropertyNames: new[] { nameof(EventModel.Subject) }),
            GetParameters(subject: CreateString(200), errorPropertyNames: new[] { nameof(EventModel.Subject) }),


            GetParameters(description: CreateString(1),   expectedResult: true),
            GetParameters(description: CreateString(499), expectedResult: true),
            GetParameters(description: CreateString(500), expectedResult: true),
            GetParameters(description: CreateString(501), errorPropertyNames: new[] { nameof(EventModel.Description) }),
            GetParameters(description: CreateString(600), errorPropertyNames: new[] { nameof(EventModel.Description) }),

            GetParameters(subject: "",    description: "",    errorPropertyNames: new[] { nameof(EventModel.Subject), nameof(EventModel.Description)}),
            GetParameters(subject: null!, description: null!, errorPropertyNames: new[] { nameof(EventModel.Subject), nameof(EventModel.Description)}),

            GetParameters(id: 0, subject: null!, description: null!, begin: default(DateTime), end: default(DateTime), errorPropertyNames: new[] { nameof(EventModel.Id), nameof(EventModel.Subject), nameof(EventModel.Description), nameof(EventModel.Begin), nameof(EventModel.End)})
        };


    public static object[] GetParameters(int id = 1 ,string subject = "subject", string description = "description", DateTime? begin = null,
        DateTime? end = null, bool expectedResult = false, IReadOnlyCollection<string>? errorPropertyNames = null) =>
        new object[]
        {
            id, subject, description, begin.GetValueOrDefault(ValidBegin), end.GetValueOrDefault(DateTime.Today.AddHours(1)), expectedResult, errorPropertyNames ?? Array.Empty<string>()
        };

    private static string CreateString(int length) => new(Enumerable.Repeat('x', length).ToArray());
}