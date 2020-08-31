'use strict';

UX.Cotorra.OrgOKRs = {
    Get: (data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'GET',
            url: CotorraAppURL + '/' + 'Orgokr' + '/' + get + '/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    Save: (data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'POST',
            url: CotorraAppURL + '/' + 'Orgokr' + '/' + action + '/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    UI: {
        CatalogName: 'OrgOKRS',
        ContainerSelector: '#np-orgokrs',
        TitleModalsString: 'Objetivos organizacionales',
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
                el: '#np-orgokrs',
                template: '#np-orgokrs-template',
                data: {
                    WorkPeriod: {
                        Period: '- - - - ',
                        Dates: 'Selecciona tu periodo'
                    },
                    Data: null
                }
            });

            //Set top action behaviors

            $('#np_btnAddOrgOKR').on('click', function () {
                UX.Cotorra.OrgOKRs.UI.OpenSave();
            });

            //Quick search
            UX.Cotorra.Common.SetQuickSearch('#np-orgokrs .quicksearch', '#np-orgokrs .np-dpo-grid');

            $('#np-orgokrs .tab-nav a').on('click', function () { $(window).resize(); });

            (async function () {
               
                UX.Cotorra.OrgOKRs.UI.LoadActiveGrid([]);
                UX.Cotorra.OrgOKRs.UI.LoadEnded([]);
                $(window).resize();
               
                let periodTypesRequest =
                    UX.Cotorra.PrePayroll.Get({}, 'GetActivePeriodTypes',
                        (data) => { UX.Cotorra.PrePayroll.UI.PeriodTypesOptions = data }, (error) => { }, (complete) => { });

                await UX.Cotorra.Catalogs.Require({
                    loader: {
                        container: '#np-orgokrs',
                        removeWhenFinish: true
                    },
                    catalogs: ['Banks'],
                    requests: [periodTypesRequest],
                    forceLoad: false
                });

                UX.Cotorra.OrgOKRs.UI.OpenPeriodSelection();

                //Auto select first period type
                setTimeout(function () {
                    let tr = $('.np-pp-selectperiod-grid tbody tr');
                    if (tr.length === 1) {
                        tr.dblclick();
                    }
                }, 50);
            })();
        },

        LoadActiveGrid: function (data = [] ) {
            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
                Description: { type: 'string' },
                DueDate: { type: 'string' }
            };
             
            //Set columns
            let columns = [
                {
                    field: 'Name', title: 'Nombre del objetivo', width: 100,
                    
                },
                {
                    field: 'Description', title: 'Descripción del objetivo', width: 200,
                    
                },
                {
                    field: "DueDate", title: "Fecha compromiso", 
                    template: UX.Cotorra.Common.GridFormatDate('DueDate'),
                    width: 50
                },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#orgokrActionTemplate').html())
                }
            ]; 

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np_obj_active', data: data, fields: fields, columns: columns
            });
        },

        LoadEnded: function (data = []) {
            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
                Description: { type: 'string' }
            };
             

            //Set columns
            let columns = [
                {
                    field: 'Name', title: 'Nombre del objetivo', width: 100,

                },
                {
                    field: 'Description', title: 'Descripción del objetivo', width: 230,
                },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#orgokrActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-dh-grid', data: data, fields: fields, columns: columns
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
                            PeriodTypesOptions: UX.Cotorra.PrePayroll.UI.PeriodTypesOptions,
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

                                    UX.Cotorra.OrgOKRs.UI.SelectPeriodType({
                                        PeriodTypeID: ptSelection.get('PeriodTypeID'),
                                        PeriodTypeName: ptSelection.get('PeriodTypesOptions').find(x => { return x.ID === ptSelection.get('PeriodTypeID'); }).Name,
                                        PeriodID: ptSelection.get('PeriodID'),
                                        CurrentYear: ptSelection.get('FiscalYearsOptions').find(x => { return x.ID === ptSelection.get('PeriodID'); }).Name,
                                        PeriodDetailID: row.dataItem.ID,
                                        PeriodDetailNumber: row.dataItem.Number,
                                        InitialDate: row.dataItem.InitialDate,
                                        FinalDate: row.dataItem.FinalDate
                                    });
                                });
                            },
                            (error) => { UX.Common.ErrorModal(error); },
                            (complete) => { UX.Loaders.RemoveLoader('#' + modalID); });
                    });

                    if (type === 'periodType') {
                        //Set active period types grid
                        UX.Cotorra.PrePayroll.UI.LoadPTGrid(UX.Cotorra.PrePayroll.UI.PeriodTypesOptions);

                        $('.np-pp-selectperiod-grid tbody tr').on('dblclick', function (ev) {
                            //Get row data
                            let row = UX.Cotorra.Common.GetRowData(this);
                            UX.Cotorra.OrgOKRs.UI.SelectPeriodType({
                                PeriodTypeID: row.dataItem.ID,
                                PeriodID: row.dataItem.PeriodID,
                                PeriodDetailID: row.dataItem.PeriodDetailID,
                                PeriodTypeName: row.dataItem.Name,
                                CurrentYear: row.dataItem.CurrentYear,
                                PeriodDetailNumber: row.dataItem.Number,
                                InitialDate: row.dataItem.InitialDate,
                                FinalDate: row.dataItem.FinalDate
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
                        UX.Cotorra.OrgOKRs.UI.LoadActiveGrid([]);
                    }
                });

            if (type == 'periodType') {
                //$('#' + modalID + ' .title a').remove();
            }
        },

        SelectPeriodType: function (data = null) {
            //clear grids
            UX.Cotorra.OrgOKRs.UI.LoadActiveGrid([]);
            UX.Cotorra.OrgOKRs.UI.LoadEnded([]);

            //Set data on header
            let model = UX.Cotorra.PrePayroll.UI.PrePayrollModel;
            model.set('Data', data);
            model.set('WorkPeriod.Period', data.PeriodTypeName + ' ' + data.CurrentYear + ' - ' + 'Periodo ' + data.PeriodDetailNumber);
            model.set('WorkPeriod.Dates', 'Del ' +
                moment(data.InitialDate).locale('es').format('dddd D [de] MMMM [de] YYYY') + ', al ' +
                moment(data.FinalDate).locale('es').format('dddd D [de] MMMM [de] YYYY'));

            //Set header values
            UX.Cotorra.PrePayroll.UI.Header.PeriodTypeID = data.PeriodTypeID;
            UX.Cotorra.PrePayroll.UI.Header.PeriodID = data.PeriodID;
            UX.Cotorra.PrePayroll.UI.Header.PeriodDetailID = data.PeriodDetailID;
            UX.Cotorra.PrePayroll.UI.Header.InitialDate = data.InitialDate;
            UX.Cotorra.PrePayroll.UI.Header.FinalDate = data.FinalDate;

            //Get the work period details
            let loaderID = '#' + $('#np-selectperiodtype').closest('.side-modal-container').attr('id');
            UX.Loaders.SetLoader(loaderID);
            UX.Cotorra.catalogs

            UX.Cotorra.Catalogs.Get('Orgokr',
                {
                    periodTypeID: data.PeriodTypeID,
                    periodDetailID: data.PeriodDetailID
                },
                (data) => {
                    var lists = {
                        DPO: data.Active,
                        DH: data.Inactive
                    };
                    //Load grids
                    UX.Cotorra.OrgOKRs.UI.LoadActiveGrid(lists.DPO);
                    UX.Cotorra.OrgOKRs.UI.LoadEnded(lists.DH);

                    //Close window
                    UX.Modals.CloseModal($('.side-modal-container').attr('id'));
                },
                (error) => { UX.Common.ErrorModal(error); },
                (complete) => { UX.Loaders.RemoveLoader(loaderID); });
         
        },

        OpenSave: (obj = null) => {

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'record_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo objetivo' : 'Editar objetivo organizacional', 's',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var orgOKRModel = new Ractive({
                        el: '#' + containerID,
                        template: '#objetive_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Name: row ? row.dataItem.Name : '',
                            DueDate: row ? row.dataItem.DueDate : '',
                            Description: row ? row.dataItem.Description : '',
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Init elements
                    $container.initUIElements();
                    $container.find('#np_dep_txtName').focus();
                    $('#np_obj_org_txtDueDate').mask('00/00/0000');

                    //Set validations
                    $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_obj_org_txtName: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el nombre del objetivo' }
                                }
                            },
                            np_obj_org_txtDesc: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la descripcion del objetivo' }
                                }
                            },
                            np_obj_org_txtDueDate: {
                                validators: {
                                    callback: {
                                        message: "",
                                        callback: function (value, validator) {
                                            return UX.Cotorra.OrgOKRs.UI.DateValidator(value, validator);
                                        }
                                    }
                                }
                            }
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);
                             
                            //Save data
                            let data = orgOKRModel.get();
                            UX.Cotorra.Catalogs.Save('Orgokr',
                                data,
                                (id) => { 
                                    let grid = row ? row.$kendoGrid : $('.np_obj_active').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = id;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.Name = data.Name;
                                        dataItem.DueDate = data.DueDate;
                                        dataItem.Description = data.Description;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

                                    UX.Modals.CloseModal(modalID);
                                },
                                (error) => {
                                    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
                                },
                                (complete) => {
                                    UX.Loaders.RemoveLoader('#' + modalID);
                                }
                            );
                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    //Get data (if applicable)
                });
        },

        DateValidator: function (value, validator) {
            if (value === "") { return true; }

            var m2 = new moment(value, 'DD/MM/YYYY', true);
            if (m2.isValid()) {
                return true;
            }

            return {
                valid: false,
                message: 'Formato de fecha inválido'
            };
        },
    }
};