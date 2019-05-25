using Elements.Interfaces;
using System;
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
        /// Vertical distance from tread to tread. The riser height is supposed to be equal for all steps of the stair flight.
        /// </summary>
        public double RiserHeight { get; }

        /// <summary>
        /// Horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair flight at the walking line.
        /// </summary>
        public double TreadLength { get; }

        /// <summary>
        /// Construct a stair type.
        /// </summary>
        /// <param name="name">The name of the stair type.</param>
        /// <param name="riserHeight">The vertical distance from tread to tread. The riser height is supposed to be equal for all steps of the stair.</param>
        /// <param name="treadLength">The horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair at the walking line.</param>
        /// <param name="waistThickness">The minimum thickness of the stair flight, measured perpendicular to the slope of the flight to the inner corner of riser and tread.</param>
        /// <param name="flightWidth">The overall width of a stair flight.</param>
        /// <param name="nosingLength">The horizontal distance from the front of the tread to the riser underneath. It is the overhang of the tread.</param>
        /// <param name="material">The material used by the stair type.</param>
        public StairType(string name, double riserHeight, double treadLength, double waistThickness, double flightWidth, double nosingLength = 0, Material material = null) : base(name)
        {
            this.RiserHeight = riserHeight;
            this.TreadLength = treadLength;
            this.WaistThickness = waistThickness;
            this.FlightWidth = flightWidth;
            this.NosingLength = nosingLength;
            this.Material = material == null ? BuiltInMaterials.Default : material;
        }
    }

}