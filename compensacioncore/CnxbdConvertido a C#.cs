
    // --------------------------------------------------
    // 0.1.>> Declaración de Variables
    // --------------------------------------------------
    private string CError;
    private ADODB.Connection ParamCnx = new ADODB.Connection();
    private ADODB.Recordset DataSalida = new ADODB.Recordset();
    private CAdminExcepcion AdmFallo = new CAdminExcepcion();
    public ADODB.Command EjeSP = new ADODB.Command();

    // --------------------------------------------------
    // 0.2.>> Declaración de Conexión
    // --------------------------------------------------
    private MSGFORMATLib.Des CCrypto = new MSGFORMATLib.Des();
    private string QUsuario;
    private string QClave;

    // Private Const GServidor = "10.1.99.20\tcredito"
    // Private Const QUsuario = "monitran"
    // Private Const QClave = "baustro201005"
    // --------------------------------------------------
    // 1.>> Cierra Conexión
    // --------------------------------------------------
    public void ConexionCierra()
    {
        AdmFallo.PuntoSistema("ConexionCierra", 3);
        ParamCnx.Close();
        DataSalida.Close();
    }
// --------------------------------------------------
// 2.>> Ejecución de Select
// --------------------------------------------------
public bool EjecutaSelectSQL(ref string QColumnas, ref string Qbase, ref string QSkema, ref string QTabla, ref string QInnerJoin, ref string QLeftJoin, ref string QCondicion, ref string QOrden, ref object QGenerar)
{
    object SentenciaArmada;
    AdmFallo.PuntoSistema("EjecutaSelectSQL", 3);
    if (DataSalida.State == ADODB.ObjectStateEnum.adStateOpen)
    {
        DataSalida.Close();
    }

    SentenciaArmada = ArmaQuerySQL("Select", QColumnas, Qbase, QSkema, QTabla, QInnerJoin, QLeftJoin, QCondicion, QOrden);
    CnxSQL02(GServidor, Qbase);
    if (DataSalida.State == ADODB.ObjectStateEnum.adStateClosed)
    {
        DataSalida.Open(SentenciaArmada, ParamCnx);
    }
   
    QGenerar = DataSalida.GetRows;
    return default;
}
// --------------------------------------------------
// 3.>> Conexion por DSN
// --------------------------------------------------
private bool CnxSQL01()
{
    var Inicia = default(object);
    var Pwd = default(object);
    var Usr = default(object);
    object Conecta_01;
    AdmFallo.PuntoSistema("CnxSQL01", 4);

    Conecta_01 = false;
    ;
    ParamCnx = null;
    ParamCnx = new ADODB.Connection();
    ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
    /* On Error GoTo 0

     */
    ParamCnx.Open("FILEDSN=OCTOCLI.dsn", Usr, Pwd);
    if (Information.Err().Number != 0)
    {
        Inicia.ErrorCodigo = Information.Err().Number;
        Inicia.ErrorDescribe = Information.Err().Description;

        // Call bsLogger.ErroresLog("W", "AUTH.LOCAL", "OpenConn:No se pudo conectar a Base OCTOCLI")
        return default;
    };

    Conecta_01 = true;
    return default;
}
// --------------------------------------------------
// 4.>> Conexion por APP
// --------------------------------------------------
private bool CnxSQL02(ref string CServidor, ref string CBase)
{
    bool CnxSQL02Ret = default;
    object cb_servidor;
    object cb_cnxapp;
    object cb_cnxbase;
    object cb_contrse;
    object cb_Usuario;
    object cb_Drivers;
    string SentenciaCnx;
    var BaseDsn = default(string);
    AdmFallo.PuntoSistema("CnxSQL02", 4);
    CnxSQL02Ret = false;
    if (ParamCnx.State == ADODB.ObjectStateEnum.adStateOpen)
    {
        ParamCnx.Close();
    }

    switch (CBase ?? "")
    {
        case "AS_BATCHPCI":
            {
                BaseDsn = "AS_BATCH"; // GB
                break;
            }

        case "octopus_pci":
            {
                BaseDsn = "AS_BATCH"; // GB 2018
                break;
            }
    }

    CCrypto.GetFileDSNUsrPass(BaseDsn, QUsuario, QClave);
    QUsuario = "usrcompensacion";
    QClave = "desa2014*";

    cb_Drivers = "DRIVER=SQL Server;";
    cb_Usuario = "USER ID=" + QUsuario + "; ";
    cb_contrse = "pwd=" + QClave + ";";
    cb_cnxbase = "Initial Catalog=" + CBase + ";";
    cb_cnxapp = "APP=Microsoft Open Database Connectivity;";
    cb_servidor = "SERVER=" + CServidor;
    SentenciaCnx = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(cb_Drivers, cb_Usuario), cb_contrse), cb_cnxbase), cb_cnxapp), cb_servidor));
    if (ParamCnx.State == ADODB.ObjectStateEnum.adStateClosed)
    {
        ParamCnx.ConnectionTimeout = GTiempoFuera;
        ParamCnx.CommandTimeout = GTiempoFuera;
        ParamCnx.Open(SentenciaCnx);
    }

    CnxSQL02Ret = true;
    return CnxSQL02Ret;
}
 // Install-Package Microsoft.VisualBasic

internal partial class SurroundingClass
{
    // ------------------------------------------------------------------------------------------
    // 5.>> Arma el Query ya se este un select, update, insert o Delete
    // ------------------------------------------------------------------------------------------
    private string ArmaQuerySQL(ref string Tipo, ref string QColumnas, ref string Qbase, ref string QSkema, ref string QTabla, ref string QInnerJoin, ref string QLeftJoin, ref string QCondicion, ref string QOrden)
    {
        string ArmaQuerySQLRet = default;
        var Sentencia = default(string);
        AdmFallo.PuntoSistema("ArmaQuerySQL", 4);
        if (Tipo == "Select")
        {
            Sentencia = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("Select " + QColumnas + "  from " + Qbase + "." + QSkema + "." + QTabla, Interaction.IIf(string.IsNullOrEmpty(QInnerJoin), "", " inner join " + QInnerJoin)), Interaction.IIf(string.IsNullOrEmpty(QLeftJoin), "", " left outer join " + QLeftJoin)), " Where "), QCondicion), Interaction.IIf(string.IsNullOrEmpty(QOrden), "", " Order by " + QOrden)));
        }

        ArmaQuerySQLRet = Sentencia;
        return ArmaQuerySQLRet;
    }

    // --------------------------------------------------
    // 6.>> Ejecución del SP
    // --------------------------------------------------
    public bool EjecutaSP(ref string Qbase, ref string Qsp, ref string QMensaje, ref object DReporta)
    {
        bool EjecutaSPRet = default;
        var TiempoFuera = default(object);
        AdmFallo.PuntoSistema("EjecutaSP", 3);
        EjecutaSPRet = false;
        if (DataSalida.State == ADODB.ObjectStateEnum.adStateOpen)
        {
            DataSalida.Close();
        };
        /* On Error GoTo Determina
         */
        CnxSQL02(GServidor, Qbase);
        EjeSP.let_ActiveConnection(ParamCnx);
        EjeSP.CommandType = ADODB.CommandTypeEnum.adCmdStoredProc;
        EjeSP.CommandText = Qsp;
        EjeSP.CommandTimeout = TiempoFuera;
        DataSalida = EjeSP.Execute;
        if (!DataSalida.EOF)
        {
            DReporta = DataSalida.GetRows;
            EjecutaSPRet = true;
        }
        else
        {
            AdmFallo.GeneraLogDetalle("No existe Data para " + QMensaje);
        }

    Determina:
        ;
        if (Information.Err().Number != 0)
            AdmFallo.GeneraLogDetalle(Information.Err().Description);
        if (DataSalida.State == ADODB.ObjectStateEnum.adStateOpen)
        {
            DataSalida.Close();
        }

        return EjecutaSPRet;
    }

    // --------------------------------------------------------------------------------------------------------------
    // 7.>> Procedimiento que Agrega los parámetros con los que se va a ejecutar el SP
    // --------------------------------------------------------------------------------------------------------------
    public void QParametros(ref string PNombre, ref ADODB.DataTypeEnum PTipoDato, ref ADODB.ParameterDirectionEnum PDirection, ref object PData)
    {
        bool ConTamano;
        int PTamano;
        ConTamano = true;
        AdmFallo.PuntoSistema("QParametros", 3);
        switch (PTipoDato)
        {
            case var @case when @case == ADODB.DataTypeEnum.adChar:
                {
                    ConTamano = true;
                    break;
                }

            case var case1 when case1 == ADODB.DataTypeEnum.adVarChar:
                {
                    ConTamano = true;
                    break;
                }

            default:
                {
                    ConTamano = false;
                    break;
                }
        }

        if (ConTamano)
        {
            if (Information.IsDBNull(PData))
            {
                PTamano = 1;
            }
            else
            {
                PTamano = Strings.Len(PData);
            }

            EjeSP.Parameters.Append(EjeSP.CreateParameter(PNombre, PTipoDato, PDirection, PTamano, PData));
        }
        else
        {
            EjeSP.Parameters.Append(EjeSP.CreateParameter(PNombre, PTipoDato, PDirection, 0, PData));
        }
    }

    // --------------------------------------------------------------------------------------------------------------
    // 8.>> Procedimiento que Agrega los parámetros con los que se va a ejecutar el SP
    // --------------------------------------------------------------------------------------------------------------
    public void DepuraParametros()
    {
        short Ind;
        AdmFallo.PuntoSistema("DepuraParametros", 3);
        var loopTo = EjeSP.Parameters.Count - 1;
        for (Ind = 0; Ind <= loopTo; Ind++)
            EjeSP.Parameters.Delete(0);
    }

