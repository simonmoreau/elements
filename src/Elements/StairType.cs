using Elements.Interfaces;
using Newtonsoft.Json;
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
        /// Construct a stair type.
        /// </summary>
        /// <param name="name">The name of the stair type.</param>
        /// <param name="material">The material used by the stair type.</param>
        public StairType(string name, Material material) : base(name)
        {
            this.Material = material;
        }
    }
}