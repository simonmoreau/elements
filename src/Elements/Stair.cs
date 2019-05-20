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
        private List<Beam> _topChord = new List<Beam>();
        private List<Beam> _bottomChord = new List<Beam>();

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
}