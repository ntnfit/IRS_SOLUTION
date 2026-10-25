
CREATE PROCEDURE APZ_ItemMaster_GetOITM (IN ItemCode nvarchar(50) default '', IN UserCode nvarchar(50) default '', IN BPLId integer default 0) 
AS 
BEGIN
declare query nvarchar(5000) = '';
 query := 'SELECT  a.ItemCode, a.ItemName, ISNULL ( a.FrgnName,'''') as FrgnName , ISNULL (a.ItmsGrpCod,0) as ItmsGrpCod, a.VatGourpSa,a.CodeBars,a.PrchseItem,
			a.SellItem, ISNULL (a.OnHand,0) as OnHand ,ISNULL( a.IsCommited,0) as IsCommited, ISNULL( a.OnOrder,0) as OnOrder, ISNULL ( a.MaxLevel,0) as MaxLevel, ISNULL (a.DfltWH,'''') as DfltWH ,
			ISNULL (a.CardCode,'''') as CardCode, ISNULL (a.SuppCatNum,'''') as SuppCatNum, ISNULL ( a.BuyUnitMsr,'''') as BuyUnitMsr,											
			ISNULL( a.NumInBuy,0) as NumInBuy , ISNULL (a.ReorderQty,0) as ReorderQty,ISNULL( a.MinLevel,0) as MinLevel,
			ISNULL ( a.SalUnitMsr,'''') as SalUnitMsr , ISNULL ( a.NumInSale,0) as NumInSale, a.EvalSystem, ISNULL( a.UserSign,3) as UserSign,
			a.ManSerNum,a.FirmCode,ISNULL (a.SalFactor1,0) as SalFactor1,
			ISNULL ( a.SalFactor2,0) as SalFactor2, ISNULL (a.SalFactor3,0) as SalFactor3, ISNULL (a.SalFactor4, 0) as SalFactor4,
			ISNULL (a.PurFactor1,0) as PurFactor1, ISNULL (a.PurFactor2,0) as PurFactor2,  ISNULL (a.PurFactor3,0) as PurFactor3, ISNULL (a.PurFactor4,0) as PurFactor4,
			a.VatGroupPu, ISNULL( a.AvgPrice,0) as AvgPrice, ISNULL (a.PurPackMsr,'''') as PurPackMsr, ISNULL ( a.PurPackUn,0) as PurPackUn,
			ISNULL (a.SalPackMsr,'''') as SalPackMsr, ISNULL ( a.SalPackUn,0) as SalPackUn, a.validFor , ISNULL( a.ObjType,4) as ObjType,
			ISNULL( a.DocEntry,0) as DocEntry,ISNULL( a.ShipType,'''')as ShipType ,a.GLMethod as GLMethods,a.ByWh,
			Case when a.ItemType =''I''    then   ''Items''
			when a.ItemType =  ''L'' then ''Labor''
			when a.ItemType =  ''T'' then ''Travel''
			end ItemTypes,
			ISNULL( a.StockValue,0) as StockValue ,ISNULL ( a.UserSign2,3) as UserSign2 , ISNULL (a.InvntryUom,'''') as InvntryUom, 												
			CASE WHEN a.UserText IS NULL then ''null''
			else a.UserText
			END as UserText , 
			CASE WHEN a.PicturName IS NULL then ''null''
			else a.PicturName
			END as PicturName	,
			isnull( a.InvntItem,''N'') as InvntItem ,
			isnull( a.validFrom, CONVERT(datetime,''01/01/1900'')) as validFrom,
			isnull( a.validTo,CONVERT(datetime,''01/01/1900'')) as validTo ,
			isnull( a.ValidComm,'''') as ValidRemarks	,																				  
			isnull (a.frozenFrom,CONVERT(datetime,''01/01/1900'')) as frozenFrom ,
			isnull (a.frozenTo,CONVERT(datetime,''01/01/1900'')) as frozenTo,
			isnull (a.FrozenComm,'''') as FrozenRemarks,
			isnull	(a.MngMethod,''R'') as MngMethod	,
			isnull	(a.ManBtchNum,''N'') as ManBtchNum,
			isnull	(a.frozenFor,''N'') as frozenFor  ,
			isnull	(Series,0) as Series,

			CASE WHEN  a.PlaningSys = ''N'' then ''None''
				 when  a.PlaningSys = ''M'' then ''MRP''
			END PlaningSys ,
			CASE WHEN a.PrcrmntMtd = ''B'' then ''Buy''
				 WHEN a.PrcrmntMtd = ''M'' then ''Make''	
			END PrcrmntMtd	 ,
			CASE WHEN a.CompoWH = ''B'' then ''From Bill of Materials Line''
				  WHEN a.CompoWH = ''P'' then ''From Parent Item Document Line''
			END CompoWH,
			isnull(a.OrdrIntrvl,-1) as OrdrIntrvl,
			isnull(a.OrdrMulti,0) as OrdrMulti,
			isnull (a.MinOrdrQty,0) as MinOrdrQty,
			isnull (a.LeadTime,0) as LeadTime,
			isnull(a.ToleranDay,0) as ToleranDay

			FROM OITM A                                                                                               
			LEFT JOIN OSHP D
			ON a.ShipType = d.TrnspCode
			LEFT JOIN OMRC E
			ON a.FirmCode = e.FirmCode
			LEFT JOIN OITB f
			ON a.ItmsGrpCod = f.ITMSGRPCOD 
			LEFT JOIN OVTG g   								   
			ON a.VatGourpSa = g.Code 
			and a.VatGroupPu = g.Code
			WHERE 1=1 ';
IF :ItemCode <> '' THEN query := :query + 'AND  a.ItemCode =''' || :ItemCode || ''' ';
END IF;
SELECT (:query) FROM DUMMY;
EXEC(:query);
END;




