'use strict';

UX.Cotorra.SalaryUpdate = {
    GetHistory: (data, onSuccess, onError, onComplete) => {

        UX.Cotorra.Catalogs.Do('GET', 'Employees', 'GetSalaryUpdateHistory', data,
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
        ModalID: '',

        Init: (row = null, model) => {
            let catNam = UX.Cotorra.Employees.UI.CatalogName;

            let containerID = 'record_save_wrapper';

            UX.Cotorra.SalaryUpdate.OriginModel = model;
            let modalID = UX.Cotorra.SalaryUpdate.UI.ModalID = UX.Modals.OpenModal(
                'Incremento de salario', 'm',
                '<div id="' + containerID + '" ></div>',
                function () {
                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');

                    let $container = $('#' + containerID);

                    //Init template
                    var updateSalaryModel = UX.Cotorra.SalaryUpdate.UpdateSalaryModel = new Ractive({
                        el: '#' + containerID,
                        template: '#employees_update_salary_template',
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
                            EmployeeSBCAdjustmentID: row ? row.dataItem.EmployeeSBCAdjustmentID : null,
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveSalary').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveSalary').on('click', function () { $('#np_emp_salaryupdate_saverecord').data('formValidation').validate(); });

                    //Init elements
                    $container.initUIElements();
                    $container.find('#np_emp_sal_txtDate').focus();
                    $('#np_emp_sal_txtDate').mask('00/00/0000');

                    //Set validations
                    $('#np_emp_salaryupdate_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_emp_sal_txtDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de aumento' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_emp_sal_txtDailySalary: {
                                validators: {
                                    notEmpty: {
                                        message: 'Debes capturar el salario diario'
                                    },
                                    callback: {
                                        callback: function (value, validator) {
                                            value = value.replace(/,/g, '');
                                            value = !isNaN(value) ? parseFloat(value) : 0;
                                            if (value <= 0) {
                                                return {
                                                    valid: false, message: 'El salario diario debe ser mayor a 0'
                                                };
                                            }
                                            return true;
                                        }
                                    }
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
                            let data = updateSalaryModel.get();
                            data.EntryDate = null;
                            UX.Cotorra.Catalogs.Do('POST', catNam, 'UpdateSalary', data,
                                (result) => {

                                    let id = result.ID;
                                    let employeeSBCAdjustmentID = result.EmployeeSBCAdjustmentID;
                                    let rowSalary = UX.Cotorra.SalaryUpdate.UI.ActualRow;
                                    let grid = rowSalary ? rowSalary.$kendoGrid : $('.np-emp-salaryupdate-grid').data('kendoGrid');
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
                                        data.EmployeeSBCAdjustmentID = employeeSBCAdjustmentID;

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
                                        dataItem.EmployeeSBCAdjustmentID = data.EmployeeSBCAdjustmentID;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    // Clean form (new)
                                    UX.Cotorra.SalaryUpdate.UI.Set(null);


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

                    UX.Cotorra.SalaryUpdate.UI.LoadGrid([]);
                    UX.Cotorra.SalaryUpdate.UI.GetActualPeriodInitialDate(row.dataItem.PeriodTypeID, modalID);

                    (async function () {
                        UX.Cotorra.SalaryUpdate.GetHistory(
                            { employeeID: UX.Cotorra.SalaryUpdate.UpdateSalaryModel.get('EmployeeID') },
                            (data) => { UX.Cotorra.SalaryUpdate.UI.LoadGrid(data); },
                            () => { },
                            () => { },
                        );
                    })();
                    $('.np-emp-salaryupdate-grid').resizeKendoGrid();
                }
            )
        },

        LoadGrid: (data = []) => {

            //Set fields
            let fields = {
                ID: { type: 'string' },
                ChangeDate: { type: 'date' },
                DailySalary: { type: 'number' },
                EmployeeID: { type: 'string' },
                EmployeeSBCAdjustmentID: { type: 'string' },
            };

            //Set columns
            let columns = [
                {
                    field: 'ChangeDate', title: 'Fecha de aplicación', width: 130,
                    template: UX.Cotorra.Common.GridFormatDate('ChangeDate')
                },
                {
                    field: 'DailySalary', title: 'Salario diario', width: 120,
                    template: UX.Cotorra.Common.GridFormatCurrency('DailySalary'),
                },
                {
                    title: ' ', width: 80,
                    template: kendo.template($('#employeeSalaryUpdateActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-emp-salaryupdate-grid', data: data, fields: fields, columns: columns });
        },

        Set: function (obj) {

            let row = UX.Cotorra.SalaryUpdate.UI.ActualRow = UX.Cotorra.Common.GetRowData(obj);
            let dataItem = row ? row.dataItem : null;
            let model = UX.Cotorra.SalaryUpdate.UpdateSalaryModel;

            $('.modal-title-editing').html(row ? '- EDITANDO INCREMENTO DE SALARIO ' + moment(dataItem.ChangeDate).format("DD/MM/YYYY") : '');
            $('.modal-title-editing').animateCSS('flash');

            //Fill form with data
            model.set('ID', dataItem ? dataItem.ID : '');

            let date = moment(dataItem ? dataItem.ChangeDate : new Date(), "DD/MM/YYYY");
            model.set('ChangeDate', '');
            model.set('ChangeDate', date.format('DD/MM/YYYY'));
            model.set('DailySalary', dataItem ? dataItem.DailySalary + '' : '0.00');
            model.set('SBCFixedPart', dataItem ? dataItem.SBCFixedPart + '' : '0.00');
            model.set('SBCVariablePart', dataItem ? dataItem.SBCVariablePart + '' : '0.00');
            model.set('SBCMax25UMA', dataItem ? dataItem.SBCMax25UMA + '' : '0.00');
            model.set('EmployeeSBCAdjustmentID', dataItem ? dataItem.EmployeeSBCAdjustmentID : '');

            $('#np_emp_salaryupdate_saverecord').data("formValidation").resetForm();
            $('#np_emp_sal_txtDate').focus();
        },

        Delete: function (obj) {

            UX.Modals.Confirm('Eliminar el incremento de salario', '¿Deseas eliminar el incremento de salario?', 'Sí, eliminar', 'No, espera',
                () => {

                    let row = UX.Cotorra.Common.GetRowData(obj);
                    UX.Loaders.SetLoader('#' + this.ModalID);
                    UX.Cotorra.Catalogs.Do('DELETE', this.CatalogName, 'DeleteEmployeeSalaryIncrease', { id: row.dataItem.ID, employeeID: row.dataItem.EmployeeID },
                        (data) => {
                            $(row.el).fadeOut(function () {

                                row.$kendoGrid.dataSource.remove(row.dataItem);
                                if (UX.Cotorra.SalaryUpdate.UpdateSalaryModel.get('ID') === row.dataItem.ID) {
                                    UX.Cotorra.SalaryUpdate.UI.Set(null);
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
                            UX.Loaders.RemoveLoader('#' + this.ModalID);
                        },
                        (complete) => {
                            UX.Loaders.RemoveLoader('#' + this.ModalID);
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
                    UX.Cotorra.SalaryUpdate.UpdateSalaryModel.set('ChangeDate', date.format('DD/MM/YYYY'));
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
                DailySalary: UX.Cotorra.SalaryUpdate.UpdateSalaryModel.get('DailySalary'),
                PeriodTypeID: UX.Cotorra.SalaryUpdate.UpdateSalaryModel.get("PeriodTypeID"),
                EntryDate: moment(UX.Cotorra.SalaryUpdate.UpdateSalaryModel.get('EntryDate'), 'YYYY/MM/DD'),
                BenefitType: UX.Cotorra.SalaryUpdate.UpdateSalaryModel.get('BenefitType'),
            }

            let sbc = UX.Cotorra.Employees.UI.CalculateSBC(params);
            UX.Cotorra.SalaryUpdate.UpdateSalaryModel.set('SBCFixedPart', sbc);
            UX.Cotorra.Employees.UI.CalculateSBCMax25UMA(UX.Cotorra.SalaryUpdate.UpdateSalaryModel);
        },
    }
}