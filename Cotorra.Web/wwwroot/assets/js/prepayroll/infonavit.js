'use strict';

UX.Cotorra.Infonavit = {
    UI: {
        CatalogName: 'Infonavit',
        ContainerSelector: '#np-infonavit',
        TitleModalsString: 'INFONAVIT',
        ActualRow: null,

        Init: function (params = {}) {
            
            let modalID = UX.Modals.OpenModal(
                'Crédito INFONAVIT',
                'xm', '<div id="np-editinfonavit"></div>',
                function () {
                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');

                    let editModel = new Ractive({
                        el: '#np-editinfonavit',
                        template: '#np-editinfonavit-template',
                        data: {
                            Employee: params.Employee,

                            ID: '',
                            CreditNumber: '',
                            Description: '',
                            InfonavitCreditType: 2,
                            MonthlyFactor: '0',
                            IncludeInsurancePayment_D14: false,
                            InitialApplicationDate: moment(new Date()).format("DD/MM/YYYY"),
                            AccumulatedAmount: '0.00',
                            AppliedTimes: 1,
                            RegisterDate: moment(new Date()).format("DD/MM/YYYY"),
                            InfonavitStatus: 1,
                            EmployeeID: params.Employee.ID,
                            HasOneActiveCredit: false,
                            InitialPeriodDate: '',
                            EmployeeConceptsRelationID: '',
                            EmployeeConceptsRelationInsuranceID: '',

                            //Options
                            InfonavitCreditTypesOptions: [ 
                                { ID: 2, Name: 'Porcentaje (Concepto D59)' },
                                { ID: 3, Name: 'Factor descuento (Concepto D15)' },
                                { ID: 4, Name: 'Cuota fija (Concepto D16)' },
                            ]
                        },
                        modifyArrays: true
                    });

                    //Buttons
                    $('#np_btnCancelSaveInfonavit').on('click', function (ev) {
                        ev.preventDefault();
                        $('#np_cat_saveinfonavit').data("formValidation").destroy();
                        UX.Modals.CloseModal(modalID);
                    });
                    $('#np_btnSaveInfonavit').on('click', function () {
                        $('#np_cat_saveinfonavit').data('formValidation').validate();
                    });
                    $('#np_btnNewInfonavit').on('click', function (ev) {
                        ev.preventDefault();
                        UX.Cotorra.Infonavit.UI.Set(null);
                    });

                    //Masks
                    $('#np_info_txtNumber').mask('00000000000000000000');
                    $('#np_info_txtApplicationInitDate').mask('00/00/0000');
                    $('#np_info_txtRegistryDate').mask('00/00/0000');
                    $('#np_info_txtTotalTimesApplied').mask('00000');
                    $("#np_info_txtMonthlyFactor").decimalMask(2);

                    //Form validation
                    //Set validations
                    $('#np_cat_saveinfonavit').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_info_txtNumber: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el no. de crédito' }
                                }
                            },
                            np_info_txtDescription: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la descripción' }
                                }
                            },
                            np_info_txtMonthlyFactor: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el monto a retener' },
                                    callback: {
                                        callback: function (value, validator) {
                                            let newValue = value ? value.replace(/\$/g, '').replace(/,/g, '') : 0;
                                            value = !isNaN(newValue) ? parseFloat(newValue) : 0;
                                            if (value <= 0) {
                                                return {
                                                    valid: false, message: 'El monto a retener debe ser mayor a 0'
                                                };
                                            }
                                            return true;
                                        }
                                    }
                                }
                            }, 
                            np_info_txtApplicationInitDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de inicio de aplicación' },
                                    callback: {
                                        callback: function (value, validator) {
                                            let validation = UX.Common.ValidDateValidator(value, validator);
                                            if (validation.valid !== undefined) {
                                                return validation;
                                            } else {

                                                let initialApplicationDate = editModel.get('InitialApplicationDate');
                                                let initial = editModel.get('InitialPeriodDate');
                                                if (moment(initialApplicationDate, "DD/MM/YYYY").isBefore(moment(initial, "DD/MM/YYYY"))) {
                                                    return {
                                                        valid: false, message: 'La fecha de inicio de aplicación debe ser posterior al inicio del periodo actual'
                                                    };
                                                }
                                                return true;
                                            }
                                        }
                                    }
                                }
                            },
                            np_info_txtAccumulatedAmount: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el monto acumulado' }
                                }
                            },
                            np_info_txtTotalTimesApplied: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar las veces aplicado' }
                                }
                            },
                            np_info_txtRegistryDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de registro' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            }
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = editModel.get();
                            data.InfonavitStatus = data.InfonavitStatus == 1 ? true : false;

                            UX.Cotorra.Catalogs.Save(
                                'Infonavit',
                                data,
                                (obj) => {
                                    let row = UX.Cotorra.Infonavit.UI.ActualRow;
                                    let grid = row ? row.$kendoGrid : $('.np-' + 'infonavit' + '-grid').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = obj.ID;
                                        data.InfonavitStatus = data.InfonavitStatus ? 1 : 0;
                                        data.EmployeeConceptsRelationID = obj.EmployeeConceptsRelationID;
                                        data.EmployeeConceptsRelationInsuranceID = obj.EmployeeConceptsRelationInsuranceID;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;

                                        dataItem.ID = data.ID;
                                        dataItem.CreditNumber = data.CreditNumber;
                                        dataItem.Description = data.Description;
                                        dataItem.InfonavitCreditType = data.InfonavitCreditType;
                                        dataItem.MonthlyFactor = data.MonthlyFactor;
                                        dataItem.IncludeInsurancePayment_D14 = data.IncludeInsurancePayment_D14;
                                        dataItem.InitialApplicationDate = data.InitialApplicationDate;
                                        dataItem.AccumulatedAmount = data.AccumulatedAmount;
                                        dataItem.AppliedTimes = data.AppliedTimes;
                                        dataItem.RegisterDate = data.RegisterDate;
                                        dataItem.InfonavitStatus = data.InfonavitStatus ? 1 : 0;
                                        dataItem.EmployeeConceptsRelationID = obj.EmployeeConceptsRelationID;
                                        dataItem.EmployeeConceptsRelationInsuranceID = obj.EmployeeConceptsRelationInsuranceID;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

                                    // Clean form (new)
                                    editModel.set('HasOneActiveCredit', data.InfonavitStatus);
                                    UX.Cotorra.Infonavit.UI.Set(null);
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

                    //template methods

                    //observers
                    UX.Cotorra.Infonavit.EditModel = editModel;

                    //Load empty grid
                    UX.Cotorra.Infonavit.UI.LoadGrid([]);

                    $('#np-editinfonavit').initUIElements();
                    UX.Cotorra.Infonavit.UI.GetActualPeriodInitialDate(params.PeriodTypeID, modalID);

                    (async function () {
                        let getInfonavitsRequest = UX.Cotorra.Catalogs.Get('Infonavit',
                            { employeeID: params.Employee.ID },
                            (data) => {
                                editModel.set('HasOneActiveCredit', data.hasOneAcviteCredit);
                                UX.Cotorra.Infonavit.UI.LoadGrid(data.infonavits);
                            },
                            () => { },
                            () => { },
                        );

                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#np-infonavit',
                                removeWhenFinish: true
                            },
                            catalogs: [],
                            requests: [getInfonavitsRequest],
                            forceLoad: false
                        });

                        $('#np_info_txtNumber').focus();
                    })();
                });
        },

        LoadGrid: function (data) {
            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                CreditNumber: { type: 'string' },
                InfonavitStatus: { type: 'number' }
            };

            //Set columns
            let columns = [
                { field: 'CreditNumber', title: 'Crédito INFONAVIT', width: 120, },
                {
                    field: 'InfonavitStatus', title: 'Estado', width: 100,
                    template: kendo.template($('#infonavitStatusTemplate').html())
                },
                {
                    title: ' ', width: 80,
                    template: kendo.template($('#infonavitActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
            });
        },

        Set: function (obj) {
            let row = UX.Cotorra.Infonavit.UI.ActualRow = UX.Cotorra.Common.GetRowData(obj);
            let dataItem = row ? row.dataItem : null;
            let model = UX.Cotorra.Infonavit.EditModel;

            $('.modal-title-editing').html(row ? '- EDITANDO CRÉDITO ' + row.dataItem.CreditNumber : '');
            $('.modal-title-editing').animateCSS('flash');

            //Fill form with data 
            model.set('ID', dataItem ? dataItem.ID : '00000000-0000-0000-0000-000000000000');
            model.set('CreditNumber', dataItem ? dataItem.CreditNumber : '');
            model.set('Description', dataItem ? dataItem.Description : '');
            model.set('InfonavitCreditType', dataItem ? dataItem.InfonavitCreditType : 2);
            model.set('MonthlyFactor', dataItem ? dataItem.MonthlyFactor : '0');
            model.set('IncludeInsurancePayment_D14', dataItem ? dataItem.IncludeInsurancePayment_D14 : false);
            model.set('InitialApplicationDate', dataItem ? dataItem.InitialApplicationDate : moment(new Date()).format("DD/MM/YYYY"));
            model.set('AccumulatedAmount', dataItem ? dataItem.AccumulatedAmount : '0.00');
            model.set('AppliedTimes', dataItem ? dataItem.AppliedTimes : 1);
            model.set('RegisterDate', dataItem ? dataItem.RegisterDate : moment(new Date()).format("DD/MM/YYYY"));
            model.set('InfonavitStatus', dataItem ? dataItem.InfonavitStatus : 1);
            model.set('EmployeeConceptsRelationID', dataItem ? dataItem.EmployeeConceptsRelationID : '00000000-0000-0000-0000-000000000000');
            model.set('EmployeeConceptsRelationInsuranceID', dataItem ? dataItem.EmployeeConceptsRelationInsuranceID : '00000000-0000-0000-0000-000000000000');

            $('#np_info_txtNumber').focus();
        },

        Delete: function (obj) {
            //Show modal
            UX.Modals.Confirm('Eliminar crédito INFONAVIT', '¿Deseas eliminar el crédito INFONAVIT?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);

                    UX.Loaders.SetLoader(this.ContainerSelector);
                    UX.Cotorra.Catalogs.Delete(
                        this.CatalogName,
                        {
                            id: row.dataItem.ID,
                            employeeID: row.dataItem.EmployeeID
                        },
                        (data) => {
                            $(row.el).fadeOut(function () {
                                row.$kendoGrid.dataSource.remove(row.dataItem);

                                if (row.dataItem.InfonavitStatus == 1) {
                                    UX.Cotorra.Infonavit.EditModel.set('HasOneActiveCredit', false);
                                }

                                if (UX.Cotorra.Infonavit.EditModel.get('ID') === row.dataItem.ID) {
                                    UX.Cotorra.Infonavit.UI.Set(null);
                                }
                            });
                        },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader(this.ContainerSelector); }
                    );
                },
                () => {
                });
        },

        GetActualPeriodInitialDate: (data, modalID) => {

            //Get actual period initial date
            UX.Cotorra.Catalogs.Do('GET', 'Periods', 'GetActualDetail', { periodTypeID: data },
                (data) => {
                    let date = data ? moment(data.InitialDate, 'YYYY/MM/DD') : '';
                    UX.Cotorra.Infonavit.EditModel.set('InitialApplicationDate', date.format('DD/MM/YYYY'));
                    UX.Cotorra.Infonavit.EditModel.set('InitialPeriodDate', date.format('DD/MM/YYYY'));
                },
                (error) => {
                    UX.Common.ErrorModal(error);
                },
                (complete) => {
                   
                }
            );

        },
    }
};