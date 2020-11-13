
internal partial class SurroundingClass
{
    private MSGFORMATLib.ExtremeLog Logger = new MSGFORMATLib.ExtremeLog(); /* TODO ERROR: Skipped SkippedTokensTrivia */
    /* TODO ERROR: Skipped SkippedTokensTrivia */
	Private Type SYSTEMTIME
        wYear As Integer
        wMonth As Integer
        wDayOfWeek As Integer
        wDay As Integer
        wHour As Integer
        wMinute As Integer
        wSecond As Integer
        wMilliseconds As Integer
	End Type
     */
    [DllImport("kernel32")]
    private static extern void GetLocalTime(SYSTEMTIME lpSystemTime);
	
	public void PuntoSistema(string NombreNivel, int Nivel)
{
    if (Nivel == 1)
    {
        PuntoSistemaNivel1 = NombreNivel;
        PuntoSistemaNivel2 = "";
        PuntoSistemaNivel3 = "";
        PuntoSistemaNivel4 = "";
        PuntoSistemaNivel5 = "";
    }

    if (Nivel == 2)
    {
        PuntoSistemaNivel2 = NombreNivel;
        PuntoSistemaNivel3 = "";
        PuntoSistemaNivel4 = "";
        PuntoSistemaNivel5 = "";
    }

    if (Nivel == 3)
        PuntoSistemaNivel3 = NombreNivel;
    if (Nivel == 4)
        PuntoSistemaNivel4 = NombreNivel;
    if (Nivel == 5)
        PuntoSistemaNivel4 = NombreNivel;
}
public void GeneraLogResumen(string sTipo, string sOrigen, string sDescripcion)

{
    string sMsgLog;
    var dtSTR = default(SYSTEMTIME);
    GetLocalTime(dtSTR);

    // sTipo =  W - WARNING
    // G - ERROR GRABE

    sMsgLog = "";
    sMsgLog = sMsgLog + "WE" + sTipo;
    sMsgLog = sMsgLog + Strings.Format(dtSTR.wYear, "0000") + Strings.Format(dtSTR.wMonth, "00") + Strings.Format(dtSTR.wDay, "00");
    sMsgLog = sMsgLog + Strings.Format(dtSTR.wHour, "00") + Strings.Format(dtSTR.wMinute, "00") + Strings.Format(dtSTR.wSecond, "00") + Strings.Format(dtSTR.wMilliseconds, "000");
    sMsgLog = sMsgLog + Strings.Left(sOrigen + string(10, " "), 10);
    sMsgLog = sMsgLog + Strings.Left(sDescripcion + string(80, " "), 80);
    Logger.BroadcastMsg(sMsgLog);
}
public void GeneraLogDetalle(string QDescribe, int MostrarHastaNivel = )
{
    string Archivo;
    long intFile;
    string LDHora;
    string LogMensaje;
    string RutaCompensa;
    string QOrigen;
    ;
#error Cannot convert OnErrorResumeNextStatementSyntax - see comment for details
    
    Input:

       On Error Resume Next

     */
    switch (MostrarHastaNivel)
    {
        case 1:
            {
                QOrigen = PuntoSistemaNivel1;
                break;
            }

        case 2:
            {
                QOrigen = PuntoSistemaNivel1 + "/" + PuntoSistemaNivel2;
                break;
            }

        case 3:
            {
                QOrigen = PuntoSistemaNivel1 + "/" + PuntoSistemaNivel2 + "/" + PuntoSistemaNivel3;
                break;
            }

        default:
            {
                QOrigen = PuntoSistemaNivel1 + "/" + PuntoSistemaNivel2 + "/" + PuntoSistemaNivel3 + "/" + PuntoSistemaNivel4;
                break;
            }
    }
}
{
    string PuntoSistemaNivel5;
    ;
#error Cannot convert AssignmentStatementSyntax - see comment for details
    
    Input: GRutaLogs & "\" & Format(Date, "YYMM")

    Context:
       RutaCompensa = GRutaLogs & "\" & Format(Date, "YYMM")

     */
    FileSystem.MkDir(RutaCompensa);
    ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
    


    Input:
       On Error GoTo 0

     */
    ;
#error Cannot convert AssignmentStatementSyntax - see comment for details
    

    Input: RutaCompensa & "\Log" & Format(Date, "yyyyMMdd") 
    Context:
       Archivo = RutaCompensa & "\Log" & Format(Date, "yyyyMMdd") & ".TXT"

     */
    intFile = FileSystem.FreeFile();
    Open(Archivo);
    LDHora = Strings.Format(Time, "hh:mm:ss");
    LogMensaje = GProceso + "> Version:" + QVersion + " > H:" + LDHora + " > O:" + QOrigen + " > D:" + QDescribe;
    FileSystem.Print(, LogMensaje);
    Close();
}

}