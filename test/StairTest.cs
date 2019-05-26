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

            StairType stairType = new StairType("test", 0.2, 0.3, 0.15, 1, 0, null);
            var Stair1 = new Stair(stairType, walkingLines, StairTypology.StraightRunStair, null);

            var model = new Model();

            Assert.Equal(0.2, Stair1.ElementType.RiserHeight);
            Assert.Equal(0.3, Stair1.ElementType.TreadLength);

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
                new Line(new Vector3(0, 0, 0),new Vector3(0, 3, 0)),
                new Line(new Vector3(0.5, 3.5, 0),new Vector3(2.5, 3.5, 0))
                };

            StairType stairType = new StairType("test", 0.2, 0.3, 0.15, 1, 0.02, null);
            var Stair1 = new Stair(stairType, walkingLines, StairTypology.QuarterTurnStair, null);

            var model = new Model();

            Assert.Equal(0.2, Stair1.ElementType.RiserHeight);
            Assert.Equal(0.3, Stair1.ElementType.TreadLength);

            this.Model.AddElement(Stair1);
            this.Model.AddElement(floor);
        }

        [Fact]
        public void WalkingLinesNumber()
        {
            Line[] walkingLines = {
                new Line(new Vector3(0, 0, 0),new Vector3(0, 2, 0))
                };

            StairType stairType = new StairType("test", 0.2, 0.3, 0.15, 1, 0, null);
            Assert.Throws<ArgumentException>(() => new Stair(stairType, walkingLines, StairTypology.QuarterTurnStair, null));
        }

        [Fact]
        public void WalkingLinesNumber2()
        {
            Line[] walkingLines = {
                new Line(new Vector3(0, 0, 0),new Vector3(0, 2, 0)),
                new Line(new Vector3(2, 2, 0),new Vector3(2, 0, 0))
                };

            StairType stairType = new StairType("test", 0.2, 0.3, 0.15, 1, 0, null);
            Assert.Throws<ArgumentException>(() => new Stair(stairType, walkingLines, StairTypology.StraightRunStair, null));
        }
    }
}