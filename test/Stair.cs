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
            this.Name = "StraightRunStair";
            Line[] walkingLine = {new Line(
                new Vector3(0, 0, 0),
                new Vector3(0, 10, 0)
            )};

            StairType stairType = new StairType("test", StairTypology.StraightRunStair, 0.15, 1, 0, null);
            var Stair1 = new Stair(stairType, walkingLine, 0.2, 0.3, null);

            var model = new Model();

            Assert.Equal(0.2, Stair1.RiserHeight);
            Assert.Equal(0.3, Stair1.TreadLength);

            this.Model.AddElement(Stair1);
        }

        [Fact]
        public void QuarterTurnStair()
        {
            this.Name = "QuarterTurnStair";
            Line[] walkingLine = {
                new Line(new Vector3(0, 0, 0),new Vector3(0, 2, 0)),
                new Line(new Vector3(2, 2, 0),new Vector3(2, 0, 0))
                };

            StairType stairType = new StairType("test", StairTypology.QuarterTurnStair, 0.15, 1, 0, null);
            var Stair1 = new Stair(stairType, walkingLine, 0.2, 0.3, null);

            var model = new Model();

            Assert.Equal(0.2, Stair1.RiserHeight);
            Assert.Equal(0.3, Stair1.TreadLength);

            this.Model.AddElement(Stair1);
        }
    }
}