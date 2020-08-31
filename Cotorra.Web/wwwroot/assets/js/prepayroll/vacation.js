'use strict';

UX.Cotorra.Vacation = {
    UI: {
        CatalogName: 'Vacation',
        ContainerSelector: '#np-vacation',
        TitleModalsString: 'VACACIONES',
        ActualRow: null,

        Init: function (params = {}) {

            UX.Cotorra.Vacation.UI.Params = params;
            let modalID = UX.Modals.OpenModal(
                'VACACIONES',
                'xm', '<div id="np-editvacation"></div>',
                function () {

                    let InitialDate = moment(new Date()).format("DD/MM/YYYY");
                    let FinalDate = moment(new Date()).format("DD/MM/YYYY");

                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');

                    let editModel = new Ractive({
                        el: '#np-editvacation',
                        template: '#np-editvacation-template',
                        data: {
                            Employee: params.Employee,
                            EmployeeSeniority: 0,
                            EmployeeBenefitType: 0,
                            ID: '',
                            VacationsCaptureType: 1,
                            VacationsCaptureTypeConfiguration: 1,
                            InitialDate: InitialDate,
                            FinalDate: FinalDate,
                            Break_Seventh_Days: '0',
                            VacationsDays: '0',
                            VacationsBonusDays: '0',
                            VacationsBonusPercentage: '0',
                            PeriodDetail: params.PeriodDetail,
                            DaysOff: '',
                            showMessage: false,
                        },
                        modifyArrays: true
                    });

                    //Buttons
                    $('#np_btnCancelSaveVacation').on('click', function (ev) {
                        ev.preventDefault();
                        $('#np_vacation_form').data("formValidation").destroy();
                        UX.Modals.CloseModal(modalID);
                    });
                    $('#np_btnNewVacation').on('click', function (ev) {
                        ev.preventDefault();
                        UX.Cotorra.Vacation.UI.Set(null);
                    });

                    //Masks
                    $('#np_vacation_txtInitDate').mask('00/00/0000');
                    $('#np_vacation_txtFinalDate').mask('00/00/0000');
                    $('#np_vacation_txtVacationsDays').mask('00000');
                    $("#np_vacation_txtBreakSeventhDays").mask('000');
                    $("#np_vacation_txtVacationsBonusDays").decimalMask(2);

                    //Form validation
                    //Set validations
                    $('#np_vacation_form').formValidation({
                        framework: 'bootstrap',
                        button: {
                            selector: 'np_btnSaveVacation'
                        },
                        live: 'disabled',
                        fields: {
                            np_vacation_txtInitDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de inicio' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_vacation_txtVacationsDays: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar los días de vacaciones' }
                                }
                            },
                            np_vacation_txtVacationsBonusDays: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la prima vacacional de los días de vacaciones' },
                                    callback: {
                                        callback: function (value, validator) {
                                            value = !isNaN(value) ? parseFloat(value) : 0;
                                            if (value <= 0) {
                                                return {
                                                    valid: false, message: 'La prima vacacional de los días de vacaciones deben ser mayor a 0'
                                                };
                                            }
                                            return true;
                                        }
                                    }
                                }
                            },
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            let validation = UX.Cotorra.Vacation.UI.ValidateFinalDate();
                            if (validation && validation.valid === false) {
                                UX.Modals.Alert('ERROR', validation.message, 'm', 'error', () => { });
                                return;
                            }

                            validation = UX.Cotorra.Vacation.UI.ValidateDaysOff(editModel.get('DaysOff'));
                            if (validation && validation.valid === false) {
                                UX.Modals.Alert('ERROR', validation.message, 'm', 'error', () => { });
                                return;
                            }

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = editModel.get();

                            UX.Cotorra.Catalogs.Save(
                                'Vacation',
                                data,
                                (show) => {

                                    UX.Cotorra.Catalogs.Get('Vacation',
                                        { employeeID: UX.Cotorra.Vacation.EditModel.get('Employee.ID'), PeriodDetailID: UX.Cotorra.Vacation.EditModel.get('PeriodDetail.ID') },
                                        (data) => { UX.Cotorra.Vacation.UI.LoadGrid(data); },
                                        () => { },
                                        () => { },
                                    );

                                    UX.Cotorra.Vacation.EditModel.set('showMessage', show);

                                    if (params.OnSaveSucessCallback) {
                                        params.OnSaveSucessCallback(data, show);
                                    }

                                    // Clean form (new)
                                    UX.Cotorra.Vacation.UI.Set(null);
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
                    editModel.observe('InitialDate FinalDate', function (newValue) {
                        let validation = UX.Common.ValidDateValidator(newValue);
                        if (validation.valid === undefined) {
                            UX.Cotorra.Vacation.UI.CalculateVacationDays(editModel);
                        }
                    }, { defer: true, init: true });

                    editModel.observe('DaysOff', function (newValue) {
                        UX.Cotorra.Vacation.UI.CalculateVacationDays(editModel);
                    }, { defer: true, init: true });

                    //Init elements
                    $('#np-editvacation').initUIElements();


                    UX.Common.InitDatePicker({
                        multidate: true,
                        model: editModel,
                        keyPath: 'DaysOff',
                        selector: '#np_vacation_txtBreakSeventhDaysCal',
                        value: ''
                    });

                    UX.Common.InitDatePicker({
                        multidate: false,
                        model: editModel,
                        keyPath: 'InitialDate',
                        selector: '#np_vacation_txtInitDate',
                        value: InitialDate
                    });

                    UX.Common.InitDatePicker({
                        multidate: false,
                        model: editModel,
                        keyPath: 'FinalDate',
                        selector: '#np_vacation_txtFinalDate',
                        value: FinalDate
                    });


                    UX.Cotorra.Vacation.EditModel = editModel;

                    //Load empty grid
                    UX.Cotorra.Vacation.UI.LoadGrid([]);

                    (async function () {
                        let getVacationsRequest = UX.Cotorra.Catalogs.Get('Vacation',
                            { employeeID: params.Employee.ID, PeriodDetailID: params.PeriodDetail.ID },
                            (data) => {
                                UX.Cotorra.Vacation.UI.LoadGrid(data);
                            },
                            () => { },
                            () => { },
                        );

                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#np-vacation',
                                removeWhenFinish: true
                            },
                            requests: [getVacationsRequest],
                            catalogs: ['BenefitTypes', 'PayrollCompanyConfiguration'],
                            forceLoad: false
                        });

                        UX.Cotorra.Vacation.EditModel.set('VacationsCaptureTypeConfiguration',
                            UX.Cotorra.Catalogs.PayrollCompanyConfiguration[0].HolidayPremiumPaymentType);

                        UX.Cotorra.Vacation.EditModel.set('VacationsCaptureType',
                            UX.Cotorra.Catalogs.PayrollCompanyConfiguration[0].HolidayPremiumPaymentType);


                        UX.Cotorra.Catalogs.GetByID('Employees',
                            { ID: params.Employee.ID },
                            (data) => {
                                UX.Cotorra.Vacation.EditModel.set('EmployeeSeniority', data.AntiquityForBenefits)
                                UX.Cotorra.Vacation.EditModel.set('EmployeeBenefitType', data.BenefitType)

                                let benefitTypeName = data.BenefitType === 2 ? 'Personalizada' : 'De ley';
                                let benefitType = UX.Cotorra.Catalogs.BenefitTypes[0][0].Name === benefitTypeName ? 0 : 1
                                let vacationsBonusPercentage = UX.Cotorra.Catalogs.BenefitTypes[benefitType][data.AntiquityForBenefits].HolidayPremiumPortion;
                                UX.Cotorra.Vacation.EditModel.set('VacationsBonusPercentage', vacationsBonusPercentage)
                            },
                            () => { },
                            () => { },
                        );

                        $('#np_vacation_drpVacationCaptureType').focus();
                    })();
                });
        },

        CalculateVacationDays(editModel) {

            let vacationsBonusPercentageFactor = editModel.get('VacationsBonusPercentage') / 100;

            let initial = moment(editModel.get('InitialDate'), "DD/MM/YYYY");
            let final = moment(editModel.get('FinalDate'), "DD/MM/YYYY");

            if (final.isSameOrAfter(initial)) {
                let days = final.diff(initial, 'days');
                let breakSeventhDays = editModel.get('DaysOff').split('/').length / 2;
                let totalDays = days - (breakSeventhDays ? parseInt(breakSeventhDays) : 0) + 1;
                // Set values
                editModel.set('VacationsDays', '' + totalDays < 0 ? 0 : totalDays);
                if (editModel.get('VacationsCaptureTypeConfiguration') === 0) {
                    editModel.set('VacationsBonusDays', '' + totalDays < 0 ? 0 : totalDays * vacationsBonusPercentageFactor);
                }
            }

        },

        LoadGrid: function (data) {
            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                InitialDate: { type: 'date' },
                FinalDate: { type: 'date' },
                VacationsDays: { type: 'number' },
                VacationsBonusDays: { type: 'number' },
                DaysOff: { type: 'string' },
                EmployeeID: { type: 'string' },
            };

            //Set columns
            let columns = [
                {
                    field: 'InitialDate', title: 'Fecha inicio', width: 95,
                    template: UX.Cotorra.Common.GridFormatDate('InitialDate')
                },
                {
                    field: 'FinalDate', title: 'Fecha final', width: 90,
                    template: UX.Cotorra.Common.GridFormatDate('FinalDate')
                },
                {
                    field: 'VacationsDays', title: 'Días', width: 80
                },
                {
                    field: 'VacationsBonusDays', title: 'Días prima', width: 80
                },
                {
                    title: ' ', width: 80,
                    template: kendo.template($('#vacationActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
            });
        },

        Set: function (obj) {
            let row = UX.Cotorra.Vacation.UI.ActualRow = UX.Cotorra.Common.GetRowData(obj);
            let dataItem = row ? row.dataItem : null;
            let model = UX.Cotorra.Vacation.EditModel;

            $('.modal-title-editing').html(row ? '- EDITANDO VACACIONES DEL ' + moment(dataItem.InitialDate).format('DD/MM/YYYY') + ' AL ' + moment(dataItem.FinalDate).format("DD/MM/YYYY") : '');
            $('.modal-title-editing').animateCSS('flash');

            //Fill form with data
            model.set('ID', dataItem ? dataItem.ID : '');
            model.set('VacationsCaptureType', dataItem ? dataItem.VacationsCaptureType : 1);
            model.set('InitialDate', '');
            model.set('FinalDate', '');
            model.set('Break_Seventh_Days', dataItem ? dataItem.Break_Seventh_Days : '0');
            model.set('VacationsDays', dataItem ? dataItem.VacationsDays : '0');
            model.set('VacationsBonusDays', dataItem ? dataItem.VacationsBonusDays : '0');
            model.set('showMessage', dataItem ? false : model.get('showMessage'));
            UX.Common.SetDatePicker({ selector: '#np_vacation_txtBreakSeventhDaysCal', value: dataItem ? dataItem.DaysOff : '' });
            UX.Common.SetDatePicker({ selector: '#np_vacation_txtInitDate', value: moment(dataItem ? dataItem.InitialDate : new Date).format('DD/MM/YYYY') });
            UX.Common.SetDatePicker({ selector: '#np_vacation_txtFinalDate', value: moment(dataItem ? dataItem.FinalDate : new Date).format('DD/MM/YYYY') });

            $('#np_vacation_form').data("formValidation").resetForm();
            $('#np_vacation_drpVacationCaptureType').focus();
        },

        Delete: function (obj) {

            let params = UX.Cotorra.Vacation.UI.Params;

            //Show modal
            UX.Modals.Confirm('Eliminar las vacaciones', '¿Deseas eliminar las vacaciones?', 'Sí, eliminar', 'No, espera',
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
                                if (UX.Cotorra.Vacation.EditModel.get('ID') === row.dataItem.ID) {
                                    UX.Cotorra.Vacation.UI.Set(null);
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

        ValidateDaysOff(value, validator) {
            if (value && value !== "") {
                let initial = UX.Cotorra.Vacation.EditModel.get('InitialDate');
                let final = UX.Cotorra.Vacation.EditModel.get('FinalDate');
                let dates = value.split(',').map(x => { return moment(x, 'DD/MM/YYYY'); });

                var badDatesInitial = dates.find(date => date.isBefore(moment(initial, "DD/MM/YYYY")));
                var badDatesFinal = dates.find(date => date.isAfter(moment(final, "DD/MM/YYYY")));

                if (badDatesInitial || badDatesFinal) {
                    return {
                        valid: false, message: 'Los dias de descanso deben estar dentro del rango de los dias de vacaciones'
                    };
                }
            }
            return true;
        },

        ValidateFinalDate(value, validator) {
            let validation = UX.Common.ValidDateValidator(UX.Cotorra.Vacation.EditModel.get('FinalDate'), validator);
            if (validation.valid !== undefined) {
                return validation;
            } else {
                let initial = UX.Cotorra.Vacation.EditModel.get('InitialDate');
                let final = UX.Cotorra.Vacation.EditModel.get('FinalDate');
                if (moment(final, "DD/MM/YYYY").isBefore(moment(initial, "DD/MM/YYYY"))) {
                    return {
                        valid: false, message: 'La fecha final no puede ser menor que la inicial'
                    };
                }
                return true;
            }
        }
    }
};
