'use strict';

UX.Cotorra.CompanyConfiguration = {

    FiscalRegimes: [],

    HolidayPremiumPaymentTypeOptions: [
        { ID: '0', Name: 'Pagar vacaciones y prima vacacional en el periodo que abarquen' },
        { ID: '1', Name: 'Pagar solo vacaciones en el periodo que abarquen' },
        { ID: '2', Name: 'Pagar solo vacaciones en el periodo que comienzan' },
    ],

    Data: null,

    UI: {
        Init: () => {

            //UX.Loaders.SetLoader('#np_company_config_view');

            UX.Cotorra.CompanyConfiguration.UI.Data = null;

            let gfrRequest = UX.Cotorra.Catalogs.Do('GET', 'CompanyAssistant', 'GetFiscalRegimes', null,
                (data) => { UX.Cotorra.CompanyConfiguration.FiscalRegimes = data; }, //success
                (error) => { }, //error
                () => { }); //complete

            let pccRequest = UX.Cotorra.Catalogs.Get('PayrollCompanyConfiguration', {},
                (data) => { UX.Cotorra.CompanyConfiguration.Data = data; },
                (error) => { },
                () => { },
            );

            (async function () {

                await UX.Cotorra.Catalogs.Require({
                    requests: [gfrRequest, pccRequest],
                    catalogs: [],
                    forceLoad: false
                });

                setTimeout(() => {
                    UX.Cotorra.CompanyConfiguration.UI.InitModels();
                }, 25);

            })();


        },

        InitModels: () => {

            UX.Cotorra.CompanyConfiguration.UI.InitGeneralData();
            UX.Cotorra.CompanyConfiguration.UI.InitAbout();
            UX.Cotorra.CompanyConfiguration.UI.VacationConfig();
        },

        InitGeneralData: () => {

            let data = UX.Cotorra.CompanyConfiguration.Data[0];
            var generalDataModel = UX.Cotorra.CompanyConfiguration.UI.GeneralDataModel = new Ractive({
                el: '#np_cc_company_data',
                template: '#np_companyconfiguration_generaldata_template',
                data: {
                    ID: data.ID,
                    RFC: data.RFC,
                    IsPerson: '',
                    IsBusiness: '',
                    SocialReason: data.SocialReason,
                    CURP: data.CURP,
                    SalaryZone: data.SalaryZone,
                    FiscalRegime: data.FiscalRegime,
                    ZipCode: data.ZipCode,
                    FederalEntity: data.FederalEntity,
                    Municipality: data.Municipality,
                    Street: data.Street,
                    ExteriorNumber: data.ExteriorNumber,
                    InteriorNumber: data.InteriorNumber,
                    Suburb: data.Suburb,
                    Reference: data.Reference,
                    FiscalRegimesOptions: UX.Cotorra.CompanyConfiguration.FiscalRegimes,
                    FederalEntityOptions: [{ id: '', description: '- - -' }],
                    MunicipalityOptions: [{ id: '', description: '- - -' }],
                    SuburbOptions: [],
                    SalaryZonesOptions: UX.Cotorra.SalaryZones
                }
            });

            //Observers
            generalDataModel.observe('RFC', function (newValue) {

                let rexExpPerson = /^([A-ZÑa-zñ\x26]{4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[A-Za-z|\d]{3})$/;
                let rexExpBusiness = /^([A-ZÑa-zñ\x26]{3}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[A-Za-z|\d]{3})$/;

                let isPerson = rexExpPerson.test(newValue);
                let isBusiness = rexExpBusiness.test(newValue);

                generalDataModel.set('IsPerson', isPerson);
                generalDataModel.set('IsBusiness', isBusiness);

            }, { init: true, defer: true });

            let curpRegExp = /^([A-Z&]|[a-z&]{1})([AEIOUX]|[aeioux]{1})([A-Z&]|[a-z&]{1})([A-Z&]|[a-z&]{1})([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([HM]|[hm]{1})([AS|as|BC|bc|BS|bs|CC|cc|CS|cs|CH|ch|CL|cl|CM|cm|DF|df|DG|dg|GT|gt|GR|gr|HG|hg|JC|jc|MC|mc|MN|mn|MS|ms|NT|nt|NL|nl|OC|oc|PL|pl|QT|qt|QR|qr|SP|sp|SL|sl|SR|sr|TC|tc|TS|ts|TL|tl|VZ|vz|YN|yn|ZS|zs|NE|ne]{2})([^A|a|E|e|I|i|O|o|U|u]{1})([^A|a|E|e|I|i|O|o|U|u]{1})([^A|a|E|e|I|i|O|o|U|u]{1})([0-9]{2})$/;

            let $fv = $('#np_er_savecompanygeneraldata').formValidation({
                framework: 'bootstrap',
                live: 'disabled',
                fields: {
                    np_er_txtCURP: {
                        validators: {
                            notEmpty: { message: 'Debes ingresar la CURP' },
                            regexp: {
                                regexp: curpRegExp,
                                message: 'El formato de la CURP es inválido'
                            }
                        }
                    },
                    np_drpSalaryZone: {
                        validators: {
                            notEmpty: { message: 'Debes seleccionar una zona de salario' }
                        }
                    },
                    np_fiscalregime: {
                        validators: {
                            notEmpty: { message: 'Debes seleccionar el régimen fiscal' }
                        }
                    },
                    np_er_txtZipCode: {
                        validators: {
                            notEmpty: { message: 'Debes capturar el código postal' }
                        }
                    },
                    np_er_drpFederalEntity: {
                        validators: {
                            notEmpty: { message: 'Debes seleccionar el estado' }
                        }
                    },
                    np_er_drpMunicipality: {
                        validators: {
                            notEmpty: { message: 'Debes seleccionar el municipio' }
                        }
                    }
                },
                onSuccess: function (ev) {
                    ev.preventDefault();
                    UX.Common.ClearFocus();

                    UX.Loaders.SetLoader('#np_company_config_view');
                    UX.Cotorra.Catalogs.Do('POST', 'PayrollCompanyConfiguration', 'SaveGeneralData',
                        generalDataModel.get(),
                        (response) => { },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader('#np_company_config_view'); }
                    );
                }
            })
                .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

            //UX.Cotorra.Common.InitSearchZipCode('#np_cc_company_data', generalDataModel, $fv);
            //Set ZipCode
            UX.Cotorra.Common.SetZipCodeInfo('#np_company_config_view', generalDataModel);

            $('#np-company-configuration').initUIElements();
        },

        InitAbout: () => {
            let data = UX.Cotorra.CompanyConfiguration.Data[0];

            var aboutModel = UX.Cotorra.CompanyConfiguration.UI.AboutModel = new Ractive({
                el: '#np_cc_about',
                template: '#np_companyconfiguration_about_template',
                data: {
                    ID: data.ID,
                    ComercialName: data.ComercialName,
                    CompanyBusinessSector: data.CompanyBusinessSector,
                    CompanyScope: data.CompanyScope,
                    CompanyCreationDate: data.CompanyCreationDate ? moment(data.CompanyCreationDate).format('DD/MM/YYYY') : '',
                    CompanyInformation: data.CompanyInformation,
                    CompanyWebSite: data.CompanyWebSite,
                    Facebook: data.Facebook,
                    Instagram: data.Instagram,
                }
            });

            $('#ca_foundationdate').mask('00/00/0000');

            let $fv = $('#np_er_savecompanyabout').formValidation({
                framework: 'bootstrap',
                live: 'disabled',
                fields: {
                    ca_commercialname: {
                        validators: {
                            stringLength: {
                                max: 100,
                                message: 'El nombre comercial no debe superar los 100 caracteres'
                            }
                        }
                    },
                    ca_foundationdate: {
                        validators: {
                            message: 'La fecha proporcionada no es válida',
                            callback: { callback: UX.Common.ValidDateValidator }
                        }
                    },
                    ca_heading: {
                        validators: {
                            notEmpty: { message: 'Debes seleccionar un giro' }
                        }
                    }
                },
                onSuccess: function (ev) {
                    ev.preventDefault();
                    UX.Common.ClearFocus();

                    UX.Loaders.SetLoader('#np_company_config_view');
                    UX.Cotorra.Catalogs.Do('POST', 'PayrollCompanyConfiguration', 'SaveAboutData',
                        aboutModel.get(),
                        (response) => { },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader('#np_company_config_view'); }
                    );
                }
            })
                .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

        },

        VacationConfig: () => {

            let data = UX.Cotorra.CompanyConfiguration.Data[0];
            var vacationConfigModel = UX.Cotorra.CompanyConfiguration.UI.VacationModel = new Ractive({
                el: '#np_cc_vacation_configuration',
                template: '#np_companyconfiguration_vacation_template',
                data: {
                    ID: data.ID,
                    HolidayPremiumPaymentType: data.HolidayPremiumPaymentType,
                    HolidayPremiumPaymentTypeOptions: UX.Cotorra.CompanyConfiguration.HolidayPremiumPaymentTypeOptions,
                }
            });

            let $fv = $('#np_er_savevacationconfigdata').formValidation({
                framework: 'bootstrap',
                live: 'disabled',
                fields: {
                    ca_HolidayPremiumPaymentType: {
                        validators: {
                            notEmpty: { message: 'Debes una opción' }
                        }
                    }
                },
                onSuccess: function (ev) {
                    ev.preventDefault();
                    UX.Common.ClearFocus();

                    UX.Loaders.SetLoader('#np_company_config_view');
                    UX.Cotorra.Catalogs.Do('POST', 'PayrollCompanyConfiguration', 'SaveVacationConfigData',
                        vacationConfigModel.get(),
                        (response) => { },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader('#np_company_config_view'); }
                    );
                }
            })
                .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });
        }
    }
};