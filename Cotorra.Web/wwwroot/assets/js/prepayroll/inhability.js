'use strict';

UX.Cotorra.Inhability = {
    UI: {
        CatalogName: 'Inhability',
        ContainerSelector: '#np-inhability',
        TitleModalsString: 'INCAPACIDADES',
        ActualRow: null,

        AllInhabilityControlsOptions: [
            { Value: 0, Name: 'Ninguna' },
            { Value: 1, Name: 'Única' },
            { Value: 2, Name: 'Inicial' },
            { Value: 3, Name: 'Subsecuente' },
            { Value: 4, Name: 'Alta médica o ST-2' },
            { Value: 5, Name: 'Valuación o ST-3' },
            { Value: 6, Name: 'Defunción o ST-3' },
            { Value: 7, Name: 'Prenatal' },
            { Value: 8, Name: 'Enlace' },
            { Value: 9, Name: 'Postnatal' }
        ],

        Init: function (params = {}) {

            UX.Cotorra.Inhability.UI.Params = params;

            let modalID = UX.Modals.OpenModal(
                'INCAPACIDADES',
                'xm', '<div id="np-editinhability"></div>',
                function () {
                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');

                    let editModel = UX.Cotorra.Inhability.EditModel = new Ractive({
                        el: '#np-editinhability',
                        template: '#np-editinhability-template',
                        data: {
                            Employee: params.Employee,

                            ID: '',
                            Folio: '',
                            IncidentTypeID: '',
                            AuthorizedDays: 0,
                            InitialDate: moment(new Date()).format("DD/MM/YYYY"),
                            CategoryInsurance: 1,
                            RiskType: 0,
                            Percentage: 0,
                            Consequence: 0,
                            InhabilityControl: 0,
                            Description: '',
                            EmployeeID: params.Employee.ID,

                            //Options
                            IncidentTypesOptions: [],
                            CategoryInsurancesOptions: [
                                { Value: 1, Name: 'Riesgo de trabajo' },
                                { Value: 2, Name: 'Enfermedad general' },
                                { Value: 3, Name: 'Maternidad' },
                                { Value: 4, Name: 'Licencia 140 bis' },
                            ],
                            RiskTypesOptions: [
                                { Value: 0, Name: 'Accidente de trabajo' },
                                { Value: 1, Name: 'Accidente de trayecto' },
                                { Value: 2, Name: 'Enfermedad profesional' }
                            ],
                            ConsequencesOptions: [
                                { Value: 0, Name: 'Ninguna', InhabilityControls: [0] },
                                { Value: 1, Name: 'Incapacidad temporal', InhabilityControls: [1, 2, 3, 4] },
                                { Value: 2, Name: 'Valuación inicial provisional', InhabilityControls: [5] },
                                { Value: 3, Name: 'Valuación inicial definitiva', InhabilityControls: [5] },
                                { Value: 4, Name: 'Defunción', InhabilityControls: [6] },
                                { Value: 5, Name: 'Recaída', InhabilityControls: [1, 2, 3, 4] },
                                { Value: 6, Name: 'Valuación post. a la fecha de alta', InhabilityControls: [5] },
                                { Value: 7, Name: 'Revaluación provisional', InhabilityControls: [5] },
                                { Value: 8, Name: 'Recaída sin alta médica', InhabilityControls: [1, 2, 3, 4] },
                                { Value: 9, Name: 'Revaluación definitiva', InhabilityControls: [5] }
                            ],
                            InhabilityControlsOptions: [
                                { Value: 0, Name: 'Ninguna' },
                                { Value: 1, Name: 'Única' },
                                { Value: 2, Name: 'Inicial' },
                                { Value: 3, Name: 'Subsecuente' },
                                { Value: 4, Name: 'Alta médica o ST-2' },
                                { Value: 5, Name: 'Valuación o ST-3' },
                                { Value: 6, Name: 'Defunción o ST-3' },
                                { Value: 7, Name: 'Prenatal' },
                                { Value: 8, Name: 'Enlace' },
                                { Value: 9, Name: 'Postnatal' }
                            ],
                        },
                        modifyArrays: true
                    });

                    //Buttons
                    $('#np_btnCancelSaveInhability').on('click', function (ev) {
                        ev.preventDefault();
                        $('#np_inhability_form').data("formValidation").destroy();
                        UX.Modals.CloseModal(modalID);
                    });
                    $('#np_btnNewInhability').on('click', function (ev) {
                        ev.preventDefault();
                        UX.Cotorra.Inhability.UI.Set(null);
                    });

                    //Masks
                    $('#np_inhability_txtAuthorizedDays').mask('0000');
                    $('#np_inhability_txtInitialDate').mask('00/00/0000');
                    $("#np_inhability_txtPercentage").decimalMask(2);

                    //Form validation
                    //Set validations
                    $('#np_inhability_form').formValidation({
                        framework: 'bootstrap',
                        button: {
                            selector: 'np_btnSaveInhability'
                        },
                        live: 'disabled',
                        fields: {
                            np_inhability_txtFolio: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el folio' }
                                }
                            },
                            np_inhability_txtInitialDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de inicio' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_inhability_txtPercentage: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el porcentaje de incapacidad' },
                                    callback: {
                                        callback: function (value, validator) {
                                            value = value.replace(',', '');
                                            value = !isNaN(value) ? parseFloat(value) : 0;
                                            if (value < 0) {
                                                return {
                                                    valid: false, message: 'El porcentaje debe ser mayor o igual a 0.00'
                                                };
                                            }
                                            else if (value > 100) {
                                                return {
                                                    valid: false, message: 'El porcentaje debe ser menor o igual a 100'
                                                };
                                            }
                                            return true;
                                        }
                                    }
                                }
                            },
                            np_inhability_txtDescription: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la descripción de los hechos' }
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

                            UX.Cotorra.Catalogs.Save(
                                'Inhability',
                                data,
                                (id) => {
                                    let row = UX.Cotorra.Inhability.UI.ActualRow;
                                    let grid = row ? row.$kendoGrid : $('.np-' + 'inhability' + '-grid').data('kendoGrid');

                                    data.InitialDate = moment(data.InitialDate, "DD/MM/YYYY");

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
                                        dataItem.Folio = data.Folio;
                                        dataItem.IncidentTypeID = data.IncidentTypeID;
                                        dataItem.AuthorizedDays = data.AuthorizedDays;
                                        dataItem.InitialDate = data.InitialDate;
                                        dataItem.CategoryInsurance = data.CategoryInsurance;
                                        dataItem.RiskType = data.RiskType;
                                        dataItem.Percentage = data.Percentage;
                                        dataItem.Consequence = data.Consequence;
                                        dataItem.InhabilityControl = data.InhabilityControl;
                                        dataItem.Description = data.Description;
                                        dataItem.EmployeeID = data.EmployeeID;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

                                    if (params.OnSaveSucessCallback) {
                                        params.OnSaveSucessCallback(data, id);
                                    }

                                    // Clean form (new)
                                    UX.Cotorra.Inhability.UI.Set(null);
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
                    editModel.observe('CategoryInsurance', function (newValue) {
                        UX.Cotorra.Inhability.UI.CategoryInsuranceOnChange(newValue);
                    }, { defer: true, init: true });

                    editModel.observe('Consequence', function (newValue) {
                        UX.Cotorra.Inhability.UI.ConsequenceOnChange(newValue);
                    }, { defer: true, init: true });

                    editModel.observe('RiskType', function (newValue) {
                        UX.Cotorra.Inhability.UI.RiskTypeOnChange(newValue);
                    }, { defer: true, init: true });

                    //Init elements
                    $('#np-editinhability').initUIElements();


                    //Load empty grid
                    UX.Cotorra.Inhability.UI.LoadGrid([]);

                    (async function () {
                        let getInhabilitiesRequest = UX.Cotorra.Catalogs.Get('Inhability',
                            { employeeID: params.Employee.ID },
                            (data) => {
                                UX.Cotorra.Inhability.UI.LoadGrid(data);
                            },
                            () => { },
                            () => { },
                        );

                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#np-inhability',
                                removeWhenFinish: true
                            },
                            catalogs: ['IncidentTypes'],
                            requests: [getInhabilitiesRequest],
                            forceLoad: false
                        });

                        editModel.set('IncidentTypesOptions', UX.Cotorra.Inhability.UI.FilterIncidentTypes(UX.Cotorra.Catalogs.IncidentTypes));

                        $('#np_inhability_txtFolio').focus();
                    })();
                });
        },

        FilterIncidentTypes: function (incidentTypes) {
            let inhability_incidents = incidentTypes.filter((incidentType) => {
                return incidentType.ItConsiders === 3; // Incapacidad
            });

            return inhability_incidents;
        },

        LoadGrid: function (data) {
            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Folio: { type: 'string' },
                AuthorizedDays: { type: 'string' },
                InitialDate: { type: 'date' },
                EmployeeID: { type: 'string' },
            };

            //Set columns
            let columns = [
                { field: 'Folio', title: 'Folio', width: 50 },
                {
                    field: 'InitialDate', title: 'Fecha inicial', width: 60,
                    template: UX.Cotorra.Common.GridFormatDate('InitialDate')
                },
                { field: 'AuthorizedDays', title: 'Días', width: 40, },
                {
                    title: ' ', width: 50,
                    template: kendo.template($('#inhabilityActionTemplate').html())
                },
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
            });
        },

        Set: function (obj) {
            let row = UX.Cotorra.Inhability.UI.ActualRow = UX.Cotorra.Common.GetRowData(obj);
            let dataItem = row ? row.dataItem : null;
            let model = UX.Cotorra.Inhability.EditModel;

            $('.modal-title-editing').html(row ? '- EDITANDO INCAPACIDAD ' + row.dataItem.Folio : '');
            $('.modal-title-editing').animateCSS('flash');

            //Fill form with data
            model.set('ID', dataItem ? dataItem.ID : '');
            model.set('Folio', dataItem ? dataItem.Folio : '');
            model.set('IncidentTypeID', dataItem ? dataItem.IncidentTypeID : UX.Cotorra.Inhability.EditModel.get('IncidentTypeID'));
            model.set('AuthorizedDays', dataItem ? dataItem.AuthorizedDays : '');
            model.set('InitialDate', '');
            model.set('InitialDate', moment(dataItem ? dataItem.InitialDate : new Date).format("DD/MM/YYYY"));
            model.set('CategoryInsurance', dataItem ? dataItem.CategoryInsurance : UX.Cotorra.Inhability.EditModel.get('CategoryInsurance'));
            model.set('RiskType', dataItem ? dataItem.RiskType : UX.Cotorra.Inhability.EditModel.get('RiskType'));
            model.set('Percentage', dataItem ? dataItem.Percentage : '0');
            model.set('Consequence', dataItem ? dataItem.Consequence : UX.Cotorra.Inhability.EditModel.get('Consequence'));
            model.set('InhabilityControl', dataItem ? dataItem.InhabilityControl : UX.Cotorra.Inhability.EditModel.get('InhabilityControl'));
            model.set('Description', dataItem ? dataItem.Description : '');


            $('#np_inhability_form').data("formValidation").resetForm();
            $('#np_inhability_txtFolio').focus();
        },

        Delete: function (obj) {

            let params = UX.Cotorra.Inhability.UI.Params;

            //Show modal
            UX.Modals.Confirm('Eliminar incapacidad', '¿Deseas eliminar la incapacidad?', 'Sí, eliminar', 'No, espera',
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
                                if (UX.Cotorra.Inhability.EditModel.get('ID') === row.dataItem.ID) {
                                    UX.Cotorra.Inhability.UI.Set(null);
                                }

                                if (params.OnSaveSucessCallback) {
                                    params.OnSaveSucessCallback(row.dataItem, data);
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

        CategoryInsuranceOnChange: function (newValue) {
            if (newValue !== 1) {
                UX.Cotorra.Inhability.EditModel.set('RiskType', 0);
                UX.Cotorra.Inhability.EditModel.set('Percentage', '0');
                UX.Cotorra.Inhability.EditModel.set('Consequence', 0);


            }

            if (newValue == 1) {
                let incidentType = UX.Cotorra.Inhability.EditModel.get('IncidentTypesOptions').find(el => el.Code === 'ATRB');
                if (incidentType) {
                    UX.Cotorra.Inhability.EditModel.set('IncidentTypeID', incidentType.ID);
                }
            }
            else if (newValue === 2) {

                let inhabilityControlsOptions = UX.Cotorra.Inhability.UI.AllInhabilityControlsOptions;
                let inhabilityControlsOptionsoApply = inhabilityControlsOptions.filter(element => [1, 2, 3, 4].includes(element.Value));
                UX.Cotorra.Inhability.EditModel.set('InhabilityControlsOptions', inhabilityControlsOptionsoApply);
                let incidentType = UX.Cotorra.Inhability.EditModel.get('IncidentTypesOptions').find(el => el.Code === 'ENFG');
                if (incidentType) {
                    UX.Cotorra.Inhability.EditModel.set('IncidentTypeID', incidentType.ID);
                }

            }

            else if (newValue === 3) {

                let inhabilityControlsOptions = UX.Cotorra.Inhability.UI.AllInhabilityControlsOptions;
                let inhabilityControlsOptionsoApply = inhabilityControlsOptions.filter(element => [7, 8, 9].includes(element.Value));
                UX.Cotorra.Inhability.EditModel.set('InhabilityControlsOptions', inhabilityControlsOptionsoApply);
                let incidentType = UX.Cotorra.Inhability.EditModel.get('IncidentTypesOptions').find(el => el.Code === 'MAT');
                if (incidentType) {
                    UX.Cotorra.Inhability.EditModel.set('IncidentTypeID', incidentType.ID);
                }
            }

            else if (newValue === 4) {
                let inhabilityControlsOptions = UX.Cotorra.Inhability.UI.AllInhabilityControlsOptions;
                let inhabilityControlsOptionsoApply = inhabilityControlsOptions.filter(element => [0].includes(element.Value));
                UX.Cotorra.Inhability.EditModel.set('InhabilityControlsOptions', inhabilityControlsOptionsoApply);
                let incidentType = UX.Cotorra.Inhability.EditModel.get('IncidentTypesOptions').find(el => el.Code === 'L140');
                if (incidentType) {
                    UX.Cotorra.Inhability.EditModel.set('IncidentTypeID', incidentType.ID);
                }
            }

        },

        ConsequenceOnChange: function (newValue) {
            let consequencesOptions = UX.Cotorra.Inhability.EditModel.get('ConsequencesOptions');
            let consequenceOptionSelected = consequencesOptions.find(el => el.Value === newValue);
            let inhabilityControlsOptionsoApplyIds = consequenceOptionSelected.InhabilityControls;
            let inhabilityControlsOptions = UX.Cotorra.Inhability.UI.AllInhabilityControlsOptions;
            let inhabilityControlsOptionsoApply = inhabilityControlsOptions.filter(element => inhabilityControlsOptionsoApplyIds.includes(element.Value));
            UX.Cotorra.Inhability.EditModel.set('InhabilityControlsOptions', inhabilityControlsOptionsoApply);
        },

        RiskTypeOnChange: function (newValue) {
            let code = '';
            if (newValue === 0 || newValue === 2) {
                code = 'ATRB';
            }
            else if (newValue === 1) {
                code = 'ATRY';
            }
            let incidentType = UX.Cotorra.Inhability.EditModel.get('IncidentTypesOptions').find(el => el.Code === code);
            if (incidentType) {
                UX.Cotorra.Inhability.EditModel.set('IncidentTypeID', incidentType.ID);
            }
        }
    }
};