using System.IO;
using Newtonsoft.Json;

namespace UserManagementService
{
    /// <summary>
    /// Helper class for serialising and deserialising data
    /// </summary>
    internal static class Serialiser
    {
        /// <summary>
        /// Deserialise a JSON object from a stream
        /// </summary>
        public static T DeserialiseObject<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var str = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(str);
            }
        }

        /// <summary>
        /// Deserialise a JSON object from a string
        /// </summary>
        public static T DeserialiseObject<T>(string payload)
        {
            return JsonConvert.DeserializeObject<T>(payload);
        }

        /// <summary>
        /// Serialise an object into a stream
        /// </summary>
        public static void SerialiseObject(object obj, Stream stream)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                var serialiser = new JsonSerializer();
                serialiser.Serialize(streamWriter, obj);
            }
        }

        /// <summary>
        /// Serialise an object into an array
        /// </summary>
        public static void SerialiseObject<T>(T obj, byte[] arrSerialised)
        {
            using (var memoryStream = new MemoryStream())
            {
                SerialiseObject(obj, memoryStream);
                arrSerialised = memoryStream.ToArray();
            }
        }
    }
}
