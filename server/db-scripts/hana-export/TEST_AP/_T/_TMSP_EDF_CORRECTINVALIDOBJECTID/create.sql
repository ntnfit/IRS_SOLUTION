-- B1 DEPENDS: BEFORE:PT:CREATE_OBJECTS AFTER:PT:CREATE_VIEWS

CREATE PROCEDURE _TmSp_EDF_CorrectInvalidObjectID
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	UPDATE
		"ECM2" T0
	SET
		T0."ObjectID" = T1."NewObjectID"
	FROM
		"ECM2" T0
		INNER JOIN
			(
				SELECT
					S0."AbsEntry",
					CONCAT (S0."AbbrevationAndSpace", S0."DocNum") AS "NewObjectID"
				FROM
					(
						-- Marketing documents
						SELECT
							SS0."AbsEntry",
							SS0."DocNum",
							SS1."ObjectID",
							SUBSTR (SS1."ObjectID", 1, 3) AS "AbbrevationAndSpace",
							CONCAT ('% ', TO_CHAR (SS0."DocNum")) AS "CorrectObjectIDPattern"
						FROM
							"B1_ElectronicDocumentView" SS0
							INNER JOIN "ECM2" SS1 ON
								SS1."AbsEntry" = SS0."AbsEntry"
						WHERE
							-- only select records, where we can correct something and ObjectID is already set
							SS0."DocNum" <> SS0."SrcObjAbs"
							AND LENGTH (SS1."ObjectID") > 3

						UNION

						-- AP (Imported) marketing documents
						SELECT
							SS0."AbsEntry",
							SS0."DocNum",
							SS1."ObjectID",
							SUBSTR (SS1."ObjectID", 1, 3) AS "AbbrevationAndSpace",
							CONCAT ('% ', TO_CHAR (SS0."DocNum")) AS "CorrectObjectIDPattern"
						FROM
							"B1_ElectronicDocumentView" SS0
							INNER JOIN "ECM2" SS1 ON
								SS1."AbsEntry" = SS0."AbsEntry"
						WHERE
							-- only select records, where we can correct something and ObjectID is already set
							SS1."ActType" = 'P'
							AND LENGTH (SS1."ObjectID") > 3

						UNION

						-- Incoming Payments
						SELECT
							SS1."AbsEntry",
							SS0."DocNum",
							SS1."ObjectID",
							SUBSTR (SS1."ObjectID", 1, 3) AS "AbbrevationAndSpace",
							CONCAT ('% ', TO_CHAR (SS0."DocNum")) AS "CorrectObjectIDPattern"
						FROM
							"ORCT" SS0
							INNER JOIN "ECM2" SS1 ON
								SS1."SrcObjAbs" = SS0."DocEntry"
						WHERE
							-- only select records, where we can correct something and ObjectID is already set
							SS1."SrcObjType" = '24'
							AND LENGTH (SS1."ObjectID") > 3
							
						UNION

						-- Payment Drafts
						SELECT
							SS1."AbsEntry",
							SS0."DocNum",
							SS1."ObjectID",
							SUBSTR (SS1."ObjectID", 1, 3) AS "AbbrevationAndSpace",
							CONCAT ('% ', TO_CHAR (SS0."DocNum")) AS "CorrectObjectIDPattern"
						FROM
							"OPDF" SS0
							INNER JOIN "ECM2" SS1 ON
								SS1."SrcObjAbs" = SS0."DocEntry"
						WHERE
							-- only select records, where we can correct something and ObjectID is already set
							SS1."SrcObjType" = '140'
							AND LENGTH (SS1."ObjectID") > 3
							
						UNION

						-- Outgoing Payments
						SELECT
							SS1."AbsEntry",
							SS0."DocNum",
							SS1."ObjectID",
							SUBSTR (SS1."ObjectID", 1, 3) AS "AbbrevationAndSpace",
							CONCAT ('% ', TO_CHAR (SS0."DocNum")) AS "CorrectObjectIDPattern"
						FROM
							"OVPM" SS0
							INNER JOIN "ECM2" SS1 ON
								SS1."SrcObjAbs" = SS0."DocEntry"
						WHERE
							-- only select records, where we can correct something and ObjectID is already set
							SS1."SrcObjType" = '46'
							AND LENGTH (SS1."ObjectID") > 3
					) AS S0
				WHERE
					-- select records, where existing ObjectID doesn't contain source document number
					S0."ObjectID" NOT LIKE S0."CorrectObjectIDPattern"
			) AS T1 ON
				T0."AbsEntry" = T1."AbsEntry"
	WHERE
		T0."AbsEntry" = T1."AbsEntry";
END;


