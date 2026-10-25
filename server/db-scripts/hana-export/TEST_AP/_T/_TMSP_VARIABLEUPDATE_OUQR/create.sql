-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_VariableUpdate_OUQR () 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
CURSOR Currs for select "QString" From OUQR Where "QType" = 'W' AND locate(cast("QString" as nchar(80000)),'?') > 0;--CHARINDEX('?',"QString")> 0; 
text nvarchar (4000);
textBefore nvarchar(4000);
textAfter nvarchar(4000);
textHasChanged nvarchar (4000);
v_index int;
n tinyint;
ntmp tinyint;
v_next int := 1;
BEGIN
Open Currs;
--Fetch next From Currs 
--Into @text;
fetch  Currs into text;
	if Currs::NOTFOUND then 
	v_next:=-1;
else
	n := 0;
	v_index := locate(:text, '?');
	textBefore := substr_before(:text,'?');
	textAfter := substr_after(:text,'?');
	ntmp := :n;
	textHasChanged := :textBefore||'[%'||cast(ntmp as nchar(3))||']' || :textAfter;
	--STUFF( :text ,:v_index , 1 , '"%'||cast(n as nchar(3))||'"' );
	n := :n + 1;
	Update OUQR Set "QString" = cast(:texthasChanged as NCLOB) Where cast("QString" as nchar(4000)) = :text;
end if;	
Close Currs;
END;


