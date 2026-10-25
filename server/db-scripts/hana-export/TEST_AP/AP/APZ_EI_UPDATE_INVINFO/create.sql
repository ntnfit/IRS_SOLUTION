CREATE PROCEDURE APZ_EI_Update_InvInfo
(
 	in ObjType 			nvarchar(100),
 	in DocEntry			nvarchar(100),
 	in InvNo			nvarchar(20),
	in InvDate			date,
	in InvSerial		nvarchar(20),
	in invSym			nvarchar(20),
	in eInvTransID		nvarchar(100),
	in eInvMTC			nvarchar(100)
 )
 AS 
 BEGIN
 	if $ObjType = '13' then
 		update OINV set "U_InvNo" 		= case when ifnull($InvNo,'') <> '' then right(cast(10000000 || $InvNo as nvarchar),8) else "U_InvNo" end
 						,"U_InvDate" 	= case when ifnull($InvDate,'') <> '' then $InvDate else "U_InvDate" end
 						,"U_InvSerial"	= case when ifnull($InvSerial,'') <> '' then $InvSerial else "U_InvSerial" end
 						,"U_invSym"		= case when ifnull($invSym,'') <> '' then $invSym else "U_invSym" end
 						,"U_eInvTransID"= case when ifnull($eInvTransID,'') <> '' then $eInvTransID else "U_eInvTransID" end
 						,"U_eInvMTC"	= case when ifnull($eInvMTC,'') <> '' then $eInvMTC else "U_eInvMTC" end				
 		where "DocEntry" = cast($DocEntry as int);
 	end if;
 	
 	if $ObjType = '19' then
 		 update ORPC set "U_InvNo" 		= case when ifnull($InvNo,'') <> '' then right(cast(10000000 || $InvNo as nvarchar),8) else "U_InvNo" end
 						,"U_InvDate" 	= case when ifnull($InvDate,'') <> '' then $InvDate else "U_InvDate" end
 						,"U_InvSerial"	= case when ifnull($InvSerial,'') <> '' then $InvSerial else "U_InvSerial" end
 						,"U_invSym"		= case when ifnull($invSym,'') <> '' then $invSym else "U_invSym" end
 						,"U_eInvTransID"= case when ifnull($eInvTransID,'') <> '' then $eInvTransID else "U_eInvTransID" end
 						,"U_eInvMTC"	= case when ifnull($eInvMTC,'') <> '' then $eInvMTC else "U_eInvMTC" end
 		where "DocEntry" = cast($DocEntry as int);
 	end if;
 	
 	if $ObjType = '14' then
 		update ORIN set "U_InvNo" 		= case when ifnull($InvNo,'') <> '' then right(cast(10000000 || $InvNo as nvarchar),8) else "U_InvNo" end
 						,"U_InvDate" 	= case when ifnull($InvDate,'') <> '' then $InvDate else "U_InvDate" end
 						,"U_InvSerial"	= case when ifnull($InvSerial,'') <> '' then $InvSerial else "U_InvSerial" end
 						,"U_invSym"		= case when ifnull($invSym,'') <> '' then $invSym else "U_invSym" end
 						,"U_eInvTransID"= case when ifnull($eInvTransID,'') <> '' then $eInvTransID else "U_eInvTransID" end
 						,"U_eInvMTC"	= case when ifnull($eInvMTC,'') <> '' then $eInvMTC else "U_eInvMTC" end				
 		where "DocEntry" = cast($DocEntry as int);
 	end if; 
 	
	if $ObjType = '15' then
 		update ODLN set "U_InvNo" 		= case when ifnull($InvNo,'') <> '' then right(cast(10000000 || $InvNo as nvarchar),8) else "U_InvNo" end
 						,"U_InvDate" 	= case when ifnull($InvDate,'') <> '' then $InvDate else "U_InvDate" end
 						,"U_InvSerial"	= case when ifnull($InvSerial,'') <> '' then $InvSerial else "U_InvSerial" end
 						,"U_invSym"		= case when ifnull($invSym,'') <> '' then $invSym else "U_invSym" end
 						,"U_eInvTransID"= case when ifnull($eInvTransID,'') <> '' then $eInvTransID else "U_eInvTransID" end
 						,"U_eInvMTC"	= case when ifnull($eInvMTC,'') <> '' then $eInvMTC else "U_eInvMTC" end				
 		where "DocEntry" = cast($DocEntry as int);
 	end if; 
 	
	if $ObjType = '67' then
 		update OWTR set "U_InvNo" 		= case when ifnull($InvNo,'') <> '' then right(cast(10000000 || $InvNo as nvarchar),8) else "U_InvNo" end
 						,"U_InvDate" 	= case when ifnull($InvDate,'') <> '' then $InvDate else "U_InvDate" end
 						,"U_InvSerial"	= case when ifnull($InvSerial,'') <> '' then $InvSerial else "U_InvSerial" end
 						,"U_invSym"		= case when ifnull($invSym,'') <> '' then $invSym else "U_invSym" end
 						,"U_eInvTransID"= case when ifnull($eInvTransID,'') <> '' then $eInvTransID else "U_eInvTransID" end
 						,"U_eInvMTC"	= case when ifnull($eInvMTC,'') <> '' then $eInvMTC else "U_eInvMTC" end				
 		where "DocEntry" = cast($DocEntry as int);
 	end if;	
 END;
