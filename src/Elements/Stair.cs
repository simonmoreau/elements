using Elements.Geometry;
using Elements.Interfaces;
using Elements.Geometry.Interfaces;
using Newtonsoft.Json;
using Hypar.Elements.Interfaces;
using Elements.Geometry.Solids;
using System;
using System.Collections.Generic;

namespace Elements
{
    /// <summary>
    /// A stair is a vertical passageway allowing occupants to walk (step) from one floor level to another floor level at a different elevation.
    /// </summary>
    public class Stair : Element, IElementType<StairType>, IAggregateElements
    {
        private List<Floor> _landings = new List<Floor>();
        private List<StairFlight> _stairFlight = new List<StairFlight>();

        /// <summary>
        /// The elements aggregated by this element.
        /// </summary>
        public List<Element> Elements { get; }

        /// <summary>
        /// The stair type of the stair.
        /// </summary>
        public StairType ElementType { get; protected set; }
        /// <summary>
        /// The walking line of the stair flight.
        /// </summary>
        public Line[] WalkingLine { get; }
        /// <summary>
        /// Vertical distance from tread to tread. The riser height is supposed to be equal for all steps of the stair flight.
        /// </summary>
        public double RiserHeight { get; }

        /// <summary>
        /// Horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair flight at the walking line.
        /// </summary>
        public double TreadLength { get; }

        /// <summary>
        /// Create a stair based on the walkingLine.
        /// </summary>
        /// <param name="elementType">The type of the stair.</param>
        /// <param name="walkingLine">The walking line is represented by an array of lines directed into the upward direction.</param>
        /// <param name="riserHeight">The vertical distance from tread to tread. The riser height is supposed to be equal for all steps of the stair.</param>
        /// <param name="treadLength">The horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair at the walking line.</param>
        /// <param name="transform">The transform of the stair.
        /// This transform will be concatenated to the transform created to place the stair along its walking line.</param>
        public Stair(StairType elementType, Line[] walkingLine, double riserHeight, double treadLength, Transform transform = null)
        {
            this.ElementType = elementType;
            this.WalkingLine = walkingLine;
            this.RiserHeight = riserHeight;
            this.TreadLength = treadLength;
            this.Transform = transform;

            CheckInput();

            switch (this.ElementType.StairTypology)
            {
                case StairTypology.StraightRunStair:
                    CreateStraightRunStair();
                    break;
                case StairTypology.QuarterTurnStair:
                    CreateTwoFlightsStair();
                    break;
                default:
                    throw new NotImplementedException();
            }


            this.Elements = new List<Element>();
            this.Elements.AddRange(this._stairFlight);
            this.Elements.AddRange(this._landings);
        }

        private void CheckInput()
        {
            switch (this.ElementType.StairTypology)
            {
                case StairTypology.StraightRunStair:
                    if (this.WalkingLine.Length != 1)
                    {
                        throw new ArgumentException("You must have only one walking line to create a straight run stair.");
                    }
                    break;
                case StairTypology.QuarterTurnStair:
                    if (this.WalkingLine.Length != 2)
                    {
                        throw new ArgumentException("You must have two walking lines to create a quarter turn stair.");
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CreateStraightRunStair()
        {
            StairFlight stairFlight = new StairFlight(this.WalkingLine[0], this.RiserHeight, this.TreadLength,
            this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, null);
            this._stairFlight.Add(stairFlight);
        }

        private void CreateTwoFlightsStair()
        {

            StairFlight stairFlight1 = new StairFlight(this.WalkingLine[0], this.RiserHeight, this.TreadLength,
            this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, this.Transform);
            this._stairFlight.Add(stairFlight1);

            Vector3 landingHeight = stairFlight1.Height() * Vector3.ZAxis;
            Line walkingLine2 = new Line(
                this.WalkingLine[1].Start + landingHeight,
                this.WalkingLine[1].End + landingHeight
            );

            StairFlight stairFlight2 = new StairFlight(walkingLine2, this.RiserHeight, this.TreadLength,
this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, this.Transform);
            this._stairFlight.Add(stairFlight2);

            CreateLanding(stairFlight1, stairFlight2);

        }

        private void CreateLanding(StairFlight stairFlight1, StairFlight stairFlight2)
        {
            Vector3 landingHeight = stairFlight1.Height() * Vector3.ZAxis;

            IList<Vector3> landingPoints = new List<Vector3>();

            Vector3 halfWidth1 = (stairFlight1.FlightWidth / 2) * Vector3.ZAxis.Cross(stairFlight1.WalkingLine.Direction().Normalized());
            Vector3 landingWidth1 = stairFlight1.FlightWidth * stairFlight1.WalkingLine.Direction().Normalized();
            landingPoints.Add(stairFlight1.End + landingHeight + halfWidth1);
            landingPoints.Add(stairFlight1.End + landingHeight + halfWidth1.Negated());
            landingPoints.Add(stairFlight1.End + landingHeight + halfWidth1 + landingWidth1);
            landingPoints.Add(stairFlight1.End + landingHeight + halfWidth1.Negated() + landingWidth1);

            Vector3 halfWidth2 = (stairFlight2.FlightWidth / 2) * Vector3.ZAxis.Cross(stairFlight2.WalkingLine.Direction().Normalized());
            Vector3 landingWidth2 = stairFlight2.FlightWidth * stairFlight2.WalkingLine.Direction().Negated().Normalized();
            landingPoints.Add(stairFlight2.Start + landingHeight + halfWidth2);
            landingPoints.Add(stairFlight2.Start + landingHeight + halfWidth2.Negated());
            landingPoints.Add(stairFlight2.Start + landingHeight + halfWidth2 + landingWidth2);
            landingPoints.Add(stairFlight2.Start + landingHeight + halfWidth2.Negated() + landingWidth2);

            List<Vector3> landingHull = ConvexHull.MakeHull(landingPoints);
            Polygon landingPolygon = new Polygon(landingHull.ToArray());
            if (!landingPolygon.Plane().Normal.IsAlmostEqualTo(Vector3.ZAxis))
            {
                landingPolygon = landingPolygon.Reversed();
            }

            Transform landingHeightTansform = new Transform(landingHeight + new Vector3(0,0,-stairFlight1.LandingThickness()));
            if (this.Transform != null) { landingHeightTansform.Concatenate(this.Transform); }

            FloorType type = new FloorType("landing", stairFlight1.LandingThickness());
            Floor landing = new Floor(landingPolygon, type, 0, landingHeightTansform, null);
            this._landings.Add(landing);
        }
    }
}