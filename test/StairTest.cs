using Elements;
using Elements.Geometry;
using System;
using System.Collections.Generic;
using Xunit;

namespace Elements.Tests
{
    public class StairTests : ModelTest
    {
        [Fact]
        public void Stair()
        {
            this.Name = "Stair";
            var p = Polygon.L(10, 20, 5);
            var StairType = new StairType("test", new Material("blue", Colors.Blue, 0.0f, 0.0f));
            Line walkingLine = new Line(
                new Vector3(0,0,0),
                new Vector3(0,10,0)
            );
            var StairFlight1 = new StairFlight(StairFlightType.Straight,walkingLine,0.2,0.3,0.15,1);

            var model = new Model();

            Assert.Equal(1, StairFlight1.FlightWidth);
            Assert.Equal(0.2, StairFlight1.RiserHeight);
            Assert.Equal(0.3, StairFlight1.TreadLength);
            Assert.Equal(0.15, StairFlight1.WaistThickness);

            this.Model.AddElement(StairFlight1);
        }

        [Fact]
        public void ZeroThickness()
        {
            var model = new Model();
            var poly = Polygon.Rectangle(width:20, height:20);
            Assert.Throws<ArgumentOutOfRangeException>(()=> {var StairType = new StairType("test", BuiltInMaterials.Concrete);});
        }

    }
}