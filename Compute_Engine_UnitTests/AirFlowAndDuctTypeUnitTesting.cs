using Compute_Engine.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Compute_Engine_UnitTests
{
    [TestClass]
    public class AirFlowAndDuctTypeUnitTesting
    {
        [TestMethod]
        public void Junction_TestMethod_1()
        {
            //Arrange
            Junction jnt_2 = new Junction() { Name = "jnt_2" };

            //Act
            jnt_2.AirFlow = 2000;

            //Assert
            Assert.IsTrue(jnt_2.Name == "jnt_2");
            Assert.AreEqual(2000, jnt_2.Inlet.AirFlow);
            Assert.AreEqual(2000, jnt_2.AirFlow);
        }

        [TestMethod]
        public void Junction_TestMethod_2()
        {
            //Arrange
            Junction jnt_1 = new Junction();

            //Act
            jnt_1.Branch.AirFlow = 2000;

            //Assert
            Assert.AreEqual(2400, jnt_1.AirFlow);
            Assert.AreEqual(400, jnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void Junction_TestMethod_3()
        {
            //Arrange
            Junction jnt_1 = new Junction();

            //Act
            jnt_1.Outlet.AirFlow = 600;
            jnt_1.Branch.AirFlow = 600;
            jnt_1.Inlet.AirFlow = 3000;

            //Assert
            Assert.AreEqual(3000, jnt_1.AirFlow);
            Assert.AreEqual(3000, jnt_1.Inlet.AirFlow);
            Assert.AreEqual(2400, jnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void Junction_TestMethod_4()
        {
            //Arrange
            Junction jnt_1 = new Junction();

            //Act
            jnt_1.Outlet.AirFlow = 600;
            jnt_1.Branch.AirFlow = 600;

            //Assert
            Assert.AreEqual(1000, jnt_1.AirFlow);
            Assert.AreEqual(1000, jnt_1.Inlet.AirFlow);
            Assert.AreEqual(400, jnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void Junction_TestMethod_5()
        {
            //Arrange
            Junction jnt_1 = new Junction();

            //Act
            jnt_1.Outlet.DuctType = Compute_Engine.Enums.DuctType.Round;

            //Assert
            Assert.IsTrue(Compute_Engine.Enums.DuctType.Round == jnt_1.Inlet.DuctType);
        }

        [TestMethod]
        public void Junction_TestMethod_6()
        {
            //Arrange
            Junction jnt_1 = new Junction();

            //Act
            jnt_1.Inlet.AirFlow = 600;

            //Assert
            Assert.AreEqual(600, jnt_1.AirFlow);
            Assert.AreEqual(400, jnt_1.Branch.AirFlow);
            Assert.AreEqual(200, jnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void Junction_TestMethod_7()
        {
            //Arrange
            Junction jnt_1 = new Junction();

            //Act
            jnt_1.Branch.AirFlow = 800;
            jnt_1.Inlet.AirFlow = 600;

            //Assert
            Assert.AreEqual(600, jnt_1.AirFlow);
            Assert.AreEqual(600, jnt_1.Branch.AirFlow);
            Assert.AreEqual(0, jnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_1()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.Outlet.DuctType = Compute_Engine.Enums.DuctType.Round;

            //Assert
            Assert.IsTrue(Compute_Engine.Enums.DuctType.Round == djnt_1.Inlet.DuctType);
        }

        [TestMethod]
        public void DJunction_TestMethod_2 ()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction() { Name = "djnt_3" };

            //Act
            djnt_1.Outlet.DuctType = Compute_Engine.Enums.DuctType.Round;

            //Assert
            Assert.IsTrue(djnt_1.Name == "djnt_3");
            Assert.AreEqual(2600, djnt_1.Inlet.AirFlow);
            Assert.AreEqual(2600, djnt_1.AirFlow);
            Assert.AreEqual(600, djnt_1.BranchRight.AirFlow);
            Assert.AreEqual(600, djnt_1.BranchLeft.AirFlow);
            Assert.AreEqual(1400, djnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_3()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.Outlet.AirFlow = 2000;

            //Assert
            Assert.AreEqual(600, djnt_1.BranchRight.AirFlow);
            Assert.AreEqual(600, djnt_1.BranchLeft.AirFlow);
            Assert.AreEqual(3200, djnt_1.Inlet.AirFlow);
            Assert.AreEqual(3200, djnt_1.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_4()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.AirFlow = 4000;
            djnt_1.BranchRight.AirFlow = 2000;

            //Assert
            Assert.AreEqual(2000, djnt_1.BranchRight.AirFlow);
            Assert.AreEqual(600, djnt_1.BranchLeft.AirFlow);
            Assert.AreEqual(4000, djnt_1.Inlet.AirFlow);
            Assert.AreEqual(1400, djnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_5()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.AirFlow = 1000;

            //Assert
            Assert.AreEqual(500, djnt_1.BranchRight.AirFlow);
            Assert.AreEqual(500, djnt_1.BranchLeft.AirFlow);
            Assert.AreEqual(1000, djnt_1.Inlet.AirFlow);
            Assert.AreEqual(0, djnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_6()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.AirFlow = 3000;
            djnt_1.BranchRight.AirFlow = 3000;

            //Assert
            Assert.AreEqual(3000, djnt_1.BranchRight.AirFlow);
            Assert.AreEqual(0, djnt_1.BranchLeft.AirFlow);
            Assert.AreEqual(3000, djnt_1.Inlet.AirFlow);
            Assert.AreEqual(0, djnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_7()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.AirFlow = 3000;
            djnt_1.BranchRight.AirFlow = 1000;
            djnt_1.BranchLeft.AirFlow = 2300;

            //Assert
            Assert.AreEqual(700, djnt_1.BranchRight.AirFlow);
            Assert.AreEqual(2300, djnt_1.BranchLeft.AirFlow);
            Assert.AreEqual(3000, djnt_1.Inlet.AirFlow);
            Assert.AreEqual(0, djnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_8()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.Inlet.AirFlow = 4000;
            djnt_1.BranchRight.AirFlow = 2000;
            djnt_1.BranchLeft.AirFlow = 2300;
            djnt_1.Outlet.AirFlow = 2000;

            //Assert
            Assert.AreEqual(1700, djnt_1.BranchRight.AirFlow);
            Assert.AreEqual(2300, djnt_1.BranchLeft.AirFlow);
            Assert.AreEqual(6000, djnt_1.Inlet.AirFlow);
            Assert.AreEqual(2000, djnt_1.Outlet.AirFlow);
        }

        [TestMethod]
        public void DJunction_TestMethod_9()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.Inlet.DuctType = Compute_Engine.Enums.DuctType.Rectangular;
            djnt_1.Inlet.DuctType = Compute_Engine.Enums.DuctType.Round;

            //Assert
            Assert.AreEqual(Compute_Engine.Enums.DuctType.Round, djnt_1.Inlet.DuctType);
            Assert.AreEqual(Compute_Engine.Enums.DuctType.Round, djnt_1.Outlet.DuctType);
        }

        [TestMethod]
        public void DJunction_TestMethod_10()
        {
            //Arrange
            DoubleJunction djnt_1 = new DoubleJunction();

            //Act
            djnt_1.Outlet.DuctType = Compute_Engine.Enums.DuctType.Rectangular;
            djnt_1.Outlet.DuctType = Compute_Engine.Enums.DuctType.Round;

            //Assert
            Assert.AreEqual(Compute_Engine.Enums.DuctType.Round, djnt_1.Inlet.DuctType);
            Assert.AreEqual(Compute_Engine.Enums.DuctType.Round, djnt_1.Outlet.DuctType);
        }

        [TestMethod]
        public void TJunction_TestMethod_1()
        {
            //Arrange
            TJunction tjnt_2 = new TJunction() { Name = "tjnt_2" };

            //Act
            tjnt_2.AirFlow = 2000;

            //Assert
            Assert.IsTrue(tjnt_2.Name == "tjnt_2");
            Assert.AreEqual(2000, tjnt_2.AirFlow);
            Assert.AreEqual(1000, tjnt_2.BranchRight.AirFlow);
            Assert.AreEqual(1000, tjnt_2.BranchLeft.AirFlow);
        }

        [TestMethod]
        public void TJunction_TestMethod_2()
        {
            //Arrange
            TJunction tjnt_1 = new TJunction();

            //Act
            tjnt_1.DuctType = Compute_Engine.Enums.DuctType.Rectangular;
            tjnt_1.DuctType = Compute_Engine.Enums.DuctType.Round;
            tjnt_1.BranchRight.DuctType = Compute_Engine.Enums.DuctType.Rectangular;
            tjnt_1.BranchLeft.DuctType = Compute_Engine.Enums.DuctType.Round;

            //Assert
            Assert.AreEqual(Compute_Engine.Enums.DuctType.Round, tjnt_1.DuctType);
            Assert.AreEqual(Compute_Engine.Enums.DuctType.Round, tjnt_1.BranchLeft.DuctType);
            Assert.AreEqual(Compute_Engine.Enums.DuctType.Round, tjnt_1.BranchRight.DuctType);
        }

        [TestMethod]
        public void TJunction_TestMethod_3()
        {
            //Arrange
            TJunction tjnt_1 = new TJunction();

            //Act
            tjnt_1.AirFlow = 3000;
            tjnt_1.BranchRight.AirFlow = 1200;
            tjnt_1.BranchLeft.AirFlow = 800;

            //Assert
            Assert.AreEqual(3000, tjnt_1.AirFlow);
            Assert.AreEqual(2200, tjnt_1.BranchRight.AirFlow);
            Assert.AreEqual(800, tjnt_1.BranchLeft.AirFlow);
        }

        [TestMethod]
        public void TJunction_TestMethod_4()
        {
            //Arrange
            TJunction tjnt_1 = new TJunction();

            //Act
            tjnt_1.AirFlow = 5000;
            tjnt_1.BranchRight.AirFlow = 7000;

            //Assert
            Assert.AreEqual(5000, tjnt_1.AirFlow);
            Assert.AreEqual(5000, tjnt_1.BranchRight.AirFlow);
            Assert.AreEqual(0, tjnt_1.BranchLeft.AirFlow);
        }
    }
}
