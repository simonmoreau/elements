using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Elements.Serialization.JSON
{
    /// <summary>
    /// Converter for objects of ElementType.
    /// </summary>
    public class ElementTypeToIdConverter : JsonConverter
    {
        private Dictionary<long, ElementType> _elementTypes;

        /// <summary>
        /// Construct an ElementTypeConverter.
        /// </summary>
        /// <param name="elementTypes"></param>
        public ElementTypeToIdConverter(Dictionary<long, ElementType> elementTypes)
        {
            this._elementTypes = elementTypes;
        }

        /// <summary>
        /// Can this converter convert and object of type objectType?
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(ElementType).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Read json.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var id = (long)reader.Value;
            if(this._elementTypes.ContainsKey(id))
            {
                return this._elementTypes[id]; 
            }
            else
            {
                throw new Exception($"The specified ElementType, {id}, cannot be found.");
            }
        }

        /// <summary>
        /// Write json.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var et = (ElementType)value;
            writer.WriteValue(et.Id);
        }
    }
}