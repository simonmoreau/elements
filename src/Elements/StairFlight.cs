using System;
using System.Collections.Generic;
using Elements.Interfaces;
using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements.Geometry.Interfaces;
using Newtonsoft.Json;

namespace Elements
{
    /// <summary>
    /// A stair flight is an assembly of building components in a single "run" of stair steps (not interrupted by a landing).
    /// </summary>
    public class StairFlight : Element, IExtrude, IMaterial
    {
        /// <summary>
        /// Total number of the risers included in the stair flight.
        /// </summary>
        public int NumberOfRiser { get; }

        /// <summary>
        /// Total number of treads included in the stair flight.
        /// </summary>
        public int NumberOfTreads { get; }

        /// <summary>
        /// Vertical distance from tread to tread. The riser height is supposed to be equal for all steps of a stair or stair flight.
        /// </summary>
        public double RiserHeight { get; }

        /// <summary>
        /// Horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair or stair flight at the walking line.
        /// </summary>
        public double TreadLength { get; }

        /// <summary>
        /// Horizontal distance from the front of the tread to the riser underneath. It is the overhang of the tread.
        /// </summary>
        public double NosingLength { get; }

        /// <summary>
        /// Minimum thickness of the stair flight, measured perpendicular to the slope of the flight to the inner corner of riser and tread. It is a pre-calculated value, in case of inconsistencies, the value derived from the shape representation shall take precedence.
        /// </summary>
        public double WaistThickness { get; }

        /// <summary>
        /// Overall width of the stair flight.
        /// </summary>
        public double FlightWidth { get; }

        /// <summary>
        /// The type of the stair flight.
        /// </summary>
        public StairFlightType StairFlightType { get; }

        /// <summary>
        /// The walking line of the stair flight.
        /// </summary>
        public Line WalkingLine { get; }

        /// <summary>
        /// The extruded direction of the stair flight.
        /// </summary>
        public Vector3 ExtrudeDirection { get; protected set; }

        /// <summary>
        /// The extruded depth of the stair flight.
        /// </summary>
        public double ExtrudeDepth { get; protected set; }

        /// <summary>
        /// The extruded area of the stair flight.
        /// </summary>
        public Profile Profile { get; protected set; }

        /// <summary>
        /// The stair flight's material.
        /// </summary>
        public Material Material { get; }

        /// <summary>
        /// Extrude to both sides?
        /// </summary>
        public virtual bool BothSides => true;

        /// <summary>
        /// Create a stair flight based on the walkingLine.
        /// </summary>
        /// <param name="stairFlightType">The type of the stair flight.</param>
        /// <param name="walkingLine">The walking line is represented by a line directed into the upward direction.</param>
        /// <param name="riserHeight">The vertical distance from tread to tread. The riser height is supposed to be equal for all steps of a stair or stair flight.</param>
        /// <param name="treadLength">The horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair or stair flight at the walking line.</param>
        /// <param name="waistThickness">The minimum thickness of the stair flight, measured perpendicular to the slope of the flight to the inner corner of riser and tread. It is a pre-calculated value, in case of inconsistencies, the value derived from the shape representation shall take precedence.</param>
        /// <param name="flightWidth">The overall width of the stair flight.</param>
        /// <param name="material">The stair flight's material.</param>
        /// <param name="nosingLength">The horizontal distance from the front of the tread to the riser underneath. It is the overhang of the tread.</param>
        public StairFlight(StairFlightType stairFlightType, Line walkingLine, double riserHeight, double treadLength, double waistThickness, double flightWidth, Material material = null, double nosingLength = 0)
        {
            this.StairFlightType = stairFlightType;
            this.WalkingLine = walkingLine;
            this.TreadLength = treadLength;
            this.NumberOfTreads = (int)Math.Floor(walkingLine.Length() / this.TreadLength);
            this.NumberOfRiser = this.NumberOfTreads;
            this.RiserHeight = riserHeight;
            this.WaistThickness = waistThickness;
            this.FlightWidth = flightWidth;
            this.NosingLength = nosingLength;
            this.Material = material == null ? BuiltInMaterials.Default : material;

            CreateStraightStairFlight();
        }

        private void CreateStraightStairFlight()
        {
            Vector3 basepoint = this.WalkingLine.Start;
            Vector3 direction = this.WalkingLine.Direction();
            Transform sectionTransform = new Transform(basepoint, direction, Vector3.ZAxis);

            this.Profile = sectionTransform.OfProfile(new Profile(StraightStairFlightSection()));
            this.ExtrudeDepth = this.FlightWidth;
            this.ExtrudeDirection = Vector3.ZAxis.Cross(direction);
        }

        private Polygon StraightStairFlightSection()
        {
            List<Vector3> flightPoints = new List<Vector3>();

            Vector3 tread = new Vector3(this.TreadLength, 0, 0);
            Vector3 riser = new Vector3(0, this.RiserHeight, 0);

            for (int i = 0; i < this.NumberOfRiser; i++)
            {
                flightPoints.Add(new Vector3(i * this.TreadLength, i * this.RiserHeight, 0));
                flightPoints.Add(new Vector3(i * this.TreadLength, (i + 1) * this.RiserHeight, 0));
            }

            // Last step
            Vector3 lastStepPoint = new Vector3(this.NumberOfRiser * this.TreadLength, this.NumberOfRiser * this.RiserHeight, 0);
            flightPoints.Add(lastStepPoint);

            // Run thickness
            Vector3 runThickness = (riser + tread).Cross(Vector3.ZAxis).Normalized() * this.WaistThickness;
            double alpha = runThickness.AngleTo(Vector3.YAxis.Negated());
            alpha = alpha * Math.PI / 180;
            Vector3 verticalThickness = (runThickness.Length() / Math.Cos(alpha)) * Vector3.YAxis.Negated();
            flightPoints.Add(lastStepPoint + verticalThickness);

            alpha = (riser + tread).Negated().AngleTo(Vector3.XAxis);
            alpha = alpha * Math.PI / 180;
            Vector3 horizontalThickness = (runThickness.Length() / Math.Sin(alpha)) * Vector3.XAxis;
            flightPoints.Add(horizontalThickness);

            return new Polygon(flightPoints.ToArray()).Reversed(); ;
        }

    }

    /// <summary>
    /// The type of the stair flight.
    /// </summary>
    public enum StairFlightType
    {
        /// <summary>
        /// A stair flight with a straight walking line.
        /// </summary>
        Straight,
        /// <summary>
        /// A stair flight with a walking line including straight and curved sections.
        /// </summary>
        Winder,
        /// <summary>
        /// A stair flight with a circular or elliptic walking line.
        /// </summary>
        Spiral,
        /// <summary>
        /// A stair flight with a curved walking line.
        /// </summary>
        Curved,
        /// <summary>
        /// A stair flight with a free form walking line (and outer boundaries).
        /// </summary>
        Freeform
    }
}