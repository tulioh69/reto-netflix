private MSGFORMATLib.Sync SyncExe;
    private bool FuncionCoop;
    private bool FuncionBrd;
    private short Minutos;
    private CAdminExcepcion AdmFallo = new CAdminExcepcion();
    private MSGFORMATLib.Des Crypto = new MSGFORMATLib.Des();
    private string RutaFecha;
    private DateTime IniProc;

// ----------------------------------------------------------------------
// 1.>> Carga el formulario
// -----------------------------------------------------------------------
{

    this.Visible = false;
    Timer1.Enabled = true;
    Minutos = 0;
    Timer1.Interval = 6000;
    piMinutos = 0;
    Minutos = 31;
    IniProc = DateAndTime.Now;
    FuncionCoop = false;
    FuncionBrd = false;
    SyncExe = new MSGFORMATLib.Sync();
    SyncExe.SetProgramOk();
    AdmFallo.GeneraLogResumen("I", "AS_DCompensa", "Proceso Iniciado");
}


// --------------------------------------------------------------------------------
// 2.>> Se setean las columnas que van a servir de quiebre
// --------------------------------------------------------------------------------
private void Timer1_Tick(object eventSender, EventArgs eventArgs)
{
    var sMensaje = default(string);
    short rc;
    Minutos = Minutos + 1;
    Trae_ini();
    if (Minutos < piMinutos | !DiaLaborable)
    {
        if (Minutos > 100)
            Minutos = 0;
        return;
    }

    AdmFallo.GeneraLogDetalle("Analizando sistemas...");
    Minutos = 0;
    GProceso = "C" + (DateAndTime.DatePart(DateInterval.Hour, Conversions.ToDate(DateAndTime.Now)) * 3600 + DateAndTime.DatePart(DateInterval.Minute, Conversions.ToDate(DateAndTime.Now)) * 60 + DateAndTime.DatePart(DateInterval.Second, Conversions.ToDate(DateAndTime.Now))) + ":";
    ;
    // rc = CompensaCoop(sMensaje)
    rc = CompensaBanred(sMensaje);
    if (Information.Err().Number != 0)
    {
        rc = 96;
        sMensaje = Information.Err().Description;
        AdmFallo.GeneraLogDetalle("Error de Inicio " + Information.Err().Description);
    };
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
    /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 1273


    Input:
            On Error GoTo 0

     */
    if ((int)rc != 0)
    {
        AdmFallo.GeneraLogResumen("W", "AS_DCompensa", "Error " + rc + " " + sMensaje);
    }

    AdmFallo.GeneraLogResumen("I", "AS_DCompensa", "Proceso Finalizado");
}

// ----------------------------------------------------------------------------------------------------------------------------
// 3.>> Proceso para la compensación contras las cooperativas
// ----------------------------------------------------------------------------------------------------------------------------
private short CompensaCoop(ref string sMsgError)
{
    short CompensaCoopRet = default;

    // ------------------------------------------------------------------------------------------
    // 3.0.>> Declaración de Variables
    // ------------------------------------------------------------------------------------------
    var LeeBD1 = new Cnxbd();
    short i;
    bool Ok;
    string QColumnas;
    string DelaBaseD;
    string DelaTabla;
    string InnerJoin;
    string Condicion;
    string Ordenadox;
    var QGenerar = default(object);
    object ComoProceso;

    // ------------------------------------------------------------------------------------------
    // 3.1.>> Validación principal, sólo a las 8 se puede ejecutar
    // ------------------------------------------------------------------------------------------
    // If Hour(Time) <> Hour(Time) Then Exit Function    'GHoraProcesa
    // If Minute(Time) > GMinutoMaxProc Then Exit Function
    // If FuncionCoop And FuncionProcesada Then Exit Function

    // ------------------------------------------------------------------------------------------
    // 3.2.>> Validación principal, sólo a las 8 se puede ejecutar
    // ------------------------------------------------------------------------------------------
    // AdmFallo.PuntoSistema "CompensaCoop", 1
    // CompensaCoop = 1
    // AdmFallo.GeneraLogDetalle "Inicio CompensaCoop"

    // ------------------------------------------------------------------------------------------
    // 3.3.>> Se obtienen todas las cooperativas que se van a procesar con la fecha descrita
    // ------------------------------------------------------------------------------------------
    LeeBD1.DepuraParametros();
    LeeBD1.QParametros("pp_razon", basGeneral.aVCarac, basGeneral.aIngres, "COOP");
    LeeBD1.QParametros("pp_qproc", basGeneral.aVCarac, basGeneral.aIngres, "Reportar");
    LeeBD1.QParametros("pp_opcion", basGeneral.aVCarac, basGeneral.aIngres, Interaction.IIf(GReproccoopfecha == DFchReproc, "E", "R"));
    LeeBD1.QParametros("pp_valor", basGeneral.aVCarac, basGeneral.aIngres, GReproccoopfecha);
    Ok = LeeBD1.EjecutaSP("AS_BATCHPCI", "pa_batchcompensacion", sMsgError, QGenerar);
    // Ok = LeeBD1.EjecutaSP("AS_BATCHPCI_comp", "pa_batchcompensacion", sMsgError, QGenerar)GB
    // modificar la base de datos

    AdmFallo.GeneraLogDetalle("Batch Compensacion ejecutado");

    // -----------------------------------------------------------------------------------------
    // 3.4.>> Por cada Cooperativa se envía a obtener los datos y generar el reporte
    // -----------------------------------------------------------------------------------------
    var loopTo = (short)Information.UBound((Array)QGenerar, 2);
    for (i = 0; i <= loopTo; i++)
    {
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QGenerar(1, i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QGenerar(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        if (ReportaCoop(QGenerar(0, i), QGenerar(1, i)))
        {
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QGenerar(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            EjecucionProcesada("COOP", "Reportar", QGenerar(0, i));
        }
    }

    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QGenerar(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    AdmFallo.GeneraLogDetalle(Operators.ConcatenateObject("Finaliza CompensaCoop ", QGenerar((object)0, (object)0)));
    FuncionCoop = FuncionProcesada;
    CompensaCoopRet = 0;
    return CompensaCoopRet;
}

// ----------------------------------------------------------------------------------------------------------------------------
// 4.>> EJECUTA EL SP PARA OBTENER LOS DATOS Y ESTOS SERAN GENERADOS EN ARCHIVO TEXTO
// ----------------------------------------------------------------------------------------------------------------------------
private bool ReportaCoop(string QEntidad, DateTime QFecha)
{
    bool ReportaCoopRet = default;

    // ----------------------------------------------------------------------
    // 4.1.>> Declara e Inicializa
    // ----------------------------------------------------------------------
    var LeeBD2 = new Cnxbd();
    var GenReporte = new CGeneraArchivo();
    string MsjLog;
    var Reportar = default(object);
    bool Ok;
    var NombreRpt = default(string);
    var TituloRpt = default(string);
    short TipoReporte;
    string QDirectorio;
    AdmFallo.PuntoSistema("ReportaCoop", 2);
    ReportaCoopRet = false;
    QDirectorio = VB6.Format(QFecha, "yyMMdd");
    if (RutaFecha != QDirectorio)
    {
        GenReporte.CreaDirectorio(QDirectorio);
        RutaFecha = QDirectorio;
    }

    // ----------------------------------------------------------------------
    // 4.2.>> Se establece que son 5 reportes los que se van a generar
    // ----------------------------------------------------------------------
    for (TipoReporte = 1; TipoReporte <= 5; TipoReporte++)
    {
        MsjLog = " Entidad: " + QEntidad + "; FchProc:" + QFecha + " TipoRpt:" + "0" + TipoReporte;
        // ----------------------------------------------------------------------
        // 4.2.1.>> Un Titulo por cada tipo de reporte
        // ----------------------------------------------------------------------
        switch (TipoReporte)
        {
            case 1:
                {
                    NombreRpt = "AS003_CompensacionResumen";
                    TituloRpt = "RESUMEN DE TOTALES A COMPENSAR";
                    break;
                }

            case 2:
                {
                    NombreRpt = "AS004_CompensacionTotales";
                    TituloRpt = "TOTALES DE TRANSACCIONES POR COOPERATIVA";
                    break;
                }

            case 3:
                {
                    NombreRpt = "AS005_CompensacionDetalle";
                    TituloRpt = "DETALLE DE TRANSACCIONES POR COOPERATIVA";
                    break;
                }

            case 4:
                {
                    NombreRpt = "AS006_TotalesporCajeros";
                    TituloRpt = "TOTALES DE TRANSACCIONES POR CAJEROS";
                    break;
                }

            case 5:
                {
                    NombreRpt = "AS007_CompensacionMensual";
                    TituloRpt = "TOTALES A COMPENSAR AL MES";
                    break;
                }
        }

        // ----------------------------------------------------------------------
        // 4.2.2.>> Pasos para ejecutar el proceso que genera el reporte
        // ----------------------------------------------------------------------
        LeeBD2.DepuraParametros();
        LeeBD2.QParametros("pp_entidad", basGeneral.aEntero, basGeneral.aIngres, QEntidad);
        LeeBD2.QParametros("pp_fecha", basGeneral.aVCarac, basGeneral.aIngres, VB6.Format(QFecha, "yyyy-mm-dd"));
        LeeBD2.QParametros("pp_tiporpt", basGeneral.aVCarac, basGeneral.aIngres, "0" + TipoReporte);
        Ok = LeeBD2.EjecutaSP("OCTOPUS_PCI", "pa_compensacioncoop", MsjLog, Reportar);
        // Ok = LeeBD2.EjecutaSP("OCTOPUS_PCI_comp", "pa_compensacioncoop", MsjLog, Reportar)GB
        // modificar la base de datos

        if (!Ok)
        {
            AdmFallo.GeneraLogDetalle("Fallo al ejecutar pa_compensacioncoop en parametro 0" + TipoReporte);
            break;
        }

        AdmFallo.GeneraLogDetalle("Proceso CompensaCoop ejecutado con parametro 0" + TipoReporte, 2);
        if (Ok & TipoReporte == 1)
            GenReporte.CreaDirectorio(VB6.Format(QFecha, "yyMMdd") + @"\COOP_" + QEntidad);
        switch (TipoReporte)
        {
            // ----------------------------------------------------------------------
            // 4.2.3.1.>> Para configurar como debe salir el reporte tipo 1
            // ----------------------------------------------------------------------
            case 1:
                {
                    if (Ok)
                    {
                        GenReporte.CreaQuiebres(2, 0, 1);
                        GenReporte.DimensionaArreglo("ConfiguraDetalle", 7, 5);
                        // -- (Registro, Campo, Espacio que ocupa, Formato, Alineacion, Nombre del Campo)
                        GenReporte.ConfiguraDetalle(0, 2, 20, "Letras", "Izqrda", "Adquiridas en:");
                        GenReporte.ConfiguraDetalle(1, 3, 25, "Letras", "Izqrda", "Autorizadas en:");
                        GenReporte.ConfiguraDetalle(2, 4, 25, "Letras", "Izqrda", "Tipo-Transac.");
                        GenReporte.ConfiguraDetalle(3, 5, 9, "Numero", "Izqrda", "#Transac.");
                        GenReporte.ConfiguraDetalle(4, 6, 12, "Moneda", "Dercha", "Comision");
                        GenReporte.ConfiguraDetalle(5, 7, 15, "Moneda", "Dercha", "Total");
                        GenReporte.ConfiguraDetalle(6, 8, 16, "Moneda", "Dercha", "A compensar");
                        // -- Recordar siempre colcar en orden las columnas
                        // -- (Registro, Campo, Tipo, Formato, Alineacion, Nombre del Campo)
                        GenReporte.DimensionaArreglo("ResumenPieQuiebre", 4, 7); // -- por los 4 parametros mas los dos quiebres y la columna final de total
                        GenReporte.CreaResumenQuiebres(0, 5, "Suma", 1, 0); // -- #Transacciones
                        GenReporte.CreaResumenQuiebres(1, 6, "Suma", 1, 0); // -- Comisiones"
                        GenReporte.CreaResumenQuiebres(2, 7, "Suma", 1, 0); // -- Totales"
                        GenReporte.CreaResumenQuiebres(3, 8, "Suma", 1, 0); // -- A Compensar"
                        GenReporte.ReporteCompensacion(QEntidad, NombreRpt, TituloRpt, 132, QFecha, Reportar);
                    }

                    break;
                }
            // ----------------------------------------------------------------------
            // 5.2.3.2.>> Para configurar como debe salir el reporte tipo 2
            // ----------------------------------------------------------------------
            case 2:
                {
                    if (Ok)
                    {
                        // MsgBox "AS0004"
                        GenReporte.CreaQuiebres(3, 0, 1, 2);
                        GenReporte.DimensionaArreglo("ConfiguraDetalle", 9, 5);
                        GenReporte.ConfiguraDetalle(0, 3, 25, "Letras", "Izqrda", "Autorizadas en:");
                        GenReporte.ConfiguraDetalle(1, 4, 25, "Letras", "Izqrda", "TipoTransac.");
                        GenReporte.ConfiguraDetalle(2, 5, 11, "Letras", "Izqrda", "Erradas");
                        GenReporte.ConfiguraDetalle(3, 6, 11, "Letras", "Izqrda", "Reversos");
                        GenReporte.ConfiguraDetalle(4, 7, 11, "Letras", "Izqrda", "RevSinTrx");
                        GenReporte.ConfiguraDetalle(5, 8, 9, "Numero", "Izqrda", "#Transac.");
                        GenReporte.ConfiguraDetalle(6, 9, 12, "Moneda", "Dercha", "Comision");
                        GenReporte.ConfiguraDetalle(7, 10, 15, "Moneda", "Dercha", "Total");
                        GenReporte.ConfiguraDetalle(8, 11, 16, "Moneda", "Dercha", "A-Compensar");
                        // -- Recordar siempre colcar en orden las columnas
                        GenReporte.DimensionaArreglo("ResumenPieQuiebre", 4, 11);
                        GenReporte.CreaResumenQuiebres(0, 8, "Suma", 3, 2, 1, 0); // -- #Transacciones
                        GenReporte.CreaResumenQuiebres(1, 9, "Suma", 3, 2, 1, 0); // -- Comisiones
                        GenReporte.CreaResumenQuiebres(2, 10, "Suma", 3, 2, 1, 0); // -- Totales
                        GenReporte.CreaResumenQuiebres(3, 11, "Suma", 3, 2, 1, 0); // -- A Compensar
                        GenReporte.ReporteCompensacion(QEntidad, NombreRpt, TituloRpt, 144, QFecha, Reportar);
                    }

                    break;
                }
            // ----------------------------------------------------------------------
            // 4.2.3.3.>> Para configurar como debe salir el reporte tipo 3
            // ----------------------------------------------------------------------
            case 3:
                {
                    if (Ok)
                    {
                        // MsgBox "AS0005"
                        GenReporte.CreaQuiebres(5, 0, 1, 2, 3, 4);
                        GenReporte.DimensionaArreglo("ConfiguraDetalle", 14, 5);
                        GenReporte.ConfiguraDetalle(0, 5, 11, "Fechas", "Izqrda", "Fecha Trx");
                        GenReporte.ConfiguraDetalle(1, 6, 13, "Letras", "Izqrda", "Hora Trx.");
                        GenReporte.ConfiguraDetalle(2, 7, 21, "Letras", "Izqrda", "Tarjeta.");
                        GenReporte.ConfiguraDetalle(3, 8, 8, "Letras", "Izqrda", "TipoCta.Org.");
                        GenReporte.ConfiguraDetalle(4, 9, 8, "Letras", "Izqrda", "TipoCta.Dest.");
                        GenReporte.ConfiguraDetalle(5, 10, 8, "Letras", "Dercha", "Cod.Resp");
                        GenReporte.ConfiguraDetalle(6, 11, 9, "Letras", "Dercha", "Disp.");
                        GenReporte.ConfiguraDetalle(7, 12, 8, "Letras", "Dercha", "Refer.");
                        GenReporte.ConfiguraDetalle(8, 13, 13, "Letras", "Dercha", "Hora-Rev.");
                        GenReporte.ConfiguraDetalle(9, 14, 10, "Letras", "Dercha", "ErrorRev.");
                        GenReporte.ConfiguraDetalle(10, 15, 14, "Moneda", "Dercha", "OK.");
                        GenReporte.ConfiguraDetalle(11, 16, 14, "Moneda", "Dercha", "Errado");
                        GenReporte.ConfiguraDetalle(12, 17, 12, "Moneda", "Dercha", "Reversado");
                        GenReporte.ConfiguraDetalle(13, 18, 12, "Moneda", "Dercha", "RevSinTrx");
                        // -- Recordar siempre colcar en orden las columnas
                        GenReporte.DimensionaArreglo("ResumenPieQuiebre", 4, 13);
                        GenReporte.CreaResumenQuiebres(0, 15, "Suma", 4, 3, 2, 1, 0); // -- Valor OK
                        GenReporte.CreaResumenQuiebres(1, 16, "Suma", 4, 3, 2, 1, 0); // -- Valor Errado
                        GenReporte.CreaResumenQuiebres(2, 17, "Suma", 4, 3, 2, 1, 0); // -- Valor Reversado
                        GenReporte.CreaResumenQuiebres(3, 18, "Suma", 4, 3, 2, 1, 0); // -- Valor Reverso sin TRX
                        GenReporte.ReporteCompensacion(QEntidad, NombreRpt, TituloRpt, 174, QFecha, Reportar);
                    }

                    break;
                }
            // ----------------------------------------------------------------------
            // 4.2.3.3.>> Para configurar como debe salir el reporte tipo 3
            // ----------------------------------------------------------------------
            case 5:
                {
                    if (Ok)
                    {
                        // MsgBox "AS0007"
                        GenReporte.CreaQuiebres(1, 0);
                        GenReporte.DimensionaArreglo("ConfiguraDetalle", 6, 5);
                        GenReporte.ConfiguraDetalle(0, 1, 40, "Fechas", "Dercha", "Fecha:");
                        GenReporte.ConfiguraDetalle(1, 2, 14, "Numero", "Dercha", "# EMI.");
                        GenReporte.ConfiguraDetalle(2, 3, 16, "Moneda", "Dercha", "Autoriz/Emisor");
                        GenReporte.ConfiguraDetalle(3, 4, 14, "Numero", "Dercha", "# ADQ.");
                        GenReporte.ConfiguraDetalle(4, 5, 16, "Moneda", "Dercha", "Adquirente");
                        GenReporte.ConfiguraDetalle(5, 6, 18, "Moneda", "Dercha", "A Compensar");
                        // -- Recordar siempre colcar en orden las columnas
                        GenReporte.DimensionaArreglo("ResumenPieQuiebre", 5, 5);
                        GenReporte.CreaResumenQuiebres(0, 2, "Suma", 0); // -- # Emi
                        GenReporte.CreaResumenQuiebres(1, 3, "Suma", 0); // -- Autorizador/Emisor
                        GenReporte.CreaResumenQuiebres(2, 4, "Suma", 0); // -- # Adq
                        GenReporte.CreaResumenQuiebres(3, 5, "Suma", 0); // -- Adquirente
                        GenReporte.CreaResumenQuiebres(4, 6, "Suma", 0); // -- A compensar
                        GenReporte.ReporteCompensacion(QEntidad, NombreRpt, TituloRpt, 126, QFecha, Reportar);
                    }

                    break;
                }
        }

        AdmFallo.GeneraLogDetalle("Se he generado Reporte " + NombreRpt, 2);
    }

    ReportaCoopRet = true;
    return ReportaCoopRet;
}
// ----------------------------------------------------------------------------------------------------------------------------
// 5.>> Proceso para la compensación de BANRED
// ----------------------------------------------------------------------------------------------------------------------------
private short CompensaBanred(ref string sMsgError)
{

    // ----------------------------------------------------------------------
    // 5.0.>> Declaracion de Variables
    // ----------------------------------------------------------------------
    var LeeBD3 = new Cnxbd();
    var QProcesar = default(object);
    string MsjLog;
    object Datar;
    short i;
    bool Ok;
    short QReporte;

    // ------------------------------------------------------------------------------------------
    // 5.1.>> Validación principal
    // ------------------------------------------------------------------------------------------
    // If Hour(Time) <> Hour(Time) Then Exit Function 'GHoraProcesa
    // If Minute(Time) > GMinutoMaxProc Then Exit Function
    if (FuncionBrd & FuncionProcesada)
        return default;
    if (!ExisteFileParaCargar)
        return default;
    AdmFallo.PuntoSistema("CompensaBanred", 1);
    AdmFallo.GeneraLogDetalle("Inicio CompensaBanred");
    // ------------------------------------------------------------------------------------------
    // 5.2.>> Se obtienen los parametros a procesar
    // ------------------------------------------------------------------------------------------
    LeeBD3.DepuraParametros();
    LeeBD3.QParametros("pp_batch", basGeneral.aVCarac, basGeneral.aIngres, "BRD");
    // UPGRADE_WARNING: Se detectó el uso de Null/IsNull(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="2EED02CB-5C0E-4DC1-AE94-4FAA3A30F51A"'
    LeeBD3.QParametros("pp_qproc", basGeneral.aVCarac, basGeneral.aIngres, DBNull.Value);
    LeeBD3.QParametros("pp_opcion", basGeneral.aVCarac, basGeneral.aIngres, Interaction.IIf(GReprocbrdfecha == DFchReproc, "E", "R"));
    LeeBD3.QParametros("pp_valor", basGeneral.aVCarac, basGeneral.aIngres, GReprocbrdfecha);
    Ok = LeeBD3.EjecutaSP("AS_BATCHPCI", "pa_batchcompensacion", sMsgError, QProcesar);
    // Ok = LeeBD3.EjecutaSP("AS_BATCHPCI_comp", "pa_batchcompensacion", sMsgError, QProcesar)GB
    // modificar la base de datos

    // -----------------------------------------------------------------------------------------
    // 5.3.>> Por cada Cooperativa se envía a obtener los datos y generar el reporte
    // -----------------------------------------------------------------------------------------
    var loopTo = (short)(Information.UBound((Array)QProcesar, 1) - 1);
    for (i = 0; i <= loopTo; i++)
    {
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(0, i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(QProcesar((object)0, (object)i), "SubirArchivo", false)))
        {
            // ----------------------------------------------------------------------
            // 5.2.2.>> Invoca al proceso que sube el archivo de BANRED
            // ----------------------------------------------------------------------
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(1, i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Ok = SubeFileBanred(QProcesar(2, i), QProcesar(1, i));
            if (Ok)
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(1, i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                EjecucionProcesada("BRD", QProcesar(0, i), QProcesar(1, i));
            }
            else
            {
                AdmFallo.GeneraLogDetalle("Fallo en ejecucion de pa_subearchivos");
                return default;
            }
        }

        Ok = false;
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(0, i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(QProcesar((object)0, (object)i), "Reportar", false)))
        {
            for (QReporte = 0; QReporte <= 3; QReporte++)
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Ok = ReportaBanred(QProcesar(2, i), "0" + QReporte);
                if (Ok)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(1, i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto QProcesar(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    EjecucionProcesada("BRD", QProcesar(0, i), QProcesar(1, i));
                }
                else if (QReporte != 1)
                    AdmFallo.GeneraLogDetalle("Fallo en ejecucion de pa_subearchivos");
            }
        }

        if (Ok)
            ArchivoProcesado(QProcesar(2, i));
    }

    FuncionBrd = FuncionProcesada;
    return default;
}

// ----------------------------------------------------------------------------------------------------------------------------
// 6.>> EJECUTA EL SP PARA OBTENER LOS DATOS Y ESTOS SERAN GENERADOS EN ARCHIVO TEXTO
// ----------------------------------------------------------------------------------------------------------------------------
private bool ReportaBanred(DateTime PFecha, string PTipo)
{
    bool ReportaBanredRet = default;

    // ----------------------------------------------------------------------
    // 6.1.>> Declaracion de Variables
    // ----------------------------------------------------------------------
    var LeeBD4 = new Cnxbd();
    var GenReporte4 = new CGeneraArchivo();
    var MsjLog = default(string);
    var Reportar = default(object);
    bool Ok;
    var NombreRpt = default(string);
    var TituloRpt = default(string);
    short TipoReporte;
    string QDirectorio;
    if (PTipo == "01")
        return ReportaBanredRet;
    // ----------------------------------------------------------------------
    // 6.2.>> Inicializaciones
    // ----------------------------------------------------------------------
    AdmFallo.PuntoSistema("ReportaBanred", 2);
    ReportaBanredRet = false;
    QDirectorio = VB6.Format(PFecha, "yyMMdd");
    if (RutaFecha != QDirectorio)
    {
        GenReporte4.CreaDirectorio(QDirectorio);
        RutaFecha = QDirectorio;
    }

    // ----------------------------------------------------------------------
    // 6.3.>> Un Titulo por cada tipo de reporte
    // ----------------------------------------------------------------------
    switch (PTipo ?? "")
    {
        case "00":
            {
                NombreRpt = "AS0008_TotalesTP";
                TituloRpt = "TOTALES TRANSACCIONES TP POR BANCO ";
                break;
            }

        case "01":
            {
                NombreRpt = "AS0009_TotalesTP";
                TituloRpt = "TOTALES DE TRANSACCIONES (TP) DEL " + PFecha;
                break;
            }

        case "02":
            {
                NombreRpt = "AS0010_CompensaTP_ADQ";
                TituloRpt = "DETALLE DE ADQUIRENTE (TP) DEL " + PFecha;
                break;
            }

        case "03":
            {
                NombreRpt = "AS0010_CompensaTP_EMI";
                TituloRpt = "DETALLE DE EMISOR (TP) DEL " + PFecha;
                break;
            }
    }

    // ----------------------------------------------------------------------
    // 6.4.>> Pasos para ejecutar el proceso que genera el reporte
    // ----------------------------------------------------------------------
    LeeBD4.DepuraParametros();
    LeeBD4.QParametros("pp_entidad", basGeneral.aECorto, basGeneral.aIngres, 0);
    LeeBD4.QParametros("pp_fecha", basGeneral.aVCarac, basGeneral.aIngres, VB6.Format(PFecha, "yyyy-mm-dd"));
    LeeBD4.QParametros("pp_tipo", basGeneral.aVCarac, basGeneral.aIngres, PTipo);
    Ok = LeeBD4.EjecutaSP("OCTOPUS_PCI", "pa_compensacionesbrd", MsjLog, Reportar);
    // Ok = LeeBD4.EjecutaSP("OCTOPUS_PCI_comp", "pa_compensacionesbrd", MsjLog, Reportar)GB
    // modificar la base de datos

    AdmFallo.GeneraLogDetalle("Proceso Compensabrd ejecutado con parametro " + PTipo, 2);
    if (Ok)
        GenReporte4.CreaDirectorio(VB6.Format(PFecha, "yyMMdd") + @"\BRD");
    if (Ok)
    {
        switch (PTipo ?? "")
        {
            case "00":
            case "xx":
                {
                    GenReporte4.CreaQuiebres(1, 0);
                    GenReporte4.DimensionaArreglo("ConfiguraDetalle", 9, 5);
                    GenReporte4.ConfiguraDetalle(0, 1, 30, "Letras", "Izqrda", "BANCO");
                    GenReporte4.ConfiguraDetalle(1, 2, 16, "Moneda", "Dercha", "ADQ-RETIRO");
                    GenReporte4.ConfiguraDetalle(2, 3, 16, "Moneda", "Dercha", "ADQ-CONSULTA");
                    GenReporte4.ConfiguraDetalle(3, 4, 16, "Moneda", "Dercha", "ADQ-TRANSFER");
                    GenReporte4.ConfiguraDetalle(4, 5, 16, "Moneda", "Dercha", "EMI-RETIRO");
                    GenReporte4.ConfiguraDetalle(5, 6, 16, "Moneda", "Dercha", "EMI-CONSULTA");
                    GenReporte4.ConfiguraDetalle(6, 7, 16, "Moneda", "Dercha", "EMI-TRANSFER");
                    GenReporte4.ConfiguraDetalle(7, 8, 10, "Numero", "Dercha", "REG.");
                    GenReporte4.ConfiguraDetalle(8, 9, 16, "Moneda", "Dercha", "A-COMPENSAR");
                    // -- Recordar siempre colcar en orden las columnas
                    GenReporte4.DimensionaArreglo("ResumenPieQuiebre", 8, 5);
                    GenReporte4.CreaResumenQuiebres(0, 2, "Suma", 0); // -- ADQ-RETIRO
                    GenReporte4.CreaResumenQuiebres(1, 3, "Suma", 0); // -- ADQ-CONSULTA
                    GenReporte4.CreaResumenQuiebres(2, 4, "Suma", 0); // -- ADQ-TRANSFER
                    GenReporte4.CreaResumenQuiebres(3, 5, "Suma", 0); // -- EMI-RETIRO
                    GenReporte4.CreaResumenQuiebres(4, 6, "Suma", 0); // -- EMI-CONSULTA
                    GenReporte4.CreaResumenQuiebres(5, 7, "Suma", 0); // -- EMI-TRANSFER
                    GenReporte4.CreaResumenQuiebres(6, 8, "Suma", 0); // -- REG."
                    GenReporte4.CreaResumenQuiebres(7, 9, "Suma", 0); // -- A-COMPENSAR
                    GenReporte4.ReporteCompensacion(0.ToString(), NombreRpt, TituloRpt, 156, Conversions.ToDate(Conversions.ToString(PFecha)), Reportar);
                    break;
                }

            case "02":
            case "03":
                {
                    GenReporte4.CreaQuiebres(5, 0, 1, 2, 3, 4);
                    GenReporte4.DimensionaArreglo("ConfiguraDetalle", 15, 5);
                    GenReporte4.ConfiguraDetalle(0, 2, 5, "Letras", "Izqrda", "Bco");
                    GenReporte4.ConfiguraDetalle(1, 5, 11, "Fechas", "Izqrda", "Fecha Trx");
                    GenReporte4.ConfiguraDetalle(2, 6, 14, "Letras", "Izqrda", "Hora Trx.");
                    GenReporte4.ConfiguraDetalle(3, 7, 21, "Letras", "Izqrda", "Tarjeta.");
                    GenReporte4.ConfiguraDetalle(4, 8, 12, "Letras", "Dercha", "Disposit.");
                    GenReporte4.ConfiguraDetalle(5, 9, 15, "Letras", "Dercha", "TipoCta.");
                    GenReporte4.ConfiguraDetalle(6, 10, 20, "Letras", "Dercha", "#Cuenta");
                    GenReporte4.ConfiguraDetalle(7, 11, 8, "Letras", "Dercha", "Refer.");
                    GenReporte4.ConfiguraDetalle(8, 12, 8, "Letras", "Dercha", "RESP");
                    GenReporte4.ConfiguraDetalle(9, 13, 4, "Letras", "Dercha", "REV");
                    GenReporte4.ConfiguraDetalle(10, 14, 4, "Letras", "Dercha", "EMV");
                    GenReporte4.ConfiguraDetalle(11, 15, 10, "Letras", "Dercha", "FchConta");
                    GenReporte4.ConfiguraDetalle(12, 16, 16, "Moneda", "Dercha", "Valor");
                    GenReporte4.ConfiguraDetalle(13, 17, 16, "Moneda", "Dercha", "A-Compensar");
                    GenReporte4.ConfiguraDetalle(14, 18, 16, "Moneda", "Dercha", "Com.Impresion"); // --JDominguez
                                                                                                   // -- Recordar siempre colcar en orden las columnas
                    GenReporte4.DimensionaArreglo("ResumenPieQuiebre", 3, 13);
                    GenReporte4.CreaResumenQuiebres(0, 16, "Suma", 4, 3, 2, 1, 0); // -- Valor
                    GenReporte4.CreaResumenQuiebres(1, 17, "Suma", 4, 3, 2, 1, 0); // -- A compensar
                    GenReporte4.CreaResumenQuiebres(2, 18, "Suma", 4, 3, 2, 1, 0); // -- Com.Impresion    '--JDominguez
                    GenReporte4.ReporteCompensacion(0.ToString(), NombreRpt, TituloRpt, 184, Conversions.ToDate(Conversions.ToString(PFecha)), Reportar);
                    break;
                }
        }
    }

    AdmFallo.GeneraLogDetalle("Reporte " + NombreRpt + " generado", 2);
    ReportaBanredRet = true;
    return ReportaBanredRet;
}

// ----------------------------------------------------------------------------------------------------------------------------
// 7.>> EJECUTA LA SUBIDA DE LOS ARCHIVOS
// ----------------------------------------------------------------------------------------------------------------------------
private bool SubeFileBanred(DateTime PFecha, string PTipo)
{
    bool SubeFileBanredRet = default;

    // ----------------------------------------------------------------------
    // 7.1.>> Declaracion de Variables
    // ----------------------------------------------------------------------
    var LeeBD5 = new Cnxbd();
    var GenReporte5 = new CGeneraArchivo();
    var MsjLog = default(string);
    var Reportar = default(object);
    var Reportar2 = default(object);
    bool Ok;
    string NombreRpt;
    string TituloRpt;
    short TipoReporte;
    string QDirectorio;

    // ----------------------------------------------------------------------
    // 7.2.>> Inicializaciones
    // ----------------------------------------------------------------------
    AdmFallo.PuntoSistema("SubeFileBanred", 2);
    SubeFileBanredRet = false;
    QDirectorio = VB6.Format(PFecha, "yyMMdd");
    if (RutaFecha != QDirectorio)
    {
        GenReporte5.CreaDirectorio(QDirectorio);
        RutaFecha = QDirectorio;
    }

    // ----------------------------------------------------------------------
    // 7.3.>> Invoca al proceso que sube el archivo de BANRED
    // ----------------------------------------------------------------------
    LeeBD5.DepuraParametros();
    LeeBD5.QParametros("tp_tipo", basGeneral.aVCarac, basGeneral.aIngres, PTipo);
    LeeBD5.QParametros("tp_FchProc", basGeneral.aVCarac, basGeneral.aIngres, PFecha);
    LeeBD5.QParametros("tp_ruta", basGeneral.aVCarac, basGeneral.aIngres, GRutaCargas);
    Ok = LeeBD5.EjecutaSP("AS_BATCHPCI", "pa_subearchivos", MsjLog, Reportar);
    // Ok = LeeBD5.EjecutaSP("AS_BATCHPCI_comp", "pa_subearchivos", MsjLog, Reportar)GB
    // modificar la base de datos

    // ----------------------------------------------------------------------
    // 7.4.>> Arma la reportería de la validación de formato
    // ----------------------------------------------------------------------
    if (Ok)
    {
        NombreRpt = "AS0011_ValidaTP";
        TituloRpt = "RESUMEN DE VALIDACIONES";
        GenReporte5.CreaDirectorio(VB6.Format(PFecha, "yyMMdd") + @"\BRD");
        GenReporte5.CreaQuiebres(1, 0);
        GenReporte5.DimensionaArreglo("ConfiguraDetalle", 8, 5);
        GenReporte5.ConfiguraDetalle(0, 1, 26, "Letras", "Dercha", "TipoRegis");
        GenReporte5.ConfiguraDetalle(1, 2, 8, "Letras", "Dercha", "Regs.");
        GenReporte5.ConfiguraDetalle(2, 3, 14, "Moneda", "Dercha", "Monto");
        GenReporte5.ConfiguraDetalle(3, 4, 22, "Letras", "Centro", "CodValidacion");
        GenReporte5.ConfiguraDetalle(4, 5, 8, "Letras", "Izqrda", "Inicia");
        GenReporte5.ConfiguraDetalle(5, 6, 8, "Letras", "Izqrda", "Finali");
        GenReporte5.ConfiguraDetalle(6, 7, 60, "Letras", "Izqrda", "CampoValida");
        GenReporte5.ConfiguraDetalle(7, 8, 99, "Letras", "Izqrda", "Validacion");
        // -- Recordar siempre colcar en orden las columnas
        GenReporte5.DimensionaArreglo("ResumenPieQuiebre", 0, 0);
        // GenReporte5.CreaResumenQuiebres 0, 3, "Suma", 2, 1, 0               '-- Valor
        GenReporte5.ReporteCompensacion(0.ToString(), NombreRpt, TituloRpt, 180, Conversions.ToDate(Conversions.ToString(PFecha)), Reportar);
    }
    else
    {
        return SubeFileBanredRet;
    }

    // ----------------------------------------------------------------------
    // 7.5.>> Invoca al proceso que sube el archivo de BANRED
    // ----------------------------------------------------------------------
    LeeBD5.DepuraParametros();
    LeeBD5.QParametros("pv_tipo", basGeneral.aVCarac, basGeneral.aIngres, PTipo);
    LeeBD5.QParametros("pv_FchProc", basGeneral.aVCarac, basGeneral.aIngres, PFecha);
    LeeBD5.QParametros("pv_valida", basGeneral.aECorto, basGeneral.aIngres, 2);
    Ok = LeeBD5.EjecutaSP("AS_BATCHPCI", "pa_valida_archivos", MsjLog, Reportar2);
    // Ok = LeeBD5.EjecutaSP("AS_BATCHPCI_comp", "pa_valida_archivos", MsjLog, Reportar2)GB
    // modificar la base de datos

    // ----------------------------------------------------------------------
    // 7.6.>> Arma la reportería de la segunda validación
    // ----------------------------------------------------------------------
    if (Ok)
    {
        NombreRpt = "AS0012_ValidaTP";
        TituloRpt = "VALIDACION TP VS MAUS";
        GenReporte5.CreaDirectorio(VB6.Format(PFecha, "yyMMdd") + @"\BRD");
        GenReporte5.CreaQuiebres(1, 0);
        GenReporte5.DimensionaArreglo("ConfiguraDetalle", 5, 5);
        GenReporte5.ConfiguraDetalle(0, 1, 10, "Letras", "Izqrda", "Orden");
        GenReporte5.ConfiguraDetalle(1, 2, 28, "Letras", "Izqrda", "Institucion");
        GenReporte5.ConfiguraDetalle(2, 3, 16, "Moneda", "Dercha", "File-TP");
        GenReporte5.ConfiguraDetalle(3, 4, 16, "Moneda", "Dercha", "File-Maus");
        GenReporte5.ConfiguraDetalle(4, 5, 16, "Moneda", "Dercha", "Diferencia");
        // -- Recordar siempre colcar en orden las columnas
        GenReporte5.DimensionaArreglo("ResumenPieQuiebre", 3, 5);
        GenReporte5.CreaResumenQuiebres(0, 3, "Suma", 0); // -- # Emi
        GenReporte5.CreaResumenQuiebres(1, 4, "Suma", 0); // -- # Emi
        GenReporte5.CreaResumenQuiebres(2, 5, "Suma", 0); // -- # Emi
        GenReporte5.ReporteCompensacion(0.ToString(), NombreRpt, TituloRpt, 96, Conversions.ToDate(Conversions.ToString(PFecha)), Reportar2);
    }
    else
    {
        return SubeFileBanredRet;
    }

    SubeFileBanredRet = true;
    return SubeFileBanredRet;
}

// ----------------------------------------------------------------------------------------------------------------------------
// 8.>> Marca como procesada la ejecución
// ----------------------------------------------------------------------------------------------------------------------------
private void EjecucionProcesada(string pp_razon, string pp_qproc, string pp_valor)
{
    bool Ok;
    var LeeBD5 = new Cnxbd();
    var ComoProceso = default(object);
    var Mensaje = default(string);
    AdmFallo.PuntoSistema("EjecucionProcesada", 3);
    LeeBD5.DepuraParametros();
    LeeBD5.QParametros("pp_razon", basGeneral.aVCarac, basGeneral.aIngres, pp_razon);
    LeeBD5.QParametros("pp_qproc", basGeneral.aVCarac, basGeneral.aIngres, VB6.Format(pp_qproc, "yyyy-mm-dd"));
    LeeBD5.QParametros("pp_opcion", basGeneral.aVCarac, basGeneral.aIngres, "P");
    LeeBD5.QParametros("pp_valor", basGeneral.aVCarac, basGeneral.aIngres, pp_valor);
    Ok = LeeBD5.EjecutaSP("AS_BATCHPCI", "pa_batchcompensacion", Mensaje, ComoProceso);
    // Ok = LeeBD5.EjecutaSP("AS_BATCHPCI_comp", "pa_batchcompensacion", Mensaje, ComoProceso)GB
    // modificar la base de datos

    if (!Ok)
        AdmFallo.GeneraLogDetalle("Lo siento no se pudo marcar como procesada");
}

// ----------------------------------------------------------------------------------------------------------------------------
// 9.>> Guarda el archivo como procesado
// ----------------------------------------------------------------------------------------------------------------------------
private void ArchivoProcesado(ref DateTime FechaFile)
{
    var oFileSys = new Scripting.FileSystemObject();
    var GenReporte9 = new CGeneraArchivo();
    short iSeq;
    string FiledeOrigen;
    string FileProcesado;
    string RutaProcesado;
    ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
    /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo Fallo' at character 576


    Input:

            On Error GoTo Fallo

     */
    AdmFallo.PuntoSistema("ArchivoProcesado", 3);
    // --------------------------------------------------------------------------------
    // 9.1.>> Crea el directorio de Procesado
    // --------------------------------------------------------------------------------
    RutaProcesado = GRutaCargas + VB6.Format(FechaFile, "YYMM");
    RutaProcesado = Strings.Replace(RutaProcesado, "'", "");
    GenReporte9.CreaDirectorio(RutaProcesado, true);

    // --------------------------------------------------------------------------------
    // 9.1.>> Guarda el archivo AUSTP
    // --------------------------------------------------------------------------------
    FiledeOrigen = GRutaCargas + "AUSTP" + VB6.Format(FechaFile, "yyMMdd") + ".txt";
    FileProcesado = RutaProcesado + @"\PROCESADO_" + VB6.Format(FechaFile, "mmdd") + "_AUSTP" + VB6.Format(FechaFile, "yyMMdd") + ".txt";
    FiledeOrigen = Strings.Replace(FiledeOrigen, "'", "");
    oFileSys.CopyFile(FiledeOrigen, FileProcesado, true);
    oFileSys.DeleteFile(FiledeOrigen);

    // --------------------------------------------------------------------------------
    // 9.2.>> Guarda el archivo MAUS
    // --------------------------------------------------------------------------------
    FiledeOrigen = GRutaCargas + "MAUS" + VB6.Format(FechaFile, "MMdd") + ".rpt";
    FileProcesado = RutaProcesado + @"\PROCESADO_" + VB6.Format(FechaFile, "mmdd") + "_MAUS" + VB6.Format(FechaFile, "MMdd") + ".rpt";
    FiledeOrigen = Strings.Replace(FiledeOrigen, "'", "");
    oFileSys.CopyFile(FiledeOrigen, FileProcesado, true);
    oFileSys.DeleteFile(FiledeOrigen);
Fallo:
    ;
    if (Information.Err().Number != 0)
        AdmFallo.GeneraLogDetalle(Information.Err().Description);
}

// ----------------------------------------------------------------------------------------------------------------------------
// 10.>> Para Verificar el día laborable
// ----------------------------------------------------------------------------------------------------------------------------
private bool DiaLaborable()
{
    bool DiaLaborableRet = default;
    switch (DateAndTime.DatePart(DateInterval.Weekday, DateAndTime.Today))
    {
        case 1:
            {
                DiaLaborableRet = DS1;
                break;
            }

        case 2:
            {
                DiaLaborableRet = DS2;
                break;
            }

        case 3:
            {
                DiaLaborableRet = DS3;
                break;
            }

        case 4:
            {
                DiaLaborableRet = DS4;
                break;
            }

        case 5:
            {
                DiaLaborableRet = DS5;
                break;
            }

        case 6:
            {
                DiaLaborableRet = DS6;
                break;
            }

        case 7:
            {
                DiaLaborableRet = DS7;
                break;
            }
    }

    return DiaLaborableRet;
}
 // Install-Package Microsoft.VisualBasic

internal partial class SurroundingClass
{
    // ----------------------------------------------------------------------------------------------------------------------------
    // 11.>> Le pone en falso cuando ha pasado un día o ya es la hora de proceso
    // ----------------------------------------------------------------------------------------------------------------------------
    private bool FuncionProcesada()
    {
        bool FuncionProcesadaRet = default;
        AdmFallo.PuntoSistema("FuncionProcesada", 3);
        FuncionProcesadaRet = true;
        // If DateDiff("n", IniProc, Now) > 1438 Then FuncionProcesada = False
        // If Time > "07:58" And Time < "08:00" Then FuncionProcesada = False
        // If Not FuncionProcesada Then
        IniProc = DateAndTime.Now;
        return FuncionProcesadaRet;
    }

    // ----------------------------------------------------------------------------------------------------------------------------
    // 12.>> Verifica si existen los archivos de carga
    // ----------------------------------------------------------------------------------------------------------------------------
    private bool ExisteFileParaCargar()
    {
        bool ExisteFileParaCargarRet = default;
        var vFiles = new Scripting.FileSystemObject();
        Scripting.Folder vFolder;
        Scripting.Files aFiles;
        var ExisteMaus = default(bool);
        var ExisteAusTP = default(bool);
        DateTime ScaneaFileini;
        DateTime ScaneaFilefin;
        ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo Fallo' at character 1339


        Input:

                On Error GoTo Fallo

         */
        ScaneaFileini = DateAndTime.Now;
        ScaneaFilefin = DateAndTime.DateAdd(DateInterval.Minute, 90d, DateAndTime.Now);
        while (ScaneaFileini < ScaneaFilefin & ExisteFileParaCargarRet == false)
        {
            ExisteFileParaCargarRet = false;
            vFolder = vFiles.GetFolder(Strings.Replace(GRutaCargas, "'", ""));
            aFiles = vFolder.Files;
            foreach (Scripting.File nFile in aFiles)
            {
                if (Strings.Mid(nFile.Name, 1, 4) == "MAUS")
                    ExisteMaus = true;
                if (Strings.Mid(nFile.Name, 1, 5) == "AUSTP")
                    ExisteAusTP = true;
                if (ExisteMaus & ExisteAusTP)
                    ExisteFileParaCargarRet = true;
            }

            ScaneaFileini = DateAndTime.Now;
        }

        if (!ExisteFileParaCargarRet)
            AdmFallo.GeneraLogDetalle("No Existe File para cargar");
        Fallo:
        ;
        if (Information.Err().Number != 0)
            AdmFallo.GeneraLogDetalle(Information.Err().Description);
        return ExisteFileParaCargarRet;
    }

    // ----------------------------------------------------------------------------------------------------------------------------
    // 13.>> SyncExe_TaskFinished
    // ----------------------------------------------------------------------------------------------------------------------------
    private void SyncExe_TaskFinished()
    {
        AdmFallo.GeneraLogResumen("I", "AS_DCompensa.Form_Load", "Proceso Cancelado");
        // UPGRADE_NOTE: El objeto Crypto no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        Crypto = null;
        Environment.Exit(0);
    }
}
