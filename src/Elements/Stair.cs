using Elements.Geometry;
using Elements.Interfaces;
using Elements.Geometry.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Hypar.Elements.Interfaces;
using Elements.Geometry.Solids;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        /// The typology of the stair.
        /// </summary>
        public StairTypology StairTypology { get; }
        /// <summary>
        /// The walking line of the stair flight.
        /// </summary>
        public Line[] WalkingLine { get; }

        /// <summary>
        /// Create a stair based on the walkingLine.
        /// </summary>
        /// <param name="elementType">The type of the stair.</param>
        /// <param name="walkingLine">The walking line is represented by an array of lines directed into the upward direction.</param>
        /// <param name="stairTypology">The typology of the stair.</param>
        /// <param name="transform">The transform of the stair.
        /// This transform will be concatenated to the transform created to place the stair along its walking line.</param>
        public Stair(StairType elementType, Line[] walkingLine, StairTypology stairTypology, Transform transform = null)
        {
            this.ElementType = elementType;
            this.WalkingLine = walkingLine;
            this.StairTypology = stairTypology;
            this.Transform = transform;

            CheckInput();

            switch (this.StairTypology)
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
            switch (this.StairTypology)
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
            StairFlight stairFlight = new StairFlight(this.WalkingLine[0], this.ElementType.RiserHeight, this.ElementType.TreadLength,
            this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, null);
            this._stairFlight.Add(stairFlight);
        }

        private void CreateTwoFlightsStair()
        {

            StairFlight stairFlight1 = new StairFlight(this.WalkingLine[0], this.ElementType.RiserHeight, this.ElementType.TreadLength,
            this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, this.Transform);
            this._stairFlight.Add(stairFlight1);

            Vector3 landingHeight = stairFlight1.Height() * Vector3.ZAxis;
            Line walkingLine2 = new Line(
                this.WalkingLine[1].Start + landingHeight,
                this.WalkingLine[1].End + landingHeight
            );

            StairFlight stairFlight2 = new StairFlight(walkingLine2, this.ElementType.RiserHeight, this.ElementType.TreadLength,
this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, this.Transform);
            this._stairFlight.Add(stairFlight2);

            CreateLanding(stairFlight1, stairFlight2);

        }

        private void CreateLanding(StairFlight stairFlight1, StairFlight stairFlight2)
        {
            IList<Vector3> landingPoints = new List<Vector3>();

            Vector3 halfWidth1 = (stairFlight1.FlightWidth / 2) * Vector3.ZAxis.Cross(stairFlight1.WalkingLine.Direction().Normalized());
            Vector3 landingWidth1 = stairFlight1.FlightWidth * stairFlight1.WalkingLine.Direction().Normalized();
            landingPoints.Add(stairFlight1.End + halfWidth1);
            landingPoints.Add(stairFlight1.End + halfWidth1.Negated());
            landingPoints.Add(stairFlight1.End + halfWidth1 + landingWidth1);
            landingPoints.Add(stairFlight1.End + halfWidth1.Negated() + landingWidth1);

            Vector3 halfWidth2 = (stairFlight2.FlightWidth / 2) * Vector3.ZAxis.Cross(stairFlight2.WalkingLine.Direction().Normalized());
            Vector3 landingWidth2 = stairFlight2.FlightWidth * stairFlight2.WalkingLine.Direction().Negated().Normalized();
            Vector3 BaseThickness = stairFlight2.BaseThickness() * stairFlight2.WalkingLine.Direction().Normalized();

            landingPoints.Add(stairFlight2.Start + halfWidth2 + BaseThickness);
            landingPoints.Add(stairFlight2.Start + halfWidth2.Negated() + BaseThickness);
            landingPoints.Add(stairFlight2.Start + halfWidth2 + landingWidth2);
            landingPoints.Add(stairFlight2.Start + halfWidth2.Negated() + landingWidth2);

            List<Vector3> landingHull = ConvexHull.MakeHull(landingPoints);
            Polygon landingPolygon = new Polygon(landingHull.ToArray());
            if (!landingPolygon.Plane().Normal.IsAlmostEqualTo(Vector3.ZAxis))
            {
                landingPolygon = landingPolygon.Reversed();
            }

            Vector3 landingHeight = (stairFlight1.Height() - stairFlight1.LandingThickness()) * Vector3.ZAxis;
            Transform landingHeightTansform = new Transform(landingHeight);
            if (this.Transform != null) { landingHeightTansform.Concatenate(this.Transform); }

            FloorType type = new FloorType("landing", stairFlight1.LandingThickness());
            Floor landing = new Floor(landingPolygon, type, 0, landingHeightTansform, null);
            this._landings.Add(landing);
        }
    }


    /// <summary>
    /// The typology of the stair.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StairTypology
    {
        /// <summary>
        /// A stair extending from one level to another without turns or winders. The stair consists of one straight flight.
        /// </summary>
        [EnumMember(Value = "StraightRunStair")]
        StraightRunStair,
        /// <summary>
        /// A straight stair consisting of two straight flights without turns but with one landing.
        /// </summary>
        [EnumMember(Value = "TwoStraightRunStair")]
        TwoStraightRunStair,
        /// <summary>
        /// A stair consisting of one flight with a quarter winder, which is making a 90° turn. The direction of the turn is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "QuarterWindingStair")]
        QuarterWindingStair,
        /// <summary>
        /// A stair making a 90° turn, consisting of two straight flights connected by a quarterspace landing. The direction of the turn is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "QuarterTurnStair")]
        QuarterTurnStair,
        /// <summary>
        /// A stair consisting of one flight with one half winder, which makes a 180° turn. The orientation of the turn is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "HalfWindingStair")]
        HalfWindingStair,
        /// <summary>
        /// A stair making a 180° turn, consisting of two straight flights connected by a halfspace landing. The orientation of the turn is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "HalfTurnStair")]
        HalfTurnStair,
        /// <summary>
        /// A stair consisting of one flight with two quarter winders, which make a 90° turn. The stair makes a 180° turn. The direction of the turns is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "TwoQuarterWindingStair")]
        TwoQuarterWindingStair,
        /// <summary>
        /// A stair making a 180° turn, consisting of three straight flights connected by two quarterspace landings. The direction of the turns is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "TwoQuarterTurnStair")]
        TwoQuarterTurnStair,
        /// <summary>
        /// A stair consisting of one flight with three quarter winders, which make a 90° turn. The stair makes a 270° turn. The direction of the turns is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "ThreeQuarterWindingStair")]
        ThreeQuarterWindingStair,
        /// <summary>
        /// A stair making a 270° turn, consisting of four straight flights connected by three quarterspace landings. The direction of the turns is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "ThreeQuarterTurnStair")]
        ThreeQuarterTurnStair,
        /// <summary>
        /// A stair constructed with winders around a circular newel often without landings. Depending on outer boundary it can be either a circular, elliptical or rectangular spiral stair. The orientation of the winding stairs is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "SpiralStair")]
        SpiralStair,
        /// <summary>
        /// A stair having one straight flight to a wide quarterspace landing, and two side flights from that landing into opposite directions. The stair is making a 90° turn. The direction of traffic is determined by the walking line.
        /// </summary>
        [EnumMember(Value = "DoubleReturnStair")]
        DoubleReturnStair,
        /// <summary>
        /// A stair extending from one level to another without turns or winders. The stair is consisting of one curved flight.
        /// </summary>
        [EnumMember(Value = "CurvedRunStair")]
        CurvedRunStair,
        /// <summary>
        /// A curved stair consisting of two curved flights without turns but with one landing.
        /// </summary>
        [EnumMember(Value = "TwoCurvedRunStair")]
        TwoCurvedRunStair,
        /// <summary>
        /// Free form stair (user defined operation type)
        /// </summary>
        [EnumMember(Value = "OtherOperation")]
        OtherOperation
    }
}