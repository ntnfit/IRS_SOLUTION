
create procedure APZ_Cfl_ProjectManagerItemDocument
(
in objectType int,
in absEntry int,
in lineNum int
)
as
begin
if :objectType=1470000113
		then
			select a.*,:objectType as "TypeDocument",c."DocNum"  from prq1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join oprq c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum;  
-- purchase request
		
elseif :objectType=20
		then
			select a.*,:objectType as "TypeDocument",c."DocNum" from PDN1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join oPDN c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum;  
			
elseif :objectType=22
		then
			select a.*,:objectType as "TypeDocument",c."DocNum" from POR1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join OPOR c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum;  
			
elseif :objectType=15
		then
			select a.*,:objectType as "TypeDocument",c."DocNum" from DLN1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join oDLN c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum;  
		
elseif :objectType=18
		then
			select a.*,:objectType as "TypeDocument",c."DocNum" from PCH1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join oPCH c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum;  
elseif :objectType=13
		then
			select a.*,:objectType as "TypeDocument",c."DocNum" from INV1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join oINV c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum;
				
elseif :objectType=540000006
		then
			select a.*,:objectType as "TypeDocument",c."DocNum" from PQT1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join OPQT c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum; 

elseif :objectType=60
		then
			select a.*,:objectType as "TypeDocument",c."DocNum" from IGE1  a 
			left join APZ_PRG4 b on b."AbsEntry"=:absEntry and b."LineNum"= :lineNum  
			left join OIGE c on c."DocEntry"= a."DocEntry"  where  a."U_OriginType"='1470000132' and  a."U_OriginCode"=:absEntry and a."U_OriginLine"= :lineNum; 
end	if;
end;




