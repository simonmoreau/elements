using Elements;
using Elements.Geometry;
using System;
using System.Collections.Generic;
using Xunit;

namespace Elements.Tests
{
    public class StairFlightTests : ModelTest
    {
        [Fact]
        public void StairFlight()
        {
            this.Name = "StairFlight";
            Line walkingLine = new Line(
                new Vector3(0, 0, 0),
                new Vector3(0, 10, 0)
            );
            var StairFlight1 = new StairFlight(walkingLine, 0.2, 0.3, 0.15, 1);

            var model = new Model();

            Assert.Equal(1, StairFlight1.FlightWidth);
            Assert.Equal(0.2, StairFlight1.RiserHeight);
            Assert.Equal(0.3, StairFlight1.TreadLength);
            Assert.Equal(0.15, StairFlight1.WaistThickness);

            this.Model.AddElement(StairFlight1);
        }

        [Fact]
        public void ZeroRiserHeight()
        {
            var model = new Model();
            Line walkingLine = new Line(
    new Vector3(0, 0, 0),
    new Vector3(0, 10, 0)
);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var StairFlight1 = new StairFlight(walkingLine, 0, 0.3, 0.15, 1); });
        }

        [Fact]
        public void ZeroTreadLength()
        {
            var model = new Model();
            Line walkingLine = new Line(
    new Vector3(0, 0, 0),
    new Vector3(0, 10, 0)
);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var StairFlight1 = new StairFlight(walkingLine, 0.2, 0, 0.15, 1); });
        }

        [Fact]
        public void ZeroWaistThickness()
        {
            var model = new Model();
            Line walkingLine = new Line(
    new Vector3(0, 0, 0),
    new Vector3(0, 10, 0)
);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var StairFlight1 = new StairFlight(walkingLine, 0.2, 0.3, 0, 1); });
        }

    }
}