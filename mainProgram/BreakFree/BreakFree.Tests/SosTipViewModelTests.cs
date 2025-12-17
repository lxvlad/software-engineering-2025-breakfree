namespace BreakFree.Tests
{
    using BreakFree.BLL.Services;

    public class SosTipViewModelTests
    {
        [Fact]
        public void ActionId_ShouldStoreAndReturnValue()
        {
            var tip = new SosTipViewModel();
            tip.ActionId = 123;
            Assert.Equal(123, tip.ActionId);
        }

        [Fact]
        public void Text_ShouldStoreAndReturnValue()
        {
            var tip = new SosTipViewModel();
            tip.Text = "Test text";
            Assert.Equal("Test text", tip.Text);
        }

        [Fact]
        public void ToolTipText_ShouldReturnNoData_WhenUsageCountIsZero()
        {
            var tip = new SosTipViewModel
            {
                UsageCount = 0,
                Efficiency = 0,
            };

            Assert.Equal("Ще не використовувалось", tip.ToolTipText);
        }

        [Fact]
        public void ToolTipText_ShouldReturnFormattedText_WhenUsageCountIsGreaterThanZero()
        {
            var tip = new SosTipViewModel
            {
                UsageCount = 5,
                Efficiency = 60,
            };

            Assert.Equal("Ефективність: 60% (5 спроб)", tip.ToolTipText);
        }

        [Fact]
        public void StatsText_ShouldReturnNew_WhenUsageCountIsZero()
        {
            var tip = new SosTipViewModel
            {
                UsageCount = 0,
                Efficiency = 0,
            };

            Assert.Equal("New", tip.StatsText);
        }

        [Fact]
        public void StatsText_ShouldReturnEfficiencyPercent_WhenUsageCountGreaterThanZero()
        {
            var tip = new SosTipViewModel
            {
                UsageCount = 3,
                Efficiency = 75,
            };

            Assert.Equal("75%", tip.StatsText);
        }

        [Fact]
        public void StatsColor_ShouldReturnGray_WhenEfficiencyLessThan50()
        {
            var tip = new SosTipViewModel
            {
                UsageCount = 5,
                Efficiency = 40,
            };

            Assert.Equal("Gray", tip.StatsColor);
        }

        [Fact]
        public void StatsColor_ShouldReturnGreen_WhenEfficiency50OrGreater()
        {
            var tip = new SosTipViewModel
            {
                UsageCount = 5,
                Efficiency = 50,
            };

            Assert.Equal("Green", tip.StatsColor);

            tip.Efficiency = 90;
            Assert.Equal("Green", tip.StatsColor);
        }
    }
}
