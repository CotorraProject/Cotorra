'use strict';

UX.Cotorra.Fonacot = {
    UI: {
        CatalogName: 'Fonacot',
        ContainerSelector: '#np-fonacot',
        TitleModalsString: 'FONACOT',
        ActualRow: null,

        Init: (params = {}) => {
            let modalID = UX.Modals.OpenModal(
                'Créditos FONACOT',
                'xm', '<div id="np-editfonacot"></div>',
                function () {
                    let actualYear = new Date().getFullYear();
                    let yearsOptions = [];
                    for (let i = actualYear - 5; i <= actualYear + 5; i++) {
                        yearsOptions.push({
                            Value: i,
                            Name: i
                        });
                    }

                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');

                    let editModel = new Ractive({
                        el: '#np-editfonacot',
                        template: '#np-editfonacot-template',
                        data: {
                            Employee: params.Employee,

                            //Fields
                            ID: '',
                            Name: '',
                            Number: '',
                            CreditNumber: '',
                            Description: '',
                            Month: new Date().getMonth() + 1,
                            Year: new Date().getFullYear(),
                            RetentionType: 1,
                            CreditAmount: '0.00',
                            OverdraftDetailAmount: '0.00',
                            PaymentsMadeByOtherMethod: '0.00',
                            AccumulatedAmountWithHeldCalculated: '0.00',
                            BalanceCalculated: '0.00',
                            FonacotMovementStatus: 1,
                            Observations: '',
                            ConceptPaymentID: '',
                            WorkCenter: '',
                            EmployeeID: params.Employee.ID,
                            EmployeeConceptsRelationID: '',
                            EditingCredit: '',

                            //Options
                            MonthsOptions: [
                                { Value: 1, Name: 'Enero' },
                                { Value: 2, Name: 'Febrero' },
                                { Value: 3, Name: 'Marzo' },
                                { Value: 4, Name: 'Abril' },
                                { Value: 5, Name: 'Mayo' },
                                { Value: 6, Name: 'Junio' },
                                { Value: 7, Name: 'Julio' },
                                { Value: 8, Name: 'Agosto' },
                                { Value: 9, Name: 'Septiembre' },
                                { Value: 10, Name: 'Octubre' },
                                { Value: 11, Name: 'Noviembre' },
                                { Value: 12, Name: 'Diciembre' }
                            ],
                            YearsOptions: yearsOptions,
                            RetentionTypeOptions: [
                                { Value: 1, Name: 'Importe fijo' },
                                { Value: 2, Name: 'Proporción a días pagados' }
                            ],
                            FonacotMovementStatusOptions: [
                                { Value: 1, Name: 'Activo' },
                                { Value: 0, Name: 'Inactivo' }
                            ]
                        },
                        modifyArrays: true
                    });

                    //template methods

                    //observers
                    editModel.observe('CreditAmount PaymentsMadeByOtherMethod AccumulatedAmountWithHeldCalculated', function (newValue) {
                        if (newValue === '') { newValue = 0 }

                        let creditAmount = parseFloat(editModel.get('CreditAmount').toString().replace(/,/g, ''));
                        let paymentsMade = parseFloat(editModel.get('PaymentsMadeByOtherMethod').toString().replace(/,/g, ''));
                        let accAmount = parseFloat(editModel.get('AccumulatedAmountWithHeldCalculated'));
                        
                        let balance = creditAmount - paymentsMade - accAmount;
                        editModel.set('BalanceCalculated', balance);
                    }, { defer: true, init: true });

                    //Masks
                    $('#np_fona_txtCreditNumber').mask('00000000000000000000');

                    //Buttons
                    $('#np_btnCancelFonacot').on('click', function (ev) {
                        ev.preventDefault();
                        $('#np_fonacot_form').data("formValidation").destroy();
                        UX.Modals.CloseModal(modalID);
                    });
                    $('#np_btnNewFonacot').on('click', function (ev) {
                        ev.preventDefault();
                        UX.Cotorra.Fonacot.UI.Set(null);
                    });

                    //Set validations
                    let $fv = $('#np_fonacot_form').formValidation({
                        framework: 'bootstrap',
                        button: {
                            selector: 'np_btnSaveFonacot'
                        },
                        live: 'disabled',
                        fields: {
                            np_fona_txtCreditNumber: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el número' }
                                }
                            },                           
                            np_fona_txtCreditAmount: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el importe del crédito' },
                                    greaterThan: {
                                        value: 0.00001,
                                        message: 'El importe de crédito debe ser mayor a 0'
                                    }
                                }
                            },
                            np_fona_txtDiscountAmount: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el importe de descuento' },
                                    greaterThan: {
                                        value: 0.00001,
                                        message: 'El descuento debe ser mayor a 0'
                                    }
                                }
                            },
                            np_fona_txtPeriodRetentionAmount: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la retención mensual' },
                                    greaterThan: {
                                        value: 0,
                                        message: 'La retención mensual debe ser mayor a 0'
                                    }
                                }
                            },
                            np_fona_txtPaymentsMadeByOtherEmployers: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar los pagos hechos por otros patrones' }
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
                            data.EmployeeID = data.Employee.ID;

                            UX.Cotorra.Catalogs.Save(
                                'Fonacot',
                                data,
                                (obj) => {
                                    let row = UX.Cotorra.Fonacot.UI.ActualRow;
                                    let grid = row ? row.$kendoGrid : $('.np-' + 'fonacot' + '-grid').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = obj.ID;
                                        data.EmployeeConceptsRelationID = obj.EmployeeConceptsRelationID;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.ID = data.ID;
                                        dataItem.Name = data.Name;
                                        dataItem.Number = data.Number;
                                        dataItem.CreditNumber = data.CreditNumber;
                                        dataItem.Description = data.Description;
                                        dataItem.Month = data.Month;
                                        dataItem.Year = data.Year;
                                        dataItem.RetentionType = data.RetentionType;
                                        dataItem.EmployeeID = data.EmployeeID;
                                        dataItem.EmployeeConceptsRelationID = obj.EmployeeConceptsRelationID;
                                        dataItem.CreditAmount = data.CreditAmount;
                                        dataItem.OverdraftDetailAmount = data.OverdraftDetailAmount;
                                        dataItem.PaymentsMadeByOtherMethod = data.PaymentsMadeByOtherMethod;
                                        dataItem.AccumulatedAmountWithHeldCalculated = data.AccumulatedAmountWithHeldCalculated;
                                        dataItem.BalanceCalculated = data.BalanceCalculated;
                                        dataItem.FonacotMovementStatus = data.FonacotMovementStatus;
                                        dataItem.Observations = data.Observations;
                                        dataItem.WorkCenter = data.WorkCenter;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

                                    // Clean form (new)
                                    UX.Cotorra.Fonacot.UI.Set(null);
                                },
                                (error) => {
                                    UX.Common.ErrorModal(error);
                                },
                                (complete) => {
                                    UX.Loaders.RemoveLoader('#' + modalID);
                                }
                            );
                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    //Init elements
                    $('#np-editfonacot').initUIElements();

                    // GetData

                    UX.Cotorra.Fonacot.EditModel = editModel;

                    //Load empty grid
                    UX.Cotorra.Fonacot.UI.LoadGrid([]);

                    (async function () {
                        //Load data
                        let getFonacotCredits = UX.Cotorra.Catalogs.Get('Fonacot',
                            { employeeID: params.Employee.ID },
                            (data) => {
                                UX.Cotorra.Fonacot.UI.LoadGrid(data);
                            },
                            () => { },
                            () => { },
                        );

                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#np-fonacot',
                                removeWhenFinish: true
                            },
                            catalogs: [],
                            requests: [getFonacotCredits],
                            forceLoad: false
                        });

                        $('#np_fona_txtCreditNumber').focus();
                    })();
                });
        },

        LoadGrid: function (data) {
            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                CreditNumber: { type: 'string' }
            };

            //Set columns
            let columns = [
                { field: 'CreditNumber', title: 'Crédito FONACOT' },
                {
                    title: ' ', width: 90,
                    template: kendo.template($('#fonacotActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
            });
        },

        Set: function (obj) {
            let row = UX.Cotorra.Fonacot.UI.ActualRow = UX.Cotorra.Common.GetRowData(obj);
            let dataItem = row ? row.dataItem : null;
            let model = UX.Cotorra.Fonacot.EditModel;

            $('.modal-title-editing').html(row ? '- EDITANDO CRÉDITO ' + row.dataItem.CreditNumber : '');
            $('.modal-title-editing').animateCSS('flash');
           
            //Fill form with data
            model.set('ID', dataItem ? dataItem.ID : '00000000-0000-0000-0000-000000000000');
            model.set('Name', dataItem ? dataItem.Name : '');
            model.set('Number', dataItem ? dataItem.Number : '');
            model.set('CreditNumber', dataItem ? dataItem.CreditNumber : '');
            model.set('Description', dataItem ? dataItem.Description : '');
            model.set('Month', dataItem ? dataItem.Month : new Date().getMonth() + 1);
            model.set('Year', dataItem ? dataItem.Year : new Date().getFullYear());
            model.set('RetentionType', dataItem ? dataItem.RetentionType : 1);
            model.set('CreditAmount', dataItem ? dataItem.CreditAmount : '0.00');
            model.set('OverdraftDetailAmount', dataItem ? dataItem.OverdraftDetailAmount : '0.00');
            model.set('PaymentsMadeByOtherMethod', dataItem ? dataItem.PaymentsMadeByOtherMethod : '0.00');
            model.set('AccumulatedAmountWithHeldCalculated', dataItem ? dataItem.AccumulatedAmountWithHeldCalculated : '0.00');
            model.set('BalanceCalculated', dataItem ? dataItem.BalanceCalculated : '0.00');
            model.set('FonacotMovementStatus', dataItem ? dataItem.FonacotMovementStatus : 1);
            model.set('Observations', dataItem ? dataItem.Observations : '');
            model.set('ConceptPaymentID', dataItem ? dataItem.ConceptPaymentID : '');
            model.set('EmployeeID', dataItem ? dataItem.EmployeeID : '');
            model.set('EmployeeConceptsRelationID', dataItem ? dataItem.EmployeeConceptsRelationID : '00000000-0000-0000-0000-000000000000');
            model.set('WorkCenter', dataItem ? dataItem.WorkCenter : '');

            $('#np_fona_txtCreditNumber').focus();
        },

        Delete: function (obj) {
            //Show modal
            UX.Modals.Confirm('Eliminar crédito FONACOT', '¿Deseas eliminar el crédito FONACOT?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);

                    UX.Loaders.SetLoader(this.ContainerSelector);
                    UX.Cotorra.Catalogs.Delete(
                        this.CatalogName,
                        {
                            id: row.dataItem.ID,
                            employeeConceptsRelationID: row.dataItem.EmployeeConceptsRelationID,
                            employeeID: row.dataItem.EmployeeID
                        },
                        (data) => {
                            $(row.el).fadeOut(function () {
                                row.$kendoGrid.dataSource.remove(row.dataItem);
                                if (UX.Cotorra.Fonacot.EditModel.get('ID') === row.dataItem.ID) {
                                    UX.Cotorra.Fonacot.UI.Set(null);
                                }
                            });
                        },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => {
                            UX.Loaders.RemoveLoader(this.ContainerSelector);
                            UX.Cotorra.Fonacot.UI.Set(null);
                        }
                    );
                },
                () => {
                });
        }
    }
};