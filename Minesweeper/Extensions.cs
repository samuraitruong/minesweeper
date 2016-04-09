using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Xml.Serialization;

namespace Minesweeper
{
    public static class Extensions
    {
        public static string SerializeAsXml(this object obj)
      {
          var type = obj.GetType();
          var xmlSerializer = new XmlSerializer(type);
 
          using (var memStream = new MemoryStream())
          {
              xmlSerializer.Serialize(memStream, obj);
              return Encoding.Default.GetString(memStream.ToArray());
          }
      }
        public static T DeserializeAsXml<T>(this string xmlString)
     {
         var xmlSerializer = new XmlSerializer(typeof(T));
         using (var memStream = new MemoryStream(Encoding.Default.GetBytes(xmlString)))
         {
             return (T)xmlSerializer.Deserialize(memStream);
         }
     }

    



}
}