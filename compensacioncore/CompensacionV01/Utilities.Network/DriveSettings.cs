using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Network
{
    public class DriveSettings
    {
        private enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }
        private enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }
        private enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010
        }
        private enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct NETRESOURCE
        {
            public ResourceScope oResourceScope;
            public ResourceType oResourceType;
            public ResourceDisplayType oDisplayType;
            public ResourceUsage oResourceUsage;
            public string sLocalName;
            public string sRemoteName;
            public string sComments;
            public string sProvider;
        }
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(ref NETRESOURCE oNetworkResource, string sPassword, string sUserName, int iFlags);
        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string sLocalName, uint iFlags, int iForce);
        public static void MapNetworkDrive(string sNetworkPath, string sDriveLetter, string user, string password, ref bool successConnection, ref int codigoSalida, ref string msj)
        {
            msj = "";
            successConnection = false;
            //Checks if the last character is  as this causes error on mapping a drive.
            if (sNetworkPath.Substring(sNetworkPath.Length - 1, 1) == @"")
            {
                sNetworkPath = sNetworkPath.Substring(0, sNetworkPath.Length - 1);
            }

            NETRESOURCE oNetworkResource = new NETRESOURCE();
            oNetworkResource.oResourceType = ResourceType.RESOURCETYPE_DISK;
            oNetworkResource.sLocalName = sDriveLetter + ":";
            oNetworkResource.sRemoteName = sNetworkPath;

            //If Drive is already mapped disconnect the current mapping before adding the new mapping
            if (IsDriveMapped(sDriveLetter))
            {
                msj += " Drive is already mapped. Disconnecting the current mapping before adding the new mapping, ";
                bool succDis = false;
                DisconnectNetworkDrive(sDriveLetter, true, ref succDis, ref msj);
            }

            int result = WNetAddConnection2(ref oNetworkResource, password, user, 0);

            if (result == 0) { msj += " Mapeo Unidad Red: La unidad de red se mapeó correctamente."; successConnection = true; }
            else if (result == 53) { msj += " Mapeo Unidad Red: ERROR_BAD_NETPATH."; Exception e = new Exception(msj); throw e; }
            else if (result == 54) { msj += " Mapeo Unidad Red: ERROR_NETWORK_BUSY."; Exception e = new Exception(msj); throw e; }
            else if (result == 65) { msj += " Mapeo Unidad Red: ERROR_NETWORK_ACCESS_DENIED."; Exception e = new Exception(msj); throw e; }
            else if (result == 66) { msj += " Mapeo Unidad Red: The network resource type is not correct."; Exception e = new Exception(msj); throw e; }
            else if (result == 67) { msj += " Mapeo Unidad Red: (ERROR_BAD_NET_NAME) - The remote path cannot be found.This can be an intermittent network issue or a bad path."; Exception e = new Exception(msj); throw e; }
            else if (result == 85) { msj += " Mapeo Unidad Red: La unidad de red ya está mapeada." + " ERROR_ALREADY_ASSIGNED 85. The local device name is already in use."; successConnection = true; }
            else if (result == 86) { msj += " Mapeo Unidad Red: ERROR_INVALID_PASSWORD."; Exception e = new Exception(msj); throw e; }
            else if (result == 1200) { msj += " Mapeo Unidad Red: ERROR_BAD_DEVICE 1200. The specified device name is invalid."; Exception e = new Exception(msj); throw e; }
            else if (result == 1201) { msj += " Mapeo Unidad Red: (ERROR_CONNECT_UNAVAIL) - This indicates the mapping is available but idled out."; Exception e = new Exception(msj); throw e; }
            else if (result == 1219)
            {
                /*
                 * 1219 (ERROR_SESSION_CREDENTIAL_CONFLICT) - This indicates you are trying to connect to the same UNC path with different credentials. Windows doesn't like this and will require that you drop any connections that aren't using the same credentials. You see this in Explorer periodically if you have lingering connections.
                 */
                msj = " Mapeo Unidad Red: No se pudo mapear la unidad de red. Demasiadas conexiones abiertas al recurso de red."; Exception e = new Exception(msj); throw e;
            }
            else if (result == 1326) { msj += " Mapeo Unidad Red: No se pudo mapear la unidad de red. Revise el archivo de configuración. Error de usuario/contraseña."; Exception e = new Exception(msj); throw e; }
            else if (result == 2202) { msj += " Mapeo Unidad Red: ERROR_BAD_USERNAME"; Exception e = new Exception(msj); throw e; }
            else { msj += " Mapeo Unidad Red: No se pudo mapear la unidad de red. Error desconocido. Error " + result + " de WNetAddConnection2"; Exception e = new Exception(msj); throw e; }
            codigoSalida = result;
        }
        public static int DisconnectNetworkDrive(string sDriveLetter, bool bForceDisconnect, ref bool successDisconnected, ref string msj)
        {
            int res = -1;
            successDisconnected = false;
            if (bForceDisconnect)
            {
                res = WNetCancelConnection2(sDriveLetter + ":", 0, 1);
                if (res == 0)
                {
                    msj = " Desconeccion Forsoza Unidad Red " + sDriveLetter + ": satisfactoria.";
                    successDisconnected = true;
                }
                else
                {
                    msj = " FALLO desconeccion Forsoza de la unidad de red. "+ sDriveLetter ;
                    successDisconnected = false;
                }
                return res;
            }
            else
            {
                res = WNetCancelConnection2(sDriveLetter + ":", 0, 0);
                if (res == 0)
                {
                    msj = " Desconeccion Unidad Red " + sDriveLetter + ": satisfactoria.";
                    successDisconnected = true;
                }
                else
                {
                    msj = " FALLO desconeccion de la unidad de red. " + sDriveLetter;
                    successDisconnected = false;
                }
                return res;
            }
        }
        public static bool IsDriveMapped(string sDriveLetter)
        {
            string[] DriveList = Environment.GetLogicalDrives();
            for (int i = 0; i < DriveList.Length; i++)
            {
                if (sDriveLetter + ":\\" == DriveList[i].ToString())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
