using Compute_Engine.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compute_Engine_UnitTests
{
    [TestClass]
    public class NoiseAndAttenuationUnitTesting
    {
        [TestMethod]
        public void Junction_Branch_Noise()
        {
            //Arrange
            Junction jnt_2 = new Junction
            {
                AirFlow = 20388
            };
            jnt_2.Branch.AirFlow = 2039;
            jnt_2.Inlet.Width = jnt_2.Outlet.Width = 914;
            jnt_2.Inlet.Height = jnt_2.Inlet.Height = 304;
            jnt_2.Branch.Width = jnt_2.Branch.Height = 254;

            //Act
            var output = jnt_2.Branch.Noise().ToArray();
            var expected = new List<double>() { 73, 71, 67, 63, 59, 53, 47, 40 };

            //Assert
            for (int i = 0; i < output.Length; i++)
            {
                Assert.AreEqual(expected[i], Math.Round(output[i]));
            }
        }

        [TestMethod]
        public void Junction_Main_Noise()
        {
            //Arrange
            Junction jnt_2 = new Junction
            {
                AirFlow = 20388
            };
            jnt_2.Branch.AirFlow = 2039;
            jnt_2.Inlet.Width = jnt_2.Outlet.Width = 914;
            jnt_2.Inlet.Height = jnt_2.Inlet.Height = 304;
            jnt_2.Branch.Width = jnt_2.Branch.Height = 254;

            //Act
            var output = jnt_2.Noise().ToArray();
            var expected = new List<double>() { 79, 77, 74, 70, 65, 59, 53, 46 };

            //Assert
            for (int i = 0; i < output.Length; i++)
            {
                Assert.IsTrue(Enumerable.Range((int)expected[i] - 1, (int)expected[i] + 2).Contains((int)Math.Round(output[i])));
            }
        }

        [TestMethod]
        public void TJunction_Main_Noise()
        {
            //Arrange
            TJunction tjnt_2 = new TJunction
            {
                AirFlow = 20388
            };
            tjnt_2.BranchLeft.AirFlow = 10194;
            tjnt_2.Width = 914;
            tjnt_2.Height = tjnt_2.BranchRight.Height = tjnt_2.BranchLeft.Height = 304;
            tjnt_2.BranchRight.Width = tjnt_2.BranchLeft.Width = 914 / 2;

            //Act
            var output = tjnt_2.Noise().ToArray();
            var expected = new List<double>() { 93, 90, 85, 80, 74, 67, 59, 50 };

            //Assert
            for (int i = 0; i < output.Length; i++)
            {
                Assert.IsTrue(Enumerable.Range((int)expected[i] - 1, (int)expected[i] + 2).Contains((int)Math.Round(output[i])));
            }
        }

        [TestMethod]
        public void TJunction_Main_Branch()
        {
            //Arrange
            TJunction tjnt_2 = new TJunction
            {
                AirFlow = 20388
            };
            tjnt_2.BranchLeft.AirFlow = 10194;
            tjnt_2.Width = 914;
            tjnt_2.Height = tjnt_2.BranchRight.Height = tjnt_2.BranchLeft.Height = 304;
            tjnt_2.BranchRight.Width = tjnt_2.BranchLeft.Width = 914 / 2;

            //Act
            var output = tjnt_2.BranchRight.Noise().ToArray();
            var expected = new List<double>() { 90, 87, 82, 77, 71, 64, 56, 47 };

            //Assert
            for (int i = 0; i < output.Length; i++)
            {
                Assert.IsTrue(Enumerable.Range((int)expected[i] - 1, (int)expected[i] + 2).Contains((int)Math.Round(output[i])));
            }
        }

        [TestMethod]
        public void DJunction_Branch_Noise()
        {
            //Arrange
            DoubleJunction djnt_2 = new DoubleJunction
            {
                AirFlow = 20388
            };
            djnt_2.BranchRight.AirFlow = djnt_2.BranchLeft.AirFlow = 2039;
            djnt_2.Inlet.Width = djnt_2.Outlet.Width = 914;
            djnt_2.Inlet.Height = djnt_2.Inlet.Height = 304;
            djnt_2.BranchRight.Width = djnt_2.BranchRight.Height = djnt_2.BranchLeft.Width = djnt_2.BranchLeft.Height = 254;

            //Act
            var output_1 = djnt_2.BranchRight.Noise().ToArray();
            var output_2 = djnt_2.BranchLeft.Noise().ToArray();
            var expected = new List<double>() { 73, 71, 67, 63, 58, 53, 47, 40 };

            //Assert
            for (int i = 0; i < output_1.Length; i++)
            {
                Assert.IsTrue(Enumerable.Range((int)expected[i] - 1, (int)expected[i] + 2).Contains((int)Math.Round(output_1[i])));
            }

            for (int i = 0; i < output_2.Length; i++)
            {
                Assert.IsTrue(Enumerable.Range((int)expected[i] - 1, (int)expected[i] + 2).Contains((int)Math.Round(output_2[i])));
            }
        }

        [TestMethod]
        public void DJunction_Main_Noise()
        {
            //Arrange
            DoubleJunction djnt_2 = new DoubleJunction
            {
                AirFlow = 20388
            };
            djnt_2.BranchRight.AirFlow = djnt_2.BranchLeft.AirFlow = 2039;
            djnt_2.Inlet.Width = djnt_2.Outlet.Width = 914;
            djnt_2.Inlet.Height = djnt_2.Inlet.Height = 304;
            djnt_2.BranchRight.Width = djnt_2.BranchRight.Height = djnt_2.BranchLeft.Width = djnt_2.BranchLeft.Height = 254;

            //Act
            var output = djnt_2.Noise().ToArray();
            var expected = new List<double>() { 82, 80, 77, 73, 68, 63, 56, 49 };

            //Assert
            for (int i = 0; i < output.Length; i++)
            {
                Assert.IsTrue(Enumerable.Range((int)expected[i] - 1, (int)expected[i] + 2).Contains((int)Math.Round(output[i])));
            }
        }
    }
}
