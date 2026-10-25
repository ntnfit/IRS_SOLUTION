

CREATE PROCEDURE APZ_DocumentSettings_SaveDoc
(
	in LevelWarn char(1),	
	OrderBlock char(1),
	AllowFuPos char(1),
	ChangeRdr char(1),
	RevisionPo char(1),
	ICDifExPe1 numeric(19, 6),
	ICDifExPe3 numeric(19, 6),
	ClsZoDiffR char(1),
	ClsNoConfi char(1),
	AllowPostZ char(1),
	PostDiffR numeric(19, 6),
	ClosedQuot char(1),
	BlockZeroQ char(1),
	NegStoLv char(1),
	ChkQunty char(1),
	AllowFuPosSale char(1),
	ReopOrderSale char(1),
	ForceReOrdSale char(1),
	CpyPRPrice char(1),
	ClosePQ char(1),
	ReopOrderPurchase char(1),
	ForceReOrdPurchase char(1),
	ObjList int
)
AS
BEGIN
	UPDATE OADM SET "LevelWarn" = :LevelWarn, "OrderBlock" = :OrderBlock, "AllowFuPos" = :AllowFuPos, "ChangeRdr" = :ChangeRdr, "RevisionPo" = :RevisionPo,
	"ICDifExPe1" = :ICDifExPe1, "ICDifExPe3" = :ICDifExPe3, "ClsZoDiffR" = :ClsZoDiffR, "ClsNoConfi" = :ClsNoConfi, "AllowPostZ" = :AllowPostZ, "PostDiffR" = :PostDiffR, "ClosedQuot" = :ClosedQuot;
	
	 UPDATE CINF SET "BlockZeroQ" = :BlockZeroQ, "NegStoLv" = :NegStoLv, "ChkQunty" = :ChkQunty;  
	
	 UPDATE ADP1 SET "AllowFuPos" = :AllowFuPosSale WHERE "ObjType" = '23';
	 UPDATE ADP1 SET "ReopOrder" = :ReopOrderSale, "ForceReOrd" = :ForceReOrdSale WHERE "ObjType" = '17';
	 UPDATE ADP1 SET "CpyPRPrice" = :CpyPRPrice WHERE "ObjType" = '1470000113';
	 UPDATE ADP1 SET "ClosePQ" = :ClosePQ WHERE "ObjType" = '540000006';
	 UPDATE ADP1 SET "ReopOrder" = :ReopOrderPurchase, "ForceReOrd" = :ForceReOrdPurchase WHERE "ObjType" = '22';

	 UPDATE OADP SET "ObjList"= :ObjList; 
END;



