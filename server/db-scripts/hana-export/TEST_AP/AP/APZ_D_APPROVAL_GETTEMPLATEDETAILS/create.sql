
CREATE PROCEDURE APZ_D_Approval_GetTemplateDetails(IN wddCode int, IN wtmCode int)
AS
BEGIN

	declare statusCnt int;
	select count(1) into statusCnt default 0 from WDD1 where "WddCode" = :wddCode and "Status" in ('W', 'N');

	SELECT a."WddCode", a."WtmCode", b."Name" AS "WtmName", a."DraftEntry" AS "DocNo", 
    CASE a."ObjType" 
        WHEN 23 THEN 'Sales Quotation' 
        WHEN 17 THEN 'Sales Order' 
        WHEN 15 THEN 'Delivery' 
        WHEN 16 THEN 'Return' 
        WHEN 203 THEN 'A/R Down Payment' 
        WHEN 13 THEN 'A/R Invoice' 
        WHEN 165 THEN 'A/R Correction Invoice' 
        WHEN 166 THEN 'A/R Correction Invoice Reversal' 
        WHEN 14 THEN 'A/R Credit Memo' 
        WHEN 132 THEN 'Correction Invoice' 
        WHEN 1470000113 THEN 'Purchase Request' 
        WHEN 540000006 THEN 'Purchase Quotation' 
        WHEN 22 THEN 'Purchase Order' 
        WHEN 20 THEN 'Goods Receipt PO' 
        WHEN 21 THEN 'Goods Returns' 
        WHEN 204 THEN 'A/P Down Payment' 
        WHEN 18 THEN 'A/P Invoice' 
        WHEN 163 THEN 'A/P Correction Invoice' 
        WHEN 164 THEN 'A/P Correction Invoice Reversal' 
        WHEN 19 THEN 'A/P Credit Memo' 
        WHEN 59 THEN 'Goods Receipt' 
        WHEN 60 THEN 'Goods Issue' 
        WHEN 1250000001 THEN 'Inventory Transfer Request' 
        WHEN 67 THEN 'Inventory Transfer' 
        WHEN 310000001 THEN 'Inventory Opening Balance' 
        WHEN 46 THEN 'Outgoing Payment' 
        WHEN 1470000065 THEN 'Inventory Counting' 
        WHEN 10000071 THEN 'Inventory Posting' 
        WHEN 112 THEN 'Documents - Drafts' 
        WHEN 140 THEN 'Payment Drafts' 
        WHEN 1470000109 THEN 'Inventory Counting - Drafts' 
        WHEN 1470000136 THEN 'Inventory Posting - Drafts' 
        WHEN 1470000131 THEN 'Inventory Opening Balance - Drafts' 
        ELSE '' 
    END AS "DocType", a."OwnerID" AS "UserId", g.U_NAME AS "UserName", 
    CAST(a."CreateDate" AS varchar) AS "CreateDate", a."CreateTime", b."Remarks", c."WstCode", 
    CAST(c."SortId" AS varchar) || '.' || d."Name" AS "WstName", e."UserID" AS "AuthId", h.U_NAME AS "AuthName", 
    CASE 
        WHEN f."Status" = 'W' THEN 'W' 
        WHEN f."Status" = 'Y' THEN 'Y' 
        WHEN f."Status" = 'N' THEN 'N' 
        WHEN :statusCnt > 0 THEN 'S' 
        WHEN :statusCnt = 0 THEN 'W' 
    END AS "StatusCode", 
    CASE 
        WHEN f."Status" = 'W' THEN 'Pending' 
        WHEN f."Status" = 'Y' THEN 'Approved' 
        WHEN f."Status" = 'N' THEN 'Rejected' 
        WHEN :statusCnt > 0 THEN 'Not started' 
        WHEN :statusCnt = 0 THEN 'Pending' 
    END AS "Status", CAST(f."UpdateDate" AS varchar) AS "UpdateDate", f."UpdateTime", f."Remarks" AS "AuthRemarks", 
    CASE 
        WHEN IFNULL(i."DocTotalFC", 0) > 0 THEN IFNULL(i."DocTotalFC", 0) 
        ELSE IFNULL(i."DocTotal", 0) 
    END AS "Total", i."DocCur" AS "Currency", c."SortId", a."IsDraft" 
	FROM OWDD a 
	    INNER JOIN OWTM b ON a."WtmCode" = b."WtmCode" 
	    INNER JOIN WTM2 c ON a."WtmCode" = c."WtmCode" 
	    INNER JOIN OWST d ON c."WstCode" = d."WstCode" 
	    INNER JOIN WST1 e ON d."WstCode" = e."WstCode" 
	    LEFT OUTER JOIN WDD1 f ON e."WstCode" = f."StepCode" AND e."UserID" = f."UserID" AND f."WddCode" = a."WddCode" 
	    INNER JOIN OUSR g ON a."OwnerID" = g.USERID 
	    INNER JOIN OUSR h ON e."UserID" = h.USERID 
	    INNER JOIN ODRF i ON a."DraftEntry" = i."DocEntry" 
	WHERE a."WddCode" = :wddCode AND a."WtmCode" = :wtmCode 
	ORDER BY c."SortId" ASC;

END




