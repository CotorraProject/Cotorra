'Use strict';

UX.Cotorra.PermanentMovements = {
    UI: {
        CatalogName: 'PermanentMovements',
        ContainerSelector: '#np-permanentmovements',
        TitleModalsString: 'MOVIMIENTOS PERMANENTES',
        ActualRow: null,
        DeductionsTypesOptions: [],
        PerceptionsTypesOptions: [],

        Init: (params = {}) => {
            let modalID = UX.Modals.OpenModal(
                'Captura de movimientos permanentes',
                'm', '<div id="np-editpermanentmovements"></div>',
                function () {
                    let editModel = new Ractive({
                        el: '#np-editpermanentmovements',
                        template: '#np-editpermanentmovements-template',
                        data: {
                            Employee: params.Employee,

                            ID: '',
                            Description: '',
                            ConceptType: 3,
                            ConceptPaymentID: '',
                            InitialApplicationDate: moment(new Date()).format("DD/MM/YYYY"),
                            PermanentMovementType: 1,
                            Amount: '0.00',
                            TimesToApply: 0,
                            TimesApplied: 0,
                            LimitAmount: '0.00',
                            AccumulatedAmount: '0.00',
                            RegistryDate: moment(new Date()).format("DD/MM/YYYY"),
                            ControlNumber: 0,
                            PermanentMovementStatus: 1,
                            EmployeeID: params.Employee.ID,

                            //Options
                            ConceptTypesOptions: [
                                { Value: 3, Description: 'Deducción' },
                                { Value: 2, Description: 'Percepción' }
                            ],
                            PermanentMovementTypesOptions: [
                                { Value: 1, Description: 'Importe' },
                                { Value: 2, Description: 'Valor' }
                            ],
                            PermanentMovementStatusOptions: [
                                { Value: 1, Description: 'Activo' },
                                { Value: 2, Description: 'Inactivo' }
                            ],

                            //Deductions, Perceptions
                            DeductionsTypesOptions: [],
                            PerceptionsTypesOprions: [],
                            ConceptPaymentsOptions: [],
                        },
                        modifyArrays: true
                    });

                    //Buttons
                    $('#np_btnClosePermanentMovements').on('click', function (ev) {
                        ev.preventDefault();
                        $('#np_permanentmovements_form').data("formValidation").destroy();
                        UX.Modals.CloseModal(modalID);
                    });
                    $('#np_btnNewPermanentMovements').on('click', function (ev) {
                        ev.preventDefault();
                        UX.Cotorra.PermanentMovements.UI.Set(null);
                    });

                    //Masks
                    $('#np_permanentmovements_txtInitialApplicationDate').mask('00/00/0000');
                    $("#np_permanentmovements_txtAmount").decimalMask(2);
                    $('#np_permanentmovements_txtTimesToApply').mask('0000');
                    $('#np_permanentmovements_txtTimesApplied').mask('0000');
                    $("#np_permanentmovements_txtLimitAmount").decimalMask(2);
                    $("#np_permanentmovements_txtAccumulatedAmount").decimalMask(2);
                    $('#np_permanentmovements_txtRegistryDate').mask('00/00/0000');
                    $('#np_permanentmovements_txtControlNumber').mask('00000000');

                    //Form validation
                    //Set validations
                    $('#np_permanentmovements_form').formValidation({
                        framework: 'bootstrap',
                        button: {
                            selector: 'np_btnSavePermanentMovements'
                        },
                        live: 'disabled',
                        fields: {
                            np_permanentmovements_txtDescription: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la descripción' }
                                }
                            },
                            np_permanentmovements_txtInitialApplicationDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha inicio de aplicación' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_permanentmovements_txtAmount: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el importe' },
                                    callback: {
                                        callback: function (value, validator) {
                                            value = value.replace(/,/g, '');
                                            value = !isNaN(value) ? parseFloat(value) : 0;
                                            if (value <= 0.0) {
                                                return {
                                                    valid: false, message: 'El importe debe ser mayor a 0.00'
                                                };
                                            }
                                            return true;
                                        }
                                    }
                                }
                            },
                            np_permanentmovements_txtTimesToApply: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar las veces a aplicar' }
                                }
                            },
                            np_permanentmovements_txtTimesApplied: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar las veces aplicadas' }
                                }
                            },
                            np_permanentmovements_txtLimitAmount: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el monto límite' }
                                }
                            },
                            np_permanentmovements_txtAccumulatedAmount: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el monto acumulado' }
                                }
                            },
                            np_permanentmovements_txtRegistryDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de registro' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_permanentmovements_txtControlNumber: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el número de control' }
                                }
                            },
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = editModel.get();

                            data.Amount = typeof data.Amount === 'string' ? parseFloat(data.Amount.replace(/,/g, '')) : data.Amount;
                            data.LimitAmount = typeof data.LimitAmount === 'string' ? parseFloat(data.LimitAmount.replace(/,/g, '')) : data.LimitAmount;
                            data.AccumulatedAmount = typeof data.AccumulatedAmount === 'string' ? parseFloat(data.AccumulatedAmount.replace(/,/g, '')) : data.AccumulatedAmount;

                            UX.Cotorra.Catalogs.Save(
                                'PermanentMovements',
                                data,
                                (id) => {
                                    let row = UX.Cotorra.PermanentMovements.UI.ActualRow;
                                    let grid = row ? row.$kendoGrid : $('.np-' + 'permanentmovements' + '-grid').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = id;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;

                                        dataItem.ID = data.ID;
                                        dataItem.Description = data.Description;
                                        dataItem.ConceptType = data.ConceptType;
                                        dataItem.ConceptPaymentID = data.ConceptPaymentID;
                                        dataItem.InitialApplicationDate = data.InitialApplicationDate;
                                        dataItem.PermanentMovementType = data.PermanentMovementType;
                                        dataItem.Amount = data.Amount;
                                        dataItem.TimesToApply = data.TimesToApply;
                                        dataItem.TimesApplied = data.TimesApplied;
                                        dataItem.LimitAmount = data.LimitAmount;
                                        dataItem.AccumulatedAmount = data.AccumulatedAmount;
                                        dataItem.RegistryDate = data.RegistryDate;
                                        dataItem.ControlNumber = data.ControlNumber;
                                        dataItem.PermanentMovementStatus = data.PermanentMovementStatus;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

                                    // Clean form (new)
                                    UX.Cotorra.PermanentMovements.UI.Set(null);
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
                    editModel.observe('ConceptType', function (newValue) {
                        let options = [];
                        if (newValue === 2) {
                            options = UX.Cotorra.PermanentMovements.UI.PerceptionsTypesOptions;
                        }
                        else if (newValue === 3) {
                            options = UX.Cotorra.PermanentMovements.UI.DeductionsTypesOptions;
                        }
                        editModel.set('ConceptPaymentsOptions', options);
                    }, { defer: true, init: true });

                    //Init elements
                    $('#np-editpermanentmovements').initUIElements();

                    UX.Cotorra.PermanentMovements.EditModel = editModel;

                    //Load empty grid
                    UX.Cotorra.PermanentMovements.UI.LoadGrid([]);

                    (async function () {
                        let getPermanentMovementsRequest = UX.Cotorra.Catalogs.Get('PermanentMovements',
                            { employeeID: params.Employee.ID },
                            (data) => {
                                UX.Cotorra.PermanentMovements.UI.LoadGrid(data);
                            },
                            () => { },
                            () => { },
                        );

                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#np-permanentmovements',
                                removeWhenFinish: true
                            },
                            catalogs: ['Concepts'],
                            requests: [getPermanentMovementsRequest],
                            forceLoad: false
                        });

                        UX.Cotorra.PermanentMovements.UI.FilterConcepts(editModel, UX.Cotorra.Catalogs.Concepts);

                        $('#np_permanentmovements_txtDescription').focus();
                    })();
                });
        },

        FilterConcepts: function (model, concepts) {
            let deductions = concepts.filter(concept => concept.ConceptType === 3);
            deductions = deductions.map(deduction => {
                return { ID: deduction.ID, Description: deduction.Code + ' - ' + deduction.Name };
            });

            let perceptions = concepts.filter(concept => concept.ConceptType === 2);
            perceptions = perceptions.map(perception => {
                return { ID: perception.ID, Description: perception.Code + ' - ' + perception.Name };
            });

            UX.Cotorra.PermanentMovements.UI.DeductionsTypesOptions = deductions;
            UX.Cotorra.PermanentMovements.UI.PerceptionsTypesOptions = perceptions;
            model.set('ConceptPaymentsOptions', UX.Cotorra.PermanentMovements.UI.DeductionsTypesOptions);
        },

        LoadGrid: function (data) {
            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Description: { type: 'string' }
            };

            //Set columns
            let columns = [
                { field: 'Description', title: 'Descripción', width: 140 },
                {
                    title: ' ', width: 80,
                    template: kendo.template($('#permanentMovementsActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500 }
            });
        },

        Set: function (obj) {
            let row = UX.Cotorra.PermanentMovements.UI.ActualRow = UX.Cotorra.Common.GetRowData(obj);
            let dataItem = row ? row.dataItem : null;
            let model = UX.Cotorra.PermanentMovements.EditModel;

            $('.modal-title-editing').html(row ? '- EDITANDO MOVIMIENTO PERMANENTE ' + row.dataItem.Folio : '');
            $('.modal-title-editing').animateCSS('flash');
            let type = UX.Cotorra.PermanentMovements.UI.GetConceptType(dataItem);
            let firstConcept = UX.Cotorra.PermanentMovements.UI.DeductionsTypesOptions[0].ID;

            //Fill form with data
            model.set('ID', dataItem ? dataItem.ID : '');
            model.set('Description', dataItem ? dataItem.Description : '');
            model.set('ConceptType', type);
            model.set('ConceptPaymentID', dataItem ? dataItem.ConceptPaymentID : firstConcept);
            model.set('InitialApplicationDate', moment(dataItem ? dataItem.InitialApplicationDate : new Date).format("DD/MM/YYYY"));
            model.set('PermanentMovementType', dataItem ? dataItem.PermanentMovementType : 1);
            model.set('Amount', dataItem ? dataItem.Amount : '0.00');
            model.set('TimesToApply', dataItem ? dataItem.TimesToApply : 0);
            model.set('TimesApplied', dataItem ? dataItem.TimesApplied : 0);
            model.set('LimitAmount', dataItem ? dataItem.LimitAmount : '0.00');
            model.set('AccumulatedAmount', dataItem ? dataItem.AccumulatedAmount : '0.00');
            model.set('RegistryDate', moment(dataItem ? dataItem.RegistryDate : new Date).format("DD/MM/YYYY"));
            model.set('ControlNumber', dataItem ? dataItem.ControlNumber : 0);
            model.set('PermanentMovementStatus', dataItem ? dataItem.PermanentMovementStatus : 1);

            $('#np_permanentmovements_form').data("formValidation").resetForm();
            $('#np_permanentmovements_txtDescription').focus();
        },

        GetConceptType: function (dataItem) {
            let type = 3;
            if (dataItem) {
                let concept = UX.Cotorra.PermanentMovements.UI.PerceptionsTypesOptions.filter(perception => perception.ID === dataItem.ConceptPaymentID);
                if (concept.length > 0) {
                    type = 2;
                }
            }

            return type;
        },

        Delete: function (obj) {
            //Show modal
            UX.Modals.Confirm('Eliminar los movimientos permanentes', '¿Deseas eliminar los movimientos permanentes?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);

                    UX.Loaders.SetLoader(this.ContainerSelector);
                    UX.Cotorra.Catalogs.Delete(
                        this.CatalogName,
                        {
                            id: row.dataItem.ID
                        },
                        (data) => {
                            $(row.el).fadeOut(function () {
                                row.$kendoGrid.dataSource.remove(row.dataItem);
                                if (UX.Cotorra.PermanentMovements.EditModel.get('ID') === row.dataItem.ID) {
                                    UX.Cotorra.PermanentMovements.UI.Set(null);
                                }
                            });
                        },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader(this.ContainerSelector); }
                    );
                },
                () => {
                });
        }
    }
};