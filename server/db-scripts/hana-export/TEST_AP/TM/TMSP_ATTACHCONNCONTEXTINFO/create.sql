-- B1 DEPENDS: BEFORE:PT:PROCESS_START
create procedure TmSp_AttachConnContextInfo(session_information_in nvarchar(4000))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
	v_session_info nvarchar(4000);
	v_section_session_info nvarchar(500);
	v_key nvarchar(255);
	v_value nvarchar(255);
begin
	v_session_info := :session_information_in;
	v_section_session_info := substr_before(:v_session_info, ';');

	while (length(:v_section_session_info) > 0)
	do 
		v_key := substr_before(:v_section_session_info, '|');
		v_value := substr_after(:v_section_session_info, '|');
		if (length(:v_key) > 0 and length(:v_value) > 0)
		then
			--select 'set session ''' || :v_key || ''' = ''' || :v_value || '''' from dummy;
			exec 'set session ''' || :v_key || ''' = ''' || :v_value || '''';
		end if;
		v_session_info := substr_after(v_session_info, ';');
		v_section_session_info := substr_before(:v_session_info, ';');
	end while;
end;


