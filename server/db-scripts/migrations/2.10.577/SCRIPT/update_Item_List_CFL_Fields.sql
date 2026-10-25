
update APZ_Item_ListCFLField set "QueryField"='CASE WHEN IFNULL(:ecVatGroup, '''') = '''' THEN CASE WHEN IFNULL(A."VatGroupPu", '''') = '''' THEN IFNULL((select "Rate" from OVTG where "Code"=:DfPVatItem), 0) ELSE IFNULL(k."Rate", 0) END ELSE (select "Rate" from OVTG where "Code"=:ecVatGroup) END' where "FieldName"='RatePu';

GO

update APZ_Item_ListCFLField set "QueryField"='CASE WHEN IFNULL(:ecVatGroup, '''') = '''' THEN CASE WHEN IFNULL(A."VatGourpSa", '''') = '''' THEN IFNULL((select "Rate" from OVTG where "Code"=:DfSVatItem), 0) ELSE IFNULL(g."Rate", 0) END ELSE (select "Rate" from OVTG where "Code"=:ecVatGroup) END' where "FieldName"='RateSa'; 