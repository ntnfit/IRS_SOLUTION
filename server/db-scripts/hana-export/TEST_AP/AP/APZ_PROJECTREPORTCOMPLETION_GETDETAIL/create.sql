
create procedure APZ_ProjectReportCompletion_GetDetail
(
in ids nvarchar(30)
)
as
begin
  /*
  declare _items nvarchar(100) ARRAY;

  declare _text nvarchar(100);

  declare _index integer;

  _text := :ids;

  _index := 1;

  WHILE LOCATE(:_text,',') > 0 DO

  _items[:_index] := SUBSTR_BEFORE(:_text,',');

  _text := SUBSTR_AFTER(:_text,',');

  _index := :_index + 1;

  END WHILE;

  _items[:_index] := :_text;

  rst = UNNEST(:_items) AS ("items");
  
select T0.* from APZ_RTC1  T0 INNER JOIN APZ_ORTC T1 ON T1."DocEntry"=T0."DocEntry" where T1."DocEntry" in  ( SELECT * FROM :rst) ;
*/
select T0.* from APZ_RTC1  T0 INNER JOIN APZ_ORTC T1 ON T1."DocEntry"=T0."DocEntry" where T1."DocEntry" =:ids;
end;




