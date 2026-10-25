CREATE PROCEDURE ILD_LayDanhSachNhanVien
as 
begin

Select "empID"  ,"Code" as "empCode",
	case 
					when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') != '' then b."lastName" || ' ' || b."middleName" || ' ' || b."firstName"
					when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') = '' then b."middleName" || ' ' || b."firstName"
					when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') = '' and ifnull(b."lastName", '') != '' then b."lastName" || ' ' || b."firstName"
					when ifnull(b."firstName", '') = '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') != '' then b."lastName" || ' ' || b."middleName"
					when ifnull(b."firstName", '') = '' and ifnull(b."middleName", '') = '' and ifnull(b."lastName", '') != '' then b."lastName"
					when ifnull(b."firstName", '') = '' and ifnull(b."middleName", '') != '' and ifnull(b."lastName", '') = '' then b."middleName"
					when ifnull(b."firstName", '') != '' and ifnull(b."middleName", '') = '' and ifnull(b."lastName", '') = '' then b."firstName"
					else '' end 
					
					as "FullName"
				
	
From  OHEM b;

end;


