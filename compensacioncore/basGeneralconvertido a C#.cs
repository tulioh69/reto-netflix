

{
    // VARIABLES PARA PARAMETROS DE ENVIO DE TRANSACCIONES
    public short piMinutos; // TIEMPO EN QUE SE DEBE
    public string GProceso; // Codigo de Proceso representado por la C y la cantidad de segundos transcurridos en el día
    private CAdminExcepcion AdmFallo = new CAdminExcepcion();
    // --------------------------------------------------
    // Variables para el Generales
    // --------------------------------------------------
    public string GRutaExe;
    public string GFileIni_In;
    public string GFileIniOut;
    public short GTiempoFuera;
    public string GServidor;
    public string GRutaLogs;
    public string GRutaCargas;
    public string GRutaReport;
    public short GHoraProcesa;
    public short GMinutoMaxProc;
    public DateTime GReproccoopfecha;
    public string GReproccooprazon;
    public DateTime GReprocbrdfecha;
    public string GReprocbrdrazon;
    public bool DS1;
    public bool DS2;
    public bool DS3;
    public bool DS4;
    public bool DS5;
    public bool DS6;
    public bool DS7;
   // --------------------------------------------------
    // Constantes por defectos a las Variables generales
    // --------------------------------------------------
    public const short DTiempoFuera = 1000;
    public const string DRutaLogs = @"D:\ExtremeLog";
    // Public Const DRutaCargas     As String = "'\\10.16.0.28\Archivos\Files_Brd\" GB
    // modificar ruta para grabar logs
    public const string DRutaCargas = @"'\\10.1.99.250\Archivos\Files_BRD\";
    // Public Const DRutaCargas     As String = "'\\10.1.115.25\Archivos\Files_Brd\"
    // modificar ruta de la carpeta que contiene los archivos a cargar
    public const string DRutaReport = @"D:\Reportes_comp\Compensacion\";
    // modificar la ruta para los reportes
    public const short DHoraProcesa = 8;
    public const short DMinutoMaxProc = 10;
    public static DateTime DFchReproc = DateTime.Parse("1978-03-31");


    // --------------------------------------------------
    // Variables para el CGeneraArchio
    // --------------------------------------------------
    public bool DirectorioFechaCreado;
    public string PuntoSistemaNivel1;
    public string PuntoSistemaNivel2;
    public string PuntoSistemaNivel3;
    public string PuntoSistemaNivel4;
    public string PuntoSistemaNivel5;

    // --------------------------------------------------
    // Constantes Generales
    // --------------------------------------------------
    public const string QVersion = "0.0.1";
    // Public Const GRutaLogs = "C:\ExtremeLog"
    // Public Const GRutaCargas = "'D:\Files_Brd\"
    // Public Const RutaReport = "D:\Reportes_comp\Compensacion\"

    // -------------------------------------------------------------------------
    // Constantes para adicionar los parametros de una ejecución SP
    // -------------------------------------------------------------------------

    public ADODB.DataTypeEnum aEntero = ADODB.DataTypeEnum.adInteger;
    public ADODB.DataTypeEnum aECorto = ADODB.DataTypeEnum.adSmallInt;
    public ADODB.DataTypeEnum aPFecha = ADODB.DataTypeEnum.adDate;
    public ADODB.DataTypeEnum aCaract = ADODB.DataTypeEnum.adChar;
    public ADODB.DataTypeEnum aVCarac = ADODB.DataTypeEnum.adVarChar;
    public ADODB.ParameterDirectionEnum aIngres = ADODB.ParameterDirectionEnum.adParamInput;
    // UPGRADE_NOTE: aSalida ha cambiado de Constant a Variable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C54B49D7-5804-4D48-834B-B3D81E4C2F13"'
    public ADODB.ParameterDirectionEnum aSalida = ADODB.ParameterDirectionEnum.adParamOutput;


    // --------------------------------------------------------'
    // Procedimiento que carga los datos principales para el  '
    // sistema desde el archivo de inicialización .INI        '
    // --------------------------------------------------------'
public void Trae_ini()
{
    int rc, Locale;
    string Linea, Separador;
    object NomFecha;
    short Paso;
    AdmFallo.PuntoSistema("Trae_ini", 2);
    ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
    /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorIni' at character 199


    Input:

            On Error GoTo ErrorIni

     */
    Paso = (short)1;
    // UPGRADE_WARNING: App propiedad App.EXEName tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
    GFileIni_In = FileSystem.CurDir() + @"\" + global::My.Application.Info.AssemblyName + ".ini";
    // UPGRADE_WARNING: Dir tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
    if (string.IsNullOrEmpty(FileSystem.Dir(GFileIni_In)))
    {
        Linea = "No existe el archivo : " + GFileIni_In;
        AdmFallo.GeneraLogDetalle(Linea);
        Environment.Exit(0);
    }

    Paso = 2;
    GTiempoFuera = Val(Lee_ini("TIMEOUT", GFileIni_In));
    Paso = 3;
    GServidor = Lee_ini("SERVBD", GFileIni_In);
    Paso = 4;
    GRutaLogs = Lee_ini("RUTALOGS", GFileIni_In);
    if (GRutaLogs == "")
    {
        GRutaLogs = DRutaLogs;
        Paso = 5;
    }

    GRutaCargas = Lee_ini("Rutacargas", GFileIni_In);
    if (GRutaCargas == "")
    {
        GRutaCargas = DRutaCargas;
        Paso = 6;
    }

    GRutaReport = Lee_ini("RutaReport", GFileIni_In);
    if (GRutaReport == "")
    {
        GRutaReport = DRutaReport;
        Paso = 7;
    }

    GHoraProcesa = Lee_ini("HoraProcesa", GFileIni_In);
    Paso = 8;
    GMinutoMaxProc = Lee_ini("MinutoMaxProc", GFileIni_In);
    Paso = 9;
    GReproccoopfecha = Conversions.ToDate(VB6.Format(Lee_ini("Reproccoopfecha", GFileIni_In), "YYYY-MM-DD"));
    Paso = 10;
    GReproccooprazon = Lee_ini("Reproccooprazon", GFileIni_In);
    Paso = 11;
    GReprocbrdfecha = Conversions.ToDate(Lee_ini("Reprocbrdfecha", GFileIni_In));
    Paso = 12;
    GReprocbrdrazon = Lee_ini("Reprocbrdrazon", GFileIni_In);
    Paso = 13;
    // UPGRADE_WARNING: App propiedad App.EXEName tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
    GFileIniOut = Lee_ini("Rutainiout", GFileIni_In) + @"\" + global::My.Application.Info.AssemblyName + ".ini";
    Paso = 14;
    DS1 = Lee_ini("DS1", GFileIni_In);
    Paso = 15;
    DS2 = Lee_ini("DS2", GFileIni_In);
    Paso = 16;
    DS3 = Lee_ini("DS3", GFileIni_In);
    Paso = 17;
    DS4 = Lee_ini("DS4", GFileIni_In);
    Paso = 18;
    DS5 = Lee_ini("DS5", GFileIni_In);
    Paso = 19;
    DS6 = Lee_ini("DS6", GFileIni_In);
    Paso = 20;
    DS7 = Lee_ini("DS7", GFileIni_In);
    Paso = 21;

    // UPGRADE_WARNING: Dir tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
    if (!string.IsNullOrEmpty(FileSystem.Dir(GFileIniOut)))
    {
        GHoraProcesa = Interaction.IIf(GHoraProcesa == Lee_ini("HoraProcesa", GFileIniOut), GHoraProcesa, Lee_ini("HoraProcesa", GFileIniOut));
        Paso = 22;
        GMinutoMaxProc = Interaction.IIf(GMinutoMaxProc == Lee_ini("MinutoMaxProc", GFileIniOut), GMinutoMaxProc, Lee_ini("MinutoMaxProc", GFileIniOut));
        Paso = 23;
        GReproccoopfecha = Interaction.IIf(GReproccoopfecha == Conversions.ToDate(Lee_ini("Reproccoopfecha", GFileIniOut)), GReproccoopfecha, Lee_ini("Reproccoopfecha", GFileIniOut));
        Paso = 24;
        GReproccooprazon = Interaction.IIf(GReproccooprazon == Lee_ini("Reproccooprazon", GFileIniOut), GReproccooprazon, Lee_ini("Reproccooprazon", GFileIniOut));
        Paso = 25;
        GReprocbrdfecha = Interaction.IIf(GReprocbrdfecha == Conversions.ToDate(Lee_ini("Reprocbrdfecha", GFileIniOut)), GReprocbrdfecha, Lee_ini("Reprocbrdfecha", GFileIniOut));
        Paso = 26;
        GReprocbrdrazon = Interaction.IIf(GReprocbrdrazon == Lee_ini("Reprocbrdrazon", GFileIniOut), GReprocbrdrazon, Lee_ini("Reprocbrdrazon", GFileIniOut));
        Paso = 27;
        DS1 = Interaction.IIf(DS1 == Lee_ini("DS1", GFileIniOut), DS1, Lee_ini("DS1", GFileIniOut));
        Paso = 28;
        DS2 = Interaction.IIf(DS2 == Lee_ini("DS2", GFileIniOut), DS2, Lee_ini("DS2", GFileIniOut));
        Paso = 29;
        DS3 = Interaction.IIf(DS3 == Lee_ini("DS3", GFileIniOut), DS3, Lee_ini("DS3", GFileIniOut));
        Paso = 30;
        DS4 = Interaction.IIf(DS4 == Lee_ini("DS4", GFileIniOut), DS4, Lee_ini("DS4", GFileIniOut));
        Paso = 30;
        DS5 = Interaction.IIf(DS5 == Lee_ini("DS5", GFileIniOut), DS5, Lee_ini("DS5", GFileIniOut));
        Paso = 31;
        DS6 = Interaction.IIf(DS6 == Lee_ini("DS6", GFileIniOut), DS6, Lee_ini("DS6", GFileIniOut));
        Paso = 32;
        DS7 = Interaction.IIf(DS7 == Lee_ini("DS7", GFileIniOut), DS7, Lee_ini("DS7", GFileIniOut));
        Paso = 33;
    }
    else
    {
        Linea = "No existe el archivo : " + GFileIniOut;
        AdmFallo.GeneraLogDetalle(Linea);
        Paso = 34;
    }

    if (GTiempoFuera == 0)
    {
        GTiempoFuera = DTiempoFuera;
        AdmFallo.GeneraLogDetalle("Advertencia: TIMEOUT en archivo .INI es igual a CERO");
    }

    Paso = 35;
    return;
ErrorIni:
    ;
    AdmFallo.GeneraLogDetalle("Error en TraeIni en el paso " + Paso.ToString());
    Environment.Exit(0);
// ---------------------------------------------------'
// Lee el Dato desde el archivo .INI y retorna Valor. '
// ---------------------------------------------------'
public string Lee_ini(ref string Dato, ref string Qini)
{
    string Lee_iniRet = default;
    string Linea;
    float posigual;
    AdmFallo.PuntoSistema("Lee_ini", 3);
    ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
    /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo Error_Lee_ini' at character 354


    Input:

            On Error GoTo Error_Lee_ini

     */
    Lee_iniRet = "";
    FileSystem.FileOpen(1, Qini, OpenMode.Input);
    while (!FileSystem.EOF(1))
    {
        Linea = FileSystem.LineInput(1);
        posigual = Strings.InStr(Strings.UCase(Linea), Strings.UCase(Strings.Trim(Dato))); // Posicion del dato
        if (posigual > 0f)
        {
            if (Strings.Mid(Linea, 1, 1) != ";") // Si no esta como comentario
            {
                posigual = Strings.InStr(Linea, "="); // Posicion del signo igual
                Lee_iniRet = Strings.Trim(Strings.Mid(Linea, (int)(posigual + 1f), (int)(Strings.Len(Linea) - posigual)));
                Lee_iniRet = Strings.Replace(Lee_iniRet, "[", "");
                Lee_iniRet = Strings.Replace(Lee_iniRet, "]", "");
                break;
            }
        }
    }

    FileSystem.FileClose(1);
    return Lee_iniRet;
Error_Lee_ini:
    ;
    FileSystem.FileClose(1);
    MsgBox("Numero: " + Information.Err().Number + Constants.vbCr + "Modulo: " + Information.Err().Source + Constants.vbCr + "Mensaj: " + Information.Err().Description + Constants.vbCr + Constants.vbCr + "Error en GET Archivo .INI : " + Dato);
}

