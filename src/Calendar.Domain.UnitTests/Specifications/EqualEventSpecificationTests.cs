using System;
using System.Collections.Generic;
using Calendar.Domain.Specifications;
using FluentAssertions;
using Xunit;

namespace Calendar.Domain.UnitTests.Specifications;

public class EqualEventSpecificationTests
{
    private const int ValidUserId = 1;
    private const string ValidSubject = nameof(ValidSubject);
    private const string ValidDescription = nameof(ValidDescription);
    private static readonly DateTime ValidBegin = DateTime.Today;
    private static readonly DateTime ValidEnd = ValidBegin.AddHours(1);

    [Theory]
    [MemberData(nameof(TestData))]
    public void IsSatisfiedBy_DifferentData_DifferentResults(int userId, string subject, string description, DateTime begin, DateTime end, bool expectedResult)
    {
        var eventEntity = new EventEntity
        {
            UserId = userId,
            Subject = subject,
            Description = description,
            Begin = begin,
            End = end
        };
        var sut = new EqualEventSpecification(eventEntity);

        var testEvent = new EventEntity
        {
            UserId = ValidUserId,
            Subject = ValidSubject,
            Description = ValidDescription,
            Begin = ValidBegin,
            End = ValidEnd
        };




        var result = sut.IsSatisfiedBy.Compile().Invoke(testEvent);




        result.Should().Be(expectedResult);
    }

    private static IEnumerable<object[]> TestData() =>
        new[]
        {
            GetParameters(expectedResult: true),

            GetParameters(userId: int.MinValue),
            GetParameters(userId: -1),
            GetParameters(userId: 0),
            GetParameters(userId: int.MaxValue),

            GetParameters(subject: null!),
            GetParameters(subject: string.Empty),
            GetParameters(subject: "12345"),

            GetParameters(description: null!),
            GetParameters(description: string.Empty),
            GetParameters(description: "12345"),

            GetParameters(begin: default(DateTime)),
            GetParameters(begin: DateTime.MinValue),
            GetParameters(begin: DateTime.MaxValue),

            GetParameters(end: default(DateTime)),
            GetParameters(end: DateTime.MinValue),
            GetParameters(end: DateTime.MaxValue)
        };

    public static object[] GetParameters(int userId = ValidUserId, string subject = ValidSubject, string description = ValidDescription, DateTime? begin = null, DateTime? end = null, bool expectedResult = false) =>
        new object[]
        {
            userId, subject, description, begin.GetValueOrDefault(ValidBegin), end.GetValueOrDefault(ValidEnd), expectedResult
        };
}