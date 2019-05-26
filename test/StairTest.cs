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
            Floor floor = new Floor(square, new FloorType("floor", 0.1), -0.1, null, null);

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
            Floor floor = new Floor(square, new FloorType("floor", 0.1), -0.1, null, null);

            this.Name = "QuarterTurnStair";
            StairType stairType = new StairType("test", 0.2, 0.3, 0.15, 1, 0.02, null);

            Line[] walkingLines1 = {
                new Line(new Vector3(0, 0, 0),new Vector3(0, 3, 0)),
                new Line(new Vector3(0.5, 3.5, 0),new Vector3(2.5, 3.5, 0))
                };

            var Stair1 = new Stair(stairType, walkingLines1, StairTypology.QuarterTurnStair, null);

            Line[] walkingLines2 = {
                new Line(new Vector3(4, 0, 0),new Vector3(4, 3, 0)),
                new Line(new Vector3(5, 3, 0),new Vector3(5, 0, 0))
                };

            var Stair2 = new Stair(stairType, walkingLines2, StairTypology.QuarterTurnStair, null);

            Line[] walkingLines3 = {
                new Line(new Vector3(-3, 0, 0),new Vector3(-3, 3, 0)),
                new Line(new Vector3(-6, 3, 0),new Vector3(-6,0, 0))
                };

            var Stair3 = new Stair(stairType, walkingLines3, StairTypology.QuarterTurnStair, null);

            var model = new Model();

            Assert.Equal(0.2, Stair1.ElementType.RiserHeight);
            Assert.Equal(0.3, Stair1.ElementType.TreadLength);

            this.Model.AddElement(Stair1);
            this.Model.AddElement(Stair2);
            this.Model.AddElement(Stair3);
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