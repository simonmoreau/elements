using Elements.Geometry;
using Elements.Interfaces;
using Elements.Geometry.Interfaces;
using Newtonsoft.Json;
using Hypar.Elements.Interfaces;
using Elements.Geometry.Solids;
using System.Collections.Generic;

namespace Elements
{
    /// <summary>
    /// A floor is a horizontal element defined by a perimeter and one or several voids.
    /// </summary>
    public class Floor : Element, IElementType<FloorType>, ISolid, IExtrude, IHasOpenings
    {
        /// <summary>
        /// The elevation from which the floor is extruded.
        /// </summary>
        public double Elevation { get; }

        /// <summary>
        /// The floor type of the floor.
        /// </summary>
        public FloorType ElementType { get; }

        /// <summary>
        /// The untransformed profile of the floor.
        /// </summary>
        public Profile Profile { get; }

        /// <summary>
        /// The floor's geometry.
        /// </summary>
        public Solid Geometry { get; }

        /// <summary>
        /// The openings in the floor.
        /// </summary>
        public List<Opening> Openings { get; }

        /// <summary>
        /// The extrude direction of the floor.
        /// </summary>
        public Vector3 ExtrudeDirection {get;}

        /// <summary>
        /// The extrude depth of the floor.
        /// </summary>
        public double ExtrudeDepth => this.Thickness();

        /// <summary>
        /// Extrude to both sides?
        /// </summary>
        public bool BothSides => false;

        /// <summary>
        /// Create a floor.
        /// </summary>
        /// <param name="profile">The profile of the floor.</param>
        /// <param name="elementType">The floor type of the floor.</param>
        /// <param name="elevation">The elevation of the top of the floor.</param>
        /// <param name="transform">The floor's transform. If set, this will override the floor's elevation.</param>
        /// <param name="openings">An array of openings in the floor.</param>
        public Floor(Polygon profile, FloorType elementType, double elevation = 0.0, Transform transform = null, List<Opening> openings = null)
        {
            this.Profile = new Profile(profile);
            this.Openings = openings != null ? openings : new List<Opening>();
            this.Elevation = elevation;
            this.ElementType = elementType;
            var thickness = elementType.Thickness();
            this.Transform = transform != null ? transform : new Transform(new Vector3(0, 0, elevation));
            this.ExtrudeDirection = Vector3.ZAxis;
        }

        /// <summary>
        /// Create a floor.
        /// </summary>
        /// <param name="profile">The profile of the floor.</param>
        /// <param name="start">A tranform used to pre-transform the profile and direction vector before sweeping the geometry.</param>
        /// <param name="direction">The direction of the floor's sweep.</param>
        /// <param name="elementType">The floor type of the floor.</param>
        /// <param name="elevation">The elevation of the floor.</param>
        /// <param name="transform">The floor's transform. If set, this will override the elevation.</param>
        public Floor(Profile profile, Transform start, Vector3 direction, FloorType elementType, double elevation = 0.0, Transform transform = null)
        {
            // this.Profile = new Profile(profile);
            this.Elevation = elevation;
            this.ElementType = elementType;
            this.Transform = transform != null ? transform : new Transform(new Vector3(0, 0, elevation));
            this.Profile = start.OfProfile(profile);
            this.ExtrudeDirection = start.OfVector(direction);
        }

        [JsonConstructor]
        internal Floor(Profile profile, FloorType elementType, double elevation = 0.0, Transform transform = null, List<Opening> openings = null)
        {
            this.Profile = profile;
            this.Openings = openings != null ? openings : new List<Opening>();
            this.Elevation = elevation;
            this.ElementType = elementType;
            var thickness = elementType.Thickness();
            this.Transform = transform != null ? transform : new Transform(new Vector3(0, 0, elevation));
            this.ExtrudeDirection = Vector3.ZAxis;
        }

        /// <summary>
        /// The area of the floor.
        /// </summary>
        /// <returns>The area of the floor, not including the area of openings.</returns>
        public double Area()
        {
            return this.Profile.Area();
        }

        /// <summary>
        /// Get the profile of the floor transformed by the floor's transform.
        /// </summary>
        public Profile ProfileTransformed()
        {
            return this.Transform != null ? this.Transform.OfProfile(this.Profile) : this.Profile;
        }

        /// <summary>
        /// Calculate thickness of the floor's extrusion.
        /// </summary>
        public double Thickness()
        {
            return this.ElementType.Thickness();
        }
    }
}