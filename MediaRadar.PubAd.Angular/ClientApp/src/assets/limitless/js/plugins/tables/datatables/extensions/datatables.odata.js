function fnServerOData(sUrl, aoData, fnCallback, oSettings) {

    var oParams = {};
    $.each(aoData, function (i, value) {
        oParams[value.name] = value.value;
    });

    var data = {};

    var bJSONP = oSettings.oInit.bUseODataViaJSONP;
    if (oSettings.oFeatures.bServerSide) {
        data.$skip = oSettings._iDisplayStart;
        if (oSettings._iDisplayLength > -1) {
            data.$top = oSettings._iDisplayLength;
        }
        if (oSettings.oInit.iODataVersion !== null && oSettings.oInit.iODataVersion < 4) {
            data.$inlinecount = "allpages";
        } else {
            data.$count = true;
        }

        if (!asFilters)
            asFilters = [];

        $.each(oSettings.aoColumns,
            function (i, value) {
                let allowSearch = false;
                if (value.sClass === "quick-searchable") {
                    allowSearch = true;
                }
                if (allowSearch) {
                    var sFieldName = value.sName || value.mData;
                    if (oParams.sSearch !== null && oParams.sSearch !== "" && value.bSearchable && value.searching) {
                        switch (value.sType) {
                            case 'string':
                            case 'html':
                                asFilters.push("indexof(" + sFieldName + ", '" + oParams.sSearch + "') gt -1");
                                break;
                            case 'date':
                            case 'numeric':
                            default:
                            // Currently, we cannot search date and numeric fields (exception on the OData service side)
                        }
                    }
                }
            });

        //Added if condition to fix blank filter issue
        if (asFilters.length > 0) {
            data.$filter = asFilters.join(" and ");
        }

        var asOrderBy = [];
        for (var i = 0; i < oParams.iSortingCols; i++) {
            asOrderBy.push(oParams["mDataProp_" + oParams["iSortCol_" + i]] + " " + (oParams["sSortDir_0"] || ""));
        }
        data.$orderby = asOrderBy.join();
    }

    $.ajax({
        "url": sUrl,
        "data": data,
        "jsonp": bJSONP,
        "dataType": bJSONP ? "jsonp" : "json",
        "jsonpCallback": data["$callback"],  //$callback is not supported by web api right now
        "cache": false,
        "error": function () {
            alert("Unexpected Error!");
            UIUtiliy.unblockControl(".blocky");
        },
        "success": function (data) {
            var oDataSource = {};
            // Probe data structures for V4, V3, and V2 versions of OData response
            oDataSource.aaData = data.value || (data.d && data.d.results) || data.d;
            var iCount = data.d.__count;

            if (iCount == null) {
                if (oDataSource.aaData.length === oSettings._iDisplayLength) {
                    oDataSource.iTotalRecords = oSettings._iDisplayStart + oSettings._iDisplayLength + 1;
                } else {
                    oDataSource.iTotalRecords = oSettings._iDisplayStart + oDataSource.aaData.length;
                }
            } else {
                oDataSource.iTotalRecords = iCount;
            }

            oDataSource.iTotalDisplayRecords = oDataSource.iTotalRecords;

            if (UIUtiliy) {
                UIUtiliy.unblockControl(".blocky");
            }

            fnCallback(oDataSource);
        }
    });

} // end fnServerData