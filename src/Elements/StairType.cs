using Elements.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Elements
{
    /// <summary>
    /// A container for properties common to stairs.
    /// </summary>
    public class StairType : ElementType, IMaterial
    {
        /// <summary>
        /// The type of the stair type.
        /// </summary>
        public override string Type
        {
            get { return "stairType"; }
        }

        /// <summary>
        /// The material used by the stair type.
        /// </summary>
        public Material Material { get; }
        /// <summary>
        /// The typology of the stair.
        /// </summary>
        public StairTypology StairTypology { get; }

        /// <summary>
        /// Horizontal distance from the front of the tread to the riser underneath. It is the overhang of the tread.
        /// </summary>
        public double NosingLength { get; }

        /// <summary>
        /// Minimum thickness of a stair flight, measured perpendicular to the slope of the flight to the inner corner of riser and tread.
        /// </summary>
        public double WaistThickness { get; }

        /// <summary>
        /// Overall width of a stair flight.
        /// </summary>
        public double FlightWidth { get; }

        /// <summary>
        /// Construct a stair type.
        /// </summary>
        /// <param name="name">The name of the stair type.</param>
        /// <param name="stairTypology">The typology of the stair.</param>
        /// <param name="waistThickness">The minimum thickness of the stair flight, measured perpendicular to the slope of the flight to the inner corner of riser and tread.</param>
        /// <param name="flightWidth">The overall width of a stair flight.</param>
        /// <param name="nosingLength">The horizontal distance from the front of the tread to the riser underneath. It is the overhang of the tread.</param>
        /// <param name="material">The material used by the stair type.</param>
        public StairType(string name, StairTypology stairTypology, double waistThickness, double flightWidth, double nosingLength = 0, Material material = null) : base(name)
        {
            this.StairTypology = stairTypology;
            this.WaistThickness = waistThickness;
            this.FlightWidth = flightWidth;
            this.NosingLength = nosingLength;
            this.Material = material == null ? BuiltInMaterials.Default : material;
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