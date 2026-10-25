
create table APZ_Item_ListCFLField
(
	"FieldName" nvarchar(30) not null primary key,
	"QueryField" nvarchar(300),
	"JoinAlias" nvarchar(3)
);

GO

insert into APZ_Item_ListCFLField values ('ItemCode', null, null);

GO

insert into APZ_Item_ListCFLField values ('ItemDescription', 'A."ItemName"', null);

GO

insert into APZ_Item_ListCFLField values ('ItemName', null, null);

GO

insert into APZ_Item_ListCFLField values ('Substitute', '''''', null);

GO

insert into APZ_Item_ListCFLField values ('FrgnName', null, null);

GO

insert into APZ_Item_ListCFLField values ('ItmsGrpCod', null, null);

GO

insert into APZ_Item_ListCFLField values ('ItmsGrpNam', 'f."ItmsGrpNam"', 'f');

GO

insert into APZ_Item_ListCFLField values ('SalesVATGroup', 'CASE WHEN IFNULL(:ecVatGroup, '''') = '''' THEN CASE WHEN IFNULL(A."VatGourpSa", '''') = '''' THEN :DfSVatItem ELSE A."VatGourpSa" END ELSE :ecVatGroup END', null);

GO

insert into APZ_Item_ListCFLField values ('VatGroupPu', 'CASE WHEN IFNULL(:ecVatGroup, '''') = '''' THEN CASE WHEN IFNULL(A."VatGroupPu", '''') = '''' THEN :DfPVatItem ELSE A."VatGroupPu" END ELSE :ecVatGroup END', null);

GO

insert into APZ_Item_ListCFLField values ('VatGourpSa', 'CASE WHEN IFNULL(:ecVatGroup, '''') = '''' THEN CASE WHEN IFNULL(A."VatGourpSa", '''') = '''' THEN :DfSVatItem ELSE A."VatGourpSa" END ELSE :ecVatGroup END', null);

GO

insert into APZ_Item_ListCFLField values ('WarehouseCode', 'CASE WHEN a."DfltWH" <> '''' THEN a."DfltWH" ELSE :WhsDefault END', null);

GO

insert into APZ_Item_ListCFLField values ('DefaultWarehouse', 'CASE WHEN a."DfltWH" <> '''' THEN a."DfltWH" ELSE :WhsDefault END', null);

GO

insert into APZ_Item_ListCFLField values ('dfltWH', 'CASE WHEN a."DfltWH" <> '''' THEN a."DfltWH" ELSE :WhsDefault END', null);

GO

insert into APZ_Item_ListCFLField values ('QuantityOnStock', 'a."OnHand"', null);

GO

insert into APZ_Item_ListCFLField values ('PriceList', ':infoPriceList', null);

GO

insert into APZ_Item_ListCFLField values ('RatePu', 'CASE WHEN IFNULL(:ecVatGroup, '''') = '''' THEN CASE WHEN IFNULL(A."VatGourpPu", '''') = '''' THEN IFNULL((select "Rate" from OVTG where "Code"=:DfPVatItem), 0) ELSE IFNULL(k."Rate", 0) END ELSE (select "Rate" from OVTG where "Code"=:ecVatGroup) END', 'k');

GO

insert into APZ_Item_ListCFLField values ('RateSa', 'CASE WHEN IFNULL(:ecVatGroup, '''') = '''' THEN CASE WHEN IFNULL(A."VatGroupSa", '''') = '''' THEN IFNULL((select "Rate" from OVTG where "Code"=:DfSVatItem), 0) ELSE IFNULL(g."Rate", 0) END ELSE (select "Rate" from OVTG where "Code"=:ecVatGroup) END', 'g');

GO

insert into APZ_Item_ListCFLField values ('BOMTreeType', 'l."TreeType"', 'l');

GO

insert into APZ_Item_ListCFLField values ('VatPuInActive', 'IFNULL(k."Inactive", '''')', 'k');

GO

insert into APZ_Item_ListCFLField values ('VatSaInActive', 'IFNULL(g."Inactive", '''')', 'g');

GO

insert into APZ_Item_ListCFLField values ('TreeType', 'CASE IFNULL(l."TreeType", ''N'') WHEN ''A'' THEN 1 WHEN ''S'' THEN 2 WHEN ''P'' THEN 3 WHEN ''T'' THEN 4 WHEN ''N'' THEN CASE IFNULL((SELECT MAX("TreeType") FROM OITT x6 WHERE exists(SELECT 1 FROM itt1 WHERE "Code" = a."ItemCode" and x6."Code" = "Father")), ''N'') WHEN ''S'' THEN 5 ELSE 0 END END', 'l');

GO

insert into APZ_Item_ListCFLField values ('BuyUnitMsr', null, null);

GO

insert into APZ_Item_ListCFLField values ('SalUnitMsr', null, null);

GO

insert into APZ_Item_ListCFLField values ('NumInBuy', null, null);

GO

insert into APZ_Item_ListCFLField values ('NumInSale', null, null);

GO

insert into APZ_Item_ListCFLField values ('InvntryUom', null, null);

GO

insert into APZ_Item_ListCFLField values ('UgpCode', 'a."UgpEntry"', null);

GO

insert into APZ_Item_ListCFLField values ('NumInInven', 'IFNULL(a1."AltQty", 1) / IFNULL(a1."BaseQty", 1)', 'a1');

GO

insert into APZ_Item_ListCFLField values ('Price', 'IFNULL(p."Price", 0)', 'p');

GO

insert into APZ_Item_ListCFLField values ('IsGrossPrc', 'IFNULL(p."IsGrossPrc", ''N'')', 'p');

GO

insert into APZ_Item_ListCFLField values ('Currency', 'p."Currency"', 'p');

GO

insert into APZ_Item_ListCFLField values ('Discount', 'IFNULL(p."Discount", 0)', 'p');

GO

insert into APZ_Item_ListCFLField values ('ManBatchSerial', 'CASE WHEN a."ManBtchNum" = ''Y'' THEN ''B'' WHEN a."ManSerNum" = ''Y'' THEN ''S'' ELSE ''N'' END', null);

GO

insert into APZ_Item_ListCFLField values ('OnStock', 'IFNULL(a."OnHand", 0)', null);

GO

insert into APZ_Item_ListCFLField values ('OnWhs', 'IFNULL(t1."OnHand", 0)', 't1');

GO

insert into APZ_Item_ListCFLField values ('IsCommited', null, null);

GO

insert into APZ_Item_ListCFLField values ('IsCommitedInWhs', 'IFNULL(t1."IsCommited", 0)', 't1');

GO

insert into APZ_Item_ListCFLField values ('OnOrder', null, null);

GO

insert into APZ_Item_ListCFLField values ('OnOrderInWhs', 'IFNULL(t1."OnOrder", 0)', 't1');

GO

insert into APZ_Item_ListCFLField values ('CardCode', null, null);

GO

insert into APZ_Item_ListCFLField values ('SubCatNum', '(SELECT x."Substitute" FROM (select "Substitute","ItemCode","CardCode", row_number() over(order by "Substitute") rowentry  from oscn WHERE oscn."ItemCode" = A."ItemCode" AND oscn."CardCode" = :CardCode) x where x.rowentry = 1)', null);

GO

insert into APZ_Item_ListCFLField values ('IssueMthd', null, null);

GO

insert into APZ_Item_ListCFLField values ('SalPackUn', null, null);

GO

insert into APZ_Item_ListCFLField values ('PurPackUn', null, null);

GO

insert into APZ_Item_ListCFLField values ('LinkThumbnails', 'ifnull((select t."LinkThumbnails" from #APZ_D_UUPIM t where t."ObjectCode" = A."ItemCode" and t.Row_Num = 1),'''')', null);

GO

--insert into APZ_Item_ListCFLField values ('AgrNo', 'v."AgrNo"', 'v');

GO

--insert into APZ_Item_ListCFLField values ('AgrLnNum', 'v."AgrLineNum"', 'v');