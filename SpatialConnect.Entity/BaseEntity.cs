using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace SpatialConnect.Entity
{
    public abstract class BaseEntity
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty("py/object")]
        public string py_object { get; set; }

        public void Write(string path)
        {
            string json = JsonConvert.SerializeObject(this);

            while (IsFileOpen(path))
            {
                Thread.Sleep(1000);
            }

            File.WriteAllText(path, json);
        }

        public bool IsFileOpen(string path)
        {
            FileStream stream = null;

            try 
            {
                stream = File.OpenRead(path);
            }
            catch (IOException ex)
            {
                _log.Debug("file locked, will retry (File: " + path + ")");

                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }
    }
}
