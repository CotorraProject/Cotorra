'use stric';

//Check for namespaces
if (!UX.Cotorra) { UX.Cotorra = {}; }
if (!UX.Cotorra.EmployerRegistration) { UX.Cotorra.EmployerRegistration = {}; }

UX.Cotorra.Common = {
    SetQuickSearch: (inputSelector, dataGridSelector) => {
        $(inputSelector).val('');
        $(inputSelector).off('keyup').on('keyup', function (e) {
            var textToSearch = this.value.toLowerCase();

            let selectors = dataGridSelector.split(',');

            let dataSourcesArray = [];
            let filtersArray = [];

            for (let x = 0; x < selectors.length; x++) {
                let selector = $.trim(selectors[x]);
                var $kendoData = $(selector);

                if ($kendoData.data('kendoGrid')) {
                    var dataSource = $kendoData.data('kendoGrid').dataSource;
                    var columsName = $kendoData.data('kendoGrid').columns;
                    var field = dataSource.options.schema.model.fields;
                    var filters = [];
                    for (var i = 0; i < columsName.length - 1; i++) {

                        var colum = columsName[i];
                        if (!field[colum.field] || !field[colum.field].type) { continue; }

                        if (!textToSearch || textToSearch === '') { continue; }

                        switch (field[colum.field].type) {
                            case 'number': filters.push({ field: colum.field, operator: 'equals', value: textToSearch }); break;
                            case 'string': filters.push({ field: colum.field, operator: 'contains', value: textToSearch }); break;
                        }
                    }

                    dataSourcesArray.push(dataSource);
                    filtersArray.push({ logic: 'or', filters: filters })
                }

                for (let x = 0; x < dataSourcesArray.length; x++) {
                    dataSourcesArray[x].filter(filtersArray[x]);
                }
            }

        });
    },

    GetRowData: (obj = null) => {

        if (obj === null) { return null; };

        //Get kendogrid
        let $kendoGrid = $($(obj).closest('.full-kendo-grid.k-grid')).data('kendoGrid');

        //Try with kendotreelist
        if ($kendoGrid === undefined) { $kendoGrid = $($(obj).closest('.full-kendo-grid.k-grid')).data('kendoTreeList'); }

        if ($kendoGrid === undefined) { return null; }

        //Get row to delete
        let uid = $(obj).closest('tr').data('uid');
        var el = $(obj).closest('tr');
        var dataItem = $kendoGrid.dataSource.getByUid(uid);

        return { uid, el, dataItem, $kendoGrid };
    },

    GetZipCodeInfo: (zipCode, onSuccess = null, onError = null, onComplete = null) => {
        return $.ajax({
            method: 'GET',
            url: CotorraAppURL + '/Home/GetZipCodeInfo/',
            data: {
                zipCode: zipCode
            },
            success: function (data) {

                let result = [];
                if ($(data).find('#dataTablesearch')) {
                    let trs = $(data).find('#dataTablesearch tbody tr');

                    for (var i = 0; i < trs.length; i++) {
                        let tr = $(trs[i]);

                        if (tr.children().length > 1) {
                            result.push({
                                suburb: $(tr.children()[0]).find('a').html(),
                                municipality: $(tr.children()[3]).find('a').html(),
                                state: $(tr.children()[6]).find('a').html()
                            });
                        }
                    }
                }
                onSuccess.call(this, result);

            },
            error: onError,
            complete: onComplete
        });
    },

    InitSearchZipCode: (container, model, fv) => {

        let $container = $(container);

        let lookupZipCode = function (zipCode) {
            //Set new data
            UX.Loaders.SetLoader(container);
            UX.Cotorra.EmployerRegistration.UI.SetZipCodeInfo({ zipCode: zipCode }, model,
                function () {
                    UX.Loaders.RemoveLoader(container);
                    $(fv).data('formValidation').revalidateField('np_er_drpFederalEntity');
                    $(fv).data('formValidation').revalidateField('np_er_drpMunicipality');
                });
            return false;
        };

        $container.find('#np_er_txtZipCode').mask('00000');

        $container.find('#np_er_txtZipCode').off('keypress').on('keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();
                //Get zip code
                let zipCode = $(this).val();
                lookupZipCode(zipCode);
            }
        });

        $container.find('#np_btnSearchZipCode').off('click').on('click', function (e) {
            e.preventDefault();
            //Get zip code
            let zipCode = $("#np_er_txtZipCode").val();
            lookupZipCode(zipCode);
        });
    },

    SetZipCodeInfo: (container, model) => {
        UX.Loaders.SetLoader(container);
        UX.Cotorra.EmployerRegistration.UI.SetZipCodeInfo(
            {
                zipCode: model.get('ZipCode'),
                State: model.get('FederalEntity'),
                Municipality: model.get('Municipality'),
                Suburb: model.get('Suburb')
            },
            model,
            function () { UX.Loaders.RemoveLoader(container); }
        );
    },

    FormatCurrency: (value) => {
        if (value === null) { return '-'; }
        return UX.Common.FormatCurrencyCustomValues(value, 2, '.', ',', '$', 1);
    },

    GridFormatCurrency: (field) => {
        let template = '<div class="currency-grid-value # if(' + field + ' < 0) { # negative # } #"> #= UX.Cotorra.Common.FormatCurrency(' + field + ') # </div>';
        return kendo.template(template);
    },

    GridFormatDate: (field) => {
        let template = '#= moment(' + field + ').format("DD/MM/YYYY") #';
        return template;
    },

    GridFormatPerc: (field) => {
        let template = '#= ' + field + ' #' + '%';
        return template;
    },

    GridFormatBoolean: (field) => {
        let template =
            '<i class="# if(' + field + ') { # fad fa-check-circle boolean-grid-value-true # } else { # fad fa-minus boolean-grid-value-false # } #"> </i>';
        return kendo.template(template);
    },

    GridFormatEmployeeTrustLevelToBoolean: (field) => {
        let template =
            '<i class="# if(' + field + ' == 1) { # fad fa-check-circle boolean-grid-value-true # } else { # fad fa-minus boolean-grid-value-false # } #"> </i>';
        return kendo.template(template);
    },

    GridFormatCatalogProp: (array, columnName, propCompare, propResult) => {
        return "#= UX.Common.GetCatalogPropertyValue(" + array + ", " + columnName + ", '" + propCompare + "', '" + propResult + "') #";
    },

    GridCommonActionsTemplate: (namespace) => {

        let html =
            '<div class="text-left">' +
            '   <button class="btn btn-icon-small btn-dblclick" title="Editar" onclick="UX.Cotorra.' + namespace + '.UI.OpenSave(this);">' +
            '      <i class="far fa-pencil"></i>' +
            '   </button>' +
            '   &nbsp;' +
            '   <button class="btn btn-icon-small" title="Eliminar" onclick="UX.Cotorra.' + namespace + '.UI.Delete(this);">' +
            '      <i class="far fa-trash-alt"></i>' +
            '   </button>' +
            '</div >';

        return kendo.template(html);
    }
};

UX.Common.GetCatalogPropertyValue = (array, fieldValue, propCompare, propResult) => {

    if (!array || array.length === 0) {
        return "N/A";
    }

    let item = null;
    item = array.find((e) => {
        if (!e[propCompare]) {
            return false;
        }

        let typeOf = typeof e[propCompare];
        if (typeOf === 'string') {
            return e[propCompare] === fieldValue;
        } else if (typeOf === 'number') {
            try {
                return e[propCompare] === parseFloat(fieldValue);
            } catch (e) {
                return false;
            }
        } else if (typeOf === 'boolean') {
            try {
                return e[propCompare] === (fieldValue === 'true');
            } catch (e) {
                return false;
            }
        }

    });

    if (item) {
        if (propResult.indexOf('()') > -1) {
            return item[propResult.replace('()', '')]();
        } else {
            if (item[propResult]) {
                return item[propResult];
            }
        }
    }
    return "N/A";
};

UX.Common.InitGrid = (params = {}) => {

    let kg = $(params.selector).data('kendoGrid');
    if (kg) { kg.destroy(); $(params.selector).html(''); }

    $(params.selector).kendoGrid({
        dataSource: {
            data: params.data,
            schema: { model: { fields: params.fields } },
            aggregate: params.aggregate ? params.aggregate : null,
            pageSize: params.pageSize ? params.pageSize : 100 /*UX.Common.GetDefaultPageSize()*/
        },
        pageable: params.pageable === undefined
            ? { refresh: false, alwaysVisible: true, pageSizes: [10, 50, 100, 'all'], }
            : params.pageable,
        editable: params.editable === undefined ? false : params.editable,
        filterable: params.filterable === undefined ? false : params.filterable,
        scrollable: params.scrollable === undefined ? true : params.scrollable,
        resizable: params.resizable === undefined ? true : params.resizable,
        sortable: params.sortable === undefined ? true : params.sortable,
        columnMenu: params.columnMenu === undefined ? false : params.columnMenu,
        selectable: params.selectable === undefined ? false : params.selectable,
        refresh: false,
        theme: UX.Common.UserSettings.KendoTheme,
        columns: params.columns,
        dataBound:
            params.dataBound === undefined ?
                function (e) {
                    $(this.table).find('tr').off('dblclick').on('dblclick', function () { $(this).find('.btn-dblclick').click(); });
                } :
                params.dataBound,
        excelExport: params.excelExport === undefined ? { fileName: "Export.xlsx" } : params.excelExport,
    });

    $(params.selector).resizeKendoGrid();

    return $(params.selector).data('kendoGrid');
};

UX.Common.ErrorModal = (error) => {
    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
};

UX.Common.ValidDateValidator = function (value, validator) {
    //Do notEmpty validation
    if (value === "") { return true; }

    var m1 = new moment(value, 'DD/MM/YY', true);
    var m2 = new moment(value, 'DD/MM/YYYY', true);
    if (m1.isValid() || m2.isValid()) {

        if (m1.isAfter('2100-12-31', 'year') || m2.isAfter('2100-12-31', 'year')) {
            return {
                valid: false, message: 'Formato de fecha inválido'
            };
        }

        if (m1.isBefore('1900-01-01', 'year') || m2.isBefore('1900-01-01', 'year')) {
            return {
                valid: false, message: 'Formato de fecha inválido'
            };
        }

        return true;
    }

    return {
        valid: false, message: 'Formato de fecha inválido'
    };
};

UX.Common.StrToDate = function (dateString) {
    let date = UX.Common.ValidateDate(dateString);
    return moment(date, "DD/MM/YYYY")._d;
};

UX.Common.ClearFocus = function () {
    document.activeElement.blur();
};


UX.Common.InitDatePicker = (params = {}) => {
    $(params.selector).datepicker({
        multidate: params.multidate ? params.multidate : false,
        format: 'dd/mm/yyyy',
        language: 'es',
    });

    if (params.model) {
        $(params.selector).datepicker()
            .on('changeDate', function (e) {
                params.model.set(params.keyPath, e.dates.map(x => { return moment(x).format('DD/MM/YYYY') }).join(','));
            });
    }

    if (params.value) {
        if (params.multidate && params.multidate === true) {
            $(params.selector).datepicker('setDates', params.value.split(',').map(x => { return moment(x, 'DD/MM/YYYY')._d; }));
        } else {
            $(params.selector).datepicker('setDate', moment(params.value, 'DD/MM/YYYY')._d);
        }
    }
};

UX.Common.SetDatePicker = (params = {}) => {
    $(params.selector).datepicker('setDates',
        params.value.includes(',') ? params.value.split(',').map(x => { return moment(x, 'DD/MM/YYYY')._d; }) : params.value);
};


UX.Common.KendoGridRemoveHTMLExcelExport = (params = {}) => {
    var rows = params.e.workbook.sheets[0].rows;
    for (var ri = 0; ri < rows.length; ri++) {
        var row = rows[ri];
        if (row.type == "group-footer" || row.type == "footer") {
            for (var ci = 0; ci < row.cells.length; ci++) {
                var cell = row.cells[ci];
                if (cell.value) {
                    cell.value = $(cell.value).text();
                    cell.hAlign = "right";
                }
            }
        }
    }
}

UX.Loaders.SetLoader = function (container) {

    //If container exister, then get out
    if ($(container + " .loader-container").length > 0) {
        return;
    }

    let loader = '<div class="lds-ring"><div></div><div></div><div></div><div></div></div>';
    let loaderContainer = $("<div class='loader-container'></div>");
    $(loaderContainer).append(loader);
    $(container).append(loaderContainer);

    $(loaderContainer)
        .css('height', $(loaderContainer).parent().outerHeight() + "px")
        .css('width', $(loaderContainer).parent().outerWidth() + "px");

    return $(container).last();
};

UX.Loaders.RemoveLoader = function (container) {
    //Get the loader
    let loader = $(container).find(".loader-container, .loader-container-alt");

    //If loader is visible, fade it out, if not, just remove it
    if (loader.is(":visible")) {
        loader.remove();
    } else {
        loader.remove();
    }
};

UX.Modals.OpenCentralModal = function (title, size, container, html, callback, onClose) {

    if (size === undefined) { size = "medium"; }
    else if (size === "s") { size = "small"; }
    else if (size === "m") { size = "medium"; }
    else if (size === "xm") { size = "x-medium"; }
    else if (size === "l") { size = "large"; }
    else { size = "medium"; }

    //Generate the unique ID
    var guid = Guid.create().value;
    var modalID = "centralmodal_" + guid;

    container = "body";

    $(container).append("<div class='modal-main-container " + guid + "'><div class='side-modal-wrapper'></div></div>");
    container = container + " > .modal-main-container." + guid + " >" + ".side-modal-wrapper";

    //Add the backdrop and the modal
    $(container).append("<div class='side-modal-container " + size + "' id='" + modalID + "'>" + "<div class='title'>" + "<label>" + title + "</label>" +
        "<a href='#'><i class='fas fa-times'></i></a>" +
        "</div>" + "<div class='body'> " + html + "</div>" + "</div>");

    //Hide them :)
    $(container + " > .side-modal-container").hide();

    //Animate them
    var delay = 250;
    //$(container + " > .side-modal-container").show().fadeIn(100).addClass('animated bounceIn');
    $(container + " > .side-modal-container").fadeIn(200).addClass('animated fadeInDown');

    //Set close behavior
    $(container + " > .side-modal-container > .title > a").on("click", function (ev) {
        UX.Modals.CheckIfCanClose(modalID, onClose);
    });

    //Lets make a little pause to call the callback
    setTimeout(function () {
        //If there is a callbak, then call it
        if (callback !== undefined && callback !== null) {
            callback.call();
        }
    }, 10);

    return modalID;
};