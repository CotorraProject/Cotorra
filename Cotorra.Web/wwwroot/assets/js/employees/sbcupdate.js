'use strict';

UX.Cotorra.SBCUpdate = {
    GetHistory: (data, onSuccess, onError, onComplete) => {

        UX.Cotorra.Catalogs.Do('GET', 'Employees', 'GetSBCUpdateHistory', data,
            (data) => {
                onSuccess(data);
            },
            (error) => {
                onError(error);
            },
            (complete) => {
                onComplete();
            }
        );
    },

    UI: {
        CatalogName: 'Employees',

        Init: (row = null, model) => {
            let catNam = UX.Cotorra.Employees.UI.CatalogName;
            UX.Cotorra.SalaryUpdate.OriginModel = model;

            let containerID = 'record_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                'Modificación de SBC', 'm',
                '<div id="' + containerID + '" ></div>',
                function () {
                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');

                    let $container = $('#' + containerID);

                    //Init template
                    var updateSBCModel = UX.Cotorra.SBCUpdate.UpdateSBCModel = new Ractive({
                        el: '#' + containerID,
                        template: '#employees_update_sbc_template',
                        data: {
                            ID: null,
                            Name: row ? row.dataItem.Name + ' ' + row.dataItem.FirstLastName : '',
                            ChangeDate: row ? row.dataItem.DueDate : '',

                            DailySalary: model ? model.get('DailySalary') : null,
                            SBCFixedPart: model ? model.get('SBCFixedPart') : '0.00',
                            SBCVariablePart: model ? model.get('SBCVariablePart') : '0.00',
                            SBCMax25UMA: model ? model.get('SBCMax25UMA') : '0.00',
                            EmployeeID: row ? row.dataItem.ID : null,
                            PeriodTypeID: row ? row.dataItem.PeriodTypeID : null,
                            EntryDate: row ? row.dataItem.EntryDate : null,
                            BenefitType: row ? row.dataItem.BenefitType : null,
                            ContributionBase: row ? row.dataItem.ContributionBase : null,
                            ContributionBasesOptions: UX.Cotorra.ContributionBases,

                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveSBC').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveSBC').on('click', function () { $('#np_emp_sbcupdate_saverecord').data('formValidation').validate(); });

                    //Init elements
                    $container.initUIElements();
                    $container.find('#np_emp_sal_txtDate').focus();
                    $('#np_emp_sal_txtDate').mask('00/00/0000');

                    //Set validations
                    $('#np_emp_sbcupdate_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_emp_sal_txtDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de aumento' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_emp_sal_txtSBCFixedPart: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el SBC parte fija' },
                                }
                            },
                            np_emp_sal_txtSBCVariablePart: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el SBC parte variable' },
                                }
                            },
                            np_emp_sal_txtSBCMax25UMA: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar SBC (tope 25 UMA)' },
                                }
                            },
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = updateSBCModel.get();
                            data.EntryDate = null;
                            UX.Cotorra.Catalogs.Do('POST', catNam, 'UpdateSBC', data,
                                (result) => {

                                    let id = result.ID;
                                    let rowSalary = UX.Cotorra.SBCUpdate.UI.ActualRow;
                                    let grid = rowSalary ? rowSalary.$kendoGrid : $('.np-emp-sbcupdate-grid').data('kendoGrid');
                                    let gridEmployee = row.$kendoGrid

                                    data.ChangeDate = moment(data.ChangeDate, "DD/MM/YYYY");

                                    //Employee grid
                                    if (result.UpdateGrid) {
                                        let dataItemEmployee = row.dataItem;
                                        dataItemEmployee.DailySalary = data.DailySalary;
                                        dataItemEmployee.SBCFixedPart = data.SBCFixedPart;
                                        UX.Common.KendoFastRedrawRow(gridEmployee, dataItemEmployee);
                                        $('tr[data-uid="' + dataItemEmployee.uid + '"]').animateCSS('flash');

                                        if (model) {
                                            model.set('DailySalary', data.DailySalary);
                                            model.set('SBCFixedPart', data.SBCFixedPart);
                                            model.set('SBCMax25UMA', data.SBCMax25UMA);
                                            model.set('SBCVariablePart', data.SBCVariablePart);
                                        }
                                    }

                                    if (!rowSalary) {
                                        //Set data
                                        data.ID = id;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    } else {
                                        //Update data
                                        let dataItem = rowSalary.dataItem;

                                        dataItem.ID = data.ID;
                                        dataItem.EmployeeID = data.EmployeeID;
                                        dataItem.ChangeDate = data.ChangeDate;
                                        dataItem.DailySalary = data.DailySalary;
                                        dataItem.SBCFixedPart = data.SBCFixedPart;
                                        dataItem.SBCVariablePart = data.SBCVariablePart;
                                        dataItem.SBCMax25UMA = data.SBCMax25UMA;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    // Clean form (new)
                                    UX.Cotorra.SBCUpdate.UI.Set(null);


                                },
                                (error) => {
                                    UX.Common.ErrorModal(error);
                                    UX.Loaders.RemoveLoader('#' + modalID);
                                },
                                (complete) => {
                                    UX.Loaders.RemoveLoader('#' + modalID);
                                }
                            );
                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    //Observers
                    updateSBCModel.observe('SBCFixedPart',
                        function (newValue, oldValue) {
                            UX.Cotorra.Employees.UI.CalculateSBCMax25UMA(UX.Cotorra.SBCUpdate.UpdateSBCModel);
                        },
                        { init: false, defer: true });

                    updateSBCModel.observe('SBCVariablePart',
                        function (newValue, oldValue) {
                            UX.Cotorra.Employees.UI.CalculateSBCMax25UMA(UX.Cotorra.SBCUpdate.UpdateSBCModel);
                        },
                        { init: false, defer: true });

                    UX.Cotorra.SBCUpdate.UI.LoadGrid([]);
                    UX.Cotorra.SBCUpdate.UI.GetActualPeriodInitialDate(row.dataItem.PeriodTypeID, modalID);

                    (async function () {
                        UX.Cotorra.SBCUpdate.GetHistory(
                            { employeeID: UX.Cotorra.SBCUpdate.UpdateSBCModel.get('EmployeeID') },
                            (data) => { UX.Cotorra.SBCUpdate.UI.LoadGrid(data); },
                            () => { },
                            () => { },
                        );
                    })();
                    $('.np-emp-sbcupdate-grid').resizeKendoGrid();
                }
            )
        },

        LoadGrid: (data = []) => {

            //Set fields
            let fields = {
                ID: { type: 'string' },
                ChangeDate: { type: 'date' },
                SBCMax25UMA: { type: 'number' },
                EmployeeID: { type: 'string' },
            };

            //Set columns
            let columns = [
                {
                    field: 'ChangeDate', title: 'Fecha de modificación', width: 130,
                    template: UX.Cotorra.Common.GridFormatDate('ChangeDate')
                },
                {
                    field: 'SBC', title: 'SBC', width: 80,
                    template: UX.Cotorra.Common.GridFormatCurrency('SBCMax25UMA'),
                },
                {
                    title: ' ', width: 80,
                    template: kendo.template($('#employeeSBCUpdateActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-emp-sbcupdate-grid', data: data, fields: fields, columns: columns });
        },

        Set: function (obj) {

            let row = UX.Cotorra.SBCUpdate.UI.ActualRow = UX.Cotorra.Common.GetRowData(obj);
            let dataItem = row ? row.dataItem : null;
            let model = UX.Cotorra.SBCUpdate.UpdateSBCModel;

            $('.modal-title-editing').html(row ? '- EDITANDO MODIFICACIÓN SBC ' + moment(dataItem.ChangeDate).format("DD/MM/YYYY") : '');
            $('.modal-title-editing').animateCSS('flash');

            //Fill form with data
            model.set('ID', dataItem ? dataItem.ID : '');

            let date = moment(dataItem ? dataItem.ChangeDate : new Date(), "DD/MM/YYYY");
            model.set('ChangeDate', '');
            model.set('ChangeDate', date.format('DD/MM/YYYY'));
            model.set('DailySalary', dataItem ? dataItem.DailySalary : '0.00');
            model.set('SBCFixedPart', dataItem ? dataItem.SBCFixedPart + '' : '0.00');
            model.set('SBCVariablePart', dataItem ? dataItem.SBCVariablePart + '' : '0.00');
            model.set('SBCMax25UMA', dataItem ? dataItem.SBCMax25UMA + '' : '0.00');

            $('#np_emp_sbcupdate_saverecord').data("formValidation").resetForm();
            $('#np_emp_sal_txtDate').focus();
        },

        Delete: function (obj) {

            UX.Modals.Confirm('Eliminar la modificación de SBC', '¿Deseas eliminar la modificación de SBC?', 'Sí, eliminar', 'No, espera',
                () => {

                    let row = UX.Cotorra.Common.GetRowData(obj);
                    UX.Loaders.SetLoader(this.ContainerSelector);
                    UX.Cotorra.Catalogs.Do('DELETE', this.CatalogName, 'DeleteEmployeeSBCAdjustment', { id: row.dataItem.ID, employeeID: row.dataItem.EmployeeID },
                        (data) => {
                            $(row.el).fadeOut(function () {

                                row.$kendoGrid.dataSource.remove(row.dataItem);
                                if (UX.Cotorra.SBCUpdate.UpdateSBCModel.get('ID') === row.dataItem.ID) {
                                    UX.Cotorra.SBCUpdate.UI.Set(null);
                                }
                            });

                            if (UX.Cotorra.SalaryUpdate.OriginModel) {
                                UX.Cotorra.SalaryUpdate.OriginModel.set('DailySalary', data.DailySalary);
                                UX.Cotorra.SalaryUpdate.OriginModel.set('SBCFixedPart', data.SBCFixedPart);
                                UX.Cotorra.SalaryUpdate.OriginModel.set('SBCMax25UMA', data.SBCMax25UMA);
                                UX.Cotorra.SalaryUpdate.OriginModel.set('SBCVariablePart', data.SBCVariablePart);
                            }
                        },
                        (error) => {
                            UX.Common.ErrorModal(error);
                        },
                        (complete) => {

                        }
                    );
                },
                () => {
                });
        },

        GetActualPeriodInitialDate: (data, modalID) => {

            //Get actual period initial date
            UX.Loaders.SetLoader('#' + modalID);
            UX.Cotorra.Catalogs.Do('GET', 'Periods', 'GetActualDetail', { periodTypeID: data },
                (data) => {
                    let date = data ? moment(data.InitialDate, 'YYYY/MM/DD') : '';
                    UX.Cotorra.SBCUpdate.UpdateSBCModel.set('ChangeDate', date.format('DD/MM/YYYY'));
                },
                (error) => {
                    UX.Common.ErrorModal(error);
                },
                (complete) => {
                    UX.Loaders.RemoveLoader('#' + modalID);
                }
            );

        },

        SetCalculateSBC: () => {

            let params = {
                DailySalary: UX.Cotorra.SBCUpdate.UpdateSBCModel.get('DailySalary'),
                PeriodTypeID: UX.Cotorra.SBCUpdate.UpdateSBCModel.get("PeriodTypeID"),
                EntryDate: moment(UX.Cotorra.SBCUpdate.UpdateSBCModel.get('EntryDate'), 'YYYY/MM/DD'),
                BenefitType: UX.Cotorra.SBCUpdate.UpdateSBCModel.get('BenefitType'),
            }

            let sbc = UX.Cotorra.Employees.UI.CalculateSBC(params);
            UX.Cotorra.SBCUpdate.UpdateSBCModel.set('SBCFixedPart', sbc);
            UX.Cotorra.Employees.UI.CalculateSBCMax25UMA(UX.Cotorra.SBCUpdate.UpdateSBCModel);

        },
    }
};
