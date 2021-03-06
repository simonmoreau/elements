using Elements.Geometry;
using Elements.Interfaces;
using Elements.Geometry.Interfaces;
using Newtonsoft.Json;
using Hypar.Elements.Interfaces;
using Elements.Geometry.Solids;

namespace Elements
{
    /// <summary>
    /// A floor is a horizontal element defined by a perimeter and one or several voids.
    /// </summary>
    public class Floor : Element, IElementType<FloorType>, IGeometry3D, IProfile
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
        public Solid[] Geometry { get; }

        /// <summary>
        /// The openings in the floor.
        /// </summary>
        public Opening[] Openings{get;}

        /// <summary>
        /// Create a Floor.
        /// </summary>
        /// <param name="profile">The profile of the floor.</param>
        /// <param name="elementType">The floor type of the floor.</param>
        /// <param name="elevation">The elevation of the top of the floor.</param>
        /// <param name="material">The floor's material.</param>
        /// <param name="transform">The floor's transform.</param>
        /// <returns>A floor.</returns>
        [JsonConstructor]
        public Floor(Profile profile, FloorType elementType, double elevation = 0.0, Material material = null, Transform transform = null)
        {
            this.Profile = profile;
            this.Elevation = elevation;
            this.ElementType = elementType;
            var thickness = elementType.Thickness();
            this.Transform = transform != null ? transform : new Transform(new Vector3(0, 0, elevation - thickness));
            this.Geometry = new[]{Solid.SweepFace(this.Profile.Perimeter, this.Profile.Voids, thickness)};
        }

        /// <summary>
        /// Create a floor.
        /// </summary>
        /// <param name="profile">The profile of the floor.</param>
        /// <param name="elementType">The floor type of the floor.</param>
        /// <param name="elevation">The elevation of the top of the floor.</param>
        /// <param name="material">The floor's material.</param>
        /// <param name="transform">The floor's transform. If set, this will override the floor's elevation.</param>
        /// <param name="openings">An array of openings in the floor.</param>
        public Floor(Polygon profile, FloorType elementType, double elevation = 0.0, Material material = null, Transform transform = null, Opening[] openings = null)
        {
            if (openings != null && openings.Length > 0)
            {
                var voids = new Polygon[openings.Length];
                for (var i = 0; i < voids.Length; i++)
                {
                    var o = openings[i];
                    voids[i] = o.Perimeter;
                }
                this.Profile = new Profile(profile, voids);
            }
            else
            {
                this.Profile = new Profile(profile);
            }

            this.Openings = openings;
            this.Elevation = elevation;
            this.ElementType = elementType;
            var thickness = elementType.Thickness();
            this.Transform = transform != null ? transform : new Transform(new Vector3(0, 0, elevation - thickness));
            this.Geometry = new[]{Solid.SweepFace(this.Profile.Perimeter, this.Profile.Voids, thickness)};
        }

        /// <summary>
        /// Create a floor.
        /// </summary>
        /// <param name="profile">The profile of the floor.</param>
        /// <param name="start">A tranforms used to pre-transform the profile and direction vector before sweeping the geometry.</param>
        /// <param name="direction">The direction of the floor's sweep.</param>
        /// <param name="elementType">The floor type of the floor.</param>
        /// <param name="elevation">The elevation of the floor.</param>
        /// <param name="material">The floor's material.</param>
        /// <param name="transform">The floor's transform. If set, this will override the elevation.</param>
        public Floor(Profile profile, Transform start, Vector3 direction, FloorType elementType, double elevation = 0.0, Material material = null, Transform transform = null)
        {
            this.Profile = profile;
            this.Elevation = elevation;
            this.ElementType = elementType;
            this.Transform = transform != null ? transform : new Transform(new Vector3(0, 0, elevation));
            var outer = start.OfPolygon(this.Profile.Perimeter);
            var inner = this.Profile.Voids != null ? start.OfPolygons(this.Profile.Voids) : null;
            this.Geometry = new[]{Solid.SweepFace(outer, inner, start.OfVector(direction), this.Thickness())};
        }
        
        /// <summary>
        /// The area of the floor.
        /// Overlapping openings and openings which are outside of the floor's perimeter,
        /// will result in incorrect area results.
        /// </summary>
        /// <returns>The area of the floor.</returns>
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