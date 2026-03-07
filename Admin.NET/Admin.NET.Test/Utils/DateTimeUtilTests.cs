// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Core;
using Xunit;

namespace Admin.NET.Test.Utils;

public class DateTimeUtilTests
{
    [Fact]
    public void Init_WithTimeSpan_ReturnsCorrectDateTime()
    {
        // Arrange
        var timeSpan = new TimeSpan(1, 0, 0, 0); // 1 day

        // Act
        var dateTimeUtil = DateTimeUtil.Init(timeSpan);

        // Assert
        Assert.Equal(DateTime.Now.AddDays(1).Date, dateTimeUtil.Date.Date);
    }

    [Fact]
    public void Init_WithDateTime_ReturnsCorrectDateTime()
    {
        // Arrange
        var dateTime = new DateTime(2023, 10, 1);

        // Act
        var dateTimeUtil = DateTimeUtil.Init(dateTime);

        // Assert
        Assert.Equal(dateTime, dateTimeUtil.Date);
    }

    [Fact]
    public void GetTodayRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15, 12, 30, 0));

        // Act
        var (start, end) = dateTimeUtil.GetTodayRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 15), start); // Start time of the day
        Assert.Equal(new DateTime(2023, 10, 15, 23, 59, 59), end); // end of day
    }

    [Fact]
    public void GetMonthRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetMonthRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 1), start); // first day of month
        Assert.Equal(new DateTime(2023, 10, 31, 23, 59, 59), end); // last day of month
    }

    [Fact]
    public void GetFirstDayOfMonth_ReturnsCorrectDate()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var firstDay = dateTimeUtil.GetFirstDayOfMonth();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 1), firstDay); // first day of month
    }

    [Fact]
    public void GetLastDayOfMonth_ReturnsCorrectDate()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var lastDay = dateTimeUtil.GetLastDayOfMonth();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 31, 23, 59, 59), lastDay); // last day of month
    }

    [Fact]
    public void GetYearRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetYearRange();

        // Assert
        Assert.Equal(new DateTime(2023, 1, 1), start); // first day of the year
        Assert.Equal(new DateTime(2023, 12, 31, 23, 59, 59), end); // last day of this year
    }

    [Fact]
    public void GetFirstDayOfYear_ReturnsCorrectDate()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var firstDay = dateTimeUtil.GetFirstDayOfYear();

        // Assert
        Assert.Equal(new DateTime(2023, 1, 1), firstDay); // first day of the year
    }

    [Fact]
    public void GetLastDayOfYear_ReturnsCorrectDate()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var lastDay = dateTimeUtil.GetLastDayOfYear();

        // Assert
        Assert.Equal(new DateTime(2023, 12, 31, 23, 59, 59), lastDay); // last day of this year
    }

    [Fact]
    public void GetDayBeforeYesterdayRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetDayBeforeYesterdayRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 13), start); // Start time the day before yesterday
        Assert.Equal(new DateTime(2023, 10, 13, 23, 59, 59), end); // end time the day before yesterday
    }

    [Fact]
    public void GetYesterdayRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetYesterdayRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 14), start); // Yesterday's start time
        Assert.Equal(new DateTime(2023, 10, 14, 23, 59, 59), end); // Yesterday's end time
    }

    [Fact]
    public void GetLastWeekRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15)); // 2023-10-15 is Sunday

        // Act
        var (start, end) = dateTimeUtil.GetLastWeekRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 8), start); // The first day of last week (Monday)
        Assert.Equal(new DateTime(2023, 10, 14, 23, 59, 59), end); // Last day of last week (Sunday)
    }

    [Fact]
    public void GetThisWeekRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15)); // 2023-10-15 is Sunday

        // Act
        var (start, end) = dateTimeUtil.GetThisWeekRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 15), start); // First day of the week (Monday)
        Assert.Equal(new DateTime(2023, 10, 21, 23, 59, 59), end); // Last day of the week (Sunday)
    }

    [Fact]
    public void GetLastMonthRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetLastMonthRange();

        // Assert
        Assert.Equal(new DateTime(2023, 9, 1), start); // first day of last month
        Assert.Equal(new DateTime(2023, 9, 30, 23, 59, 59), end); // last day of last month
    }

    [Fact]
    public void GetLast3DaysRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetLast3DaysRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 13), start); // Start time 3 days ago
        Assert.Equal(new DateTime(2023, 10, 15, 23, 59, 59), end); // end time of current date
    }

    [Fact]
    public void GetLast7DaysRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetLast7DaysRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 9), start); // Start time 7 days ago
        Assert.Equal(new DateTime(2023, 10, 15, 23, 59, 59), end); // end time of current date
    }

    [Fact]
    public void GetLast15DaysRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetLast15DaysRange();

        // Assert
        Assert.Equal(new DateTime(2023, 10, 1), start); // Start time 15 days ago
        Assert.Equal(new DateTime(2023, 10, 15, 23, 59, 59), end); // end time of current date
    }

    [Fact]
    public void GetLast3MonthsRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetLast3MonthsRange();

        // Assert
        Assert.Equal(new DateTime(2023, 7, 15), start); // Start time 3 months ago
        Assert.Equal(new DateTime(2023, 10, 15, 23, 59, 59), end); // end time of current date
    }

    [Fact]
    public void GetFirstHalfYearRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetFirstHalfYearRange();

        // Assert
        Assert.Equal(new DateTime(2023, 1, 1), start); // Start time of first half
        Assert.Equal(new DateTime(2023, 6, 30, 23, 59, 59), end); // End of the first half of the year
    }

    [Fact]
    public void GetSecondHalfYearRange_ReturnsCorrectRange()
    {
        // Arrange
        var dateTimeUtil = DateTimeUtil.Init(new DateTime(2023, 10, 15));

        // Act
        var (start, end) = dateTimeUtil.GetSecondHalfYearRange();

        // Assert
        Assert.Equal(new DateTime(2023, 7, 1), start); // Start time of the second half of the year
        Assert.Equal(new DateTime(2023, 12, 31, 23, 59, 59), end); // The end of the second half of the year
    }
}