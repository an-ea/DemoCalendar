using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Calendar.Api.Validation;
using FluentAssertions;
using Xunit;

namespace Calendar.Api.UnitTests.Validation;

public class ValidIdAttributeTests
{
    [Theory]
    [InlineData(int.MinValue, false)]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(int.MaxValue, true)]
    public void IsValid_DifferentData_DifferentResults(int id, bool expectedResult)
    {
        var sut = new TestClass(id);
        var context = new ValidationContext(sut);
        var validationResults = new List<ValidationResult>();



        var result = Validator.TryValidateObject(sut, context, validationResults, true);



        result.Should().Be(expectedResult);
        if (result) return;
        validationResults.Should().HaveCount(1);
        validationResults[0].ErrorMessage.Should().Contain(nameof(TestClass.Id));
    }

    [Theory]
    [InlineData("1")]
    [InlineData(1.0)]
    public void IsValid_InvalidDataType_ThrowsException(object value)
    {
        var sut = new ObjectPropertyTestClass(value);
        var context = new ValidationContext(sut);

        var act = () => Validator.TryValidateObject(sut, context, new List<ValidationResult>(), true);


        act.Should().Throw<ArgumentException>()
            .Where(e => e.Message.Contains(nameof(Int32), StringComparison.InvariantCultureIgnoreCase));
    }

    private class TestClass
    {
        public TestClass(int id)
        {
            Id = id;
        }

        [ValidId]
        public int Id { get; }
    }


    private class ObjectPropertyTestClass
    {
        public ObjectPropertyTestClass(object value)
        {
            Value = value;
        }

        [ValidId]
        public object Value { get; }
    }
}