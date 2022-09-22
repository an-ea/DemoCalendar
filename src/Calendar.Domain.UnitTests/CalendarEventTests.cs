using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Calendar.Domain.UnitTests;

public class CalendarEventTests
{
    [Theory]
    [MemberData(nameof(ArgumentOutOfRangeTestData))]
    public void Constructor_InvalidParameters_ThrowsArgumentOutOfRangeException(int id, int userId, string subject, string description, DateTime begin, DateTime end, string partOfMessage) =>
        ThrowsException<ArgumentOutOfRangeException>(id, userId, subject, description, begin, end, partOfMessage);



    [Theory]
    [MemberData(nameof(ArgumentNullTestData))]
    public void Constructor_InvalidParameters_ThrowsNullReferenceException(int id, int userId, string subject, string description, DateTime begin, DateTime end, string partOfMessage) =>
        ThrowsException<ArgumentNullException>(id, userId, subject, description, begin, end, partOfMessage);



    private static void ThrowsException<T>(int id, int userId, string subject, string description, DateTime begin,
        DateTime end, string partOfMessage) where T : Exception
    {
        var act = () => new CalendarEvent(id, userId, subject, description, begin, end);


        act.Should().Throw<T>()
            .Where(e => e.Message.Contains(partOfMessage, StringComparison.InvariantCultureIgnoreCase));
    }

    private static IEnumerable<object[]> ArgumentOutOfRangeTestData() =>
        new[]
        {
            GetParameters(nameof(CalendarEvent.Id), id: int.MinValue),
            GetParameters(nameof(CalendarEvent.Id), id: -1),
            GetParameters(nameof(CalendarEvent.Id), id: 0),

            GetParameters(nameof(CalendarEvent.UserId), userId: int.MinValue),
            GetParameters(nameof(CalendarEvent.UserId), userId: -1),
            GetParameters(nameof(CalendarEvent.UserId), userId: 0),

            GetParameters(nameof(CalendarEvent.Begin), begin: DateTime.Today, end: DateTime.Today.AddHours(-1)),
            GetParameters(nameof(CalendarEvent.End)  , begin: DateTime.Today, end: DateTime.Today.AddHours(-1)),
        };

    private static IEnumerable<object[]> ArgumentNullTestData() =>
        new[]
        {
            GetParameters(nameof(CalendarEvent.Subject), subject: null!),
            GetParameters(nameof(CalendarEvent.Description), description: null!),
        };

    public static object[] GetParameters(string partOfMessage, int id = 1, int userId = 1, string subject = "subject", 
        string description = "description", DateTime? begin = null, DateTime? end = null)
    {
        var today = DateTime.Today;

        return new object[]
        {
            id, userId, subject, description, begin.GetValueOrDefault(today), end.GetValueOrDefault(today.AddHours(1)), partOfMessage
        };
    }
}