
create procedure APZ_MarketingCampaign_Remove
(
in Id int
)
as
begin

if(  :Id IN(select "DocEntry" from APZ_OCLG where "DocType" = '1470000132')
 or :Id IN(select "U_RelatedEnt" from OQUT where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from ORDR where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from OPRQ where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from OPDN where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from OPOR where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from ODLN where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from OPCH where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from OINV where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
  or :Id IN(select "U_RelatedEnt" from OPQT where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from ODPI where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
 or :Id IN(select "U_RelatedEnt" from ODPO where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" =:Id)
  or :Id IN(select "DocEntry" from APZ_OSMC where "ParentCampaign" >0)
 )
 then

 select 1, 'Cannot remove name because it has been used' from dummy;
 return;

 end if;


Delete from OATC where "AbsEntry" = (select top 1 "AtcEntry" from APZ_OSMC where "DocEntry" = :Id);
Delete from ATC1 where "AbsEntry" = (select top 1 "AtcEntry" from APZ_OSMC where "DocEntry" = :Id);
Delete from APZ_OSMC where "DocEntry"=:Id;
Delete from APZ_SMC1 where "DocEntry"=:Id;
Delete from APZ_SMC2 where "DocEntry"=:Id;
Delete from APZ_SMC3 where "DocEntry"=:Id;
Delete from APZ_SMC4 where "DocEntry"=:Id;
Delete from APZ_SMC5 where "DocEntry"=:Id;
Delete from APZ_OSMC where "ParentCampaign"=:Id;

select :Id, '' from dummy;
end;



