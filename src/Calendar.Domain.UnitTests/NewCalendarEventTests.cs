using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Calendar.Domain.UnitTests;

public class NewCalendarEventTests
{
    [Theory]
    [MemberData(nameof(ArgumentOutOfRangeTestData))]
    public void Constructor_InvalidParameters_ThrowsArgumentOutOfRangeException(int userId, string subject, string description, DateTime begin, DateTime end, string partOfMessage) =>
        ThrowsException<ArgumentOutOfRangeException>(userId, subject, description, begin, end, partOfMessage);



    [Theory]
    [MemberData(nameof(ArgumentNullTestData))]
    public void Constructor_InvalidParameters_ThrowsNullReferenceException(int userId, string subject, string description, DateTime begin, DateTime end, string partOfMessage) =>
        ThrowsException<ArgumentNullException>(userId, subject, description, begin, end, partOfMessage);



    private static void ThrowsException<T>(int userId, string subject, string description, DateTime begin,
        DateTime end, string partOfMessage) where T : Exception
    {
        var act = () => new NewCalendarEvent(userId, subject, description, begin, end);


        act.Should().Throw<T>()
            .Where(e => e.Message.Contains(partOfMessage, StringComparison.InvariantCultureIgnoreCase));
    }

    private static IEnumerable<object[]> ArgumentOutOfRangeTestData() =>
        new[]
        {
            GetParameters(nameof(NewCalendarEvent.UserId), userId: int.MinValue),
            GetParameters(nameof(NewCalendarEvent.UserId), userId: -1),
            GetParameters(nameof(NewCalendarEvent.UserId), userId: 0),

            GetParameters(nameof(NewCalendarEvent.Begin), begin: DateTime.Today, end: DateTime.Today),
            GetParameters(nameof(NewCalendarEvent.End)  , begin: DateTime.Today, end: DateTime.Today),

            GetParameters(nameof(NewCalendarEvent.Begin), end: DateTime.Today.AddHours(-1)),
            GetParameters(nameof(NewCalendarEvent.End)  , end: DateTime.Today.AddHours(-1))
        };

    private static IEnumerable<object[]> ArgumentNullTestData() =>
        new[]
        {
            GetParameters(nameof(NewCalendarEvent.Subject), subject: null!),
            GetParameters(nameof(NewCalendarEvent.Description), description: null!),
        };

    public static object[] GetParameters(string partOfMessage, int userId = 1, string subject = "subject",
        string description = "description", DateTime? begin = null, DateTime? end = null) =>
        new object[]
        {
            userId, subject, description, begin.GetValueOrDefault(DateTime.Today), end.GetValueOrDefault(DateTime.Today.AddHours(1)), partOfMessage
        };
}