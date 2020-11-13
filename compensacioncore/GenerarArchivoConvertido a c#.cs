{
    
    private object VB_Name = "CGeneraArchivo";
    private object VB_GlobalNameSpace = false;
    private object VB_Creatable = true;
    private object VB_PredeclaredId = false;
    private object VB_Exposed = false;
    // Constantes

    private const string PieFinal = "// Sistema TD del Banco del Austro, AS_DCompensa ";
    // Variables
    private short iFile;
    private string RutaReporte;
    private short AnchoRpt;
    private object[,] CabQuiebre;
    private object[,] PieQuiebre;
    private object[,] ConfigDet;
    private string[,] LineaTot;
    private short LineasPag;


 // Install-Package Microsoft.VisualBasic

 // Clases
    private CAdminExcepcion AdmExcep = new CAdminExcepcion();

    // --------------------------------------------------------------------------------
    // 1.>> Crea el Directorio - Archivo y abre el archivo para escritura
    // --------------------------------------------------------------------------------
    public void CreaDirectorio(ref string DirCarpeta, [Optional, DefaultParameterValue(false)] ref bool Forzado)
{
    object VLman_arch;
    string FechaRpt;
    AdmExcep.PuntoSistema("CreaDirectorio", 3);
    VLman_arch = new Scripting.FileSystemObject();
    if (Forzado)
    {
        RutaReporte = DirCarpeta + @"\";
    }
    else
    {
        RutaReporte = GRutaReport + DirCarpeta + @"\";
    }

    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto VLman_arch.FolderExists. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    if (Conversions.ToBoolean(VLman_arch.FolderExists(RutaReporte)))
    {
        AdmExcep.GeneraLogDetalle("Directorio " + RutaReporte + " ya esta creado.");
    }
    else
    {
        FileSystem.MkDir(RutaReporte);
    }
}


    // --------------------------------------------------------------------------------
    // 2.>> Se setean las columnas que van a servir de quiebre
    // --------------------------------------------------------------------------------
    public void DimensionaArreglo(ref string QArreglo, ref short RRegs, ref short RCols)
{
    AdmExcep.PuntoSistema("DimensionaArreglo", 3);
    switch (QArreglo ?? "")
    {
        case "ResumenPieQuiebre":
            {
                 ReDim PieQuiebre(RRegs, RCols)
                 break;
            }

        case "ConfiguraDetalle":
            {
                ReDim ConfigDet(RRegs, RCols)
                break;
            }
    }
}


    // --------------------------------------------------------------------------------
    // 3.>> Se configura como se va a registra el detalle del archivo
    // --------------------------------------------------------------------------------
   public void ConfiguraDetalle(ref short CReg, ref short Ccolumna, ref short CEspacio, ref string CFormato, ref string CAlineacion, ref string CNombre)
{
    AdmExcep.PuntoSistema("ConfiguraDetalle", 3);
    ConfigDet(CReg, 0) = Ccolumna;
    ConfigDet(CReg, 1) = CEspacio;
    ConfigDet(CReg, 2) = CFormato;
    ConfigDet(CReg, 3) = CAlineacion;
    ConfigDet(CReg, 4) = CNombre;
}

}
// --------------------------------------------------------------------------------
// 4.>> Se setean las columnas que van a servir de quiebre
// --------------------------------------------------------------------------------
// --------------------------------------------------------------------------------
// 4.>> Se setean las columnas que van a servir de quiebre
// --------------------------------------------------------------------------------
public void CreaQuiebres(ref short Quiebres, ref short ColQuiebre1, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre2, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre3, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre4, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre5, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre6, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre7, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre8, [Optional, DefaultParameterValue(-1)] ref short ColQuiebre9)
{
    AdmExcep.PuntoSistema("CreaQuiebres", 3);
    ;
#error Cannot convert ReDimStatementSyntax - see comment for details
    ReDim CabQuiebre(Quiebres, 2)

    
    CabQuiebre(0, 0) = ColQuiebre1;
    if (ColQuiebre2 > -1)
        CabQuiebre(1, 0) = ColQuiebre2;
    if (ColQuiebre3 > -1)
        CabQuiebre(2, 0) = ColQuiebre3;
    if (ColQuiebre4 > -1)
        CabQuiebre(3, 0) = ColQuiebre4;
    if (ColQuiebre5 > -1)
        CabQuiebre(4, 0) = ColQuiebre5;
    if (ColQuiebre6 > -1)
        CabQuiebre(5, 0) = ColQuiebre6;
    if (ColQuiebre7 > -1)
        CabQuiebre(6, 0) = ColQuiebre7;
}


// --------------------------------------------------------------------------------
// 5.>> Se setean las columnas que van a servir de quiebre
// --------------------------------------------------------------------------------
public void CreaResumenQuiebres(ref short RReg, ref short RColumna, ref string RTipo, ref short RQuiebre1, [Optional, DefaultParameterValue(-1)] ref short Rquiebre2, [Optional, DefaultParameterValue(-1)] ref short Rquiebre3, [Optional, DefaultParameterValue(-1)] ref short Rquiebre4, [Optional, DefaultParameterValue(-1)] ref short Rquiebre5, [Optional, DefaultParameterValue(-1)] ref short Rquiebre6, [Optional, DefaultParameterValue(-1)] ref short Rquiebre7, [Optional, DefaultParameterValue(-1)] ref short Rquiebre8, [Optional, DefaultParameterValue(-1)] ref short Rquiebre9)
{
    AdmExcep.PuntoSistema("CreaResumenQuiebres", 3);
    PieQuiebre(RReg, 0) = RColumna;
    PieQuiebre(RReg, 1) = RTipo;
    PieQuiebre(RReg, 2) = RQuiebre1;
    if (Rquiebre2 > -1)
        PieQuiebre(RReg, 3) = Rquiebre2;
    if (Rquiebre3 > -1)
        PieQuiebre(RReg, 4) = Rquiebre3;
    if (Rquiebre4 > -1)
        PieQuiebre(RReg, 5) = Rquiebre4;
    if (Rquiebre5 > -1)
        PieQuiebre(RReg, 6) = Rquiebre5;
    if (Rquiebre6 > -1)
        PieQuiebre(RReg, 7) = Rquiebre6;
    if (Rquiebre7 > -1)
        PieQuiebre(RReg, 8) = Rquiebre7;
    if (Rquiebre8 > -1)
        PieQuiebre(RReg, 9) = Rquiebre8;
}


 // ----------------------------------------------------------------------------------------------------------------------------
    // 6.>> Proceso que genera los Reportes de compensación
    // ----------------------------------------------------------------------------------------------------------------------------
public bool ReporteCompensacion(ref string REntidad, ref string RNombreRpt, ref string TituloRpt, ref short RAnchoRpt, ref DateTime RFecha, ref object RData)
{
    bool ReporteCompensacionRet = default;

    // --------------------------------------------------------------------------------
    // Crea el Directorio y Archivo
    // --------------------------------------------------------------------------------
    int Reg;
    short Q;
    string GArchivo;
    short RPaginas;
    short Registros;

    // --------------------------------------------------------------------------------
    // Inicializa
    // --------------------------------------------------------------------------------
    AdmExcep.PuntoSistema("ReporteCompensacion", 3);
    AnchoRpt = RAnchoRpt;
    RPaginas = 1;
    Registros = 1;
    LineasPag = 1;
    ReporteCompensacionRet = false;
    GArchivo = RutaReporte + @"\" + RNombreRpt + "_" + VB6.Format(RFecha, "yyMMdd") + ".txt";
    iFile = FileSystem.FreeFile();

    // ------------------------------------------------------------------------------------
    // Escritura del Archivo
    // ------------------------------------------------------------------------------------
    FileSystem.FileOpen(iFile, GArchivo, OpenMode.Output);
    ArmaCabecera(TituloRpt, Conversions.ToString(RFecha), RPaginas);
    var loopTo = Information.UBound((Array)RData, 2);
    for (Reg = 0; Reg <= loopTo; Reg++)
    {
        ArmaQuiebres("Encabezado", RData, Reg);
        ArmaDetalle(RData, Reg);
        ArmaQuiebres("PieQuiebre", RData, Reg);
        if (Registros > 30 | LineasPag > 40)
        {
            EscribeLinea("");
            EscribeLinea(Strings.Right(new string(" ", AnchoRpt) + PieFinal + QVersion + "//", AnchoRpt));
            EscribeLinea("");
            RPaginas = (short)(RPaginas + 1);
            ArmaCabecera(TituloRpt, Conversions.ToString(RFecha), RPaginas);
            Registros = 1;
            LineasPag = 1;
        }

        Registros = (short)(Registros + 1);
    }

    FileSystem.FileClose(iFile);
    ReporteCompensacionRet = true;
    return ReporteCompensacionRet;
}

// ----------------------------------------------------------------------------------------------------------------------------
// 7.>> Proceso que genera los Reportes de compensación
// ----------------------------------------------------------------------------------------------------------------------------
private void ArmaCabecera(string TituloArc, ref string FechaRept, ref short Paginas)
{
    object FechaImpr;
    object NumPagina;
    object Principal;

    // ----------------------------------
    // Declara Variables
    // ----------------------------------
    string QRellenar;
    string LineaCab;
    short i;
    short e;
    string NombreCol;
    string LineaNombres;
    short MargenTit;
    short MargenCab;



    // ----------------------------------
    // Inicializa Variables
    // ----------------------------------
    AdmExcep.PuntoSistema("ArmaCabecera", 4);
    MargenCab = AnchoRpt / 3;
    MargenTit = (short)(MargenCab - 12);
    NombreCol = new string(' ', 4);
    LineaCab = new string("-", AnchoRpt);
    QRellenar = new string(' ', MargenCab);
    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Principal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    Principal = "Banco del Austro";
    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NumPagina. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    NumPagina = "Pagina:" + Paginas.ToString();
    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto FechaImpr. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    FechaImpr = "Impreso: " + VB6.Format(DateAndTime.Today, "yy-mm-dd") + " " + Conversions.ToString(DateAndTime.TimeOfDay);
    TituloArc = FormatearTitulo(TituloArc);
    MargenTit = (AnchoRpt - Strings.Len(TituloArc)) / 2;
    FechaRept = "Fecha Contable: " + FechaRept;

    // --------------------------------------------------------------------------------------------------------
    // Coloca la plantilla con el Titulo, nombre del Banco, Pagina, Fecha-Contable y Fecha de Impresión
    // --------------------------------------------------------------------------------------------------------
    EscribeLinea("");
    EscribeLinea(Strings.Left(QRellenar + QRellenar, MargenTit) + TituloArc + Strings.Right(QRellenar + QRellenar, MargenTit));
    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NumPagina. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Principal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    EscribeLinea(Strings.Left(Conversions.ToString(Operators.ConcatenateObject(Principal, QRellenar)), MargenCab) + QRellenar + Strings.Right(Conversions.ToString(Operators.ConcatenateObject(QRellenar, NumPagina)), MargenCab));
    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto FechaImpr. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
    EscribeLinea(Strings.Left(FechaRept + QRellenar, MargenCab) + QRellenar + Strings.Right(Conversions.ToString(Operators.ConcatenateObject(QRellenar, FechaImpr)), MargenCab));

    // --------------------------------------------------------------------------------------------------------
    // Se coloca los nombres de los campos o columnas del reporte según lo configurado en [ConfiguraDetalle]
    // --------------------------------------------------------------------------------------------------------
    EscribeLinea(LineaCab);
    var loopTo = (short)Information.UBound(ConfigDet);
    for (i = 0; i <= loopTo; i++)
    {
        switch (ConfigDet(i, 3))
        {
            case "Dercha":
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(i, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(i, 4). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    NombreCol = NombreCol + Strings.Right(new string(" ", ConfigDet(i, 1)) + ConfigDet(i, 4), ConfigDet(i, 1));
                    break;
                }

            case "Centro":
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    e = (ConfigDet(i, 1) - Len(ConfigDet(i, 4))) / 2;
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(i, 4). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    NombreCol = NombreCol + Strings.Left(new string(" ", ConfigDet(i, 1)), e) + ConfigDet(i, 4) + Strings.Right(new string(" ", ConfigDet(i, 1)), e);
                    break;
                }

            case "Izqrda":
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(i, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    NombreCol = NombreCol + Strings.Left(ConfigDet(i, 4) + new string(" ", ConfigDet(i, 1)), ConfigDet(i, 1));
                    break;
                }
        }
    }

    EscribeLinea(NombreCol);
    EscribeLinea(LineaCab);
    EscribeLinea("");
}
// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// 8.>> Arma el Quiebre Tanto el Encabezado como el Pie de Quiebre según la configuración realizada al invocar [CreaQuiebres] y [CreaResumenQuiebres]
// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
private void ArmaQuiebres(ref string Seccion, [Optional, DefaultParameterValue(null)] ref object DataParaQuiebre, [Optional, DefaultParameterValue(0)] ref int R)
{
    short C;
    short Q;
    short x;
    short ResumirCol;
    short ColResumen;
    var ColQuiebre = default(short);
    short QuiebreIni;
    short QuiebreFin;
    short EnQuiebre;
    short LenPie;
    var Valor = default(string);
    short ColDetalle;
    bool EncerarQuiebre;
    AdmExcep.PuntoSistema("ArmaQuiebres", 4);
    switch (Seccion ?? "")
    {
        case "Encabezado":
            {
                var loopTo = (short)(Information.UBound(CabQuiebre, 1) - 1);
                for (Q = (short)0; Q <= loopTo; Q++)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto CabQuiebre(Q, 0). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    x = CabQuiebre(Q, 0);
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DataParaQuiebre(x, R). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto CabQuiebre(Q, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    if (!Operators.ConditionalCompareObjectEqual(CabQuiebre(Q, 1), DataParaQuiebre((object)x, (object)R), false))
                    {
                        EscribeLinea(DataParaQuiebre((object)x, (object)R));
                    }
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DataParaQuiebre(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto CabQuiebre(Q, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CabQuiebre(Q, 1) = DataParaQuiebre((object)x, (object)R);
                }

                break;
            }
        // Redimensiona según las columnas y los Quiebres lo que sera el arreglo temporal que contendrá lo que se imprimirá como resumen
        // ----------------------------------------------------------------
        case "PieQuiebre": // ----------------------------------------------------------------
            {
                ;
#error Cannot convert ReDimStatementSyntax - see comment for details
                /* Cannot convert ReDimStatementSyntax, System.InvalidCastException: Unable to cast object of type 'Microsoft.CodeAnalysis.VisualBasic.Symbols.ErrorTypeSymbol' to type 'Microsoft.CodeAnalysis.IArrayTypeSymbol'.
                   at ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.CreateNewArrayAssignment(ExpressionSyntax vbArrayExpression, ExpressionSyntax csArrayExpression, List`1 convertedBounds, Int32 nodeSpanStart) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\MethodBodyExecutableStatementVisitor.cs:line 366
                   at ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.ConvertRedimClauseAsync(RedimClauseSyntax node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\MethodBodyExecutableStatementVisitor.cs:line 266
                   at ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitReDimStatement>b__40_0(RedimClauseSyntax arrayExpression) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\MethodBodyExecutableStatementVisitor.cs:line 248
                   at ICSharpCode.CodeConverter.Shared.AsyncEnumerableTaskExtensions.SelectAsync[TArg,TResult](IEnumerable`1 source, Func`2 selector)
                   at ICSharpCode.CodeConverter.Shared.AsyncEnumerableTaskExtensions.SelectManyAsync[TArg,TResult](IEnumerable`1 nodes, Func`2 selector) in D:\GitWorkspace\CodeConverter\CodeConverter\Shared\AsyncEnumerableTaskExtensions.cs:line 18
                   at ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.VisitReDimStatement(ReDimStatementSyntax node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\MethodBodyExecutableStatementVisitor.cs:line 248
                   at ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.AddLocalVariablesAsync(VisualBasicSyntaxNode node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\HoistedNodeStateVisitor.cs:line 47
                   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisitInnerAsync(SyntaxNode node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\CommentConvertingMethodBodyVisitor.cs:line 29

                Input:
                                ' Redimensiona según las columnas y los Quiebres lo que sera el arreglo temporal que contendrá lo que se imprimirá como resumen
                                '----------------------------------------------------------------
                                ReDim LineaTot(UBound(CabQuiebre, 1), 2)

                 */
                if (Information.UBound(PieQuiebre, 1) == 0)
                    return;
                // ----------------------------------------------------------------
                // Inicializa las Variables
                // ----------------------------------------------------------------
                QuiebreIni = (short)2; // Siempre empezará en 2
                QuiebreFin = (short)(2 + Information.UBound(CabQuiebre, 1) - 1);
                EnQuiebre = (short)0;

                // ----------------------------------------------------------------
                // Para ir realizando las operaciones de resumen por cada quiebre
                // ----------------------------------------------------------------
                var loopTo1 = QuiebreFin;
                for (EnQuiebre = QuiebreIni; EnQuiebre <= loopTo1; EnQuiebre++)
                {
                    ResumirCol = (short)0;
                    x = (short)(Information.UBound(CabQuiebre, 1) + (int)EnQuiebre);
                    // ----------------------------------------------------------------
                    // Para ir revisando cada columna
                    // ----------------------------------------------------------------
                    var loopTo2 = (short)(Information.UBound(ConfigDet, 1) - 1);
                    for (ColDetalle = (short)0; ColDetalle <= loopTo2; ColDetalle++)
                    {

                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(ResumirCol, EnQuiebre). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        ColQuiebre = PieQuiebre(ResumirCol, EnQuiebre); // Cambiara cuando sea una columna de resumen
                                                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(ResumirCol, 0). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        ColResumen = PieQuiebre(ResumirCol, 0);

                        // ----------------------------------------------------------------
                        // Verifica que sea una columna de resumen
                        // ----------------------------------------------------------------
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(ColDetalle, 0). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        if (ConfigDet(ColDetalle, 0) < ColResumen)
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(ColDetalle, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            LineaTot(ColQuiebre, 0) = LineaTot(ColQuiebre, 0) + new string(ColDetalle.ToString(), ConfigDet(ColDetalle, 1));
                        }
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(ColDetalle, 0). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        else if (ConfigDet(ColDetalle, 0) == ColResumen)
                        {
                            if ((int)ResumirCol == 0)
                            {
                                LenPie = (short)Len(LineaTot(ColQuiebre, 0));
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DataParaQuiebre(ColQuiebre, R). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                LineaTot(ColQuiebre, 0) = new string(' ', 4) + Strings.Right(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(new string(' ', (int)LenPie) + "Totales por ", DataParaQuiebre((object)ColQuiebre, (object)R)), ": ")), (int)LenPie);
                                LineaTot(ColQuiebre, 1) = "No";
                            }
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DataParaQuiebre(ColResumen, R). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(ResumirCol, x). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            PieQuiebre(ResumirCol, x) = PieQuiebre(ResumirCol, x) + DataParaQuiebre((object)ColResumen, (object)R);
                            switch (ConfigDet(ColDetalle, 2))
                            {
                                case "Numero":
                                    {
                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                        Valor = Strings.FormatNumber(PieQuiebre(ResumirCol, x), 0);
                                        break;
                                    }

                                case "Moneda":
                                    {
                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                        Valor = Strings.FormatCurrency(PieQuiebre(ResumirCol, x));
                                        break;
                                    }
                            }

                            switch (ConfigDet(ColDetalle, 3))
                            {
                                case "Dercha":
                                    {
                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(ColDetalle, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                        Valor = Strings.Right(new string(" ", ConfigDet(ColDetalle, 1)) + Valor, ConfigDet(ColDetalle, 1));
                                        break;
                                    }

                                case "Centro":
                                    {
                                        MsgBox("Centrar");
                                        break;
                                    }

                                case "Izqrda":
                                    {
                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(ColDetalle, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                        Valor = Strings.Left(Valor + new string(" ", ConfigDet(ColDetalle, 1)), ConfigDet(ColDetalle, 1));
                                        break;
                                    }
                            }

                            LineaTot(ColQuiebre, 0) = LineaTot(ColQuiebre, 0) + Valor;
                            ResumirCol = (short)((int)ResumirCol + 1);
                        }
                    }

                    // ----------------------------------------------------------------
                    // Cuando se realiza un quiebre se debe mostrar el total
                    // ----------------------------------------------------------------
                    if (Information.UBound((Array)DataParaQuiebre, 2) > R & (int)ColDetalle == Information.UBound(ConfigDet, 1))
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DataParaQuiebre(ColQuiebre, R + 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DataParaQuiebre(ColQuiebre, R). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(DataParaQuiebre((object)ColQuiebre, (object)R), DataParaQuiebre((object)ColQuiebre, (object)(R + 1)), false)))
                        {
                            EscribeLinea(Strings.Right(new string(" ", AnchoRpt) + new string("-", Len(Strings.LTrim(LineaTot(ColQuiebre, 0)))), AnchoRpt));
                            EscribeLinea(LineaTot(ColQuiebre, 0));
                            EncerarQuiebre = true;
                            var loopTo3 = (short)(Information.UBound(PieQuiebre, 1) - 1);
                            for (ColDetalle = (short)0; ColDetalle <= loopTo3; ColDetalle++)
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(ColDetalle, x). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                PieQuiebre(ColDetalle, x) = 0;
                            EscribeLinea("");
                        }
                    }

                    if (Information.UBound((Array)DataParaQuiebre, 2) == R & (int)ColDetalle == Information.UBound(ConfigDet, 1))
                    {
                        EscribeLinea(Strings.Right(new string(" ", AnchoRpt) + new string("-", Len(Strings.LTrim(LineaTot(ColQuiebre, 0)))), AnchoRpt));
                        EscribeLinea(LineaTot(ColQuiebre, 0));
                        EncerarQuiebre = true;
                        var loopTo4 = (short)(Information.UBound(PieQuiebre, 1) - 1);
                        for (ColDetalle = (short)0; ColDetalle <= loopTo4; ColDetalle++)
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto PieQuiebre(ColDetalle, x). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            PieQuiebre(ColDetalle, x) = 0;
                        EscribeLinea("");
                    }
                }

                break;
            }
    }
}

 // Install-Package Microsoft.VisualBasic

internal partial class SurroundingClass
{
    // ----------------------------------------------------------------------------------------------------------------------------
    // 9.>> Arma la linea de Detalle según la configuarción que se haya inicializado al invocar el [Sub ConfiguraDetalle]
    // ----------------------------------------------------------------------------------------------------------------------------
    private void ArmaDetalle(ref object RData, ref int RFila)
    {
        short i;
        string EDetalle;
        var DData = default(string);
        short e;
        AdmExcep.PuntoSistema("ArmaDetalle", 4);
        EDetalle = new string(' ', 4);
        var loopTo = (short)(Information.UBound(ConfigDet) - 1);
        for (i = 0; i <= loopTo; i++)
        {
            switch (ConfigDet(i, 2))
            {
                case "Fechas":
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto RData(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DData = Strings.FormatDateTime(RData(ConfigDet(i, 0), (object)RFila));
                        break;
                    }

                case "Letras":
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto RData(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DData = Strings.RTrim(RData(ConfigDet(i, 0), (object)RFila));
                        break;
                    }

                case "Numero":
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto RData(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DData = Strings.FormatNumber(RData(ConfigDet(i, 0), (object)RFila), 0);
                        break;
                    }

                case "Moneda":
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto RData(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DData = Strings.FormatCurrency(RData(ConfigDet(i, 0), (object)RFila));
                        break;
                    }
            }

            switch (ConfigDet(i, 3))
            {
                case "Dercha":
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(i, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DData = Strings.Right(new string(" ", ConfigDet(i, 1)) + DData, ConfigDet(i, 1));
                        break;
                    }

                case "Centro":
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        e = (ConfigDet(i, 1) - Strings.Len(DData)) / 2;
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DData = Strings.Left(new string(" ", ConfigDet(i, 1)), e) + DData + Strings.Right(new string(" ", ConfigDet(i, 1)), e);
                        break;
                    }

                case "Izqrda":
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(i, 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ConfigDet(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DData = Strings.Left(DData + new string(" ", ConfigDet(i, 1)), ConfigDet(i, 1));
                        break;
                    }
            }

            EDetalle = EDetalle + DData;
        }

        EscribeLinea(ref EDetalle);
    }

    private void EscribeLinea(ref string Escribir)
    {
        AdmExcep.PuntoSistema("EscribeLinea", 5);
        LineasPag = LineasPag + 1;
        FileSystem.PrintLine(iFile, Escribir);
    }

    private string FormatearTitulo(ref string Ftitulo)
    {
        string FormatearTituloRet = default;
        object C;
        string TituloFormateado;
        AdmExcep.PuntoSistema("FormatearTitulo", 5);
        TituloFormateado = "";
        var loopTo = Strings.Len(Ftitulo);
        for (C = 1; C <= loopTo; C++)
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto C. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            TituloFormateado = TituloFormateado + Strings.Mid(Ftitulo, Conversions.ToInteger(C), 1) + " ";
        FormatearTituloRet = TituloFormateado;
        return FormatearTituloRet;
    }
}



}