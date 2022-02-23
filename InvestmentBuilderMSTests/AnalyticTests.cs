namespace InvestmentBuilderMSTests
{
    using InvestmentBuilderCore;
    #region References

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    [TestClass]
    public class AnalyticTests
    {
        [TestMethod]
        public void GetProjectedNav()
        {
            var result = AnalyticsCalculator.CalculateProjection(1000, 10, 5, 0.05, 0.02);
            Assert.IsTrue(1809.700076.AreSame(result));
        }

        [TestMethod]
        public void GetAverageYield()
        {
            var result = AnalyticsCalculator.CalculateAverageYield(1, 3.30343773736454, 20);
            Assert.IsTrue(6.157.AreSame(result, 4));
        }


        [TestMethod]
        public void GetAverageNoYield()
        {
            var result = AnalyticsCalculator.CalculateAverageYield(1, 1, 20);
            Assert.IsTrue(result.IsZero());
        }


        [TestMethod]
        public void GetAverageNegativeYield()
        {
            var result = AnalyticsCalculator.CalculateAverageYield(1, 0.960750957, 20);
            Assert.IsTrue((-0.2).AreSame(result, 2));
        }


    }
}
