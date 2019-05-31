using Elements.Geometry;
using Elements.Interfaces;
using Elements.Geometry.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Hypar.Elements.Interfaces;
using Elements.Geometry.Solids;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// The actual height of the riser. It can be shorter than the riser height of the StairType to take into account the height of the stair.
        /// </summary>
        public double ActualRiserHeight { get; }

        /// <summary>
        /// The list of stair flights in the stair, ordered from bottom to top.
        /// </summary>
        public List<StairFlight> StairFlights { get {return _stairFlight;} }

        /// <summary>
        /// Create a stair based on a typology and the walking lines.
        /// </summary>
        /// <param name="elementType">The type of the stair.</param>
        /// <param name="walkingLine">The walking line is represented by an array of lines directed into the upward direction.</param>
        /// <param name="stairTypology">The typology of the stair.</param>
        /// <param name="transform">The transform of the stair.
        /// This transform will be concatenated to the transform created to place the stair along its walking line.</param>
        [JsonConstructor]
        public Stair(StairType elementType, Line[] walkingLine, StairTypology stairTypology, Transform transform = null)
        {
            this.ElementType = elementType;
            this.ActualRiserHeight = this.ElementType.RiserHeight;
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
                case StairTypology.HalfTurnStair:
                    CreateTwoFlightsStair();
                    break;
                default:
                    throw new NotImplementedException();
            }


            this.Elements = new List<Element>();
            this.Elements.AddRange(this._stairFlight);
            this.Elements.AddRange(this._landings);
        }

        /// <summary>
        /// Create a half turn stair.
        /// </summary>
        /// <param name="elementType">The type of the stair.</param>
        /// <param name="origin">The starting point of the walking line.</param>
        /// <param name="direction">The direction of the first walking line.</param>
        /// <param name="height">The height of the stair.</param>
        /// <param name="space">The space between the two stair flights.</param>
        /// <param name="transform">The transform of the stair.
        /// This transform will be concatenated to the transform created to place the stair along its walking line.</param>
        public Stair(StairType elementType, Vector3 origin, Vector3 direction, double height, double space, Transform transform = null)
        {
            this.ElementType = elementType;
            this.ActualRiserHeight = this.ElementType.RiserHeight;
            this.StairTypology = StairTypology.HalfTurnStair;
            this.Transform = transform;

            int riser_number = (int)Math.Ceiling(height / elementType.RiserHeight);
            this.ActualRiserHeight = height / riser_number;

            int firstRunRiserNumber = (int)Math.Ceiling((double)riser_number / 2);
            int secondRunRiserNumber = riser_number - firstRunRiserNumber;

            Vector3 walkingline1Vector = direction.Normalized() * (firstRunRiserNumber * this.ElementType.TreadLength);

            Line walkingline1 = new Line(
                origin,
                origin + walkingline1Vector
                );

            Vector3 walkingline2Origin = origin + walkingline1Vector + Vector3.ZAxis.Cross(direction.Normalized()) * (this.ElementType.FlightWidth + space);
            Vector3 walkingline2Vector = direction.Normalized().Negated() * (secondRunRiserNumber * this.ElementType.TreadLength);

            Line walkingline2 = new Line(
                walkingline2Origin,
                walkingline2Origin + walkingline2Vector
                );

            this.WalkingLine = new Line[2] {
                walkingline1,
                walkingline2
            };

            CreateTwoFlightsStair();

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
                case StairTypology.HalfTurnStair:
                    if (this.WalkingLine.Length != 2)
                    {
                        throw new ArgumentException("You must have two walking lines to create a half turn stair.");
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CreateStraightRunStair()
        {
            StairFlight stairFlight = new StairFlight(this.WalkingLine[0], this.ActualRiserHeight, this.ElementType.TreadLength,
            this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, null);
            this._stairFlight.Add(stairFlight);
        }

        private void CreateTwoFlightsStair()
        {

            StairFlight stairFlight1 = new StairFlight(this.WalkingLine[0], this.ActualRiserHeight, this.ElementType.TreadLength,
            this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, this.Transform);
            this._stairFlight.Add(stairFlight1);

            Vector3 landingHeight = stairFlight1.Height() * Vector3.ZAxis;
            Line walkingLine2 = new Line(
                this.WalkingLine[1].Start + landingHeight,
                this.WalkingLine[1].End + landingHeight
            );

            StairFlight stairFlight2 = new StairFlight(walkingLine2, this.ActualRiserHeight, this.ElementType.TreadLength,
this.ElementType.WaistThickness, this.ElementType.FlightWidth, this.ElementType.Material, this.ElementType.NosingLength, this.Transform);
            this._stairFlight.Add(stairFlight2);

            CreateLanding(stairFlight1, stairFlight2);

        }

        private void CreateLanding(StairFlight stairFlight1, StairFlight stairFlight2)
        {
            List<Vector3> landingPoints1 = new List<Vector3>();

            Vector3 halfWidth1 = (stairFlight1.FlightWidth / 2) * Vector3.ZAxis.Cross(stairFlight1.WalkingLine.Direction().Normalized());
            Vector3 landingWidth1 = stairFlight1.FlightWidth * stairFlight1.WalkingLine.Direction().Normalized();
            landingPoints1.Add(stairFlight1.End + halfWidth1);
            landingPoints1.Add(stairFlight1.End + halfWidth1 + landingWidth1);
            landingPoints1.Add(stairFlight1.End + halfWidth1.Negated() + landingWidth1);
            landingPoints1.Add(stairFlight1.End + halfWidth1.Negated());
            Polygon landing1 = new Polygon(landingPoints1.ToArray());

            Vector3 halfWidth2 = (stairFlight2.FlightWidth / 2) * Vector3.ZAxis.Cross(stairFlight2.WalkingLine.Direction().Normalized());
            Vector3 landingWidth2 = stairFlight2.FlightWidth * stairFlight2.WalkingLine.Direction().Negated().Normalized();
            Vector3 BaseThickness = stairFlight2.BaseThickness() * stairFlight2.WalkingLine.Direction().Normalized();

            List<Vector3> landingPoints2 = new List<Vector3>();
            landingPoints2.Add(stairFlight2.Start + halfWidth2 + BaseThickness);
            landingPoints2.Add(stairFlight2.Start + halfWidth2);
            landingPoints2.Add(stairFlight2.Start + halfWidth2 + landingWidth2);
            landingPoints2.Add(stairFlight2.Start + halfWidth2.Negated() + landingWidth2);
            landingPoints2.Add(stairFlight2.Start + halfWidth2.Negated());
            landingPoints2.Add(stairFlight2.Start + halfWidth2.Negated() + BaseThickness);
            Polygon landing2 = new Polygon(landingPoints2.ToArray());

            if (landing1.Contains(stairFlight2.Start + halfWidth2 + landingWidth2)) { landingPoints2.Remove(stairFlight2.Start + halfWidth2 + landingWidth2); }
            if (landing1.Contains(stairFlight2.Start + halfWidth2.Negated() + landingWidth2)) { landingPoints2.Remove(stairFlight2.Start + halfWidth2.Negated() + landingWidth2); }

            if (landing2.Contains(stairFlight1.End + halfWidth1 + landingWidth1)) { landingPoints1.Remove(stairFlight1.End + halfWidth1 + landingWidth1); }
            if (landing2.Contains(stairFlight1.End + halfWidth1.Negated() + landingWidth1)) { landingPoints1.Remove(stairFlight1.End + halfWidth1.Negated() + landingWidth1); }

            landingPoints1.AddRange(landingPoints2);
            Vector3EqualityComparer comparer = new Vector3EqualityComparer();
            landingPoints1 = landingPoints1.Distinct(comparer).ToList();
            Vector3 center = stairFlight2.Start + landingWidth2 * 0.5;
            List<Vector3> landingPoints = Vector3Extensions.SortClockwise(landingPoints1, center);

            Polygon landingPolygon = new Polygon(landingPoints.ToArray());

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