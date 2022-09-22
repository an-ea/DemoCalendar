using System;
using FluentAssertions;
using Xunit;

namespace Calendar.Domain.UnitTests;

public class DateTimeRangeTests
{
    [Fact]
    public void Constructor_BeginGreaterThanEnd_ThrowsArgumentOutOfRangeException()
    {
        var act = () => new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(-1));


        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}