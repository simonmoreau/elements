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
    public class Stair : Element, IElementType<StairType>, ISolid, IAggregateElements
    {
        /// <summary>
        /// The stair's geometry.
        /// </summary>
        public Solid Geometry { get; }

        private List<Floor> _landings = new List<Floor>();
        private List<StairFlight> _stairFlight = new List<StairFlight>();


        /// <summary>
        /// The elements aggregated by this element.
        /// </summary>
        public List<Element> Elements { get; }

        /// <summary>
        /// The stair type of the stair.
        /// </summary>
        public StairType ElementType { get; }

        /// <summary>
        /// Create a stair from an arbitrary geometry.
        /// </summary>
        /// <param name="geometry">The geometry of the stair.</param>
        /// <param name="elementType">The stair type of the stair.</param>
        /// <param name="transform">The stair's transform</param>
        public Stair(Solid geometry, StairType elementType, Transform transform = null)
        {
            this.Geometry = geometry;
            this.ElementType = elementType;
            this.Transform = transform;
        }
    }

    /// <summary>
    /// The type of the stair.
    /// </summary>
    public enum StairTypology
    {
        /// <summary>
        /// A stair extending from one level to another without turns or winders. The stair consists of one straight flight.
        /// </summary>
        StraightRunStair,
        /// <summary>
        /// A straight stair consisting of two straight flights without turns but with one landing.
        /// </summary>
        TwoStraightRunStair,
        /// <summary>
        /// A stair consisting of one flight with a quarter winder, which is making a 90° turn. The direction of the turn is determined by the walking line.
        /// </summary>
        QuarterWindingStair,
        /// <summary>
        /// A stair making a 90° turn, consisting of two straight flights connected by a quarterspace landing. The direction of the turn is determined by the walking line.
        /// </summary>
        QuarterTurnStair,
        /// <summary>
        /// A stair consisting of one flight with one half winder, which makes a 180° turn. The orientation of the turn is determined by the walking line.
        /// </summary>
        HalfWindingStair,
        /// <summary>
        /// A stair making a 180° turn, consisting of two straight flights connected by a halfspace landing. The orientation of the turn is determined by the walking line.
        /// </summary>
        HalfTurnStair,
        /// <summary>
        /// A stair consisting of one flight with two quarter winders, which make a 90° turn. The stair makes a 180° turn. The direction of the turns is determined by the walking line.
        /// </summary>
        TwoQuarterWindingStair,
        /// <summary>
        /// A stair making a 180° turn, consisting of three straight flights connected by two quarterspace landings. The direction of the turns is determined by the walking line.
        /// </summary>
        TwoQuarterTurnStair,
        /// <summary>
        /// A stair consisting of one flight with three quarter winders, which make a 90° turn. The stair makes a 270° turn. The direction of the turns is determined by the walking line.
        /// </summary>
        ThreeQuarterWindingStair,
        /// <summary>
        /// A stair making a 270° turn, consisting of four straight flights connected by three quarterspace landings. The direction of the turns is determined by the walking line.
        /// </summary>
        ThreeQuarterTurnStair,
        /// <summary>
        /// A stair constructed with winders around a circular newel often without landings. Depending on outer boundary it can be either a circular, elliptical or rectangular spiral stair. The orientation of the winding stairs is determined by the walking line.
        /// </summary>
        SpiralStair,
        /// <summary>
        /// A stair having one straight flight to a wide quarterspace landing, and two side flights from that landing into opposite directions. The stair is making a 90° turn. The direction of traffic is determined by the walking line.
        /// </summary>
        DoubleReturnStair,
        /// <summary>
        /// A stair extending from one level to another without turns or winders. The stair is consisting of one curved flight.
        /// </summary>
        CurvedRunStair,
        /// <summary>
        /// A curved stair consisting of two curved flights without turns but with one landing.
        /// </summary>
        TwoCurvedRunStair,
        /// <summary>
        /// Free form stair (user defined operation type)
        /// </summary>
        OtherOperation
    }
}