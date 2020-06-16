using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AutoClicker
{
    class DataHandler
    {
        //save list method
        public static bool SaveList<T>(string filepath, List<T> Objects)
        {
            try
            {
                using (Stream s = File.Open(filepath, FileMode.Create))
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    binForm.Serialize(s, Objects);
                }
            }
            catch (Exception e)
            {
                // error saving data
                Console.WriteLine("Error saving list: " + e.ToString());
                return false;
            }
            //data saved
            return true;
        }
        // Load list method, loads data from serialised file
        public static List<T> LoadList<T>(string FilePath)
        {
            List<T> returnList = new List<T>();
            try
            {
                using (Stream s = File.Open(FilePath, FileMode.Open))
                {
                    BinaryFormatter binRead = new BinaryFormatter();
                    returnList = (List<T>)binRead.Deserialize(s);
                    return returnList;
                }
            }
            catch (Exception e)
            {
                // show errors in console
                Console.WriteLine("Error reading the file" + e.ToString());
                return null;
            }
        }
    }
}
