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
        public void StraightRunStair()
        {
            Polygon square = Polygon.Rectangle(10, 10);
            Floor floor = new Floor(square, new FloorType("floor", 0.1),-0.1,null,null);

            this.Name = "StraightRunStair";
            Line[] walkingLines = {new Line(
                new Vector3(0, 0, 0),
                new Vector3(0, 10, 0)
            )};

            StairType stairType = new StairType("test", StairTypology.StraightRunStair, 0.15, 1, 0, null);
            var Stair1 = new Stair(stairType, walkingLines, 0.2, 0.3, null);

            var model = new Model();

            Assert.Equal(0.2, Stair1.RiserHeight);
            Assert.Equal(0.3, Stair1.TreadLength);

            this.Model.AddElement(Stair1);
            this.Model.AddElement(floor);
        }

        [Fact]
        public void QuarterTurnStair()
        {
            Polygon square = Polygon.Rectangle(10, 10);
            Floor floor = new Floor(square, new FloorType("floor", 0.1),-0.1,null,null);

            this.Name = "QuarterTurnStair";
            Line[] walkingLines = {
                new Line(new Vector3(0, 0, 0),new Vector3(0, 2, 0)),
                new Line(new Vector3(2, 3, 0),new Vector3(2, 0, 0))
                };

            StairType stairType = new StairType("test", StairTypology.QuarterTurnStair, 0.15, 1, 0, null);
            var Stair1 = new Stair(stairType, walkingLines, 0.2, 0.3, null);

            var model = new Model();

            Assert.Equal(0.2, Stair1.RiserHeight);
            Assert.Equal(0.3, Stair1.TreadLength);

            this.Model.AddElement(Stair1);
            this.Model.AddElement(floor);
        }

        [Fact]
        public void WalkingLinesNumber()
        {
            Line[] walkingLines = {
                new Line(new Vector3(0, 0, 0),new Vector3(0, 2, 0))
                };

            StairType stairType = new StairType("test", StairTypology.QuarterTurnStair, 0.15, 1, 0, null);
            Assert.Throws<ArgumentException>(() => new Stair(stairType, walkingLines, 0.2, 0.3, null));
        }

        [Fact]
        public void WalkingLinesNumber2()
        {
            Line[] walkingLines = {
                new Line(new Vector3(0, 0, 0),new Vector3(0, 2, 0)),
                new Line(new Vector3(2, 2, 0),new Vector3(2, 0, 0))
                };

            StairType stairType = new StairType("test", StairTypology.StraightRunStair, 0.15, 1, 0, null);
            Assert.Throws<ArgumentException>(() => new Stair(stairType, walkingLines, 0.2, 0.3, null));
        }
    }
}