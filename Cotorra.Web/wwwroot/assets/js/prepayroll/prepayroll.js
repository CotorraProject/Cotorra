'use strict';

UX.Cotorra.PrePayroll = {
    Get: (data, action = 'Get', onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'GET',
            url: CotorraAppURL + '/' + 'PrePayroll' + '/' + action + '/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    Save: (data, action = 'Save', onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'POST',
            url: CotorraAppURL + '/' + 'PrePayroll' + '/' + action + '/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    UI: {
        CatalogName: 'PrePayroll',
        ContainerSelector: '#np-prepayroll',
        TitleModalsString: 'Prenómina',
        PrePayrollModel: {},
        OverdraftModel: {},
        PeriodTypesOptions: [],
        Header: {
            PeriodTypeID: null,
            PeriodID: null,
            PeriodDetailID: null,
        },

        Init: function () {

            let catalogName = this.CatalogName;
            let containerSelector = this.ContainerSelector;

            let model = UX.Cotorra.PrePayroll.UI.PrePayrollModel = new Ractive({
                el: '#np-prepayroll',
                template: '#np-prepayroll-template',
                data: {
                    WorkPeriod: {
                        Period: '- - -',
                        PeriodType: '',
                        PeriodFiscalYear: '',
                        PeriodNumber: '',
                        Dates: 'Selecciona tu periodo',
                        PeriodStatus: 0
                    },
                    Data: null,
                    ShowAuthorize: false,
                    ShowStamp: false,
                }
            });

            //Set top action behaviors
            $('#np-prepayroll #np_btnRecalculatePayroll').off('click').on('click', function (ev) {
                UX.Cotorra.PrePayroll.UI.AuthorizePayroll();
            });

            $('#np-prepayroll #np_btnStampPayroll').off('click').on('click', function (ev) {
                UX.Cotorra.PrePayroll.UI.StampingPayroll();
            });

            $('#np-prepayroll #np_btnSummaryPayroll').off('click').on('click', function (ev) {
                UX.Cotorra.PrePayroll.UI.OpenSummary();
            });

            $('#np-prepayroll #np_btnDispersePayroll').off('click').on('click', function (ev) {
                UX.Cotorra.PrePayroll.UI.Disperse();
            });

            $('#workPeriod').on('click', function () {
                UX.Cotorra.PrePayroll.UI.OpenPeriodSelection('period');
            });

            //Quick search
            UX.Cotorra.Common.SetQuickSearch('#np-prepayroll .quicksearch', '#np-prepayroll .np-dpo-grid, #np-prepayroll .np-dh-grid');

            $('#np-prepayroll .tab-nav a').off('click').on('click', function () {

                setTimeout(function () {
                    $('#dpo-quicksearch').keyup();
                    $('#np_prepayroll').click();
                }, 25);
            });

            (async function () {

                UX.Cotorra.PrePayroll.UI.LoadDPOGrid([]);
                UX.Cotorra.PrePayroll.UI.LoadDHGrid([]);

                let periodTypesRequest =
                    UX.Cotorra.Catalogs.Do('GET', 'PrePayroll', 'GetActivePeriodTypes', {},
                        (data) => { UX.Cotorra.PeriodTypesOptions = data }, (error) => { }, (complete) => { });

                setTimeout(async function () {

                    await UX.Cotorra.Catalogs.Require({
                        loader: {
                            container: '#np-prepayroll',
                            removeWhenFinish: false
                        },
                        catalogs: ['Banks', 'Concepts', 'Employees', 'Positions'],
                        requests: [periodTypesRequest],
                        //requests: [UX.Cotorra.PeriodTypesRequest],
                        forceLoad: false
                    });

                    if (UX.Cotorra.PeriodTypesOptions.length == 1) {

                        let row = {
                            dataItem: UX.Cotorra.PeriodTypesOptions[0]
                        };

                        UX.Cotorra.PrePayroll.UI.SelectPeriodType({
                            PeriodTypeID: row.dataItem.ID,
                            PeriodID: row.dataItem.PeriodID,
                            PeriodDetailID: row.dataItem.PeriodDetailID,
                            PeriodTypeName: row.dataItem.Name,
                            CurrentYear: row.dataItem.CurrentYear,
                            PeriodDetailNumber: row.dataItem.Number,
                            InitialDate: row.dataItem.InitialDate,
                            FinalDate: row.dataItem.FinalDate,
                            PeriodStatus: row.dataItem.PeriodStatus,
                            LoaderID: '#np-prepayroll',
                        });
                    }
                    else {
                        UX.Cotorra.PrePayroll.UI.OpenPeriodSelection();
                    }

                }, 150);


                //Auto select first period type
                //setTimeout(function () {
                //    let tr = $('.np-pp-selectperiod-grid tbody tr');
                //    if (tr.length === 1) {
                //        tr.dblclick();
                //    }
                //}, 50);
            })();
        },

        LoadDPOGrid: function (data = [], cols = []) {

            if (data.length > 0) {
                data = data.map(x => {
                    x.TextSearch = x.EmployeeCode + '|' + x.EmployeeName
                    return x;
                })
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                OverdraftStatus: { type: 'number' },
                OverdraftType: { type: 'number' },

                EmployeeID: { type: 'string' },
                EmployeeCode: { type: 'number' },
                EmployeeDailySalary: { type: 'number' },
                EmployeeJobPosition: { type: 'string' },
                EmployeeName: { type: 'string' },

                TotalPerceptions: { type: 'number' },
                TotalDeductions: { type: 'number' },
                TotalLiabilities: { type: 'number' },

                UUID: { type: 'string' },

                TextSearch: { type: 'string' }
            };

            //Set columns
            let columns = [
                //{
                //    field: 'EmployeeCode', title: 'Código', width: 80,
                //},
                //{
                //    field: 'EmployeeName', title: 'Colaborador',
                //},
                {
                    title: 'Colaborador', field: 'EmployeeCode',
                    template: kendo.template($('#dpoEmployeeTemplate').html()),
                },

                {
                    field: 'TotalPerceptions', title: 'Percepciones', width: 150,
                    template: UX.Cotorra.Common.GridFormatCurrency('TotalPerceptions')
                },
                {
                    field: 'TotalDeductions', title: 'Deducciones', width: 150,
                    template: UX.Cotorra.Common.GridFormatCurrency('TotalDeductions')
                },
                {
                    field: 'TotalLiabilities', title: 'Obligaciones', width: 150,
                    template: UX.Cotorra.Common.GridFormatCurrency('TotalLiabilities')
                },
                {
                    field: 'OverdraftStatus', title: 'Estatus', width: 100,
                    template: kendo.template($('#overdraftStatusActionTemplate').html()),
                },
                //{
                //    field: 'OverdraftType', title: 'Tipo', width: 80,
                //    template: kendo.template($('#overdraftTypeActionTemplate').html()),
                //},
                {
                    field: 'TextSearch', title: '', width: 0,
                    hidden: true,
                },
                {
                    title: ' ', width: 60,
                    template: kendo.template($('#prepayrollActionTemplate').html()),
                },

            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-dpo-grid', data: data, fields: fields, columns: columns,
                pageSize: 20,
                dataBound: function () {
                    $(this.table).find('tr').off('dblclick').on('dblclick', function () {
                        $(this).find('.btn-dblclick').click();
                    });

                    $(this.table).find('div[data-toggle="tooltip"]').tooltip();
                }
            });
        },

        LoadDHGrid: function (data = []) {

            if (data.length > 0) {
                data = data.map(x => {
                    x.TextSearch = x.Code + '|' + x.FullName
                    return x;
                })
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                EmployeeID: { type: 'string' },
                Code: { type: 'string' },
                FullName: { type: 'string' },
                TextSearch: { type: 'string' }
            };

            //Set columns
            let columns = [
                //{
                //    title: ' ', width: 45,
                //},
                {
                    title: 'Colaborador', width: 350,
                    template: kendo.template($('#dhEmployeeTemplate').html()),
                },
                {
                    field: 'TextSearch', title: '', width: 0,
                    hidden: true,
                },
                //{
                //    field: 'Code', title: ' ', width: 50,
                //},
                //{
                //    field: 'FullName', title: 'Colaborador', width: 250,
                //},
            ];

            if (UX.Cotorra.PrePayroll.UI.Header.InitialDate && UX.Cotorra.PrePayroll.UI.Header.FinalDate) {
                let id = UX.Cotorra.PrePayroll.UI.Header.InitialDate;
                let fd = UX.Cotorra.PrePayroll.UI.Header.FinalDate;

                let checkDays = true;
                let addDays = 0;

                while (checkDays) {
                    moment.locale('es');
                    let date = moment(id).add(addDays, 'days').format("YYYYMMDD");
                    let finalDate = moment(fd).add(0, 'days').format("YYYYMMDD");
                    let dateTitle = moment(id).add(addDays, 'days').format("DD-MMM-YYYY");
                    let dataDate = moment(id).add(addDays, 'days').format("DD/MM/YYYY");
                    let dateDay = moment(id).add(addDays, 'days').format("dddd");
                    dateDay = dateDay.substr(0, 1).toUpperCase() + dateDay.substr(1);

                    columns.push({
                        field: "F_" + date, title: dateDay + '<br>' + dateTitle, width: 180,
                        sortable: false,
                        encoded: false
                    });

                    addDays++;

                    if (date === finalDate) {
                        checkDays = false;
                    }
                }
            } else {
                columns.push({
                    title: ' ', width: 100,
                    template: ''
                });
            }

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-dh-grid', data: data, fields: fields,
                columns: columns,
                pageSize: 20,
                dataBound: function (e) {

                    $(this.table).find('div[data-toggle="tooltip"]').tooltip();
                    $(this.table).find('.pp-ods-compensatiosettlement').closest('tr').addClass('compensation-row');
                    $(this.table).find('.pp-ods-ordinarysettlement').closest('tr').addClass('settlement-row');

                    let periodStatus = UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('WorkPeriod.PeriodStatus');
                    if (periodStatus > 1) {
                        $('.np-dh-grid .k-grid-content tr td').addClass('no-editable');
                        return;
                    }

                    //Set double click on cell
                    $('.np-dh-grid .k-grid-content tr:not(.compensation-row):not(.settlement-row) td').off('dblclick').on('dblclick', function (ev) {
                        //Get row data and td
                        let row = UX.Cotorra.Common.GetRowData(this);
                        let $td = $(this);

                        //Get id and date
                        let employee = {
                            ID: row.dataItem.ID,
                            Name: row.dataItem.FullName,
                            Code: row.dataItem.Code
                        }

                        let field = $($('.np-dh-grid .k-grid-header .k-grid-header-wrap tr th')[$td.index()]).data('field');
                        if (field === undefined) { return; }
                        let date = field.split('_');
                        let mDate = moment(date, "YYYYMMDD");

                        UX.Cotorra.PrePayroll.UI.EditIncidents(employee, mDate,
                            UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.PeriodDetailID'), row);
                    });
                }
            });
        },

        //Period types grid (selection)
        LoadPTGrid: function (data) {
            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
            };

            //Set columns
            let columns = [
                {
                    field: 'Name', title: 'Tipo de periodo'
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-pp-selectperiod-grid', data: data, fields: fields, columns: columns,
                pageable: { alwaysVisible: false, pageSizes: 500, },
            });
        },

        //Periods details grid (selection)
        LoadPDGrid: function (data, model = {}) {
            ////test data
            //if (data.length > 0) {
            //    data[0].PeriodStatus = 3;
            //    data[1].PeriodStatus = 3;
            //    data[2].PeriodStatus = 3;
            //    data[3].PeriodStatus = 2;
            //    data[4].PeriodStatus = 1;
            //}

            //Transform data
            data = data.map((x) => {
                return {
                    ID: x.ID,
                    PeriodStatus: x.PeriodStatus,
                    Number: x.Number,
                    InitialDate: x.InitialDate,
                    FinalDate: x.FinalDate,
                    PeriodDetail:
                        moment(x.InitialDate).locale('es').format('DD/MM/YYYY') + ' al ' +
                        moment(x.FinalDate).locale('es').format('DD/MM/YYYY')
                };
            })

            //Set fields
            let fields = {
                ID: { type: 'string' },
                PeriodStatus: { type: 'number' },
                Number: { type: 'string' },
                InitialDate: { type: 'date' },
                FinalDate: { type: 'date' },
            };

            //Set columns
            let columns = [
                {
                    field: 'PeriodStatus', title: 'Estatus', width: 80,
                    template: kendo.template($('#prepayrollPTActionTemplate').html()),
                },
                {
                    field: 'Number', title: 'No.', width: 70,
                },
                {
                    field: 'PeriodDetail', title: 'Periodo'
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-pp-selectperiod-grid', data: data, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
                dataBound: function () {
                    let data = $(this)[0].dataSource.data();

                    for (var i = 0; i < data.length; i++) {
                        let row = data[i];
                        let $tr = $('tr[data-uid="' + row.uid + '"]');

                        if (row.PeriodStatus === 0) {
                            $('tr[data-uid="' + row.uid + '"]').addClass('closed-period');
                        }
                    }
                }
            });
        },

        OpenPeriodSelection: function (type = 'periodType') {

            //Open period type selection
            let modalID = UX.Modals.OpenModal(
                'Selecciona tu tipo de periodo', type === 'periodType' ? 's' : 'm', '<div id="np-selectperiodtype"></div>',
                function () {
                    //Set ractive model
                    let ptSelection = new Ractive({
                        el: '#np-selectperiodtype',
                        template: '#np-selectperiodtype-template',
                        data: {
                            type: type,
                            PeriodTypeID: '',
                            PeriodID: '',
                            PeriodTypesOptions: UX.Cotorra.PeriodTypesOptions,
                            FiscalYearsOptions: []
                        }
                    });

                    $('#btnFilterPeriods').off('click').on('click', function (ev) {
                        UX.Cotorra.PrePayroll.UI.LoadPDGrid([]);
                        //Get period details
                        UX.Loaders.SetLoader('#' + modalID);
                        UX.Cotorra.PrePayroll.Get(
                            { periodID: ptSelection.get('PeriodID') },
                            'GetPeriodDetails',
                            (data) => {
                                UX.Cotorra.PrePayroll.UI.LoadPDGrid(data);

                                $('.np-pp-selectperiod-grid tr').off('dblclick').on('dblclick', function (ev) {
                                    //Get row data
                                    let row = UX.Cotorra.Common.GetRowData(this);

                                    if (row.dataItem.PeriodStatus === 0) {
                                        return;
                                    }

                                    UX.Cotorra.PrePayroll.UI.SelectPeriodType({
                                        PeriodTypeID: ptSelection.get('PeriodTypeID'),
                                        PeriodTypeName: ptSelection.get('PeriodTypesOptions').find(x => { return x.ID === ptSelection.get('PeriodTypeID'); }).Name,
                                        PeriodID: ptSelection.get('PeriodID'),
                                        CurrentYear: ptSelection.get('FiscalYearsOptions').find(x => { return x.ID === ptSelection.get('PeriodID'); }).Name,
                                        PeriodDetailID: row.dataItem.ID,
                                        PeriodDetailNumber: row.dataItem.Number,
                                        InitialDate: moment(row.dataItem.InitialDate).format('YYYY-MM-DDT00:00:00'),
                                        FinalDate: moment(row.dataItem.FinalDate).format('YYYY-MM-DDT00:00:00'),
                                        PeriodStatus: row.dataItem.PeriodStatus
                                    });
                                });
                            },
                            (error) => { UX.Common.ErrorModal(error); },
                            (complete) => { UX.Loaders.RemoveLoader('#' + modalID); });
                    });

                    if (type === 'periodType') {

                        //Set active period types grid
                        UX.Cotorra.PrePayroll.UI.LoadPTGrid(UX.Cotorra.PeriodTypesOptions);

                        $('.np-pp-selectperiod-grid tbody tr').on('dblclick', function (ev) {
                            //Get row data
                            let row = UX.Cotorra.Common.GetRowData(this);

                            UX.Cotorra.PrePayroll.UI.SelectPeriodType({
                                PeriodTypeID: row.dataItem.ID,
                                PeriodID: row.dataItem.PeriodID,
                                PeriodDetailID: row.dataItem.PeriodDetailID,
                                PeriodTypeName: row.dataItem.Name,
                                CurrentYear: row.dataItem.CurrentYear,
                                PeriodDetailNumber: row.dataItem.Number,
                                InitialDate: row.dataItem.InitialDate,
                                FinalDate: row.dataItem.FinalDate,
                                PeriodStatus: row.dataItem.PeriodStatus
                            });
                        });

                        $('#' + modalID).find('.title a').remove();
                    } else {
                        var removeLoader = false;
                        //Set observer for period type
                        ptSelection.observe('PeriodTypeID', function (newValue, oldValue, keypath) {
                            //Get fiscal years for period type
                            UX.Loaders.SetLoader('#' + modalID);
                            UX.Cotorra.PrePayroll.Get(
                                { periodTypeID: newValue },
                                'GetFiscalYears',
                                (data) => {
                                    let pid = UX.Cotorra.PrePayroll.UI.Header.PeriodID;
                                    let year = data.find((x) => { return x.ID === pid });
                                    //Set fiscal years
                                    ptSelection.set('FiscalYearsOptions', data);
                                    ptSelection.set('PeriodID', year ? year.ID : data[0].ID);

                                    $('#btnFilterPeriods').click();
                                },
                                (error) => { UX.Common.ErrorModal(error); },
                                (complete) => {
                                    if (removeLoader) {
                                        UX.Loaders.RemoveLoader('#' + modalID);
                                        removeLoader = true;
                                    }
                                });
                        }, { init: false, defer: true });

                        //Set actual values
                        ptSelection.set('PeriodTypeID', UX.Cotorra.PrePayroll.UI.Header.PeriodTypeID);

                        //Set empty grid
                        UX.Cotorra.PrePayroll.UI.LoadPDGrid([]);
                    }
                });

            if (type == 'periodType') {
                //$('#' + modalID + ' .title a').remove();
            }
        },

        RefreshWorkPeriod: function (params) {
        },

        SelectPeriodType: function (data = null) {

            //Set refresh
            UX.Cotorra.PrePayroll.UI.RefreshWorkPeriod = function (params) {
                UX.Cotorra.PrePayroll.UI.SelectPeriodType({
                    PeriodTypeID: data.PeriodTypeID,
                    PeriodTypeName: data.PeriodTypeName,
                    PeriodID: data.PeriodID,
                    CurrentYear: data.CurrentYear,
                    PeriodDetailID: data.PeriodDetailID,
                    PeriodDetailNumber: data.PeriodDetailNumber,
                    InitialDate: data.InitialDate,
                    FinalDate: data.FinalDate,
                    PeriodStatus: params.PeriodStatus,
                    LoaderID: '#np-prepayroll',
                    KeepGrids: true,
                });
            };

            //clear grids
            if (data.KeepGrids && data.KeepGrids === true) {

            } else {
                UX.Cotorra.PrePayroll.UI.LoadDPOGrid([]);
                UX.Cotorra.PrePayroll.UI.LoadDHGrid([]);
            }

            //Set data on header
            let model = UX.Cotorra.PrePayroll.UI.PrePayrollModel;
            model.set('Data', data);
            model.set('WorkPeriod.Period', data.PeriodTypeName + ' ' + data.CurrentYear + ' - ' + 'Periodo ' + data.PeriodDetailNumber);
            model.set('WorkPeriod.Dates', 'Del ' +
                moment(data.InitialDate).locale('es').format('dddd D [de] MMMM [de] YYYY') + ', al ' +
                moment(data.FinalDate).locale('es').format('dddd D [de] MMMM [de] YYYY'));
            model.set('WorkPeriod.PeriodStatus', data.PeriodStatus);

            model.set('WorkPeriod.PeriodType', data.PeriodTypeName);
            model.set('WorkPeriod.PeriodFiscalYear', data.CurrentYear);
            model.set('WorkPeriod.PeriodNumber', data.PeriodDetailNumber);
            model.set('WorkPeriod.FromDate', moment(data.InitialDate).locale('es').format('dddd D [de] MMMM [de] YYYY'));
            model.set('WorkPeriod.ToDate', moment(data.FinalDate).locale('es').format('dddd D [de] MMMM [de] YYYY'));

            //Set header values
            UX.Cotorra.PrePayroll.UI.Header.PeriodTypeID = data.PeriodTypeID;
            UX.Cotorra.PrePayroll.UI.Header.PeriodID = data.PeriodID;
            UX.Cotorra.PrePayroll.UI.Header.PeriodDetailID = data.PeriodDetailID;
            UX.Cotorra.PrePayroll.UI.Header.InitialDate = data.InitialDate;
            UX.Cotorra.PrePayroll.UI.Header.FinalDate = data.FinalDate;
            UX.Cotorra.PrePayroll.UI.Header.PeriodStatus = data.PeriodStatus;

            //Get the work period details
            let loaderID = '#' + $('#np-selectperiodtype').closest('.side-modal-container').attr('id');

            if (data.LoaderID) {
                loaderID = data.LoaderID;
            }

            UX.Loaders.SetLoader(loaderID);
            UX.Cotorra.PrePayroll.Get(
                {
                    periodDetailID: data.PeriodDetailID,
                    periodInitialDate: data.InitialDate,
                    periodEndDate: data.FinalDate
                },
                'GetWorkPeriod',
                (data) => {

                    //Transform data
                    let lists = UX.Cotorra.PrePayroll.UI.TransformWorkPeriodData(data);

                    let showStamp = lists.DPO.find(el => el.OverdraftStatus === 1) ? true : false;
                    UX.Cotorra.PrePayroll.UI.PrePayrollModel.set('ShowStamp', showStamp);
                    let showAuthorize = lists.DPO.find(el => el.OverdraftStatus === 0) ? true : false;
                    UX.Cotorra.PrePayroll.UI.PrePayrollModel.set('ShowAuthorize', showAuthorize);

                    //Load grids
                    UX.Cotorra.PrePayroll.UI.LoadDPOGrid(lists.DPO);
                    UX.Cotorra.PrePayroll.UI.LoadDHGrid(lists.DH);

                    //Close window
                    UX.Modals.CloseModal($('.side-modal-container').attr('id'));
                },
                (error) => { UX.Common.ErrorModal(error); },
                (complete) => {
                    UX.Loaders.RemoveLoader('#np-prepayroll');
                    UX.Loaders.RemoveLoader(loaderID);
                });
        },

        ReloadEmployeeOverdraft: function (row = null) {

            if (!row) { return; }

            let periodDetailID = UX.Cotorra.PrePayroll.UI.Header.PeriodDetailID;
            let periodInitialDate = UX.Cotorra.PrePayroll.UI.Header.InitialDate;
            let periodEndDate = UX.Cotorra.PrePayroll.UI.Header.FinalDate;
            let overdraftID = row.dataItem.OverdraftID;

            UX.Cotorra.PrePayroll.Get(
                {
                    periodDetailID: periodDetailID,
                    employeeID: row.dataItem.EmployeeID,
                    overdraftID: overdraftID,
                    periodInitialDate: periodInitialDate,
                    periodEndDate: periodEndDate
                },
                'GetWorkPeriod',
                (data) => {

                    let lists = UX.Cotorra.PrePayroll.UI.TransformWorkPeriodData(data);

                    //Get grids
                    let $dpoGrid = $('.np-dpo-grid').data('kendoGrid');
                    let $dhGrid = $('.np-dh-grid').data('kendoGrid');

                    //Get dataItems
                    let dpoDataItem = $('.np-dpo-grid').data('kendoGrid').dataSource.data().find(x => { return x.OverdraftID == overdraftID });
                    let dhDataItem = $('.np-dh-grid').data('kendoGrid').dataSource.data().find(x => { return x.OverdraftID == overdraftID });

                    //Refresh DPO
                    dpoDataItem.TotalDeductions = lists.DPO[0].TotalDeductions;
                    dpoDataItem.TotalLiabilities = lists.DPO[0].TotalLiabilities;
                    dpoDataItem.TotalPerceptions = lists.DPO[0].TotalPerceptions;
                    UX.Common.KendoFastRedrawRow($dpoGrid, dpoDataItem);

                    //Refresh DH
                    //Clear all columns from dh row
                    let dhDataItemCols = Object.keys(dhDataItem).filter(x => { return x.indexOf('F_') > -1 });
                    for (let i = 0; i < dhDataItemCols.length; i++) {
                        let colName = dhDataItemCols[i];
                        dhDataItem[colName] = '';
                    }

                    //Get columns from updated dh item
                    let dhListItemCols = Object.keys(lists.DH[0]).filter(x => { return x.indexOf('F_') > -1 });
                    for (let i = 0; i < dhListItemCols.length; i++) {
                        let colName = dhListItemCols[i];
                        dhDataItem[colName] = lists.DH[0][colName];
                    }

                    UX.Common.KendoFastRedrawRow($dhGrid, dhDataItem);

                },
                (error) => { },
                (complete) => { });
        },

        TransformWorkPeriodData: function (data) {
            var lists = {
                DPO: data.DPOData.map(overdraft => {
                    let employee = UX.Cotorra.Catalogs.Employees.find(x => x.ID == overdraft.EmployeeID);

                    let newEmployee = {
                        ID: overdraft.EmployeeID,
                        Code: employee.Code,
                        FullName: employee.FullName,
                        Position: UX.Cotorra.Catalogs.Positions.find(x => { return x.ID == employee.JobPositionID; }).Name,
                        OverdraftType: overdraft.OverdraftType,
                        OverdraftID: overdraft.OverdraftID,
                        EmployeeID: overdraft.EmployeeID
                    };

                    overdraft.EmployeeCode = newEmployee.Code;
                    overdraft.EmployeeName = newEmployee.FullName;
                    overdraft.EmployeeJobPosition = newEmployee.Position;

                    return overdraft;

                }).sort((a, b) => {
                    return a.EmployeeCode > b.EmployeeCode ? 1 : -1;
                }),
                DH: data.DPOData.map(overdraft => {
                    let employee = UX.Cotorra.Catalogs.Employees.find(x => x.ID == overdraft.EmployeeID);

                    let newEmployee = {
                        ID: overdraft.EmployeeID,
                        Code: employee.Code,
                        FullName: employee.FullName,
                        Position: overdraft.EmployeeJobPosition,
                        OverdraftType: overdraft.OverdraftType,
                        OverdraftID: overdraft.OverdraftID,
                        EmployeeID: overdraft.EmployeeID
                    };

                    let employeeIncidents = data.DHData.filter(ei => {
                        return ei.EmployeeID == overdraft.EmployeeID;
                    });

                    for (let i = 0; i < employeeIncidents.length; i++) {
                        let incident = employeeIncidents[i];
                        let columnName = 'F_' + moment(incident.Date).format("YYYYMMDD");

                        if (!newEmployee[columnName]) {
                            newEmployee[columnName] = '';
                            //newEmployee[columnName] += incident.Value + incident.Code;
                            newEmployee[columnName] += '<div class="pp-incident-' + incident.Code.toLowerCase() + '">' + (incident.TypeOfIncident === 3 ? incident.Value + ' ' : '') + incident.Name + '</div>';
                        } else {
                            //newEmployee[columnName] += ', ' + incident.Value + incident.Code;
                            newEmployee[columnName] += '<div class="pp-incident-' + incident.Code.toLowerCase() + '">' + (incident.TypeOfIncident === 3 ? incident.Value + ' ' : '') + incident.Name + '</div>'
                        }
                    }

                    return newEmployee;

                }).sort((a, b) => {
                    return a.Code > b.Code ? 1 : -1;
                })
            };

            return lists;
        },

        EditOverdraft: function (obj = null) {

            let row = UX.Cotorra.Common.GetRowData(obj);

            UX.Cotorra.Overdraft.UI.Init({
                Employee: {
                    ID: row.dataItem.EmployeeID,
                    Name: row.dataItem.EmployeeName,
                    Code: row.dataItem.EmployeeCode,
                    JobPositionName: row.dataItem.EmployeeJobPosition,
                    Salary: UX.Cotorra.Common.FormatCurrency(row.dataItem.EmployeeDailySalary)
                },
                Overdraft: {
                    ID: row.dataItem.ID,
                    Status: row.dataItem.OverdraftStatus,
                    UUID: row.dataItem.UUID,
                    OverdraftType: row.dataItem.OverdraftType
                },
                WorkPeriod: {
                    Period:
                        UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.PeriodTypeName') + ' ' +
                        UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.CurrentYear')
                },
                PrePayrollHeader: {
                    InitialDate: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.InitialDate').split('T')[0],
                    FinalDate: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.FinalDate').split('T')[0],
                },
                Row: row
            });

        },

        EditInfonavit: function (obj = null) {
            let row = UX.Cotorra.Common.GetRowData(obj);

            UX.Cotorra.Infonavit.UI.Init({
                Employee: {
                    ID: row.dataItem.EmployeeID,
                    Name: row.dataItem.EmployeeName,
                    Code: row.dataItem.EmployeeCode,
                },
                PeriodTypeID: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data').PeriodTypeID,
            });
        },

        EditFonacot: function (obj = null) {
            let row = UX.Cotorra.Common.GetRowData(obj);

            UX.Cotorra.Fonacot.UI.Init({
                Employee: {
                    ID: row.dataItem.EmployeeID,
                    Name: row.dataItem.EmployeeName,
                    Code: row.dataItem.EmployeeCode
                }
            });
        },

        EditIncidents: (employee, date, periodDetailID, row) => {
            UX.Cotorra.Incidents.UI.Init({
                PeriodDetailID: periodDetailID,
                Employee: employee,
                Date: date,
                OnSaveSucessCallback: function (sentData, responseData) {
                    //Incidencias (días y horas)
                    UX.Cotorra.PrePayroll.UI.ReloadEmployeeOverdraft(row);
                },
            });
        },

        EditInhability: function (obj = null) {
            let row = UX.Cotorra.Common.GetRowData(obj);

            UX.Cotorra.Inhability.UI.Init({
                Employee: {
                    ID: row.dataItem.ID,
                    Name: row.dataItem.FullName,
                    Code: row.dataItem.Code,
                },
                OnSaveSucessCallback: function (savedData, responseData) {
                    //Incapacidades(maternidad, enfermedad, accidente, etc)
                    UX.Cotorra.PrePayroll.UI.ReloadEmployeeOverdraft(row);
                }
            });
        },

        EditVacation: function (obj = null) {
            let row = UX.Cotorra.Common.GetRowData(obj);

            UX.Cotorra.Vacation.UI.Init({
                Employee: {
                    ID: row.dataItem.ID,
                    Name: row.dataItem.FullName,
                    Code: row.dataItem.Code
                },
                PeriodDetail: {
                    ID: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get().Data.PeriodDetailID,
                },
                OnSaveSucessCallback: function (savedData, responseData) {
                    //Vacaciones para vacacionar
                    UX.Cotorra.PrePayroll.UI.ReloadEmployeeOverdraft(row);
                }
            });
        },

        EditPermanentMovements: function (obj = null) {
            let row = UX.Cotorra.Common.GetRowData(obj);

            UX.Cotorra.PermanentMovements.UI.Init({
                Employee: {
                    ID: row.dataItem.EmployeeID,
                    Name: row.dataItem.FullName,
                    Code: row.dataItem.Code
                }
            });
        },

        EditAccumulated: function (obj = null) {
            let row = UX.Cotorra.Common.GetRowData(obj);
            UX.Cotorra.Accumulated.UI.Init({
                Employee: {
                    ID: row.dataItem.EmployeeID,
                    Name: row.dataItem.EmployeeName,
                    Code: row.dataItem.EmployeeCode
                }
            });
        },

        AuthorizePayroll: function () {
            UX.Modals.Confirm('AUTORIZAR NÓMINA', 'La siguiente acción autorizará la nómina de tus colaboradores cuyo estatus sea \'ABIERTO\' ¿Deseas continuar?', 'Sí, autorizar', 'No, espera',
                () => {

                    UX.Loaders.SetLoader('#np-prepayroll');
                    UX.Cotorra.Catalogs.Do('POST', 'PrePayroll', 'Authorize',
                        {
                            periodDetailID: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.PeriodDetailID')
                        },
                        (data) => {
                            UX.Cotorra.PrePayroll.UI.RefreshWorkPeriod({
                                PeriodStatus: 2
                            });
                        },
                        (error) => {
                            UX.Common.ErrorModal(error);
                            UX.Loaders.RemoveLoader('#np-prepayroll');
                        },
                        (complete) => {
                        }
                    );
                },
                () => { });
        },

        StampingPayroll: function () {
            UX.Modals.Confirm('TIMBRAR NÓMINA',
                'La siguiente acción realiza el timbrado de la nómina de todos tus colaboradores ¿Deseas continuar?',
                'Sí, timbrar', 'No, espera',
                () => {
                    UX.Loaders.SetLoader('#np-prepayroll');
                    UX.Cotorra.Catalogs.Do('POST', 'PrePayroll', 'Stamping',
                        {
                            overdrafstList: $('#np_pp_dpo').data('kendoGrid').dataSource.data().map(x => { return x.ID }),
                            periodDetailID: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.PeriodDetailID')
                        },
                        (data) => {
                            UX.Cotorra.PrePayroll.UI.RefreshWorkPeriod({
                                PeriodStatus: 3
                            });
                        },
                        (error) => {
                            UX.Common.ErrorModal(error);
                            UX.Loaders.RemoveLoader('#np-prepayroll');
                            UX.Cotorra.PrePayroll.UI.RefreshWorkPeriod({
                                PeriodStatus: 2
                            });
                        },
                        (complete) => {
                        }
                    );
                },
                () => { });
        },


        promises: [
            $.Deferred(),
            $.Deferred()
        ],

        OpenSummary: function () {
            //Open period type selection
            let modalID = UX.Modals.OpenModal(
                'Resumen de nómina', 'l', '<div id="np-prepayrollsummary"></div>',
                function () {
                    //Init template
                    var editModel = new Ractive({
                        el: '#np-prepayrollsummary',
                        template: '#np-prepayrollsummary-template',
                        data: {
                            FullDateInfo: 'Del ' +
                                moment(UX.Cotorra.PrePayroll.UI.Header.InitialDate).locale('es').format('dddd D [de] MMMM [de] YYYY') + ', al ' +
                                moment(UX.Cotorra.PrePayroll.UI.Header.FinalDate).locale('es').format('dddd D [de] MMMM [de] YYYY'),
                            NetToPay: '$0.00'
                        }
                    });

                    UX.Loaders.SetLoader('#' + modalID);
                    UX.Cotorra.Catalogs.Do('Get', 'PrePayroll', 'GetSummary',
                        {
                            periodDetailID: UX.Cotorra.PrePayroll.UI.Header.PeriodDetailID
                        },
                        (data) => {
                            let ConditionalSum = function (type) {
                                return kendo.toString('<div class="text-right ' + 'total-' + type + '">' + UX.Cotorra.PrePayroll.UI.GetSummaryTotal()[type.replace(/-/g, '')] + '</div>', "c");
                            }

                            //Set fields
                            let fields = {
                                ID: { type: 'string' },
                                ConceptCode: { type: 'number' },
                                ConceptName: { type: 'string' },
                                ConceptType: { type: 'string' },
                                TotalPerceptions: { type: 'number' },
                                TotalDeductions: { type: 'number' },
                                TotalLiabilities: { type: 'number' },
                                TotalAmount1: { type: 'number' },
                                TotalAmount2: { type: 'number' },
                                TotalAmount3: { type: 'number' },
                                TotalAmount4: { type: 'number' },
                            };

                            //Set columns
                            let columns = [
                                { field: 'ConceptCode', title: 'No.', width: 60 },
                                { field: 'ConceptName', title: 'Concepto', width: 200 }
                            ];

                            let columnsPD = columns.concat([
                                {
                                    field: 'TotalPerceptions', title: 'Percepciones', width: 150,
                                    template: UX.Cotorra.Common.GridFormatCurrency('TotalPerceptions'),
                                    footerTemplate: ConditionalSum.bind(null, "perceptions"),
                                },
                                {
                                    field: 'TotalDeductions', title: 'Deducciones', width: 150,
                                    template: UX.Cotorra.Common.GridFormatCurrency('TotalDeductions'),
                                    footerTemplate: ConditionalSum.bind(null, "deductions"),
                                },
                                {
                                    field: 'TotalAmount1', title: 'Gravado ISR',
                                    template: UX.Cotorra.Common.GridFormatCurrency('TotalAmount1'),
                                    footerTemplate: ConditionalSum.bind(null, "taxed-isr"),
                                },
                                {
                                    field: 'TotalAmount2', title: 'Exento ISR',
                                    template: UX.Cotorra.Common.GridFormatCurrency('TotalAmount2'),
                                    footerTemplate: ConditionalSum.bind(null, "exempt-isr"),
                                },
                                {
                                    field: 'TotalAmount3', title: 'Gravado IMSS',
                                    template: UX.Cotorra.Common.GridFormatCurrency('TotalAmount3'),
                                    footerTemplate: ConditionalSum.bind(null, "taxed-imss"),
                                },
                                {
                                    field: 'TotalAmount4', title: 'Exento IMSS',
                                    template: UX.Cotorra.Common.GridFormatCurrency('TotalAmount4'),
                                    footerTemplate: ConditionalSum.bind(null, "exempt-imss"),
                                },
                            ]);

                            //PyD grid
                            let $gridDyP = UX.Common.InitGrid(
                                {
                                    selector: '#np_ppsum_perdec', data: data.filter(x => { return x.ConceptType === 1 || x.ConceptType === 3 }),
                                    fields: fields, columns: columnsPD,
                                    dataBound: function () {

                                        let result = UX.Cotorra.PrePayroll.UI.GetSummaryTotal();
                                        //Set totals to footer
                                        $('#np_ppsum_perdec .total-perceptions').html(UX.Cotorra.Common.FormatCurrency(result.perceptions));
                                        $('#np_ppsum_perdec .total-deductions').html(UX.Cotorra.Common.FormatCurrency(result.deductions));
                                        $('#np_ppsum_perdec .total-taxed-isr').html(UX.Cotorra.Common.FormatCurrency(result.taxedisr));
                                        $('#np_ppsum_perdec .total-exempt-isr').html(UX.Cotorra.Common.FormatCurrency(result.exemptisr));
                                        $('#np_ppsum_perdec .total-taxed-imss').html(UX.Cotorra.Common.FormatCurrency(result.taxedimss));
                                        $('#np_ppsum_perdec .total-exempt-imss').html(UX.Cotorra.Common.FormatCurrency(result.exemptimss));

                                        //Set net to pay
                                        editModel.set('NetToPay', UX.Cotorra.Common.FormatCurrency(result.perceptions - result.deductions));
                                    },

                                    aggregate: [
                                        { field: "TotalPerceptions", aggregate: "sum", },
                                        { field: "TotalDeductions", aggregate: "sum" },
                                        { field: "TotalLiabilities", aggregate: "sum" },
                                        { field: "TotalAmount1", aggregate: "sum" },
                                        { field: "TotalAmount2", aggregate: "sum" },
                                        { field: "TotalAmount3", aggregate: "sum" },
                                        { field: "TotalAmount4", aggregate: "sum" },
                                    ],
                                    excelExport: function (e) {
                                        e.preventDefault();
                                        UX.Common.KendoGridRemoveHTMLExcelExport({ e: e });
                                        UX.Cotorra.PrePayroll.UI.PPSumPerdecWorkbook = e.workbook;
                                        UX.Cotorra.PrePayroll.UI.promises[0].resolve(e.workbook);
                                    },
                                }
                            );


                            let columnsOb = columns.concat([
                                {
                                    field: 'TotalLiabilities', title: 'Obligaciones', width: 150,
                                    template: UX.Cotorra.Common.GridFormatCurrency('TotalLiabilities'),
                                    footerTemplate: ConditionalSum.bind(null, "liabilities")
                                },
                                {
                                    title: ' '
                                }
                            ]);

                            //Ob grid
                            let $gridOb = UX.Common.InitGrid(
                                {
                                    selector: '#np_ppsum_liab', data: data.filter(x => { return x.ConceptType === 2 }),
                                    fields: fields, columns: columnsOb,
                                    dataBound: function () {
                                        let result = UX.Cotorra.PrePayroll.UI.GetSummaryTotal();

                                        //Set totals to footer
                                        $('#np_ppsum_liab .total-liabilities').html(UX.Cotorra.Common.FormatCurrency(result.liabilities));
                                    },
                                    aggregate: [
                                        { field: "TotalLiabilities", aggregate: "sum" },
                                    ],
                                    excelExport: function (e) {
                                        e.preventDefault();
                                        UX.Common.KendoGridRemoveHTMLExcelExport({ e: e });
                                        UX.Cotorra.PrePayroll.UI.PPSumLiabWorkbook = e.workbook;
                                        UX.Cotorra.PrePayroll.UI.promises[1].resolve(e.workbook);
                                    }
                                }

                            );


                        },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader('#' + modalID); }
                    );

                    //Fix resize
                    $('#np-prepayrollsummary .tab-nav a').on('click', function () {
                        $(window).resize();
                    });

                    $('#np_btnExportSummaryPayroll').off('click').on('click', function (e) {
                        $("#np_ppsum_perdec").data("kendoGrid").saveAsExcel();
                        $("#np_ppsum_liab").data("kendoGrid").saveAsExcel();
                        // wait for both exports to finish
                        $.when(UX.Cotorra.PrePayroll.UI.promises)
                            .then(function (pdWorkbook, liabilitiesWorkbook) {
                                var sheets = [
                                    UX.Cotorra.PrePayroll.UI.PPSumPerdecWorkbook.sheets[0],
                                    UX.Cotorra.PrePayroll.UI.PPSumLiabWorkbook.sheets[0]
                                ];
                                sheets[0].title = "Percepciones y Deducciones";
                                sheets[1].title = "Obligaciones";
                                var workbook = new kendo.ooxml.Workbook({
                                    sheets: sheets
                                });
                                kendo.saveAs({
                                    dataURI: workbook.toDataURL(),
                                    fileName: "Resumen de nómina.xlsx"
                                })
                            });
                    });

                });
        },

        Disperse: function () {

            let modalID = UX.Modals.OpenModal(
                'Dispersión de nómina', //Título del modal
                's', //Tamaño de la ventana (s, m, xm, l),
                '<div id="np-prepayrolldisperse"></div>',
                function () {
                    //Init template
                    var editModel = new Ractive({
                        el: '#np-prepayrolldisperse',
                        template: '#np-prepayrolldisperse-template',
                        data: {
                            BankCode: '',
                            BankName: '',
                            PeriodDetailID: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('Data.PeriodDetailID'),
                            PeriodDescription: UX.Cotorra.PrePayroll.UI.PrePayrollModel.get('WorkPeriod.Period'),
                            IncludeInactiveEmployees: false,

                            BankExtraParams: {
                                Banamex: {
                                    CustomerNumber: '',
                                    PaymentDate: moment(new Date()).format('DD/MM/YYYY'),
                                    FileNumberOfDay: '1',
                                    BranchOfficeNumber: '',
                                    ChargeAccount: '',
                                    StateID: '',
                                    CityID: '',
                                },
                                Scotiabank: {
                                    CustomerNumber: '',
                                    ChargeAccount: '',
                                    FileNumberOfDay: '1',
                                    CompanyReference: '',
                                    PaymentDate: moment(new Date()).format('DD/MM/YYYY'),
                                },
                                Santander: {
                                    GenerationDate: moment(new Date()).format('DD/MM/YYYY'),
                                    CompanyBankAccount: '',
                                    PaymentDate: moment(new Date()).format('DD/MM/YYYY'),
                                },
                                Banorte: {
                                    SystemID: '',
                                    ChargeAccount: '',
                                    PaymentDate: moment(new Date()).format('DD/MM/YYYY'),
                                },
                            },

                            //Options:
                            BanksOptions: UX.Cotorra.Catalogs.Banks.filter((x) => {
                                if (x.Code === 2 || //BANAMEX 2
                                    x.Code === 72 || //BANORTE 72
                                    x.Code === 12 || //BBVA BANCOMER
                                    x.Code === 21 || //HSBC 21
                                    x.Code === 44 || //SCOTIABANK 44
                                    x.Code === 14) { //SANTANDER 14
                                    return true;
                                }
                                return false;
                            })
                        }
                    });

                    editModel.observe('BankCode', function (newValue) {
                        editModel.set('BankName', $('#np_pd_drpBank option:selected').text().toUpperCase());
                    });

                    $('#np_btnCancelDisperse').off('click').on('click', function (ev) { ev.preventDefault(); UX.Modals.CloseModal(modalID); });


                    //Banamex
                    $('#np_pd_txtCustomerNumberBanamex').mask('000000000000');
                    $('#np_pd_txtChargeAccountBanamex').mask('00000000000000000000');
                    $('#np_pd_txtBranchOfficeNumberBanamex').mask('0000');
                    $('#np_pd_txtPaymentDateBanamex').mask('00/00/0000');
                    $('#np_pd_txtFileNumberOfDayBanamex').mask('0000');
                    $('#np_pd_txtStateIdBanamex').mask('00');
                    $('#np_pd_txtCityIdBanamex').mask('0000');

                    //Scotiabank
                    $('#np_pd_txtCustomerNumberScotiabank').mask('00000');
                    $('#np_pd_txtPaymentDateScotiabank').mask('00/00/0000');
                    $('#np_pd_txtCompanyReferenceScotiabank').mask('0000000000');
                    $('#np_pd_txtFileNumberOfDayScotiabank').mask('00');
                    $('#np_pd_txtChargeAccountScotiabank').mask('00000000000');

                    //Santander
                    $('#np_pd_txtChargeAccountSantander').mask('0000000000000000');
                    $('#np_pd_txtPaymentDateSantander').mask('00/00/0000');

                    //Banorte
                    $('#np_pd_txtChargeAccountBanorte').mask('000000000');
                    $('#np_pd_txtPaymentDateBanorte').mask('00/00/0000');
                    $('#np_pd_txtSystemIDBanorte').mask('000');

                    UX.Briones = editModel;

                    //Set validations
                    $('#np_prepayrolldisperse_form').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_pd_drpBank: {//id campo
                                validators: {
                                    notEmpty: { message: 'Debes seleccionar un banco' }
                                }
                            },
                            //BANMEX
                            np_pd_txtCustomerNumberBanamex: {
                                validators: {
                                    notEmpty: { message: 'Captura el número de cliente' }
                                }
                            },
                            np_pd_txtChargeAccountBanamex: {
                                validators: {
                                    notEmpty: { message: 'Captura la cuenta de cargo' }
                                }
                            },
                            np_pd_txtBranchOfficeNumberBanamex: {
                                validators: {
                                    notEmpty: { message: 'Captura el número de sucursal' }
                                }
                            },
                            np_pd_txtPaymentDateBanamex: {
                                validators: {
                                    message: 'La fecha proporcionada no es válida',
                                    notEmpty: { message: 'Captura la fecha de pago' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_pd_txtFileNumberOfDayBanamex: {
                                validators: {
                                    notEmpty: { message: 'Captura el secuencial del archivo' }
                                }
                            },
                            np_pd_txtStateIdBanamex: {
                                validators: {
                                    notEmpty: { message: 'Captura la clave del estado' }
                                }
                            },
                            np_pd_txtCityIdBanamex: {
                                validators: {
                                    notEmpty: { message: 'Captura la clave de ciudad' }
                                }
                            },
                            //SCOTIABANK
                            np_pd_txtPaymentDateScotiabank: {
                                validators: {
                                    message: 'La fecha proporcionada no es válida',
                                    notEmpty: { message: 'Captura la fecha de aplicación' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_pd_txtCustomerNumberScotiabank: {
                                validators: {
                                    notEmpty: { message: 'Captura el número de cliente' }
                                }
                            },
                            np_pd_txtChargeAccountScotiabank: {
                                validators: {
                                    notEmpty: { message: 'Captura la cuenta de cargo' },
                                    stringLength: {
                                        min: 11,
                                        message: 'La cuenta de cargo debe ser de 11 dígitos'
                                    }
                                }
                            },
                            np_pd_txtFileNumberOfDayScotiabank: {
                                validators: {
                                    notEmpty: { message: 'Captura no. archivo en el día' },
                                }
                            },
                            np_pd_txtCompanyReferenceScotiabank: {
                                validators: {
                                    notEmpty: { message: 'Captura la referencia empresa' },
                                }
                            },
                            //SANTANDER
                            np_pd_txtPaymentDateSantander: {
                                validators: {
                                    message: 'La fecha proporcionada no es válida',
                                    notEmpty: { message: 'Captura la fecha de pago' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_pd_txtChargeAccountSantander: {
                                validators: {
                                    notEmpty: { message: 'Captura la cuenta de cargo' }
                                }
                            },
                            //BANORTE
                            np_pd_txtChargeAccountBanorte: {
                                validators: {
                                    notEmpty: { message: 'Captura la cuenta de cargo' }
                                }
                            },
                            np_pd_txtSystemIDBanorte: {
                                validators: {
                                    notEmpty: { message: 'Captura la clave del sistema' }
                                }
                            },
                            np_pd_txtPaymentDateBanorte: {
                                validators: {
                                    message: 'La fecha proporcionada no es válida',
                                    notEmpty: { message: 'Captura la fecha de pago' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Save data
                            UX.Loaders.SetLoader('#' + modalID);
                            let data = UX.Briones.get();
                            UX.Cotorra.Catalogs.Do('POST', 'BankPaymentLayout', 'GenerateLayout',
                                data,
                                (response) => {
                                    window.open(response, 'Download');
                                    console.log(response);
                                },
                                (error) => { UX.Common.ErrorModal(error); },
                                (complete) => { UX.Loaders.RemoveLoader('#' + modalID); }
                            );
                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    $('#np-prepayrolldisperse').initUIElements();
                }
            );

            /*************************/
            ////Open period type selection
            //let modalID = UX.Modals.OpenModal(
            //    'Dispersión de nómina', 's', '<div id="np-prepayrolldisperse"></div>',
            //    function () {
            //        //Init template
            //        var editModel = new Ractive({
            //            el: '#np-prepayrolldisperse',
            //            template: '#np-prepayrolldisperse-template',
            //            data: {
            //                BankID: '',
            //                BanksOptions: UX.Cotorra.Catalogs.Banks
            //                    .sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0))
            //            }
            //        });

            //        //Set buttons
            //        $('#np_btnCancelDisperse').on('click', function (ev) {
            //            UX.Modals.CloseModal(modalID);
            //        });

            //        $('#np_btnDisperse').on('click', function (ev) {
            //            alert('DOWNLOAD FILE');
            //            UX.Modals.CloseModal(modalID);
            //        });

            //        //Init UI elements
            //        $('#np-prepayrolldisperse').initUIElements();

            //        //Initial focus
            //        $('#np_pd_drpBank').focus();
            //    });
        },

        RefreshOverdraft: function (uid) {
            let tr = $('tr[data-uid="' + uid + '"]')[1];
            let row = UX.Cotorra.Common.GetRowData(tr);
            if (row) {
                UX.Cotorra.Catalogs.Get(
                    'Overdraft',
                    { overdraftID: row.dataItem.OverdraftID },
                    (data) => {
                        for (var i = 0; i < data.length; i++) {
                            row.dataItem['F_' + data[i].ConceptPaymentID.replace(/-/g, '')] = data[i].Amount;
                        }

                        UX.Common.KendoFastRedrawRow(row.$kendoGrid, row.dataItem);
                        $('tr[data-uid="' + row.dataItem.uid + '"]').animateCSS('flash');
                    },
                    (error) => {
                        UX.Common.ErrorModal(error);
                    });
            }
        },

        GetSummaryTotal: function (type) {
            let $perdecGrid = $('#np_ppsum_perdec').data('kendoGrid');
            let $liabGrid = $('#np_ppsum_liab').data('kendoGrid');

            let result = {
                deductions: 0.00,
                perceptions: 0.00,
                taxedisr: 0.00,
                exemptisr: 0.00,
                taxedimss: 0.00,
                exemptimss: 0.00,
                liabilities: 0.00,
            };

            if ($perdecGrid) {
                //set kind css class
                let data = $perdecGrid.dataSource.data();
                for (var i = 0; i < data.length; i++) {
                    let row = data[i];
                    let kind = row.ConceptKind;
                    let $tr = $('tr[data-uid="' + row.uid + '"]');
                    if (kind) { $tr.addClass('is-kind'); }
                    else {
                        if (parseInt(row.ConceptType) === 1) {
                            result.perceptions = result.perceptions + row.TotalPerceptions;
                            result.taxedisr = result.taxedisr + row.TotalAmount1;
                            result.exemptisr = result.exemptisr + row.TotalAmount2;
                            result.taxedimss = result.taxedimss + row.TotalAmount3;
                            result.exemptimss = result.exemptimss + row.TotalAmount4;
                        } else if (parseInt(row.ConceptType) === 3) {
                            result.deductions = result.deductions + row.TotalDeductions;
                        }
                    }
                }
            }

            if ($liabGrid) {
                let data = $liabGrid.dataSource.data();
                for (var i = 0; i < data.length; i++) {
                    let row = data[i];
                    let kind = row.ConceptKind;
                    let $tr = $('tr[data-uid="' + row.uid + '"]');
                    if (kind) { $tr.addClass('is-kind'); }
                    else {
                        if (parseInt(row.ConceptType) === 2) {
                            result.liabilities = result.liabilities + row.TotalLiabilities;
                        }
                    }
                }
            }
            return result;
        },
    }
};