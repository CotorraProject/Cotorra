'use strict';

var model = {};
var CotorraAppURL = UX.UrlApp;


UX.CompanyAssistant_cotorra = {

    Initialize: async () => {

        UX.Loaders.SetLoader('.company-selection .content');
        var fiscalRegimes = [];
        await $.ajax({
            url: CotorraAppURL + '/companyassistant/GetFiscalRegimes',
            type: 'GET',
            async: true,
            success: function (fr) {
                fiscalRegimes = fr;
            }
        });

        //PeriodTypeCommonTemplate
        $.get(CotorraAppURL + '/views/common/periodtypemodal.html').done((data) => { $('body').append(data); });

        //UX.Cotorra.EmployerRegistration
        $.getScript(CotorraAppURL + '/assets/js/catalogs/employerregistration.js', () => { }).done(() => { });
        //UX.Cotorra.Common
        $.getScript(CotorraAppURL + '/assets/js/common.js', () => { }).done(() => { });

        //Get the assistant view
        $.ajax({
            url: CotorraAppURL + '/views/companyassistant/index.html',
            type: 'GET',
            async: true,
            success: function (data) {

                UX.Loaders.RemoveLoader('.company-selection .content');

                var fv = null;

                //Check if template exists
                if ($('#companyassistant-template').length > 0) {
                    $('#companyassistant-template').remove();
                    $('.company-assistant').remove();
                }

                //Insert new HTML from company assistant
                $('body').append(data);

                $.ajax({
                    url: CotorraAppURL + '/views/catalogs/employerregistration.html',
                    type: 'GET',
                    async: true,
                    success: function (employerregistration) {

                        //Insert new HTML from employer registration
                        $('body').append(employerregistration);

                        let actualYear = new Date().getFullYear();

                        //Init model
                        model = UX.CompanyAssistant_cotorra.Model = new Ractive({
                            el: '.company-assistant-cell',
                            template: '#companyassistant-template',
                            data: {
                                //Steps
                                step: 1,
                                totalSteps: 4,

                                //Step 1
                                CompanyRFC: '',
                                CompanySocialReason: '',
                                CompanyCURP: '',
                                LicenseServiceID: '',
                                CurrencyID: '57b07610-ed2a-40f5-8093-fa3eaa38b41d',
                                Currencies: [
                                    {
                                        ID: '57b07610-ed2a-40f5-8093-fa3eaa38b41d',
                                        Code: 'MXN', Name: 'Peso mexicano'
                                    }
                                ],
                                SalaryZone: '1',
                                ZipCode: '',
                                FederalEntity: '',
                                Municipality: '',
                                Street: '',
                                ExteriorNumber: '',
                                InteriorNumber: '',
                                Suburb: '',
                                Reference: '',
                                NonDeductibleFactor: '0.53',
                                FiscalRegime: '',

                                //Step 2
                                ID: null,
                                Code: '',
                                RiskClass: '',
                                RiskClassFraction: '',
                                HasIMSS: false,

                                //Step 3
                                CurrentFiscalYear: actualYear,
                                PeriodType: '',
                                InitialDate: '',
                                PeriodTotalDays: '',
                                PaymentDays: '',
                                CurrentPeriod: '',
                                FortnightPaymentDays: 1,
                                WeeklySeventhDay: '',

                                //Step4

                                //Others
                                IsPerson: false,
                                IsBusiness: false,

                                //Options
                                RiskClassOptions: UX.Cotorra.EmployerRegistration.RiskClassOptions,
                                FederalEntityOptions: [{ id: '', description: '- - -' }],
                                MunicipalityOptions: [{ id: '', description: '- - -' }],
                                SuburbOptions: [],
                                FortnightPaymentDaysOptions: [
                                    { id: 1, description: 'Pagar los días de pago' },
                                    { id: 2, description: 'Pagar los días calendario del periodo' }
                                ],
                                WeeklySeventhDayOptions: [
                                    { id: -1, description: 'No tomar en cuenta séptimo día para los días de pago' },
                                    { id: 0, description: 'Lunes' },
                                    { id: 1, description: 'Martes' },
                                    { id: 2, description: 'Miércoles' },
                                    { id: 3, description: 'Jueves' },
                                    { id: 4, description: 'Viernes' },
                                    { id: 5, description: 'Sábado' },
                                    { id: 6, description: 'Domingo' }
                                ],
                                CurrentFiscalYearOptions: [
                                    { id: (actualYear - 1), description: (actualYear - 1).toString() },
                                    { id: (actualYear), description: (actualYear).toString() },
                                    { id: (actualYear + 1), description: (actualYear + 1).toString() },
                                ],
                                CurrentPeriodOptions: [],
                                FiscalRegimesOptions: fiscalRegimes
                            }
                        });


                        //Remove non required elements
                        $('.np-assis-employerregistration .np-er-data2').remove();
                        $('.np-assis-employerregistration .np-er-address1').remove();
                        $('.np-assis-employerregistration .np-er-address2').remove();
                        $('.np-assis-employerregistration .np-er-address3').remove();

                        //Observers
                        model.observe('CompanyRFC', function (newValue) {

                            let rexExpPerson = /^([A-ZÑa-zñ\x26]{4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[A-Za-z|\d]{3})$/;
                            let rexExpBusiness = /^([A-ZÑa-zñ\x26]{3}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[A-Za-z|\d]{3})$/;

                            let isPerson = rexExpPerson.test(newValue);
                            let isBusiness = rexExpBusiness.test(newValue);

                            model.set('IsPerson', isPerson);
                            model.set('IsBusiness', isBusiness);

                        }, { init: true, defer: true });

                        model.observe('step', function (newValue) {

                        }, { init: false, defer: true });

                        model.observe('CurrentFiscalYear',
                            function (newValue, oldValue) {
                                model.set('PeriodType', '');

                                $(fv).data('formValidation').addField('ca_paymentdays');
                                $(fv).data('formValidation').removeField('ca_initialdate');
                                $(fv).data('formValidation').addField('ca_initialdate',
                                    {
                                        validators: {
                                            notEmpty: { message: 'Debes ingresar la fecha inicial' },
                                            callback: { callback: UX.Common.ValidDateValidator },
                                            date: {
                                                format: 'DD/MM/YYYY',
                                                message:
                                                    'La fecha inicial debe ser entre ' +
                                                    '26/12/' + (model.get('CurrentFiscalYear') - 1) +
                                                    ' y ' +
                                                    '07/01/' + (model.get('CurrentFiscalYear')),
                                                min: '26/12/' + (model.get('CurrentFiscalYear') - 1),
                                                max: '07/01/' + (model.get('CurrentFiscalYear'))
                                            }
                                        }
                                    });
                            },
                            { init: false });

                        model.observe('PeriodType',
                            function (newValue, oldValue) {
                                model.set('InitialDate', '01/01/' + model.get('CurrentFiscalYear'));
                                switch (newValue) {
                                    case 'Weekly':
                                        model.set('PeriodTotalDays', 7);
                                        model.set('PaymentDays', 7);
                                        break;
                                    case 'BiWeekly':
                                        model.set('PeriodTotalDays', 15);
                                        model.set('PaymentDays', 15);
                                        break;
                                    case 'Monthly':
                                        model.set('PeriodTotalDays', 30);
                                        model.set('PaymentDays', 30);
                                        break;
                                    default:
                                        model.set('PeriodTotalDays', '');
                                        model.set('PaymentDays', '');
                                        break;
                                }
                                model.set("WeeklySeventhDay", -1);
                            },
                            { init: true });

                        model.observe('HasIMSS', function (newValue) {

                            if (newValue) {
                                $('#np_er_txtCode, #np_er_drpRiskClass, #np_er_txtRiskClassFraction').parent().removeClass('disabled');
                                $('#np_er_txtCode, #np_er_drpRiskClass, #np_er_txtRiskClassFraction').removeAttr('disabled');
                            } else {
                                $('#np_er_txtCode, #np_er_drpRiskClass, #np_er_txtRiskClassFraction').parent().addClass('disabled');
                                $('#np_er_txtCode, #np_er_drpRiskClass, #np_er_txtRiskClassFraction').attr('disabled', 'disabled');
                            }
                        }, { init: true, defer: true });

                        model.observe('WeeklySeventhDay', function (newValue) {
                            model.set('PaymentDays', newValue === -1 ? 7 : 6);
                        }, { init: false, defer: true });

                        var getPeriods = function (newValue) {

                            let periodType = model.get('PeriodType');
                            let initialDate = model.get('InitialDate');
                            let periodTotalDays = model.get('PeriodTotalDays');

                            if (periodType === '' || initialDate === '' || periodTotalDays == '') {
                                model.set("CurrentPeriodOptions", []);
                                return;
                            }

                            let date = UX.Common.ValidateDate(initialDate);
                            if (date === null) {

                                model.set("CurrentPeriodOptions", []);
                                return;
                            } else {
                                initialDate = date;
                                let year = parseInt(initialDate.split('/')[2]);
                                let cfy = model.get('CurrentFiscalYear');

                                if (year !== cfy && year !== (cfy - 1)) {
                                    model.set("CurrentPeriodOptions", []);
                                    return;
                                }
                            }

                            UX.Loaders.SetLoader('.company-assistant-modal');
                            $.ajax({
                                url: CotorraAppURL + '/companyassistant/getperiods',
                                type: 'POST',
                                data: {
                                    paymentPeriodicity: periodType,
                                    initialDate: initialDate,
                                    periodTotalDays: periodTotalDays,
                                },
                                async: true,
                                success: function (periods) {
                                    model.set('CurrentPeriod', 1);
                                    model.set("CurrentPeriodOptions", periods);
                                },
                                error: function (error) { UX.Modals.Alert("ERROR", UX.Common.getMessageFromError(error).message, "m", "error", function () { }); },
                                complete: function () { UX.Loaders.RemoveLoader('.company-assistant-modal'); }
                            });
                        };

                        model.observe('PeriodType', function () { getPeriods(); }, { init: false, defer: true });
                        model.observe('CurrentFiscalYear', function () { getPeriods(); }, { init: false, defer: true });
                        model.observe('InitialDate', function () { getPeriods(); }, { init: false, defer: true });
                        $('#ca_initialdate').off('change').on('change', function () {
                            getPeriods();
                        });

                        //Then delete all from employer registration view
                        $('#employerregistrationActionTemplate').remove();
                        $('.employerregistration-actions').remove();
                        $('.employerregistration-footer').remove();
                        $('#employerregistration_save_template').remove();
                        $('.np-employerregistration-grid').remove();

                        //Show assistant
                        $('.company-assistant').hide().fadeIn();

                        //Set close button behavior
                        $('#btnCloseCompanyAssistantCross').off('click').on('click', function () {
                            $('.company-assistant').fadeOut(200, function () {
                                $('.company-assistant').remove();
                                //$('.company-selection').fadeIn();
                            });
                        });

                        //Masks
                        $('#ca_paymentdays').decimalMask(2);
                        $('#ca_initialdate').mask('00/00/0000');
                        $('#ca_foundationdate').mask('00/00/0000');

                        //Init UI elements
                        $(".company-assistant-modal").initUIElements();

                        //Init data loading
                        UX.Loaders.SetLoader('.company-assistant-modal', "ball-pulse");

                        let licenses = $.ajax({
                            url: "/licensing/LicenseServices",
                            type: 'POST',
                            success: function (licenses) { model.set("Licenses", licenses); },
                            error: function (error) { UX.Modals.Alert("ERROR", UX.Common.getMessageFromError(error).message, "m", "error", function () { }); },
                        });


                        //Init currency catalog
                        let defaultCurrency = model.get('Currencies').find((c) => { return c.Code === "MXN"; }).ID;
                        let currencyCatalog = $("#ca_currency").genericCatalog(
                            {
                                data: model.get('Currencies'),
                                dafaultValue: defaultCurrency,
                                columns: [
                                    { Width: "20%", Field: "Code", SearchType: "Contains" },
                                    { Width: "80%", Field: "Name", SearchType: "Contains" }
                                ],
                                fieldID: "ID",
                                fieldText: "Name",
                                fieldSort: "Code",
                                selectorName: "ca_currency"
                            });

                        currencyCatalog.onChange(function () {
                            model.set("CurrencyID", currencyCatalog.getValue());
                        });

                        model.set("CurrencyID", defaultCurrency);

                        $.when(licenses)
                            .done(function () {
                                //Remove loader
                                UX.Loaders.RemoveLoader(".company-assistant-modal");

                                let curpRegExp = /^([A-Z&]|[a-z&]{1})([AEIOUX]|[aeioux]{1})([A-Z&]|[a-z&]{1})([A-Z&]|[a-z&]{1})([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([HM]|[hm]{1})([AS|as|BC|bc|BS|bs|CC|cc|CS|cs|CH|ch|CL|cl|CM|cm|DF|df|DG|dg|GT|gt|GR|gr|HG|hg|JC|jc|MC|mc|MN|mn|MS|ms|NT|nt|NL|nl|OC|oc|PL|pl|QT|qt|QR|qr|SP|sp|SL|sl|SR|sr|TC|tc|TS|ts|TL|tl|VZ|vz|YN|yn|ZS|zs|NE|ne]{2})([^A|a|E|e|I|i|O|o|U|u]{1})([^A|a|E|e|I|i|O|o|U|u]{1})([^A|a|E|e|I|i|O|o|U|u]{1})([0-9]{2})$/;

                                //Init form validator
                                fv = $('.company-assistant-modal .cs-body').formValidation({
                                    framework: 'bootstrap',
                                    live: 'disabled',
                                    fields: {
                                        //Step 1
                                        ca_license: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar una licencia' }
                                            }
                                        },
                                        ca_rfc: {
                                            validators: {
                                                notEmpty: { message: 'Debes ingresar el RFC' },
                                                regexp: {
                                                    regexp: /^([A-ZÑa-zñ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[A-Za-z|\d]{3})$/,
                                                    message: "El formato del RFC es inválido"
                                                },
                                            }
                                        },
                                        ca_curp: {
                                            validators: {
                                                notEmpty: { message: 'Debes ingresar la CURP' },
                                                regexp: {
                                                    regexp: curpRegExp,
                                                    message: 'El formato de la CURP es inválido'
                                                }
                                            }
                                        },
                                        ca_socialreason: {
                                            validators: {
                                                notEmpty: { message: 'Debes ingresar el nombre o razón social' }
                                            }
                                        },
                                        ca_salaryzone: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar la zona de salario general' }
                                            }
                                        },
                                        ca_fiscalregime: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar el régimen fiscal' }
                                            }
                                        },
                                        ca_currency: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar una moneda' }
                                            }
                                        },

                                        //Step 2

                                        ca_nondeductiblefactor: {
                                            validators: {
                                                notEmpty: { message: 'Debes ingresar el factor no deducible' },
                                                between: {
                                                    min: 0,
                                                    max: 1,
                                                    message: 'El valor debe ser entre 0.00 y 1.00'
                                                }
                                            }
                                        },
                                        ca_initialdate: {
                                            validators: {
                                                notEmpty: { message: 'Debes ingresar la fecha inicial' },
                                                callback: { callback: UX.Common.ValidDateValidator },
                                                date: {
                                                    format: 'DD/MM/YYYY',
                                                    message:
                                                        'La fecha inicial debe estar entre ' +
                                                        '26/12/' + (model.get('CurrentFiscalYear') - 1) +
                                                        ' y ' +
                                                        '07/01/' + (model.get('CurrentFiscalYear')),
                                                    min: '26/12/' + (model.get('CurrentFiscalYear') - 1),
                                                    max: '07/01/' + (model.get('CurrentFiscalYear'))
                                                }
                                            }
                                        },
                                        ca_periodtype: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar el tipo de periodo' },
                                            }
                                        },
                                        ca_paymentdays: {
                                            validators: {
                                                notEmpty: { message: 'Debes ingresar los días de pago' },
                                                between: {
                                                    min: 1,
                                                    max: 9999999,
                                                    message: 'El valor debe ser mayor a 0'
                                                }
                                            }
                                        },

                                        //Step 3
                                        np_er_txtCode: {
                                            validators: {
                                                notEmpty: { message: 'Debes capturar el registro' },
                                                stringLength: {
                                                    min: 11,
                                                    message: 'El registro debe tener 11 caracteres'
                                                }
                                            }
                                        },
                                        ca_currentperiod: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar un periodo' },
                                            }
                                        },
                                        np_er_drpRiskClass: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar la clase de riesgo' },
                                            }
                                        },
                                        np_er_txtRFC: {
                                            validators: {
                                                notEmpty: { message: 'Debes capturar el RFC' },
                                            }
                                        },
                                        np_er_txtSocialReason: {
                                            validators: {
                                                notEmpty: { message: 'Debes capturar la razón social' },
                                            }
                                        },
                                        np_er_txtCURP: {
                                            validators: {
                                                notEmpty: { message: 'Debes capturar el CURP' },
                                            }
                                        },
                                        np_er_txtRiskClassFraction: {
                                            validators: {
                                                notEmpty: { message: 'Debes capturar la fracción de riesgo' },
                                                between: {
                                                    min: 0.000001,
                                                    max: 0.999999,
                                                    message: 'El valor debe ser mayor a 0 y menor a 1'
                                                }
                                            }
                                        },
                                        ca_zipcode: {
                                            validators: {
                                                notEmpty: { message: 'Debes capturar el código postal' }
                                            }
                                        },
                                        ca_federalentity: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar el estado' }
                                            }
                                        },
                                        ca_municipality: {
                                            validators: {
                                                notEmpty: { message: 'Debes seleccionar el municipio' }
                                            }
                                        },

                                        //step 4
                                        ca_foundationdate: {
                                            validators: {
                                                callback: { callback: UX.Common.ValidDateValidator }
                                            }
                                        },
                                    }
                                })
                                    .on('success.form.fv', function (e) {
                                        e.preventDefault();

                                        if (model.get("step") === 1) {

                                            //Validate license
                                            UX.Loaders.SetLoader(".company-assistant-modal");
                                            $.ajax({
                                                url: "/licensing/ValidateCompany",
                                                type: 'POST',
                                                data: {
                                                    licenseServiceID: model.get("LicenseServiceID"),
                                                    socialreason: model.get("CompanySocialReason"),
                                                    rfc: model.get("CompanyRFC")
                                                },
                                                success: function () {
                                                    model.set("step", 2);
                                                },
                                                error: function (onError) {
                                                    var error = UX.Common.getMessageFromError(onError);
                                                    UX.Modals.Alert("ERROR", error.message, "m", "error", function () { });
                                                    $("#btnCompanyAssistantNext").removeAttr("disabled").removeClass("disabled");
                                                },
                                                complete: function (onDone) {
                                                    UX.Loaders.RemoveLoader(".company-assistant-modal");
                                                }
                                            });
                                        }
                                        else if (model.get("step") === 2) {
                                            $('.employerregistration-footer').remove();
                                            model.set("step", 3);
                                        }
                                        else if (model.get("step") === 3) {
                                            model.set("step", 4);
                                        }
                                        else if (model.get("step") === 4) {

                                            UX.Loaders.SetLoader(".company-assistant-modal");

                                            model.set('Licences', []);
                                            model.set('Currencies', []);

                                            if (!model.get('IsPerson')) {
                                                model.set('CompanyCURP', '');
                                            }

                                            //Create company
                                            $.ajax({
                                                method: 'POST',
                                                url: CotorraAppURL + '/CompanyAssistant/Create/',
                                                data: model.get(),
                                                success: function (data) {
                                                    model.set("step", 99);

                                                    //Set headers
                                                    CotorraNode.RFC = model.get("CompanyRFC").toString().toUpperCase();
                                                    CotorraNode.SocialReason = model.get("CompanySocialReason").toString().toUpperCase();
                                                    CotorraNode.LicenseServiceID = model.get("LicenseServiceID").toString();

                                                    CotorraNode.CompanyID = data.CompanyID;
                                                    CotorraNode.InstanceID = data.InstanceID;
                                                    CotorraNode.LicenseID = data.LicenseID;

                                                    CotorraNode.Alias = "no-alias";

                                                    $('#btnCompanyAssistantFinish').off('click').on('click', function (ev) {
                                                        ev.stopPropagation();

                                                        $(".company-assistant").fadeOut(200, function () {
                                                            $(".company-assistant").remove();

                                                            //Show app
                                                            UX.Company.SelectCompany();
                                                        });
                                                    });
                                                },
                                                error: function (error) {
                                                    UX.Common.ErrorModal(error);
                                                },
                                                complete: function () {
                                                    UX.Loaders.RemoveLoader(".company-assistant-modal");
                                                }
                                            });
                                        }

                                    })
                                    .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });;

                                //Init HTML elements
                                $('#btnCompanyAssistantPrev').off('click').on('click', function (e) {
                                    e.preventDefault();
                                    model.set("step", model.get("step") - 1);
                                });

                                $('#ca_nondeductiblefactor').inputmask('decimal',
                                    {
                                        min: 0, max: 1, allowMinus: false, rightAlign: false
                                    });

                                $('#np_er_txtCode').mask('CCC-00000-00-0', {
                                    translation: {
                                        'C': { pattern: /[0-9a-zA-Z]/ }
                                    }
                                });

                                $('#np_er_txtRiskClassFraction').mask('0.000000');

                                var lookupZipCode = function (zipCode) {
                                    //Set new data
                                    UX.Loaders.SetLoader('.company-assistant');
                                    UX.Cotorra.EmployerRegistration.UI.SetZipCodeInfo({ zipCode: zipCode }, model,
                                        function () {
                                            UX.Loaders.RemoveLoader('.company-assistant');
                                            $(fv).data('formValidation').revalidateField('ca_federalentity');
                                            $(fv).data('formValidation').revalidateField('ca_municipality');
                                        });
                                    return false;
                                };

                                $('#ca_zipcode').off('keypress').on('keypress', function (e) {
                                    if (e.which == 13) {
                                        e.preventDefault();
                                        $('#np_btnSearchZipCode').click();
                                    }
                                });

                                $('#np_btnSearchZipCode').off('click').on('click', function (e) {
                                    e.preventDefault();
                                    lookupZipCode($("#ca_zipcode").val());
                                });

                                $('#btnCompanyAssistantNext').off('click').on('click', function (e) {
                                    e.preventDefault();
                                    $('.company-assistant-modal .cs-body').data('formValidation').validate();
                                });
                            });
                    }
                });
            },
            error: function (error) {
                error = UX.Common.getMessageFromError(error);
                UX.Modals.Alert('ERROR', error.message, 'm', 'error', function () { });
            }
        });

    }

};

//UX.CompanyAssistant_cotorra.Initialize();
