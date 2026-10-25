
CREATE PROCEDURE APZ_SP_UnPivot
(
	IN table_name VARCHAR(127), 
	IN select_columns ntext,
	IN unpivot_val_name VARCHAR(127),
	IN unpivot_col_name VARCHAR(127),
	IN unpivot_columns VARCHAR(5000),
	IN unpivot_column_values VARCHAR(5000),
	IN result_table_name VARCHAR(127),
	IN include_nulls BOOLEAN DEFAULT TRUE
)
AS
BEGIN
	USING SQLSCRIPT_STRING AS lib;

	DECLARE v_count INT= 0;
	DECLARE v_idx INT;
	DECLARE v_statement ntext;
	DECLARE v_statement_map1 VARCHAR(5000) = '';
	DECLARE v_statement_map2 VARCHAR(5000) = '';
	DECLARE a_unpivot_columns VARCHAR(127) array;
	DECLARE a_unpivot_column_values VARCHAR(100) array;

	a_unpivot_columns = LIB:split_to_array( :unpivot_columns, ',' );
	a_unpivot_column_values = LIB:split_to_array( :unpivot_column_values, ',' );
	v_count = cardinality(:a_unpivot_columns);
	tbl_pivot_columns = UNNEST(:a_unpivot_columns) AS ("EXCLUDE_COLUMNS");

	/*
		if all columns needed, use * to get the column names except tbl_pivot_columns
	*/
	IF select_columns = '*' THEN
		if substring(table_name, 1, 1) = '#' then
			SELECT string_agg(column_name,',' ORDER BY position) INTO select_columns
			FROM M_TEMPORARY_TABLE_COLUMNS a join M_TEMPORARY_TABLES b on a.TABLE_OID=b.TABLE_OID
   			where b.CONNECTION_ID=CURRENT_CONNECTION and b.TABLE_NAME=:table_name
				AND column_name NOT IN (SELECT EXCLUDE_COLUMNS FROM :tbl_pivot_columns);
		else
			SELECT string_agg(column_name,',' ORDER BY position) INTO select_columns FROM table_columns 
			WHERE table_name = :table_name
				AND schema_name = current_schema 
				AND column_name NOT IN (SELECT EXCLUDE_COLUMNS FROM :tbl_pivot_columns);
		end if;
	END IF;	

	v_statement = 'select ' || select_columns || ', ' || 'map(element_number';

	/*
		generate the statement
	*/
	FOR v_idx IN 1.. v_count DO
		v_statement_map1 = v_statement_map1 || ',' || v_idx  || ',''' || :a_unpivot_column_values[:v_idx] || '''' ;
		v_statement_map2 = v_statement_map2 || ',' || v_idx  || ',' || :a_unpivot_columns[:v_idx]  ;
	END FOR;
	v_statement = v_statement || v_statement_map1 || ') as "'|| unpivot_col_name || '", map(element_number ' || v_statement_map2 || ') as "' || unpivot_val_name || '" from ' || table_name || ', SERIES_GENERATE_INTEGER(1,1,' || v_count+1 || ') order by element_number';

	IF include_nulls = FALSE
		THEN
		v_statement = 'select * from (' || v_statement || ' ) where ' || unpivot_val_name || ' is not null';
	END IF;
	
	IF EXISTS(SELECT 1 FROM SYS.M_TEMPORARY_TABLES WHERE CONNECTION_ID = CURRENT_CONNECTION AND TABLE_NAME=:result_table_name AND SCHEMA_NAME = CURRENT_SCHEMA AND TEMPORARY_TABLE_TYPE = 'LOCAL') then
		EXEC 'drop table "' || :result_table_name || '"';
	END IF;
	EXEC 'create local temporary table "' || :result_table_name || '" as (' || :v_statement || ')';

END;

