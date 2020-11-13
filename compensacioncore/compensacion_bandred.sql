USE [OCTOPUS_PCI]
GO
/****** Object:  StoredProcedure [dbo].[compensacion_bandred]    Script Date: 8/9/2020 10:50:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[compensacion_bandred] --'2018-05-17'

  @tp_FchProc  varchar(12) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
set transaction isolation level read uncommitted   

Declare
        @tp_entidad   smallint    = 0,
     --   @tp_FchProc   varchar(12) = '2014-11-07',
        @tp_tipo      varchar(12) = '00'

    SET NOCOUNT ON; 
    SET DATEFIRST 7;
    ------------------------------------------------------------------------------------------------
    -- 0.>> Declaracion e inicialización de Variables 
    ------------------------------------------------------------------------------------------------
    Declare @l_FchProc         date   ,
            @l_Fchdesde        date   ,
            @l_cant_trx_austro integer,
            @l_cant_extractBRD integer,
            @l_cant_cuadratura integer,
            @l_fechaFile       char(16),
            -- Variables del cursor
            @c_Erradas      varchar(2) ,
            @c_Reversado    varchar(1) , -- ''-Normal ;    'R'=Reversada
            @c_TipoTrx      varchar(4) , -- Consultas='0031' , Retiros = '0001',  Transferencias = '0040'
            @c_TipoAba      varchar(6) , -- ADQ=Adquirente,  EMI=Emisor
            @c_Aba          varchar(6) = '493176'
                
	------------------------------------------------------------------------------------------------
    -- 1.>> Validar dias feriados 
    ------------------------------------------------------------------------------------------------


	------------------------------------------------------------------------------------------------
    -- 2.>> Dias laborables
    ------------------------------------------------------------------------------------------------
    Set @l_FchProc   = convert(date,@tp_FchProc)
    set @l_Fchdesde  = case when DATEPART(DW,@l_FchProc) <= 2 then DATEADD(DAY, DATEPART(DW,@l_FchProc)*(-1),@l_FchProc) else @l_FchProc end 
    set @l_fechaFile = REPLACE(convert(varchar(8),@l_FchProc,3) , '/', ''); 
        
    ---------------------------------------------------------------------------------------------------------
    --  2.>> Se crea y llena la tabla que contiene parametros de las Tarjetas Externas
    --------------------------------------------------------------------------------------------------------- 
    If OBJECT_ID('Tempdb..#Param_Tarjetas') is not null Drop Table #Param_Tarjetas ; 
    
    Create Table #Param_Tarjetas (te_Bin varchar(6), te_Banco varchar(30), te_fi integer,  te_Digitos smallint, te_orden smallint ) ;
    
    INSERT INTO #Param_Tarjetas
    Select ABA, Banco, BanredFI, 20, 0 from OCTOCLI_PCI..ATMBIN WITH(NOLOCK) where ABA in ('604869', '502916') UNION    
    Select ABA, Banco, BanredFI, 16, 0 from OCTOCLI_PCI..ATMBIN WITH(NOLOCK) where ABA not in ('360218', '360857', '365453', '604869', '502916') UNION
    Select ABA, Banco, BanredFI, 14, 0 from OCTOCLI_PCI..ATMBIN WITH(NOLOCK) where ABA in ('360218', '360857', '365453') 
    
    Update #Param_Tarjetas set te_orden =  0 where te_Banco like '%AUSTRO%'
    Update #Param_Tarjetas set te_orden =  1 where te_Banco = 'BOLIVARIANO'
    Update #Param_Tarjetas set te_orden = 24 where te_Banco = 'CITYBANK'
    Update #Param_Tarjetas set te_orden = 25 where te_Banco = 'BANCO DE FOMENTO    '
    Update #Param_Tarjetas set te_orden = 11 where te_Banco = 'BANCO DE LOJA       '
    Update #Param_Tarjetas set te_orden = 12 where te_Banco = 'BANCO DE MACHALA    '
    Update #Param_Tarjetas set te_orden =  6 where te_Banco = 'PACIFICO            '
    Update #Param_Tarjetas set te_orden = 19 where te_Banco = 'BANCO FINCA         '
    Update #Param_Tarjetas set te_orden = 17 where te_Banco = 'GUAYAQUIL           '
    Update #Param_Tarjetas set te_orden =  4 where te_Banco = 'INTERNACIONAL       '
    Update #Param_Tarjetas set te_orden = 13 where te_Banco = 'BANCO MM JARAMILLO A'
    Update #Param_Tarjetas set te_orden = 10 where te_Banco = 'PICHINCHA           '
    Update #Param_Tarjetas set te_orden = 10 where te_Banco = 'BCO. PICHINCHA      '
    Update #Param_Tarjetas set te_orden = 23 where te_Banco = 'RUMI¤AHUI           '
    Update #Param_Tarjetas set te_orden =  5 where te_Banco = 'BANCO SOLIDARIO     '
    Update #Param_Tarjetas set te_orden = 28 where te_Banco = 'UNIBANCO            '
    Update #Param_Tarjetas set te_orden = 22 where te_Banco like '%BANRED%'
    Update #Param_Tarjetas set te_orden = 14 where te_Banco like '%JEP%'
    Update #Param_Tarjetas set te_orden = 15 where te_Banco = 'COOP. 29 DE OCTUBRE '
    Update #Param_Tarjetas set te_orden =  8 where te_Banco = 'COOP. ALINZA DEL VA '
    Update #Param_Tarjetas set te_orden = 20 where te_Banco = 'COOP. OSCUS         '
    Update #Param_Tarjetas set te_orden = 18 where te_Banco = 'COOP. POLICIA NACION'
    Update #Param_Tarjetas set te_orden =  9 where te_Banco = 'DELBANK             '
    Update #Param_Tarjetas set te_orden =  3 where te_Banco = 'DINNERS             '
    Update #Param_Tarjetas set te_orden = 16 where te_Banco = 'El sagrario         '
    Update #Param_Tarjetas set te_orden =  7 where te_Banco = 'MUTUALIS. PICHINCHA '
    Update #Param_Tarjetas set te_orden = 21 where te_Banco = 'PROCREDIT           '
    
    
    ---------------------------------------------------------------------------------------------------------
    --  3.>> Se obtiene desde las bases del Austro la data a Cuadrar
    --------------------------------------------------------------------------------------------------------- 
    If OBJECT_ID('Tempdb..#Trx_Bco_Austro') is not null Drop Table #Trx_Bco_Austro ; 
        
    Create Table #Trx_Bco_Austro  
    (
            RFecha           date         NOT NULL ,            RHora            char(9)      NOT NULL ,
            RBIN             varchar(9)   NOT NULL ,            RTarjeta         varchar(20)  NOT NULL ,
            RTipoTrxCod      varchar(9)   NOT NULL ,            RTipoCuenta      tinyint      NOT NULL ,
            RValor           money        NOT NULL ,            RCodError        integer      NOT NULL ,
            RReferencia      varchar(15)  NOT NULL ,            RPosEntryMode    varchar(5)   NOT NULL ,
            RProcesador      varchar(3)   NOT NULL ,            Reversado        varchar(1)   NOT NULL ,
            RErrado          varchar(2)   NOT NULL ,            RTerminalId      varchar(8)   NOT NULL ,            
            RIndicaEMV       varchar(1)   NOT NULL ,            ROrgLink         char(8)      NOT NULL ,            
            RDestLink        char(8)      NOT NULL ,            RAcquiererCode   varchar(15)  NOT NULL ,            
            RIssuerCode      varchar(15)  NOT NULL ,            RAutoriza        varchar(6)   NOT NULL ,            
            RFchcontable     date         NOT NULL ,            R_id             int identity (1,1) ,
    )    
    Create Index #Ix_trx_01 on #Trx_Bco_Austro (RReferencia,RAutoriza,RTipoTrxCod,RTarjeta) ;       
       
    ---------------------------------------------------------------------------------------------------------
    --  4.>> Inserta la data a CONCILIAR
    ---------------------------------------------------------------------------------------------------------       
    insert into  #Trx_Bco_Austro
    Select [RFecha]        = CONVERT(date, FechaSwitch),
           [RHora]         = HoraSwitch ,
           [RBIN]          = SUBSTRING(Tarjeta,4,6),
           [RTarjeta]      = Tarjeta, 
           [RTipoTrxCod]   = RIGHT('0000' + convert(varchar,Transaccion),4),
           [RTipoCuenta]   = TipoCtaOrg, 
           [RValor]        = convert(money,[Valor]),
           [RCodError]     = RespCode,       
           [RReferencia]   = ReferenceNumber,
           [RPosEntryMode] = PosEntryMode ,
           [RProcesador]   = Case when DestLink = 'BANRED' and OrgLink  <> 'BANRED' then 'ADQ' 
                                  when OrgLink  = 'BANRED' and DestLink <> 'BANRED' then 'EMI' else '???' end,
           [Reversado]     = '',
           [RErrado]       = case when RespCode = 0 then 'No' else 'Si' end,
           
           [RTerminalId]   = TerminalID      ,
           [RIndicaEMV]    = IndicadorEMV    ,
           OrgLink,
           DestLink,
           AcquiererCode,
           IssuerCode,
           NumAutoriza,
           RFchcontable    = FechaContable
     FROM [OCTOPUS_PCI].[dbo].[SwTrans] S WITH(NOLOCK) 
    WHERE Fechacontable between CONVERT(DATE,@l_Fchdesde) and CONVERT(DATE,@l_FchProc)
      and NormalOReverso = 0
      and (OrgLink = 'Banred' or DestLink = 'Banred')
      and Tarjeta <> '0000000000000000000'  
--      and MsgId = '9051022002686454855704'   

    ---------------------------------------------------------------------------------------------------------
    --  5.>> Se Actualiza el bin para las tarjetas con # digitos distintos a 16
    ---------------------------------------------------------------------------------------------------------   
        Update #Trx_Bco_Austro
           set RBIN = te_Bin 
          from #Param_Tarjetas
         inner join #Trx_Bco_Austro on SUBSTRING(RIGHT(RTarjeta,te_digitos),1,6) = te_Bin collate SQL_Latin1_General_CP1_CI_AS
         where te_Digitos <> 16
    
    ---------------------------------------------------------------------------------------------------------
    --  6.>> Se identifican los que estan Reversados
    ---------------------------------------------------------------------------------------------------------  
      select *
	  into #TempSwTrans
	  from [OCTOPUS_PCI].[dbo].[SwTrans]  WITH(NOLOCK)
	  where NormalOReverso = 1
	  
	  
	  Update #Trx_Bco_Austro
         set Reversado = 'R'
        from #Trx_Bco_Austro
      --inner join [OCTOPUS_PCI].[dbo].[SwTrans]  WITH(NOLOCK)   
	  INNER JOIN #TempSwTrans
	  on (RTarjeta = Tarjeta collate SQL_Latin1_General_CP1_CI_AS                                              
          and FechaSwitch = RFecha
          --and NormalOReverso = 1 
          and RReferencia = ReferenceNumber collate SQL_Latin1_General_CP1_CI_AS)
                                              
    ---------------------------------------------------------------------------------------------------------
    --  7.>> Para las fechas anteriores solo se debe considerar las transacciones OK
    ---------------------------------------------------------------------------------------------------------   
    Delete #Trx_Bco_Austro where RFchcontable < CONVERT(DATE,@l_FchProc) and RErrado = 'Si'
    Delete #Trx_Bco_Austro where RFchcontable < CONVERT(DATE,@l_FchProc) and Reversado = 'R'
    Delete #Trx_Bco_Austro where RFchcontable < CONVERT(DATE,@l_FchProc) and RTipoTrxCod <> '0001'
    
  

    ------------------------------------------------------------------------------------
    --  8.>> Salida
    ------------------------------------------------------------------------------------    
    Select * 
	from #Trx_Bco_Austro WITH(NOLOCK)
        

    ------------------------------------------------------------------------------------  
    

END
