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
        /// Vertical distance from tread to tread. The riser height is supposed to be equal for all steps of the stair flight.
        /// </summary>
        public double RiserHeight { get; }

        /// <summary>
        /// Horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair flight at the walking line.
        /// </summary>
        public double TreadLength { get; }

        /// <summary>
        /// Horizontal distance from the front of the tread to the riser underneath. It is the overhang of the tread.
        /// </summary>
        public double NosingLength { get; }

        /// <summary>
        /// Minimum thickness of the stair flight, measured perpendicular to the slope of the flight to the inner corner of riser and tread.
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
        /// The end of the walking line.
        /// </summary>
        public Vector3 End { get; protected set; }

        /// <summary>
        /// The start of the walking line.
        /// </summary>
        public Vector3 Start { get; protected set; }

        /// <summary>
        /// Create a stair flight based on the walkingLine.
        /// </summary>
        /// <param name="walkingLine">The walking line is represented by a line directed into the upward direction.</param>
        /// <param name="riserHeight">The vertical distance from tread to tread. The riser height is supposed to be equal for all steps of the stair flight.</param>
        /// <param name="treadLength">The horizontal distance from the front of the thread to the front of the next tread. The tread length is supposed to be equal for all steps of the stair flight at the walking line.</param>
        /// <param name="waistThickness">The minimum thickness of the stair flight, measured perpendicular to the slope of the flight to the inner corner of riser and tread.</param>
        /// <param name="flightWidth">The overall width of the stair flight.</param>
        /// <param name="material">The stair flight's material.</param>
        /// <param name="nosingLength">The horizontal distance from the front of the tread to the riser underneath. It is the overhang of the tread.</param>
        /// <param name="transform">The transform of the stair flight.
        /// This transform will be concatenated to the transform created to place the stair along its walking line.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the height of the riser is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the lenght of the tread is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the thickness of the waist is less than or equal to zero.</exception>
        public StairFlight(Line walkingLine, double riserHeight, double treadLength, double waistThickness, double flightWidth, Material material = null, double nosingLength = 0, Transform transform = null)
        {
            if (riserHeight <= 0.0)
            {
                throw new ArgumentOutOfRangeException($"The stairflight could not be created. The height of the riser provided, {riserHeight}, must be greater than 0.0.");
            }

            if (treadLength <= 0.0)
            {
                throw new ArgumentOutOfRangeException($"The stairflight could not be created. The lenght of the tread provided, {treadLength}, must be greater than 0.0.");
            }

            if (waistThickness <= 0.0)
            {
                throw new ArgumentOutOfRangeException($"The stairflight could not be created. The thickness of the waist provided, {treadLength}, must be greater than 0.0.");
            }

            this.WalkingLine = walkingLine;
            this.TreadLength = treadLength;
            this.NumberOfTreads = (int)Math.Floor(walkingLine.Length() / this.TreadLength);
            this.NumberOfRiser = this.NumberOfTreads;
            this.RiserHeight = riserHeight;
            this.WaistThickness = waistThickness;
            this.FlightWidth = flightWidth;
            this.NosingLength = nosingLength;
            this.Material = material == null ? BuiltInMaterials.Default : material;

            // Construct a transform whose X axis is the walking line of the stair.
            // The stair is described as if it's lying flat in the XY plane of that Transform.
            Vector3 d = WalkingLine.Direction();
            Vector3 z = d.Cross(Vector3.ZAxis);
            Transform stairFlightTransform = new Transform(WalkingLine.Start, d, z);
            this.Transform = stairFlightTransform;

            if (transform != null)
            {
                stairFlightTransform.Concatenate(transform);
            }

            this.Start = this.Transform.OfPoint(new Vector3());
            this.End = this.Transform.OfPoint(new Vector3());

            this.Profile = StraightStairFlightSection();
            this.ExtrudeDepth = this.FlightWidth;
            this.ExtrudeDirection = Vector3.ZAxis;

        }

        private Profile StraightStairFlightSection()
        {
            List<Vector3> stairFlightPoints = new List<Vector3>();

            Vector3 tread = new Vector3(this.TreadLength, 0, 0);
            Vector3 riser = new Vector3(0, this.RiserHeight, 0);

            for (int i = 0; i < this.NumberOfRiser; i++)
            {
                stairFlightPoints.Add(new Vector3(i * this.TreadLength + this.NosingLength, i * this.RiserHeight, 0));
                stairFlightPoints.Add(new Vector3(i * this.TreadLength , (i + 1) * this.RiserHeight, 0));
            }

            // Last step
            Vector3 lastStepPoint = new Vector3(this.NumberOfRiser * this.TreadLength, this.NumberOfRiser * this.RiserHeight, 0);
            stairFlightPoints.Add(lastStepPoint);

            this.End = this.Transform.OfPoint(lastStepPoint);

            // Run thickness
            Vector3 runThickness = (riser + tread).Cross(Vector3.ZAxis).Normalized() * this.WaistThickness;
            double alpha = runThickness.AngleTo(Vector3.YAxis.Negated());
            alpha = alpha * Math.PI / 180;
            Vector3 verticalThickness = (runThickness.Length() / Math.Cos(alpha)) * Vector3.YAxis.Negated();
            this._landingThickness = verticalThickness.Length();
            stairFlightPoints.Add(lastStepPoint + verticalThickness);

            alpha = (riser + tread).Negated().AngleTo(Vector3.XAxis);
            alpha = alpha * Math.PI / 180;
            Vector3 horizontalThickness = (runThickness.Length() / Math.Sin(alpha)) * Vector3.XAxis;
            this._baseThickness = horizontalThickness.Length();
            stairFlightPoints.Add(horizontalThickness);

            Polygon sectionPolygon = new Polygon(stairFlightPoints.ToArray()).Reversed();
            Profile sectionProfile = new Profile(sectionPolygon);

            return sectionProfile;
        }

        /// <summary>
        /// Calculate the height of the stair flight.
        /// </summary>
        public double Height()
        {
            return this.RiserHeight * this.NumberOfRiser;
        }

        private double _landingThickness;
        /// <summary>
        /// Calculate the thickness of an associate stair landing.
        /// </summary>
        public double LandingThickness()
        {
            return this._landingThickness;
        }

                private double _baseThickness;
        /// <summary>
        /// The thickness of the base surface of the stair, required to place the stair on a floor.
        /// </summary>
        public double BaseThickness()
        {
            return this._baseThickness;
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