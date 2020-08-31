'use strict';

UX.Cotorra.SettlementProcess = {
    Get: (data, action = 'Get', catalog = 'SettlementProcess', onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'GET',
            url: CotorraAppURL + '/' + catalog + '/' + action + '/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    GetTotalDays: function (obj, override) {

        if (obj.ID == 1) {
            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }
            return UX.Cotorra.SettlementProcess.CalculateSalary();
        }
        if (obj.ID == 2) {
            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }
            return UX.Cotorra.SettlementProcess.CalculateVacationOnTime();

        }
        if (obj.ID == 3) {
            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }

            return UX.Cotorra.SettlementProcess.CalculatePendingVacation();

        }
        if (obj.ID == 4) {

            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }

            return UX.Cotorra.SettlementProcess.CalculateProportionalYearBonus();

        }
        if (obj.ID == 5) {
            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }

            return UX.Cotorra.SettlementProcess.CalculateHolidayBonus();


        }
        if (obj.ID == 6) {
            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }

            return UX.Cotorra.SettlementProcess.Calculate90Days();


        }
        if (obj.ID == 7) {
            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }

            return UX.Cotorra.SettlementProcess.Calculate20Days();

        }
        if (obj.ID == 8) {
            if (!override) {
                let ds = UX.Cotorra.SettlementProcess.GetDataFromGrid(obj);
                if (ds) {
                    return ds.TotalDays;
                }
            }

            return UX.Cotorra.SettlementProcess.SeniorityBonus();

        }

        return 15;
    },

    GetDataFromGrid: (obj) => {
        let grid = $(".np-settlement-grid").data('kendoGrid');
        if (!grid) {
            return grid;
        }

        let ds = $(".np-settlement-grid").data('kendoGrid').dataSource._data.map(x => {
            return {
                ID: x.ID,
                Code: x.Code,
                TotalDays: x.TotalDays,
                Apply: x.Apply,
            }
        }).find(el => el.ID === obj.ID);
        return ds;
    },

    Causes: [
        { value: 1, Name: 'Separación voluntaria', },
        { value: 2, Name: 'Término de contrato', },
        { value: 3, Name: 'Abandono de empleo', },
        { value: 4, Name: 'Defunción', },
        { value: 5, Name: 'Clausura', },
        { value: 6, Name: 'Otra', },
        { value: 7, Name: 'Ausentismo', },
        { value: 8, Name: 'Recisión de contrato', },
        { value: 9, Name: 'Jubilación', },
        { value: 10, Name: 'Pensión', },
        { value: 11, Name: 'Incapacidad', }
    ],

    DefaultSettlementDataItems: [
        {
            ID: '1',
            Code: 1,
            Description: 'Sueldo',
            Causes: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11],
            TotalDays: function (override) { return UX.Cotorra.SettlementProcess.GetTotalDays(this, override) },
        },
        {
            ID: '3',
            Code: 21,
            Description: 'Vac. pendientes',
            Causes: [1, 2, 4, 5, 6, 8, 9, 10, 11],
            TotalDays: function (override) { return UX.Cotorra.SettlementProcess.GetTotalDays(this, override) },
        },
        {
            ID: '4',
            Code: 24,
            Description: 'Aguinaldo',
            Causes: [1, 2, 4, 5, 6, 8, 9, 10, 11],
            TotalDays: function (override) { return UX.Cotorra.SettlementProcess.GetTotalDays(this, override) },
        },
        {
            ID: '5',
            Code: 22,
            Description: 'Prima vacacional pendiente',
            Causes: [1, 2, 4, 5, 6, 8, 9, 10, 11],
            TotalDays: function (override) { return UX.Cotorra.SettlementProcess.GetTotalDays(this, override) },
        },
        {
            ID: '6',
            Code: 27,
            Description: 'Indemnización 90 días',
            Causes: [2, 4, 5, 6, 8, 9, 10, 11],
            TotalDays: function (override) { return UX.Cotorra.SettlementProcess.GetTotalDays(this, override) },
        },
        {
            ID: '7',
            Code: 26,
            Description: '20 días por año',
            Causes: [2, 4, 5, 6, 8, 9, 10, 11],
            TotalDays: function (override) { return UX.Cotorra.SettlementProcess.GetTotalDays(this, override) },
        },
        {
            ID: '8',
            Code: 29,
            Description: 'Prima de antigüedad',
            Causes: [1, 2, 4, 5, 6, 8, 9, 10, 11],
            TotalDays: function (override) { return UX.Cotorra.SettlementProcess.GetTotalDays(this, override) },
        }
    ],

    CalculateVacationOnTime: () => {
        let daysAfterLastAniversary = 0;
        let inMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('EmployeeEntryDate'), 'YYYY-MM-DD');
        let outMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementEmployeeSeparationDate'), 'DD/MM/YYYY');
        let aniversary = moment([outMoment.year(), inMoment.month(), inMoment.day()]);
        if (outMoment.isAfter(aniversary)) {
            daysAfterLastAniversary = outMoment.diff(aniversary, 'days') + 1;
        }
        else {
            let newAniversary = moment([outMoment.year() - 1, inMoment.month(), inMoment.day()]);
            daysAfterLastAniversary = outMoment.diff(newAniversary, 'days') + 1;
        }

        if (outMoment.isValid() && inMoment.isValid()) {

            let diffYears = ((outMoment.diff(inMoment, 'days') + 1) / 365);
            let years = Math.round(diffYears)

            let benefitTypeName = UX.Cotorra.SettlementProcess.UI.EditModel.get('BenefitType') == 2 ? 'Personalizada' : 'De ley';
            let benefitType = UX.Cotorra.Catalogs.BenefitTypes[0][0].Name == benefitTypeName ? 0 : 1;
            let Holidays = UX.Cotorra.Catalogs.BenefitTypes[benefitType][years + 1].Holidays;

            let factor = Holidays / 365;
            let totalDays = factor * daysAfterLastAniversary;
            return totalDays
        }
        return 0;
    },

    CalculateSalary: () => {

        let outMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementEmployeeSeparationDate'), 'DD/MM/YYYY');
        let initialPeriodDate = UX.Cotorra.SettlementProcess.UI.EditModel.get('InitialPeriodDate');
        let totalDays = outMoment.diff(initialPeriodDate, 'days') + 1;
        return totalDays
    },

    CalculatePendingVacation: () => {
        return 0;
    },

    Calculate90Days: () => {
        return 90;
    },

    CalculateProportionalYearBonus: () => {

        let outMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementEmployeeSeparationDate'), 'DD/MM/YYYY');
        let diffYears = '-';
        if (outMoment.isValid()) {
            let inMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('EmployeeEntryDate'), 'YYYY-MM-DD');
            if (inMoment.isValid()) {
                diffYears = ((outMoment.diff(inMoment, 'days') + 1) / 365);
                let years = Math.round(diffYears)
                let DaysOfChristmasBonus = 0;
                let benefitTypeName = UX.Cotorra.SettlementProcess.UI.EditModel.get('BenefitType') == 2 ? 'Personalizada' : 'De ley';
                let benefitType = UX.Cotorra.Catalogs.BenefitTypes[0][0].Name == benefitTypeName ? 0 : 1;
                let benefit = UX.Cotorra.Catalogs.BenefitTypes[benefitType][years + 1];
                if (benefit) {
                    DaysOfChristmasBonus = UX.Cotorra.Catalogs.BenefitTypes[benefitType][years + 1].DaysOfChristmasBonus;

                }
                return outMoment.dayOfYear() / 365 * DaysOfChristmasBonus;
            }
        }
        return 0;
    },

    CalculateHolidayBonus: () => {
        return 0;
    },

    Calculate20Days: () => {

        let outMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementEmployeeSeparationDate'), 'DD/MM/YYYY');
        let diffDays = '-';
        if (outMoment.isValid()) {
            let inMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('EmployeeEntryDate'), 'YYYY-MM-DD');
            if (inMoment.isValid()) {
                diffDays = ((outMoment.diff(inMoment, 'days') + 1) / 365).toFixed(3);
            }
        }
        let seniorityBonus = isNaN(diffDays) ? 0 : Math.round(diffDays) * 20;
        return seniorityBonus;

    },

    SeniorityBonus: () => {

        let outMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementEmployeeSeparationDate'), 'DD/MM/YYYY');
        let diffDays = '-';
        if (outMoment.isValid()) {
            let inMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('EmployeeEntryDate'), 'YYYY-MM-DD');
            if (inMoment.isValid()) {
                diffDays = ((outMoment.diff(inMoment, 'days') + 1) / 365).toFixed(3);
            }
        }

        let seniorityBonus = isNaN(diffDays) ? 0 : Math.round(diffDays) * 12;
        return seniorityBonus;
    },


    UI: {
        PeriodTypesOptions: [],
        SettlementModel: {},
        Header: {
            PeriodTypeID: null,
            PeriodID: null,
            PeriodDetailID: null,
        },

        RemoveDetailsFromOrdinary(obj = {}) {

            let ordinaryOverDetails = obj.Ordinary.OverdraftDetails;

            for (var i = 0; i < ordinaryOverDetails.length; i++) {
                let odd = ordinaryOverDetails[i];
                let cp = UX.Cotorra.Catalogs.Concepts.find(x => { return x.ID == odd.ConceptPaymentID });
                if (cp) {
                    odd.ConceptPaymentName = cp.Name;
                    odd.ConceptPaymentCode = cp.Code;
                    odd.ConceptPaymentType = cp.ConceptType;
                    odd.ConceptPaymentKind = cp.Kind;
                    odd.ConceptPaymentPrint = cp.Print;
                    odd.SATGroupCode = cp.SATGroupCode;
                }
            }
            let detailsToMove = ordinaryOverDetails.filter(element => element.ConceptPaymentCode === 101 || element.SATGroupCode === "P-022" ||
                element.SATGroupCode === "P-023" || element.SATGroupCode === "P-025")

            detailsToMove.forEach(detail => {

                let indexToDelete = ordinaryOverDetails.findIndex(el => el.ID === detail.ID);
                ordinaryOverDetails.splice(indexToDelete, 1);
            });
        },

        InitModalApply: async (obj = {}) => {

            let ordinaryOver = obj.Ordinary;
            let indemnization = obj.Indemnization;
            UX.Cotorra.SettlementProcess.UI.RemoveDetailsFromOrdinary(obj);
            let modalID = UX.Cotorra.SettlementProcess.UI.ModalApplyID = UX.Modals.OpenModal(
                'Separación de finiquito e indemnización',
                'l', '<div id="np-settlement-process-modal-apply"></div>',
                function () {
                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');
                    let editModel = UX.Cotorra.SettlementProcess.UI.ApplySettlementModel = new Ractive({
                        el: '#np-settlement-process-modal-apply',
                        template: '#np-settlement-process-modal-apply-template',
                        data: {
                            EmployeeID: UX.Cotorra.SettlementProcess.UI.EditModel.get('EmployeeID'),
                            PeriodDetailID: UX.Cotorra.SettlementProcess.UI.EditModel.get('PeriodDetailID'),
                            OrdinaryOver: ordinaryOver,
                            IndemnizationOver: indemnization,
                            ShowApply: true,
                            OrdinaryPerceptions: '$10.00',
                            OrdinaryDeductions: '$10.00',
                            OrdinaryNet: '$10.00',
                            IndemnizationPerceptions: '$10.00',
                            IndemnizationDeductions: '$10.00',
                            IndemnizationNet: '$10.00',
                        }
                    });

                    (async function () {

                        //Load data
                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#' + modalID,
                                removeWhenFinish: true
                            },
                            requests: [],
                            catalogs: ['Concepts'],
                            forceLoad: false
                        });


                        for (var i = 0; i < ordinaryOver.OverdraftDetails.length; i++) {
                            let odd = ordinaryOver.OverdraftDetails[i];
                            let cp = UX.Cotorra.Catalogs.Concepts.find(x => { return x.ID == odd.ConceptPaymentID });
                            if (cp) {
                                odd.ConceptPaymentName = cp.Name;
                                odd.ConceptPaymentCode = cp.Code;
                                odd.ConceptPaymentType = cp.ConceptType;
                                odd.ConceptPaymentKind = cp.Kind;
                                odd.ConceptPaymentPrint = cp.Print;
                                odd.SATGroupCode = cp.SATGroupCode;
                            }
                        }

                        for (var i = 0; i < indemnization.OverdraftDetails.length; i++) {
                            let odd = indemnization.OverdraftDetails[i];
                            let cp = UX.Cotorra.Catalogs.Concepts.find(x => { return x.ID == odd.ConceptPaymentID });
                            if (cp) {
                                odd.ConceptPaymentName = cp.Name;
                                odd.ConceptPaymentCode = cp.Code;
                                odd.ConceptPaymentType = cp.ConceptType;
                                odd.ConceptPaymentKind = cp.Kind;
                                odd.ConceptPaymentPrint = cp.Print;
                                odd.SATGroupCode = cp.SATGroupCode;
                            }
                        }

                        UX.Cotorra.SettlementProcess.UI.LoadGridOrdinaryIndemnizationSettlementConcepts(ordinaryOver.OverdraftDetails, 'ordinary');
                        UX.Cotorra.SettlementProcess.UI.LoadGridOrdinaryIndemnizationSettlementConcepts(indemnization.OverdraftDetails, 'indemnization');
                    })();
                    UX.Loaders.SetLoader('#' + modalID);

                    //Set validations
                    $('#np-settlement-process-modal-apply').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                        },
                        onSuccess: function (ev) {
                            UX.Cotorra.SettlementProcess.UI.SaveConceptsModificationsAndApplySettlement(ev);
                        }
                    }).on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    UX.Loaders.RemoveLoader('#' + modalID);


                }
            );
        },

        InitModal: async (row = {}) => {
            UX.Cotorra.SettlementProcess.UI.Row = row;
            let modalID = UX.Cotorra.SettlementProcess.UI.ModalID = UX.Modals.OpenModal(
                'Finiquito del colaborador ',
                'l', '<div id="np-settlement-process-modal"></div>',
                function () {
                    $('#' + modalID + ' .title label').append('&nbsp;<span class="modal-title-editing"></span>');
                    let editModel = UX.Cotorra.SettlementProcess.UI.EditModel = new Ractive({
                        el: '#np-settlement-process-modal',
                        template: '#np-settlement-process-modal-template',
                        data: {
                            EmployeeID: '',
                            EmployeeName: '',
                            EmployeeCode: '',
                            EmployeeSalary: '',
                            EmployeeContractType: '',
                            EmployeeJobPositionName: '',
                            EmployeeEntryDate: '',
                            SettlementBaseSalary: '',
                            SetllementCauses: UX.Cotorra.SettlementProcess.Causes,
                            SettlementCause: 1,
                            SettlementEmployeeSeparationDate: "",
                            Seniority: 0,
                            DaysPassedInYear: 0,
                            MinimumSalaryEmpoyeeZone: 0,
                            SMDF: 0,
                            CompleteISRYears: 0,
                            ISRoSUBSDirectCalculus: true,
                            ApplyEmployeeSubsidyInISRUSMOCalculus: true,
                            SettlementItems: UX.Cotorra.SettlementProcess.DefaultSettlementDataItems,
                            InitialPeriodDate: '',
                            FinalPeriodDate: '',
                            PeriodDetailID: '',
                            OverdraftID: '',
                            IndemnizationOverdraftID: '',
                            BenefitType: 0,
                            EmployeeEntryDateFormated: '',
                            OrdinaryOverdraft: '',
                            IndemnizationOverdraft: '',
                            HasBeenSettle: false,
                        }
                    });
                    editModel.observe('SettlementCause',
                        function (newValue, oldValue) {
                            let data = UX.Cotorra.SettlementProcess.DefaultSettlementDataItems
                                .map(x => {
                                    return {
                                        ID: x.ID,
                                        Description: x.Description,
                                        Apply: x.Causes.includes(newValue),
                                        TotalDays: x.TotalDays(false),
                                        Code: x.Code,
                                    }
                                });

                            UX.Cotorra.SettlementProcess.UI.LoadGridCauses(data);

                        },
                        { init: false, defer: true });


                    editModel.observe('SettlementEmployeeSeparationDate',
                        function (newValue, oldValue) {
                            UX.Cotorra.SettlementProcess.UI.OnChangeSettlementEmployeeSeparationDate();
                            let data = UX.Cotorra.SettlementProcess.DefaultSettlementDataItems
                                .map(x => {
                                    return {
                                        ID: x.ID,
                                        Description: x.Description,
                                        Apply: x.Causes.includes(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementCause')),
                                        TotalDays: x.TotalDays(true),
                                        Code: x.Code,
                                    }
                                });

                            UX.Cotorra.SettlementProcess.UI.LoadGridCauses(data);
                        },
                        { init: false, defer: true });


                    //Get necessary catalogs
                    (async function () {

                        //Get employee
                        let getEmployeeRequest = UX.Cotorra.Catalogs.GetByID('Employees', { ID: row.dataItem.ID },
                            (data) => {

                                let contractType = UX.Cotorra.ContractTypes.find(el => el.ID == data.ContractType).Name;
                                let position = UX.Cotorra.Catalogs.Positions.find(el => el.ID == data.JobPositionID).Name;
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeID', data.ID);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeName', data.FullName);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeCode', data.Code);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeSalary', UX.Cotorra.Common.FormatCurrency(data.DailySalary));
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeContractType', contractType);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeJobPositionName', position);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeEntryDate', data.EntryDate);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeEntryDateFormated', moment(data.EntryDate).format('DD/MM/YYYY'));
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('SettlementBaseSalary', data.DailySalary);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('BenefitType', data.BenefitType);
                                UX.Cotorra.SettlementProcess.UI.OnChangeSettlementEmployeeSeparationDate();

                            },
                            (error) => { UX.Common.ErrorModal(error); },
                            (complete) => { }
                        );

                        //Get Period
                        let getPeriod = UX.Cotorra.Catalogs.Do('GET', 'Periods', 'GetActualDetail', { periodTypeID: row.dataItem.PeriodTypeID },
                            (data) => {
                                let initialMoment = moment(data ? data.InitialDate : new Date);
                                let initialDate = initialMoment.format('DD/MM/YYYY');
                                let finalMoment = moment(data ? data.FinalDate : new Date);
                                let finalDate = finalMoment.format('DD/MM/YYYY');

                                let ID = data ? data.ID : '';

                                UX.Cotorra.SettlementProcess.UI.EditModel.set('InitialPeriodDate', initialMoment);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('PeriodDetailID', ID);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('SettlementEmployeeSeparationDate', finalDate);
                                UX.Cotorra.SettlementProcess.UI.EditModel.set('FinalPeriodDate', finalMoment);

                                $('.modal-title-editing').html('periodo del ' + initialDate + ' al  '
                                    + finalDate);
                            },
                            (error) => {
                                UX.Common.ErrorModal(error);
                            },
                            (complete) => {

                            }
                        );

                        //Load data
                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#' + modalID,
                                removeWhenFinish: true
                            },
                            requests: [getEmployeeRequest, getPeriod],
                            catalogs: ['Concepts', 'Positions'],
                            forceLoad: true
                        });


                        let data = UX.Cotorra.SettlementProcess.DefaultSettlementDataItems
                            .map(x => {
                                return {
                                    ID: x.ID,
                                    Description: x.Description,
                                    Apply: x.Causes.includes(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementCause')),
                                    TotalDays: x.TotalDays(true),
                                    Code: x.Code,
                                }
                            });

                        UX.Cotorra.SettlementProcess.UI.LoadGridCauses(data);

                    })();

                    UX.Loaders.SetLoader('#' + modalID);

                    //Set validations
                    $('#np-settlement-process-modal').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_sett_txtEntryDate: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la fecha de baja' },
                                    callback: {
                                        callback: function (value, validator) {
                                            let validation = UX.Common.ValidDateValidator(value, validator);
                                            if (validation.valid !== undefined) {
                                                return validation;
                                            } else {

                                                let SettlementEmployeeSeparationDate = editModel.get('SettlementEmployeeSeparationDate');
                                                let initial = editModel.get('InitialPeriodDate');
                                                let final = editModel.get('FinalPeriodDate');
                                                if (moment(SettlementEmployeeSeparationDate, "DD/MM/YYYY").isBefore(moment(initial, "DD/MM/YYYY"))) {
                                                    return {
                                                        valid: false, message: 'La fecha de baja debe estar dentro del periodo'
                                                    };
                                                }
                                                if (moment(SettlementEmployeeSeparationDate, "DD/MM/YYYY").isAfter(moment(final, "DD/MM/YYYY"))) {
                                                    return {
                                                        valid: false, message: 'La fecha de baja debe estar dentro del periodo'
                                                    };
                                                }
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        onSuccess: function (ev) {

                            UX.Cotorra.SettlementProcess.UI.ProcessCalculation(ev);

                        }
                    }).on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    $('#np_sett_txtEntryDate').mask('00/00/0000');
                    $('#np_sett_baseSalary').decimalMask(2);
                    $('#np_sett_txtISRAntiquity').decimalMask(2);

                },
                function () {
                    UX.Cotorra.SettlementProcess.UI.ProcessCloseSettlementProcess(UX.Cotorra.SettlementProcess.UI.EditModel.get());
                    return true;
                });

        },

        ProcessCloseSettlementProcess: (data) => {

            if (data.HasBeenSettle == false && (data.OverdraftID !== '' || data.IndemnizationOverdraftID !== '')) {

                let parameters = [data.OverdraftID, data.IndemnizationOverdraftID].filter(el => el !== '');
                UX.Cotorra.Catalogs.Do('POST', 'Overdraft', 'DeleteOverdrafts', { Ids: parameters },
                    (data) => {
                    },
                    (error) => {
                        UX.Common.ErrorModal(error);
                    },
                    (complete) => {
                        UX.Loaders.RemoveLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
                    }
                );
            }
        },

        LoadGridCauses: (data) => {
            //Set fields
            let fields = {
                ID: { type: 'string' },
                Description: { type: 'string' },
                Apply: { type: 'bool' },
                TotalDays: { type: 'number' },
                Code: { type: 'number' },
            };

            //Set columns
            let columns = [
                {
                    field: 'Description', title: 'Descripción', width: 150,
                },
                {
                    field: 'Apply', title: 'Aplica', width: 80,
                    template: kendo.template($('#settlementApplyTemplate').html()),
                },
                {
                    field: 'TotalDays', title: 'Días totales', width: 150,
                    template: kendo.template($('#settlementDaysTemplate').html()),

                },
                {
                    field: 'Code', title: 'Code', width: 150,
                    hidden: true
                },
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-settlement-grid', data: data, fields: fields, columns: columns, pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
            });
        },

        LoadGridOrdinaryIndemnizationSettlementConcepts: (data, type) => {

            let filtered = data.filter(el => el.ConceptPaymentType !== 2)
            UX.Cotorra.SettlementProcess.UI.SetTotals({ Details: filtered, Type: type });
            //Set fields
            let fields = {
                ID: { type: 'string' },
                SATGroupCode: { type: 'string' },
                ConceptPaymentCode: { type: 'number' },
                ConceptPaymentName: { type: 'string' },
                Value: { type: 'number' },
                Amount: { type: 'number' },
            };

            //Set columns
            let columns = [
                {
                    field: 'SATGroupCode', title: 'Clave SAT', width: 60,
                },
                {
                    field: 'ConceptPaymentCode', title: 'Número', width: 45,
                },
                {
                    field: 'ConceptPaymentName', title: 'Concepto', width: 140,
                },
                {
                    field: 'Amount', title: 'Importe', width: 100,
                    template: UX.Cotorra.Common.GridFormatCurrency('Amount'),
                },
            ];
            let gridSelector = '.np-settlement-apply-' + type + '-grid';
            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: gridSelector, data: filtered, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
                resizable: false, selectable: "multiple, row",
            });
        },

        SetTotals: (data) => {
            let concepts = data.Details;
            let type = data.Type;
            let perceptions = 0;
            let deductions = 0;
            let net = 0;
            let salariesDetails = concepts.filter(el => el.ConceptPaymentType === 1 && el.ConceptPaymentPrint === true && el.ConceptPaymentKind === false);
            let deductionsDetails = concepts.filter(el => el.ConceptPaymentType === 3 && el.ConceptPaymentPrint === true && el.ConceptPaymentKind === false);

            salariesDetails.forEach(salary => {
                perceptions += salary.Amount;
            });

            deductionsDetails.forEach(salary => {
                deductions += salary.Amount;
            });

            net = perceptions - deductions;

            if (type === 'ordinary') {
                UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.set('OrdinaryPerceptions', UX.Cotorra.Common.FormatCurrency(perceptions));
                UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.set('OrdinaryDeductions', UX.Cotorra.Common.FormatCurrency(deductions));
                UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.set('OrdinaryNet', UX.Cotorra.Common.FormatCurrency(net));
            }
            if (type === 'indemnization') {
                UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.set('IndemnizationPerceptions', UX.Cotorra.Common.FormatCurrency(perceptions));
                UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.set('IndemnizationDeductions', UX.Cotorra.Common.FormatCurrency(deductions));
                UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.set('IndemnizationNet', UX.Cotorra.Common.FormatCurrency(net));
            }

        },


        OnChangeSettlementEmployeeSeparationDate: () => {

            let outMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('SettlementEmployeeSeparationDate'), 'DD/MM/YYYY');
            let diffDays = '-';
            let daysOfYear = '-';
            if (outMoment.isValid()) {
                let inMoment = moment(UX.Cotorra.SettlementProcess.UI.EditModel.get('EmployeeEntryDate'), 'YYYY-MM-DD');
                if (inMoment.isValid()) {
                    diffDays = ((outMoment.diff(inMoment, 'days') + 1) / 365).toFixed(3);
                    daysOfYear = outMoment.dayOfYear();
                }
            }
            UX.Cotorra.SettlementProcess.UI.EditModel.set('DaysPassedInYear', daysOfYear);
            UX.Cotorra.SettlementProcess.UI.EditModel.set('Seniority', diffDays);
            UX.Cotorra.SettlementProcess.UI.EditModel.set('CompleteISRYears', isNaN(diffDays) ? diffDays : Math.round(diffDays));
        },

        CalculateDays: (totalDays) => {
            return totalDays
        },

        CallModalApply: () => {

            let ordinary = UX.Cotorra.SettlementProcess.UI.EditModel.get('OrdinaryOverdraft');
            let indemnization = UX.Cotorra.SettlementProcess.UI.EditModel.get('IndemnizationOverdraft');
            UX.Cotorra.SettlementProcess.UI.InitModalApply({ Ordinary: ordinary, Indemnization: indemnization });
        },


        ApplyConcept: function (data) {
            let row = UX.Cotorra.Common.GetRowData(data);
            row.dataItem.Apply = $(data).is(':checked');
        },

        EditDays: function (obj) {
            //get data
            let row = UX.Cotorra.Common.GetRowData(obj);
            let $div = $(obj).closest('div');
            let $input = $('<input class="od-input-edit-amount" type="text" />');
            let oldValue = row.dataItem.TotalDays;

            $div.append($input).find('a').hide();

            //input events
            $input.off('focus').on('focus', function () { setTimeout(function () { $input.select(); }, 50); });

            $input.off('blur').on('blur', function () {
                let newValue = $input.val() === '' ? null : parseFloat($input.val().trim().replace(/,/g, ''));
                if (newValue == oldValue) {
                    UX.Common.KendoFastRedrawRow(row.$kendoGrid, row.dataItem);
                }
                else {
                    if (newValue === null) {
                        row.dataItem.TotalDays = 0;
                    } else {
                        row.dataItem.TotalDays = newValue;
                    }
                    UX.Common.KendoFastRedrawRow(row.$kendoGrid, row.dataItem);
                }
            });

            $input.off('keypress').on('keypress', function (ev) {
                if (ev.key == "Enter") {
                    $input.blur();
                }
            });

            $input.val(row.dataItem.TotalDays).decimalMask(2);
            $input.focus();
        },

        EventCalculation: function (obj) {
            $('#np-settlement-process-modal').data('formValidation').validate();
        },


        EventApplySettlement: function (obj) {
            $('#np-settlement-process-modal-apply').data('formValidation').validate();
        },

        ProcessCalculation: function (data) {
            UX.Loaders.SetLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
            let editModel = UX.Cotorra.SettlementProcess.UI.EditModel;
            let row = UX.Cotorra.SettlementProcess.UI.Row;

            let ds = $(".np-settlement-grid").data('kendoGrid').dataSource._data.map(x => {
                return {
                    ID: x.ID,
                    Code: x.Code,
                    TotalDays: x.TotalDays,
                    Apply: x.Apply,
                }
            });

            UX.Cotorra.Catalogs.Do('POST', 'SettlementProcess', 'Calculate',
                {
                    Data: {
                        EmployeeID: editModel.get('EmployeeID'),
                        SettlementBaseSalary: editModel.get('SettlementBaseSalary'),
                        SettlementEmployeeSeparationDate: editModel.get('SettlementEmployeeSeparationDate'),
                        SettlementCause: editModel.get('SettlementCause'),
                        CompleteISRYears: editModel.get('CompleteISRYears'),
                        ConceptsToApply: ds,
                        PeriodDetailID: editModel.get('PeriodDetailID'),
                    }
                },
                (data) => {

                    let ordinarySettlementOverdraft = data.find(el => el.OverdraftType === 3);
                    let indemnizationSettlementOverdraft = data.find(el => el.OverdraftType === 4);

                    editModel.set('OverdraftID', ordinarySettlementOverdraft.ID);

                    editModel.set('OrdinaryOverdraft', ordinarySettlementOverdraft);


                    if (indemnizationSettlementOverdraft) {
                        editModel.set('IndemnizationOverdraftID', indemnizationSettlementOverdraft.ID);
                        editModel.set('IndemnizationOverdraft', indemnizationSettlementOverdraft);
                    }

                    UX.Cotorra.Overdraft.UI.Init({
                        Employee: {
                            ID: row.dataItem.ID,
                            Name: row.dataItem.FullName,
                            Code: row.dataItem.Code,
                            JobPositionName: editModel.get('EmployeeJobPositionName'),
                            Salary: UX.Cotorra.Common.FormatCurrency(row.dataItem.DailySalary)
                        },
                        Overdraft: {
                            ID: ordinarySettlementOverdraft.ID,
                            OverdraftType: 3
                        },
                        WorkPeriod: {
                            Period: "Finiquito",
                        },
                        PrePayrollHeader: {
                            InitialDate: editModel.get('InitialPeriodDate')._i,
                            FinalDate: editModel.get('FinalPeriodDate')._i,
                        },
                        Row: row
                    });
                },
                (error) => {
                    UX.Common.ErrorModal(error);
                },
                (complete) => {
                    UX.Loaders.RemoveLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
                }
            );
        },

        DownloadSettlementLetter: (obj = {}) => {
            UX.Loaders.SetLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
            let editModel = UX.Cotorra.SettlementProcess.UI.EditModel;
            UX.Cotorra.Catalogs.Do('POST', 'SettlementProcess', 'GenerateSettlementLetter',
                {
                    overdraftID: editModel.get('OverdraftID'),
                },
                (data) => {
                    window.open(data, 'Download');
                },
                (error) => {
                    UX.Common.ErrorModal(error);
                },
                (complete) => {
                    UX.Loaders.RemoveLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
                });

        },

        AppllySettlement: (obj = {}) => {

            UX.Modals.Confirm('Finiquitar colaborador', '¿Deseas finiquitar al colaborador?', 'Sí, finiquitar', 'No, espera',
                () => {
                    let editModel = UX.Cotorra.SettlementProcess.UI.EditModel;
                    let ordinaryOver = editModel.get('OrdinaryOverdraft');
                    let ordinaryOverDetailsIDs = editModel.get('OrdinaryOverdraft.OverdraftDetails').map(a => a.ID)

                    UX.Cotorra.Catalogs.Do('POST', 'SettlementProcess', 'ApplySettlement',
                        {
                            EmployeeID: editModel.get('EmployeeID'),
                            SettlementEmployeeSeparationDate: editModel.get('SettlementEmployeeSeparationDate'),
                            PeriodDetailID: editModel.get('PeriodDetailID'),
                            ChangeOverdrafts: false,
                            OrdinaryID: ordinaryOver.ID,
                            OrdinaryOverDetailsIDs: ordinaryOverDetailsIDs,
                        },
                        (data) => {
                            UX.Cotorra.SettlementProcess.UI.EditModel.set('HasBeenSettle', true);
                            UX.Modals.Alert('Exito', 'El colaborador ha sido finiquitado, revisa sus recibos de nómina en la opción Mis Nóminas y emítelos desde ahí',
                                'm', 'error', () => { });
                        },
                        (error) => {
                            UX.Common.ErrorModal(error);
                        },
                        (complete) => {
                            UX.Loaders.RemoveLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
                        });
                },
                () => {
                });
        },

        AppllySettlementOrCheckConcepts: (obj = {}) => {
            UX.Loaders.SetLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
            let editModel = UX.Cotorra.SettlementProcess.UI.EditModel;

            if (editModel.get('IndemnizationOverdraftID') !== '') {
                UX.Cotorra.SettlementProcess.UI.CallModalApply()
            }
            else {
                //Validar total sobrerecibo es positivo
                UX.Cotorra.SettlementProcess.UI.AppllySettlement();
            }
            UX.Loaders.RemoveLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalID);
        },

        SaveConceptsModificationsAndApplySettlement: (obj = {}) => {

            UX.Modals.Confirm('Finiquitar colaborador', '¿Deseas finiquitar al colaborador?', 'Sí, finiquitar', 'No, espera',
                () => {
                    UX.Loaders.SetLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalApplyID);
                    let editModel = UX.Cotorra.SettlementProcess.UI.ApplySettlementModel;
                    let indemnizationOver = editModel.get('IndemnizationOver');
                    let ordinaryOver = editModel.get('OrdinaryOver');
                    let ordinaryOverDetailsIDs = editModel.get('OrdinaryOver.OverdraftDetails').map(a => a.ID)

                    let indemnizationOverDetailsIDs = editModel.get('IndemnizationOver.OverdraftDetails').map(a => a.ID)

                    UX.Cotorra.Catalogs.Do('POST', 'SettlementProcess', 'ApplySettlement',
                        {
                            EmployeeID: editModel.get('EmployeeID'),
                            SettlementEmployeeSeparationDate: editModel.get('SettlementEmployeeSeparationDate'),
                            PeriodDetailID: editModel.get('PeriodDetailID'),
                            ChangeOverdrafts: true,
                            OrdinaryID: ordinaryOver.ID,
                            IndemnizationOverID: indemnizationOver.ID,
                            OrdinaryOverDetailsIDs: ordinaryOverDetailsIDs,
                            IndemnizationOverDetailsIDs: indemnizationOverDetailsIDs
                        },
                        (data) => {
                            UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.set('ShowApply', false);
                            UX.Cotorra.SettlementProcess.UI.EditModel.set('HasBeenSettle', true);
                            UX.Modals.Alert('Exito', 'El colaborador ha sido finiquitado, revisa sus recibos de nómina en la opción Mis Nóminas y emítelos desde ahí',
                                'm', 'error', () => { });
                        },
                        (error) => {
                            UX.Common.ErrorModal(error);
                        },
                        (complete) => {
                            UX.Loaders.RemoveLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalApplyID);
                        });
                },
                () => {
                    UX.Loaders.RemoveLoader('#' + UX.Cotorra.SettlementProcess.UI.ModalApplyID);
                });

        },

        GetActualPeriodDetail: (data, modalID) => {


            UX.Cotorra.Catalogs.Do('GET', 'Periods', 'GetActualDetail', data,
                (data) => {
                    let initialMoment = moment(data[0] ? data[0].InitialDate : new Date);
                    let initialDate = initialMoment.format('DD/MM/YYYY');
                    let finalMoment = moment(data[0] ? data[0].FinalDate : new Date);
                    let finalDate = finalMoment.format('DD/MM/YYYY');

                    let ID = data[0] ? data[0].ID : '';

                    UX.Cotorra.SettlementProcess.UI.EditModel.set('InitialPeriodDate', initialMoment);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('SettlementEmployeeSeparationDate', initialDate);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('PeriodDetailID', ID);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('SettlementEmployeeSeparationDate', finalDate);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('FinalPeriodDate', finalMoment);

                    $('.modal-title-editing').html('periodo del ' + initialDate + ' al  '
                        + finalDate);
                },
                (error) => {
                    UX.Common.ErrorModal(error);
                },
                (complete) => {

                }
            );
        },

        GetEmployee: (data, modalID) => {

            UX.Cotorra.Catalogs.GetByID('Employees', data,
                (data) => {

                    let contractType = UX.Cotorra.ContractTypes.find(el => el.ID == data.ContractType).Name;
                    let position = UX.Cotorra.Catalogs.Positions.find(el => el.ID == data.JobPositionID).Name;
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeID', data.ID);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeName', data.FullName);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeCode', data.Code);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeSalary', UX.Cotorra.Common.FormatCurrency(data.DailySalary));
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeContractType', contractType);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeJobPositionName', position);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('EmployeeEntryDate', data.EntryDate);
                    UX.Cotorra.SettlementProcess.UI.EditModel.set('SettlementBaseSalary', data.DailySalary);
                    UX.Cotorra.SettlementProcess.UI.OnChangeSettlementEmployeeSeparationDate();
                },
                (error) => {
                    UX.Common.ErrorModal(error);
                },
                (complete) => {

                }
            );
        },

        MoveConceptSettlement: (data) => {

            let ordinaryConcepts = UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.get('OrdinaryOver.OverdraftDetails');
            let indemnizationConcepts = UX.Cotorra.SettlementProcess.UI.ApplySettlementModel.get('IndemnizationOver.OverdraftDetails');
            var rows = [];
            var dataItems = [];
            if (data === 'allRight' || data === 'right') {
                var entityGrid = $(".np-settlement-apply-ordinary-grid").data("kendoGrid");
                if (data === 'right') {
                    rows = entityGrid.select();
                    rows.each(function (index, row) {
                        var selectedItem = UX.Cotorra.Common.GetRowData(row).dataItem;
                        dataItems.push(selectedItem);
                    });
                }
                else {
                    dataItems = entityGrid.dataItems()
                }
                dataItems.forEach(function (row, index) {
                    var selectedItem = row;
                    if (selectedItem.ConceptPaymentType === 3 && selectedItem.ConceptPaymentCode !== 101 && selectedItem.SATGroupCode !== 'OP-002' &&
                        selectedItem.SATGroupCode !== 'OP-008' && selectedItem.SATGroupCode !== 'OP-007' && !selectedItem.ConceptPaymentKind) {
                        indemnizationConcepts.push({
                            ID: selectedItem.ID,
                            SATGroupCode: selectedItem.SATGroupCode,
                            ConceptPaymentCode: selectedItem.ConceptPaymentCode,
                            ConceptPaymentName: selectedItem.ConceptPaymentName,
                            Value: selectedItem.Value,
                            Amount: selectedItem.Amount,
                            ConceptPaymentKind: selectedItem.ConceptPaymentKind,
                            ConceptPaymentType: selectedItem.ConceptPaymentType
                        });
                        let indexToDelete = ordinaryConcepts.findIndex(el => el.ID === selectedItem.ID);
                        ordinaryConcepts.splice(indexToDelete, 1);
                    }
                });
            }

            else if (data === 'left' || data === 'allLeft') {
                var entityGrid = $(".np-settlement-apply-indemnization-grid").data("kendoGrid");
                if (data === 'left') {
                    rows = entityGrid.select();
                    rows.each(function (index, row) {
                        var selectedItem = UX.Cotorra.Common.GetRowData(row).dataItem;
                        dataItems.push(selectedItem);
                    });
                }
                else {
                    dataItems = entityGrid.dataItems()
                }
                dataItems.forEach(function (row, index) {
                    var selectedItem = row;
                    if (selectedItem.ConceptPaymentType === 3 && selectedItem.ConceptPaymentCode !== 101 && selectedItem.SATGroupCode !== 'OP002' &&
                        selectedItem.SATGroupCode !== 'OP008' && selectedItem.SATGroupCode !== 'Op007' && !selectedItem.ConceptPaymentKind) {
                        ordinaryConcepts.push({
                            ID: selectedItem.ID,
                            SATGroupCode: selectedItem.SATGroupCode,
                            ConceptPaymentCode: selectedItem.ConceptPaymentCode,
                            ConceptPaymentName: selectedItem.ConceptPaymentName,
                            Value: selectedItem.Value,
                            Amount: selectedItem.Amount,
                            ConceptPaymentKind: selectedItem.ConceptPaymentKind,
                            ConceptPaymentType: selectedItem.ConceptPaymentType,
                        });
                        let indexToDelete = indemnizationConcepts.findIndex(el => el.ID === selectedItem.ID);
                        indemnizationConcepts.splice(indexToDelete, 1);
                    }
                });
            }

            UX.Cotorra.SettlementProcess.UI.LoadGridOrdinaryIndemnizationSettlementConcepts(ordinaryConcepts, 'ordinary');
            UX.Cotorra.SettlementProcess.UI.LoadGridOrdinaryIndemnizationSettlementConcepts(indemnizationConcepts, 'indemnization');

        }
    },
};