using System;
using System.Collections.Generic;
using Calendar.Api.Models;
using Calendar.Api.Validation;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Calendar.Api.UnitTests.Validation;

public class DateTimeRangeModelValidatorTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void Validate_DifferentPeriods_DifferentResults(DateTime begin, DateTime end, bool expectedResult)
    {
        var range = new DateTimeRangeModel(begin, end);
        var sut = new DateTimeRangeModelValidator();



        var result = sut.TestValidate(range);



        result.IsValid.Should().Be(expectedResult);
        result.ShouldNotHaveValidationErrorFor(r => r.Begin);

        if (!expectedResult)
            result.ShouldHaveValidationErrorFor(r => r.End);
    }


    private static IEnumerable<object[]> TestData() =>
        new []
        {
            new object[] { DateTime.Today, DateTime.Today.AddMinutes(30), true },
            new object[] { DateTime.Today, DateTime.Today.AddMinutes(-30), false },
            new object[] { DateTime.Today, DateTime.Today, false },
            new object[] { default(DateTime) , default(DateTime), false }
        };
}