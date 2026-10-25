
CREATE PROCEDURE APZ_ItemMaster_Search
(
in ItemCode nvarchar(50) default '',
in ItemName nvarchar(200) default '',
in FrgnName nvarchar(50) default '',
in ItmsGrpCod int default 0,
in ItemTypes nvarchar(50) default '',
in ShipType int default 0,
in FirmCode int default 0,
in VatGroupPu nvarchar(50) default '',
in VatGourpSa nvarchar(50) default '',
in CardCode nvarchar(50) default '',
in UserCode nvarchar(50) default '',
in BPLId int default 0
)
as
BEGIN
declare newItCode nvarchar(50) = ltrim(rtrim(IFNULL(:ItemCode,'')));
declare newItName nvarchar(50) = ltrim(rtrim(IFNULL(:ItemName,'')));
declare newFfgName nvarchar(50) = ltrim(rtrim(IFNULL(:FrgnName,'')));
declare newItemTypes nvarchar(50) = ltrim(rtrim(IFNULL(:ItemTypes,'')));
declare newVatGroupPu nvarchar(50) = ltrim(rtrim(IFNULL(:VatGroupPu,'')));
declare newVatGourpSa nvarchar(50) = ltrim(rtrim(IFNULL(:VatGourpSa,'')));
declare newCardCode nvarchar(50) = ltrim(rtrim(IFNULL(:CardCode,'')));


SELECT  
	*
FROM
	OITM 
WHERE
	((:newItCode <> '' AND "ItemCode" like :newItCode) OR :newItCode ='')
	AND ((:newItName <> '' AND "ItemName" like :newItName) OR :newItName ='')
	AND ((:newFfgName <> '' AND "FrgnName" like :newFfgName) OR :newFfgName ='')
	AND ((:ItmsGrpCod <> 0 and :ItmsGrpCod <> -1 AND "ItmsGrpCod" = :ItmsGrpCod) OR :ItmsGrpCod = 0 OR :ItmsGrpCod = -1)
	AND ((:newItemTypes <> '' AND "FrgnName" like :newItemTypes) OR :newItemTypes ='')
	AND ((:ShipType <> 0 and :ShipType <> -1 AND "ShipType" = :ShipType) OR :ShipType = 0 OR :ShipType = -1)
	AND ((:FirmCode <> 0 and :FirmCode <> -1 AND "FirmCode" = :FirmCode) OR :FirmCode = 0 OR :FirmCode = -1)
	AND ((:newVatGroupPu <> '' AND "VatGroupPu" like :newVatGroupPu) OR :newVatGroupPu ='')
	AND ((:newVatGourpSa <> '' AND "VatGourpSa" like :newVatGourpSa) OR :newVatGourpSa ='')
	AND ((:newCardCode <> '' AND "CardCode" like :newCardCode) OR :newCardCode ='');
	
	END




