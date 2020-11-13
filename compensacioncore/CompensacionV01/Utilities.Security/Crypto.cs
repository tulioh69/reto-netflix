using CryptoConection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Security
{
    public class Crypto
    {
        private string k { get; set; }
        private string vi { get; set; }
        public Crypto() { }
        public Crypto(string k, string vi)
        {
            this.k = k;
            this.vi = vi;
        }
        private void validateCryptoConfig()
        {
            string msj = "";
            if (k == null)
            {
                msj = "ERROR Parametro de configuracion requerido no establecido: llave";
                Exception e = new Exception(msj);
                throw e;
            }
            if (vi == null)
            {
                msj = "ERROR Parametro de configuracion requerido no establecido: VI";
                Exception e = new Exception(msj);
                throw e;
            }
            if (k != null && k.Length == 0){msj = "ERROR Parametro de configuracion requerido vacio: llave";Exception e = new Exception(msj);throw e;}
            if (vi != null && vi.Length == 0){msj = "ERROR Parametro de configuracion requerido vacio: VI";Exception e = new Exception(msj);throw e;}
        }
        private string cryptAES256CBC(bool cifrar, string s, ref string msj)
        {
            validateCryptoConfig();
            string ret = "";
            EncryptionClass ec = new EncryptionClass();
            if (cifrar) // cifrar
            {
                ret = ec.EncryptionAes256_CBC(s, k, vi);
                msj = "Cifrado correcto con AES256CBC";
            }
            else // descrifrar
            {
                ret = ec.DesencryptionAes256_CBC(s, k, vi);
                msj = "Descifrado correcto con AES256CBC";
            }
            return ret;
        }
    }
}
