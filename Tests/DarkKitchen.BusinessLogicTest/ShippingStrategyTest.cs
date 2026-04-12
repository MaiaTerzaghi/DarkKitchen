using DarkKitchen.BusinessLogic.Shipping;

namespace DarkKitchen.BusinessLogicTest;

[TestClass]
public class ShippingStrategyTest
{
    [TestMethod]
    public void ExpressShipping_CalculateCost_Returns50()
    {
        var strategy = new ExpressShipping();
        Assert.AreEqual(50.0, strategy.CalculateCost());
    }

    [TestMethod]
    public void StandardShipping_CalculateCost_Returns20()
    {
        var strategy = new StandardShipping();
        Assert.AreEqual(20.0, strategy.CalculateCost());
    }
}
