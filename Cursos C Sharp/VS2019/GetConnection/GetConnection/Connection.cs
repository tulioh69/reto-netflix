using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GetConnection
{
    public class Connection
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DB { get; set; }
        private string NameConnection;
      
        public  Connection(string NameConnection= "ParametrosEntities")
        {
            this.NameConnection = NameConnection;
        }
        public void GetData()
        {
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            if (connections.Count > 0)
            {
                foreach (ConnectionStringSettings connection in connections)
                {
                    string name = connection.Name;
                    string provider = connection.ProviderName;
                    string connectionString = connection.ConnectionString;

                    if(name.Equals(NameConnection))
                    {
                        connectionString = connectionString.Split('\"')[1];
                        string[] arrElementos = connectionString.Split(';');
                        foreach (var s in arrElementos)
                        {
                            string[] arrValue = s.Split('=');
                            switch (arrValue[0].ToUpper())
                            {
                                case "DATA SOURCE":
                                    Server = arrValue[1];
                                    break;
                                case "USER ID":
                                    User = arrValue[1];
                                    break;
                                case "PASSWORD":
                                    Password = arrValue[1];
                                    break;
                                case "INITIAL CATALOG":
                                    DB = arrValue[1];
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("No existen cadenas de conexion");
            }
        }
    }
}
