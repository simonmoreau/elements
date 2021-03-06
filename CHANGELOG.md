# Changelog

## [0.2.3]
### Added
- `MaterialLayer`
- `StructuralFramingType` - `StructuralFramingType` combines a `Profile` and a `Material` to define a type for framing elements.

### Changed
- `IProfileProvider` is now `IProfile`
- `IElementTypeProvider` is now `IElementType`
- All structural framing type constructors now take a `StructuralFramingType` in place of a `Profile` and a `Material`.
- All properties serialize to JSON using camel case.
- Many expensive properties were converted to methods.
- A constructor has been added to `WallType` that takes a collection of `MaterialLayer`.

## [0.2.2]
### Added
- `Matrix.Determinant()`
- `Matrix.Inverse()`
- `Transform.Invert()`
- `Model.ToIFC()`
- `Elements.Serialization.JSON` namespace.
- `Elements.Serialization.IFC` namespace.
- `Elements.Serialization.glTF` namespace.

### Changed
- Wall constructor which uses a center line can now have a Transform specified.
- `Profile.ComputeTransform()` now finds the first 3 non-collinear points for calculating its plane. Previously, this function would break when two of the first three vertices were co-linear.
- Using Hypar.IFC2X3 for interaction with IFC.
- `Line.Thicken()` now throws an exception when the line does not have the same elevation for both end points.
- `Model.SaveGlb()` is now `Model.ToGlTF()`.

## [0.2.1]
### Added
- The `Topography` class has been added.
- `Transform.OfPoint(Vector3 vector)` has been added to transform a vector as a point with translation. This was previously `Transform.OfVector(Vector3 vector)`. All sites previously using `OfVector(...)` are now using `OfPoint(...)`.
- `Material.DoubleSided`
- `Loop.InsertEdgeAfter()`
- `Solid.Slice()`
- `Model.Extensions`
### Changed
- `Transform.OfVector(Vector3 vector)` now does proper vector transformation without translation.
- Attempting to construct a `Vector3` with NaN or Infinite arguments will throw an `ArgumentOutOfRangeException`.


## [0.2.0]
### Added
- IFC implementation has begun with `Model.FromIFC(...)`. Support includes reading of Walls, Slabs, Spaces, Beams, and Columns. Brep booleans required for Wall and Slab openings are not yet supported and are instead converted to Polygon openings in Wall and Floor profiles.
- The `Elements.Geometry.Profiles` namespace has been added. All profile servers can now be found here.
- The `Elements.Geometry.Solids` namespace has been added.
- The Frame type has been added to represent a continuous extrusion of a profile around a polygonal perimeter.
- The `ModelTest` base class has been added. Inheriting from this test class enables a test to automatically write its `Model` to a `.glb` file and to serialize and deserialize to/from JSON to ensure the stability of serialization.
- The `Solid.ToGlb` extension method has been added to enable serializing one `Solid` to glTF for testing.
### Changed
- Element identifiers are now of type `long`.
- Breps have been re-implemented in the `Solid` class. Currently only planar trimmed faces are supported.
- Many improvements to JSON serialization have been added, including the ability to serialize breps.
- '{element}.AddParameter' has been renamed to '{element}.AddProperty'.
- The `Hypar.Geometry` namespace is now `Elements.Geometry`.
- The `Hypar.Elements` namespace is now `Elements`.
### Removed
- The `IProfile` interface has been removed.
- The `Extrusion` class and `IBrep` have been replaced with the `Solid` class. The IGeometry interface now returns a `Solid[]`.
- Many uses of `System.Linq` have been removed. 
- Many uses of `IEnumerable<T>` have been replaced with `T[]`.
