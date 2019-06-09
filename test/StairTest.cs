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

            Assert.Equal(0.2, Stair1.ElementType.RiserHeight);
            Assert.Equal(0.3, Stair1.ElementType.TreadLength);

            this.Model.AddElement(Stair1);
            this.Model.AddElement(floor);
        }

        [Fact]
        public void QuarterTurnStair()
        {
            this.Name = "QuarterTurnStair";

            Polygon square = Polygon.Rectangle(30, 30,null,10,10);
            Floor floor = new Floor(square, new FloorType("floor", 0.1), -0.1, null, null);
            this.Model.AddElement(floor);

            Vector3[] directions = new Vector3[] {
                new Vector3(1,-1,0).Normalized(),
                new Vector3(1,0,0).Normalized(),
                new Vector3(1,1,0).Normalized(),
                new Vector3(-1,-1,0).Normalized(),
                new Vector3(-1,0,0).Normalized(),
                new Vector3(-1,1,0).Normalized(),
            };

            int i = 0;
            foreach (Vector3 direction in directions)
            {
                Vector3 position = new Vector3(i * 4, 0, 0);
                Stair stair1 = CreateQuarterTurnStair(direction, position, 0.5);

                Assert.Equal(0.2, stair1.ElementType.RiserHeight);
                Assert.Equal(0.3, stair1.ElementType.TreadLength);

                position = new Vector3(i * 4, 6, 0);
                Stair stair2 = CreateQuarterTurnStair(direction, position, 1);

                this.Model.AddElement(stair1);
                this.Model.AddElement(stair2);
                i++;
            }

        }

        private Stair CreateQuarterTurnStair(Vector3 direction, Vector3 position, double offset)
        {
            StairType stairType = new StairType("test", 0.2, 0.3, 0.15, 1, 0.02, null);

            Vector3 vector = new Vector3(0.5, 0, 0);
            if (direction.X < 0) { vector = new Vector3(-0.5, 0, 0); }
            Vector3 start = vector + offset * Vector3.ZAxis.Cross(direction).Normalized();
            if (direction.X < 0) { start = vector + offset * Vector3.ZAxis.Cross(direction).Normalized().Negated(); }

            Line[] walkingLines1 = {
                new Line(new Vector3(0, -3, 0),new Vector3(0, 0, 0)),
                new Line(start,direction,3 )
                };

            Transform transform = new Transform(position);

            Stair stair = new Stair(stairType, walkingLines1, StairTypology.QuarterTurnStair, transform);

            return stair;

        }

        [Fact]
        public void HalfTurnStair()
        {
            Polygon square = Polygon.Rectangle(10, 10);
            Floor floor = new Floor(square, new FloorType("floor", 0.1), -0.1, null, null);

            this.Name = "HalfTurnStair";
            StairType stairType = new StairType("test", 0.2, 0.3, 0.15, 1, 0.02, null);

            var Stair1 = new Stair(stairType, new Vector3(), Vector3.XAxis, 4, 0.2, null);

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