
CREATE PROCEDURE apz_Penalization_Remove
(in DocNum int)
as
BEGIN
DELETE FROM APZ_HPT1 WHERE "DocNum" = :DocNum;
DELETE FROM APZ_HPT2 WHERE "DocNum" = :DocNum;
END




