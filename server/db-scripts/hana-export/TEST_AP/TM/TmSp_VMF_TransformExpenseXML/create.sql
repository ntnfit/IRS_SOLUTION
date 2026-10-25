CREATE PROCEDURE "TmSp_VMF_TransformExpenseXML" (
       XML nvarchar(8000),
       SystemID int, 
       AutoFill smallint default 0,
       IgnoreError smallint default 0)
AS
BEGIN

       declare BomXML nvarchar(8000);
       declare BomResult nvarchar(8000);
       declare Error nvarchar(300);
       
       declare bchar nvarchar(5);
       declare echar nvarchar(5);
       declare blen smallint;
       declare elen smallint;
       
       declare item nvarchar(1000);
       declare b1item nvarchar(100);
       declare count1 int;
       
       declare tagName nvarchar(200);
       declare ctagName nvarchar(200);
       declare position1 int;
       declare position2 int;
       declare position3 int;
       declare position4 int;
       
       declare ObjectID int;
       declare ObjAbbr nvarchar(10);

       declare fullLen int;
       
       declare b1emp nvarchar(100);
       
       declare sqltxt nvarchar(1000);
       declare tableName nvarchar(100);
       declare fieldname nvarchar(100);
       
       create local temporary table  #objList (TagName nvarchar(200), ObjectID int, ObjAbbr nvarchar(10));
       
       BomXML = coalesce(:XML, ''); 
       BomResult = ''; 

       bchar = '<';
       echar = '>';
       blen = LENGTH(:bchar);
       elen = LENGTH(:echar);
       
       insert into #objList (TagName, ObjectID, ObjAbbr) values ('ExpenseType', 242000007, 'EXT');
       --insert into @objList (TagName, ObjectID, ObjAbbr) values ('VatGroup', 5, 'VTG')
       insert into #objList (TagName, ObjectID, ObjAbbr) values ('Currency', 37, 'CRN');
       insert into #objList (TagName, ObjectID, ObjAbbr) values ('CardCode', 171, 'HEM');
       insert into #objList (TagName, ObjectID, ObjAbbr) values ('ProjectCode', 63, 'PRJ');
       insert into #objList (TagName, ObjectID, ObjAbbr) values ('CostingCode', 61, 'PRC'); 

       fullLen = LENGTH(:BomXML);

       position1 = LOCATE(:BomXML, :bchar, 1);

       while :position1 > 0
       DO
             position2 = LOCATE(:BomXML, :echar, :position1 +:blen); -- (+1) - position of the > for begin tag
             
             if (SUBSTRING(:BomXML, :position2 -1, 1) <> '/' AND SUBSTRING(:BomXML, position1 + :blen, 1) <> '/') -- empty tag or closing tag
             THEN
                    tagName = SUBSTRING(:BomXML, position1 + :blen, :position2 - :position1 -:blen);

                    position3 = LOCATE(:BomXML, :bchar, :position2 + :elen);
                    if (SUBSTRING(:BomXML, :position3 + :blen, 1) = '/')
                    THEN
                           position4 = LOCATE(:BomXML, :echar, :position3 + :blen);
                           ctagName = SUBSTRING(:BomXML, :position3 +:blen+1, :position4 - :position3 -:blen -1);

                           select count(*) into count1 from #objList where TagName = :tagName;
                           
                           if (:tagName = :ctagName AND :count1 = 1) -- TODO: namespaces can be in begin tag
                           THEN
                                  item = SUBSTRING(:BomXML, :position2 + :elen, :position3 - :position2 - :elen);

                                  if (TRIM(:item) <> '')
                                  THEN
                                        select ObjectID, ObjAbbr into ObjectID, ObjAbbr from #objList where TagName = :tagName;

                                        select count(*) into count1 from "OMPO"
                                        inner join "MPO1" on "OMPO"."AbsEntry" = "MPO1"."AbsEntry"
                                        where "ThirdPVal" = :item and "ThirdPID" = :SystemID and "ObjectId" = :ObjectID;

                                        if :count1 = 1
                                        THEN
                                               select "ObAbsEntry" into b1item from "OMPO"
                                               inner join "MPO1" on "OMPO"."AbsEntry" = "MPO1"."AbsEntry"
                                               where "ThirdPVal" = :item and "ThirdPID" = :SystemID and "ObjectId" = :ObjectID;

                                               -- emp to crd
                                               IF :ObjAbbr = 'HEM'
                                               THEN                                                  
                                                      b1emp = :b1item;
                                                      select "BPLink" into b1item from "OHEM" where "empID" = :b1emp;
                                               END IF;

                                               BomResult = :BomResult || SUBSTRING(:BomXML, 1, :position2 + :elen -1) || :b1item;
                                               BomXML = SUBSTRING(:BomXML, :position2 + :elen + LENGTH(:item), :fullLen);
                                               position2 = 0;
                                        
                                        ELSE                                           
                                               if :count1 > 1
                                               THEN
                                                      Error = '<error>Inconsistent Mapping!</error>';                                               
                                               ELSE
                                                      Error = '<error>Missing Mapping!</error>';
                                                      if :AutoFill = 1
                                                      THEN                                                  

                                                            tableName = '@BE_VM_' || CAST(:SystemID as nvarchar(10)) || '_' || :ObjAbbr;
                                                            select 'U_' || "ColAlias" into fieldname from "UKD1" where "TableName" = :tableName;                                                          
                                                            
                                                            sqltxt = 
                                                            'insert into "' || :tableName || '" ("' || :fieldname || '", "Code") ' ||
                                                            'select ''' || :item || ''' as "' || :fieldname || '", (select COALESCE(max("Code") +1,1) from "' || :tableName || '") as "Code" from DUMMY t1 ' ||
                                                            'left join "' || :tableName || '" t2 on t2."' || :fieldname || '" = ''' || :item || ''' ' ||
                                                            'where t2."' || :fieldname || '" is null';
                                                            
                                                            EXEC :sqltxt;
                                                            
                                                      END IF;
                                               END IF;
                                        END IF;
                                  END IF;
                           END IF;
                    END IF;
             END IF;

             BomResult = :BomResult || SUBSTRING(:BomXML, 1, :position2 + :elen -1);
             BomXML = SUBSTRING(:BomXML, :position2 + :elen, :fullLen);

             position1 = LOCATE(:BomXML, :bchar, 1);
       END WHILE;

       DROP TABLE #objList;
       
       
       if (:IgnoreError = 2)
       then
             SELECT :BomResult from dummy;
       else   
             if (:IgnoreError = 1)
             then
                    SELECT :BomResult || coalesce(CHAR(13) || :Error, '') from dummy;         
             else         
                    if (coalesce(:Error, '') = '')
                    then
                           SELECT :BomResult from dummy;                 
                    else                
                           SELECT :Error from dummy;
                    end if;
             end if;
       end if;
END;


