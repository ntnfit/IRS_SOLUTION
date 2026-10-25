CREATE function getListChildAccount (
	father	nvarchar(4000)
)
returns table (Data nvarchar(30))
as
begin
	declare tblFather table (Code nvarchar(30));
	declare tblChild table (Code nvarchar(30));
	declare tblRtn table (Data nvarchar(30));
	
	insert into :tblRtn (Data)
	select "AcctCode" from OACT where "AcctCode" in (select "DATA" from Split(father,','));
	
	insert into :tblFather (Code)
	select Data from :tblRtn;
	
	insert into :tblChild (Code)
	select "AcctCode" from OACT where "FatherNum" in (select Code from :tblFather);
	
	while exists(select 1 from :tblChild) do
		insert into :tblRtn (Data)
		select Code from :tblChild;
		
		delete from :tblFather;
		
		insert into :tblFather (Code)
		select Code from :tblChild;
		
		delete from :tblChild;
		
		insert into :tblChild (Code)
		select "AcctCode" from OACT where "FatherNum" in (select Code from :tblFather);
	end while;
	
	return :tblRtn;
end



