CREATE procedure Finish_Goods (
	in StartDatee timestamp,
	in EndDatee	timestamp,
	out TB  table (
		ItemCode                        nvarchar(20),
        ItemName                        nvarchar(100),
        ItmsGrpCod                        int,
        ItmsGrpNam                        nvarchar(30),

 

        --Receipt
        "Qty (Goods Receipt)"            numeric(21,6),
        "Amount (Goods Receipt)"        numeric(21,6),

 

        "Qty (Receipt from PO)"            numeric(21,6),
        "Amount (Receipt from PO)"        numeric(21,6),

 

        "Receipt Qty (Outsourcing)"        numeric(21,6),
        "Receipt Amount (Outsourcing)"    numeric(21,6),

 

        --Issue
        "Qty (Goods Issue)"                numeric(21,6),
        "Amount (Goods Issue)"            numeric(21,6),

 

        "Qty (Issue for PO)"            numeric(21,6),
        "Amount (Issue for PO)"            numeric(21,6),

 

        "Issue Qty (Outsourcing)"        numeric(21,6),
        "Issue Amount (Outsourcing)"    numeric(21,6),

 		 "Revaluation Amount" 			numeric(21,6), --Inventory Revaluation Amount

        --     Sum
        "FG Cmplt Qty"                    numeric(21,6),
        "FG Cmplt Amount"                numeric(21,6)
	)
)

as
begin
	declare StartDate timestamp = :StartDatee;
	declare EndDate timestamp = :EndDatee;
-------------
    create local temporary table #OIGN (
        DocEntry    nvarchar(20),
        DocNum        nvarchar(20),
        ItemCode    nvarchar(20),
        ItemName    nvarchar(100),
        ItmsGrpCod    int,
        ItmsGrpNam    nvarchar(30),
        Quantity    numeric(21,6),
        LineTotal    numeric(21,6),
        BaseType    nvarchar(50),
        ReceiptID    int,
        IssueID        int,
        JrnlMemo    nvarchar(50),
        Type        nvarchar(50)
    );
    insert into #OIGN  
    SELECT A."DocEntry" ,B."DocNum",A."ItemCode",c."ItemName",d."ItmsGrpCod",d."ItmsGrpNam", A."Quantity" ,A."LineTotal", A."BaseType", E."U_receiptID", null as "U_issueID", B."JrnlMemo"
    ,case
    when    A."BaseType" = 202 then 'Receipt from Production'
    when    A."BaseType" = -1 and  ifnull(E."U_receiptID",0) > 0 then 'Receipt Outsourcing'
    when     A."BaseType" = -1    then 'Goods Receipt'
    end as "Type" 
    FROM IGN1 A 
    LEFT JOIN OIGN B  ON A."DocEntry" = B."DocEntry"
    left join OITM c on c."ItemCode" = A."ItemCode"
    LEFT join OITB d on d."ItmsGrpCod" = c."ItmsGrpCod"
    left join "@TBL_OUTS_DOCNUM" E on E."U_receiptID" = A."DocEntry"
    Where d."ItmsGrpCod" in (120,121) and A."DocDate"  between StartDate and EndDate 
    ;
----------------    
    create local temporary table #OIGE (
        DocEntry    nvarchar(20),
        DocNum        nvarchar(20),
        ItemCode    nvarchar(20),
        ItemName    nvarchar(100),
        ItmsGrpCod    int,
        ItmsGrpNam    nvarchar(30),
        Quantity    numeric(21,6),
        LineTotal    numeric(21,6),
        BaseType    nvarchar(50),
        ReceiptID    int,
        IssueID        int,
        JrnlMemo    nvarchar(50),
        Type        nvarchar(50)
    );
    insert into #OIGE 
    SELECT A."DocEntry" ,B."DocNum",A."ItemCode",c."ItemName",d."ItmsGrpCod",d."ItmsGrpNam", A."Quantity" ,A."LineTotal" , A."BaseType", null as "U_receiptID" , E."U_issueID", B."JrnlMemo"
    ,case
    when    A."BaseType" = 202 then 'Issue for Production'
    when    A."BaseType" = -1 and  ifnull(E."U_issueID",0) > 0 then 'Issue Outsourcing'
    when     A."BaseType" = -1    then 'Goods Issue'
    end as "Type"
    FROM IGE1 A 
    LEFT JOIN OIGE B ON A."DocEntry" = B."DocEntry"
    left join OITM c on c."ItemCode" = A."ItemCode"
    LEFT join OITB d on d."ItmsGrpCod" = c."ItmsGrpCod"
    left join "@TBL_OUTS_DOCNUM" E on E."U_receiptID" = A."DocEntry"
    Where d."ItmsGrpCod" in (120,121) and A."DocDate"  between StartDate and EndDate
    ;

 

    create local temporary table #tbl (
        ItemCode                        nvarchar(20),
        ItemName                        nvarchar(100),
        ItmsGrpCod                        int,
        ItmsGrpNam                        nvarchar(30),

 

        --Receipt
        "Qty (Goods Receipt)"            numeric(21,6),
        "Amount (Goods Receipt)"        numeric(21,6),

 

        "Qty (Receipt from PO)"            numeric(21,6),
        "Amount (Receipt from PO)"        numeric(21,6),

 

        "Receipt Qty (Outsourcing)"        numeric(21,6),
        "Receipt Amount (Outsourcing)"    numeric(21,6),

 

        --Issue
        "Qty (Goods Issue)"                numeric(21,6),
        "Amount (Goods Issue)"            numeric(21,6),

 

        "Qty (Issue for PO)"            numeric(21,6),
        "Amount (Issue for PO)"            numeric(21,6),

 

        "Issue Qty (Outsourcing)"        numeric(21,6),
        "Issue Amount (Outsourcing)"     numeric(21,6),
        
        "Revaluation Amount" 			numeric(21,6), --Inventory Revaluation Amount

 

        --     Sum
        "FG Cmplt Qty"                    numeric(21,6),
        "FG Cmplt Amount"                numeric(21,6)

 

    );
    insert into #tbl (ItemCode,ItemName,ItmsGrpCod,ItmsGrpNam)
    select distinct "ItemCode","ItemName","ItmsGrpCod",''
    from OITM
    where "ItmsGrpCod" in (120,121)
    ;
    -----------------------Receipt
    update x set "Qty (Goods Receipt)" =  ( select sum(Quantity) from #OIGN where Type = 'Goods Receipt' and ItemCode = x.ItemCode) from #tbl x;
    update x set "Amount (Goods Receipt)" =  ( select sum(LineTotal) from #OIGN where Type = 'Goods Receipt'  and ItemCode = x.ItemCode) from #tbl x;
    --
    update x set "Qty (Receipt from PO)"     =  ( select sum(Quantity) from #OIGN where Type = 'Receipt from Production'  and ItemCode = x.ItemCode) from #tbl x;
    update x set "Amount (Receipt from PO)" =  ( select sum(LineTotal) from #OIGN where Type = 'Receipt from Production'  and ItemCode = x.ItemCode) from #tbl x;
    --
    update x set "Receipt Qty (Outsourcing)"     =  ( select sum(Quantity) from #OIGN where Type = 'Receipt Outsourcing'  and ItemCode = x.ItemCode) from #tbl x;
    update x set "Receipt Amount (Outsourcing)" =  ( select sum(LineTotal) from #OIGN where Type = 'Receipt Outsourcing'  and ItemCode = x.ItemCode) from #tbl x;
    -----------------------Receipt End

 

 

    -----------------------Issue
    update x set "Qty (Goods Issue)"     =  ( select sum(Quantity) from #OIGE where Type = 'Goods Issue'  and ItemCode = x.ItemCode) from #tbl x;
    update x set "Amount (Goods Issue)" =  ( select sum(LineTotal) from #OIGE where Type = 'Goods Issue'  and ItemCode = x.ItemCode) from #tbl x;
    --
    update x set "Qty (Issue for PO)"         =  ( select sum(Quantity) from #OIGE where Type = 'Issue for Production'  and ItemCode = x.ItemCode) from #tbl x;
    update x set "Amount (Issue for PO)" =  ( select sum(LineTotal) from #OIGE where Type = 'Issue for Production'  and ItemCode = x.ItemCode) from #tbl x;
    --
    update x set "Issue Qty (Outsourcing)"     =  ( select sum(Quantity) from #OIGE where Type = 'Issue Outsourcing'  and ItemCode = x.ItemCode) from #tbl x;
    update x set "Issue Amount (Outsourcing)" =  ( select sum(LineTotal) from #OIGE where Type = 'Issue Outsourcing'  and ItemCode = x.ItemCode) from #tbl x;
    -----------------------Issue End
	-------------"Inventory Revaluation"
 
	update x 
	set "Revaluation Amount" = (select sum("TransValue") 
							from OINM a 
							where a."TransType" = 162 and a."ItemCode" = x.ItemCode and a."DocDate"  between StartDate and EndDate)
							 from #tbl x;
 
  	-------------"Inventory Revaluation" End

    -----------------------Sum
    update x set  "FG Cmplt Qty"     = ifnull("Qty (Goods Receipt)",0) + ifnull("Qty (Receipt from PO)",0) + ifnull("Receipt Qty (Outsourcing)",0) - ifnull("Qty (Goods Issue)",0) - ifnull("Qty (Issue for PO)",0) - ifnull("Issue Qty (Outsourcing)",0) from #tbl x ;
    update x set  "FG Cmplt Amount"     = ifnull("Amount (Goods Receipt)",0) + ifnull("Amount (Receipt from PO)",0) 
    										+ ifnull("Receipt Amount (Outsourcing)",0) 
    										 + ifnull("Revaluation Amount",0)
    					 		- ifnull("Amount (Goods Issue)",0) - ifnull("Amount (Issue for PO)",0) - ifnull("Issue Amount (Outsourcing)",0) from #tbl x ;
    -----------------------Sum End
    insert into #tbl (ItemName,"Qty (Goods Receipt)"
    ,"Amount (Goods Receipt)" 
    ,"Qty (Receipt from PO)" 
    ,"Amount (Receipt from PO)"
    ,"Receipt Qty (Outsourcing)"
    ,"Receipt Amount (Outsourcing)"

    ,"Qty (Goods Issue)" 
    ,"Amount (Goods Issue)"
    ,"Qty (Issue for PO)"
    ,"Amount (Issue for PO)"
    ,"Issue Qty (Outsourcing)"
    ,"Issue Amount (Outsourcing)"
    
    , "Revaluation Amount" 

    ,"FG Cmplt Qty"
    ,"FG Cmplt Amount")
    select 'TOTAL',sum("Qty (Goods Receipt)")
    ,sum("Amount (Goods Receipt)" )
    ,sum("Qty (Receipt from PO)" )
    ,sum("Amount (Receipt from PO)")
    ,sum("Receipt Qty (Outsourcing)")
    ,sum("Receipt Amount (Outsourcing)")

    ,sum("Qty (Goods Issue)" )
    ,sum("Amount (Goods Issue)")
    ,sum("Qty (Issue for PO)")
    ,sum("Amount (Issue for PO)")
    ,sum("Issue Qty (Outsourcing)")
    ,sum("Issue Amount (Outsourcing)")
    , sum( "Revaluation Amount" )

    ,sum("FG Cmplt Qty")
    ,sum("FG Cmplt Amount" )
     from #tbl;

 

    update x set ItmsGrpNam = (select "ItmsGrpNam" from OITB where "ItmsGrpCod" = x.ItmsGrpCod) from #tbl x;

 

 

    TB = select * from #tbl;

 

 

    drop table #OIGN;
    drop table #OIGE;
    drop table #tbl;
end;

