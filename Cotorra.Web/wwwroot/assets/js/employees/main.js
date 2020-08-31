'use strict';

UX.Cotorra.Employees = {

    UI: {

        CatalogName: 'Employees',
        ContainerSelector: '#np-employees',
        TitleModalsString: 'Colaborador',

        Init: function () {
            let catalogName = this.CatalogName;
            let containerSelector = this.ContainerSelector;

            let model = UX.Cotorra.Employees.Model = new Ractive({
                el: containerSelector,
                template: '#employees_template',
                data: {
                    Status: '0'
                }
            });

            //Set top action behaviors
            $('#np-employees #np_btnAddEmployee').off('click').on('click', function (ev) {
                UX.Cotorra.Employees.UI.Create();
            });

            (async function () {

                UX.Cotorra.Employees.UI.LoadGrid([]);

                //Get employee create assistant templates
                let employeeGetTemplates = UX.Cotorra.Employees.UI.GetTemplates();

                await UX.Cotorra.Catalogs.Require({
                    loader: {
                        container: containerSelector,
                        removeWhenFinish: false
                    },
                    catalogs: ['PeriodTypes', 'Positions', 'EmployerRegistration', 'Departments', 'Positions', 'Shifts', 'Banks', 'BenefitTypes', 'UMA'],
                    requests: employeeGetTemplates,
                    forceLoad: false
                });

                UX.Cotorra.GenericCatalog.UI.Init(catalogName, containerSelector);

                UX.Cotorra.Common.SetQuickSearch('#np-employees .quicksearch', '#np-employees .np-employees-grid');
            })();

        },

        GetTemplates: () => {

            let setHTMLOnBody = (data) => {
                $('body #' + $(data).attr('id')).remove();
                $('body').append(data);
            };

            let empCreateAssistantReq = $.get(CotorraAppURL + '/views/employees/employee-create-assistant.html').done(setHTMLOnBody);
            let empUpdatetReq = $.get(CotorraAppURL + '/views/employees/employee-update.html').done(setHTMLOnBody);
            let empPersonalDataReq = $.get(CotorraAppURL + '/views/employees/employee-personal-data.html').done(setHTMLOnBody);
            let empGeneralDataReq = $.get(CotorraAppURL + '/views/employees/employee-general-data.html').done(setHTMLOnBody);
            let empEmploymentReq = $.get(CotorraAppURL + '/views/employees/employee-employment.html').done(setHTMLOnBody);
            let empJobPositionReq = $.get(CotorraAppURL + '/views/employees/employee-jobposition.html').done(setHTMLOnBody);
            let empSocialSecurityReq = $.get(CotorraAppURL + '/views/employees/employee-social-security.html').done(setHTMLOnBody);
            let empContributionSalaryReq = $.get(CotorraAppURL + '/views/employees/employee-contribution-salary.html').done(setHTMLOnBody);
            let empKiosk = $.get(CotorraAppURL + '/views/employees/employee-kiosk.html').done(setHTMLOnBody);

            return [empCreateAssistantReq, empPersonalDataReq, empGeneralDataReq, empEmploymentReq,
                empJobPositionReq, empSocialSecurityReq, empContributionSalaryReq, empKiosk];
        },

        LoadGrid: function (data = [], setEmployees = true) {

            let cn = this.CatalogName.toLowerCase();
            if (data.length > 0 && setEmployees) {

                $('#employees_filter_status').off('change').on('change', function (ev) {

                    let status = UX.Cotorra.Employees.Model.get('Status');
                    UX.Cotorra.Employees.UI.LoadGrid(data.filter(element => element.LocalStatus == status), false);
                    $('#np-employees .quicksearch').val('');
                });

                $('#employees_filter_status').change();
                return;
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Code: { type: 'number' },
                Name: { type: 'string' },
                FullName: { type: 'string' },
                FirstLastName: { type: 'string' },
                SecondLastName: { type: 'string' },
                Email: { type: 'string' },
                Gender: { type: 'number' },
                AreaID: { type: 'string' },
                DepartmentID: { type: 'string' },
                JobPositionID: { type: 'string' },
                BankID: { type: 'string' },
                WorkCenterID: { type: 'string' },
                WorkShiftID: { type: 'string' },
                Active: { type: 'boolean' },
                PeriodTypeID: { type: 'string' },
                Antiquity: { type: 'string' },
                TextSearch: { type: 'string' }
            };

            //Set columns
            let columns = [
                //{
                //    title: ' ', width: 40,
                //    template: kendo.template($('#avatarEmployeeTemplate').html())
                //},
                //{
                //    field: 'Code', title: 'Código', width: 70,
                //},
                //{
                //    field: 'FullName', title: 'Nombre del colaborador', width: 310,
                //},
                {
                    title: 'Colaborador', width: 350, field: 'Code',
                    template: kendo.template($('#nameEmployeeTemplate').html())
                },
                {
                    title: 'Tipo de periodo', width: 150,
                    template: UX.Cotorra.Common.GridFormatCatalogProp('UX.Cotorra.Catalogs.PeriodTypes', 'PeriodTypeID', 'ID', 'Name'),
                },
                //{
                //    field: 'JobPositionID', title: 'Puesto', width: 200, hidden: false,
                //    template: UX.Cotorra.Common.GridFormatCatalogProp('UX.Cotorra.Catalogs.Positions', 'JobPositionID', 'ID', 'Name'),
                //    hidden: false
                //},
                {
                    title: 'Departamento', width: 230,
                    template: UX.Cotorra.Common.GridFormatCatalogProp('UX.Cotorra.Catalogs.Departments', 'DepartmentID', 'ID', 'Name'),
                    hidden: false
                },
                {
                    title: 'Antigüedad',
                    template: '#= Antiquity#'
                },
                {
                    field: 'TextSearch', title: 'TextSearch',
                    hidden: true,
                },
                {
                    title: ' ', width: 80,
                    template: kendo.template($('#' + cn + 'ActionTemplate').html())
                }
            ];

            if (data.length > 0) {
                data = data.map(x => {
                    x.TextSearch = x.FullName + '|' + x.Code;
                    return x;
                })
            }

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns });

        },

        OpenSave: function (obj = null, externalEdition = false) {
            this.Create(obj, externalEdition);
        },

        Create: function (obj = null, externalEdition = false) {
            let catName = UX.Cotorra.Employees.UI.CatalogName;
            let row = UX.Cotorra.Common.GetRowData(obj);
            let isNew = row === null;

            if (externalEdition) {
                row = externalEdition.row;
                catName = externalEdition.catName;
                isNew = false;
            }
            //else {
            //    catNam = this.CatalogName;
            //    titStr = this.TitleModalsString;
            //    row = UX.Cotorra.Common.GetRowData(obj);
            //    templateName = catNam;
            //}

            let containerID = isNew ? 'employee-create-assistant' : 'employee-update';

            let modalID = UX.Modals.OpenModal(
                isNew ? 'Nuevo colaborador' : (externalEdition ? 'Editar histórico del colaborador' : 'Editar colaborador'),
                isNew ? 'm' : 'l',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);
                    let autoCode = null;

                    if (!row) {
                        if (UX.Cotorra.Catalogs.Employees.length == 0) {
                            autoCode = 1;
                        } else {

                            let maxCode = UX.Cotorra.Catalogs.Employees.reduce((a, b) => {
                                return a.Code > b.Code ? a : b;
                            }).Code;

                            autoCode = maxCode + 1;
                        }
                    }

                    //Init templateUX.Cotorra.Employees.SaveModel
                    let saveModel = UX.Cotorra.Employees.SaveModel = new Ractive({
                        el: '#' + containerID,
                        template: '#' + containerID + '-template',
                        data: {
                            Step: 1,
                            TotalSteps: 3,
                            IsNew: isNew,
                            UMA: 0,
                            IsHistoricEmployee: (externalEdition ? true : false),

                            ID: row ? row.dataItem.ID : null,
                            Antiquity: row ? row.dataItem.Antiquity : null,

                            //Step 1
                            Name: row ? row.dataItem.Name : null,
                            FirstLastName: row ? row.dataItem.FirstLastName : null,
                            SecondLastName: row ? row.dataItem.SecondLastName : null,
                            RFC: row ? row.dataItem.RFC : null,
                            CURP: row ? row.dataItem.CURP : null,
                            BirthDate: row ? (row.dataItem.BirthDate != null ? moment(row.dataItem.BirthDate).format("DD/MM/YYYY") : null) : null,
                            Gender: row ? row.dataItem.Gender : null,
                            CivilStatus: row ? row.dataItem.CivilStatus : null,
                            Phone: row ? row.dataItem.Phone : null,
                            Cellphone: row ? row.dataItem.Cellphone : null,
                            Email: row ? row.dataItem.Email : null,
                            ZipCode: row ? row.dataItem.ZipCode : '',
                            FederalEntity: row ? row.dataItem.FederalEntity : '',
                            Municipality: row ? row.dataItem.Municipality : '',
                            Street: row ? row.dataItem.Street : '',
                            ExteriorNumber: row ? row.dataItem.ExteriorNumber : '',
                            InteriorNumber: row ? row.dataItem.InteriorNumber : '',
                            Suburb: row ? row.dataItem.Suburb : '',
                            Reference: row ? row.dataItem.Reference : '',
                            FederalEntityOptions: [{ id: '', description: '- - -' }],
                            MunicipalityOptions: [{ id: '', description: '- - -' }],
                            SuburbOptions: [],

                            //Step2
                            Code: row ? row.dataItem.Code : autoCode,
                            EntryDate: moment(row ? row.dataItem.EntryDate : new Date).format("DD/MM/YYYY"),
                            ContractType: row ? row.dataItem.ContractType : 1,
                            RegimeType: row ? row.dataItem.RegimeType : 2,
                            PeriodTypeID: row ? row.dataItem.PeriodTypeID : null,
                            WorkShiftID: row ? row.dataItem.WorkShiftID : null,
                            EmployeeTrustLevel: row ? row.dataItem.EmployeeTrustLevel : 2,
                            BenefitType: row ? row.dataItem.BenefitType : 1,
                            SalaryZone: row ? row.dataItem.SalaryZone : (UX.Cotorra.CompanyConfiguration.Data[0].SalaryZone),
                            DailySalary: row ? row.dataItem.DailySalary : null,
                            DepartmentID: row ? row.dataItem.DepartmentID : null,
                            JobPositionID: row ? row.dataItem.JobPositionID : null,
                            JobPosition: row ? UX.Cotorra.Catalogs.Positions.find(x => {
                                return x.ID === row.dataItem.JobPositionID;
                            }).Name : null,
                            ImmediateLeaderEmployeeID: row ? row.dataItem.ImmediateLeaderEmployeeID : null,

                            //Step3
                            NSS: row ? row.dataItem.NSS : null,
                            EmployerRegistrationID: row ? row.dataItem.EmployerRegistrationID : null,
                            UMF: row ? row.dataItem.UMF : null,
                            ContributionBase: row ? row.dataItem.ContributionBase : 1,
                            SBCFixedPart: row ? row.dataItem.SBCFixedPart : '0.00',
                            SBCVariablePart: row ? row.dataItem.SBCVariablePart : '0.00',
                            SBCMax25UMA: row ? row.dataItem.SBCMax25UMA : '0.00',

                            //Step4 Kiosk
                            IsKioskEnabled: row ? row.dataItem.IsKioskEnabled : false,

                            //Other
                            IdentityUserID: null, //row ? row.dataItem.IdentityUserID : null,
                            PaymentBase: row ? row.dataItem.PaymentBase : null,
                            PaymentMethod: row ? row.dataItem.PaymentMethod : null,
                            FonacotNumber: row ? row.dataItem.FonacotNumber : null,
                            AFORE: row ? row.dataItem.AFORE : null,
                            IsForeignWithoutCURP: row ? row.dataItem.IsForeignWithoutCURP : null,
                            BankID: row ? row.dataItem.BankID : null,
                            BankBranchNumber: row ? row.dataItem.BankBranchNumber : null,
                            BankAccount: row ? row.dataItem.BankAccount : null,
                            CLABE: row ? row.dataItem.CLABE : null,

                            //IsIdentityVinculated
                            IsIdentityVinculated: false, //row ? row.dataItem.IsIdentityVinculated : false,

                            //Options
                            PeriodTypesOptions: UX.Cotorra.Catalogs.PeriodTypes.filter(x => { return x.IsEnabled }),
                            ContractTypesOptions: UX.Cotorra.ContractTypes,
                            ContributionBasesOptions: UX.Cotorra.ContributionBases,
                            EmployerRegistrationOptions: UX.Cotorra.Catalogs.EmployerRegistration,
                            CivilStatusOptions: UX.Cotorra.CivilStatus,
                            GenderOptions: UX.Cotorra.Gender,
                            DepartmentsOptions: UX.Cotorra.Catalogs.Departments,
                            JobPositionsOptions: UX.Cotorra.Catalogs.Positions,
                            PaymentBasesOptions: UX.Cotorra.PaymentBases,
                            PaymentMethodsOptions: UX.Cotorra.PaymentMethods,
                            WorkShiftsOptions: UX.Cotorra.Catalogs.Shifts,
                            SalaryZonesOptions: UX.Cotorra.SalaryZones,
                            RegimeTypesOptions: UX.Cotorra.RegimeTypes,
                            ImmediateLeaderOptions: UX.Cotorra.Catalogs.Employees,
                            BanksOptions: UX.Cotorra.Catalogs.Banks,

                            //For update modal
                            UpdateType: '',
                        }
                    });

                    //Observers
                    saveModel.observe('ContributionBase',
                        function (newValue, oldValue) {
                            if (newValue === 1 && isNew)//fija
                            {
                                UX.Cotorra.Employees.SaveModel.set('SBCVariablePart', '0.00');

                            }
                            else if (newValue === 2 && isNew) { //variable
                                UX.Cotorra.Employees.SaveModel.set('SBCFixedPart', '0.00');
                            }
                        },
                        { init: false, defer: true });

                    saveModel.observe('SBCVariablePart',
                        function (newValue, oldValue) {
                            UX.Cotorra.Employees.UI.SetSBCMax25UMA();
                        },
                        { init: false, defer: true });

                    saveModel.observe('SBCFixedPart',
                        function (newValue, oldValue) {
                            UX.Cotorra.Employees.UI.SetSBCMax25UMA();
                        },
                        { init: false, defer: true });

                    saveModel.observe('PeriodTypeID',
                        function (newValue, oldValue) {
                            UX.Cotorra.Employees.UI.SetUMA();
                        },
                        { init: false, defer: true });

                    saveModel.observe('EntryDate',
                        function (newValue, oldValue) {
                            UX.Cotorra.Employees.UI.SetUMA();
                        },
                        { init: false, defer: true });

                    //Masks
                    $('#np_emp_txtBirthDate').mask('00/00/0000');
                    $('#np_emp_txtPhone').mask('(000) 000-0000');
                    $('#np_emp_txtCellPhone').mask('(000) 000-0000');

                    $('#np_emp_txtCode').mask('0000000000');
                    $('#np_emp_txtEntryDate').mask('00/00/0000');
                    $('#np_emp_txtFonacotNumber').mask('00000000000000000000');
                    $('#np_emp_txtUMF').mask('00000000000000000000000000000000000000000000000000');
                    $('#np_emp_txtNSS').mask('0000-00-0000-0');
                    $('#np_emp_txtBankBranchNumber').mask('00000000000000000000');
                    $('#np_emp_txtBankAccount').mask('00000000000000000000');
                    $('#np_emp_txtCLABE').mask('000000000000000000000000000000');

                    //Set validations
                    var $fv = $('#np_emp_saveemployee').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {

                            //STEP 1
                            np_emp_txtFirstLastName: {
                                validators: {
                                    notEmpty: { message: 'Captura el primero apellido' },
                                }
                            },
                            np_emp_txtName: {
                                validators: {
                                    notEmpty: { message: 'Captura el(los) nombre(es)' },
                                }
                            },
                            np_emp_txtRFC: {
                                validators: {
                                    notEmpty: { message: 'Captura el RFC' },
                                }
                            },
                            np_emp_txtCURP: {
                                validators: {
                                    notEmpty: { message: 'Captura el CURP' },
                                }
                            },
                            np_emp_txtBirthDate: {
                                validators: {
                                    message: 'Formato inválido',
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_er_txtZipCode: {
                                validators: {
                                    notEmpty: { message: 'Captura el CP' },
                                }
                            },
                            np_er_drpFederalEntity: {
                                validators: {
                                    notEmpty: { message: 'Selecciona la entidad' },
                                }
                            },
                            np_er_drpMunicipality: {
                                validators: {
                                    notEmpty: { message: 'Selecciona el municipio' },
                                }
                            },

                            //STEP 2
                            np_emp_txtCode: {
                                validators: {
                                    notEmpty: { message: 'Captura el código' }
                                }
                            },
                            np_emp_txtEntryDate: {
                                validators: {
                                    notEmpty: { message: 'Captura la fecha de ingreso' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_emp_drpContractType: {
                                validators: {
                                    notEmpty: { message: 'Selecciona un tipo de contrato' }
                                }
                            },
                            np_emp_drpPeriodType: {
                                validators: {
                                    notEmpty: { message: 'Selecciona la periodicidad' },
                                }
                            },
                            np_drpWorkShift: {
                                validators: {
                                    notEmpty: { message: 'Selecciona un turno' },
                                }
                            },
                            np_emp_sal_txtDailySalary: {
                                validators: {
                                    notEmpty: { message: 'Captura el salario diario' },
                                }
                            },
                            np_drpDepartment: {
                                validators: {
                                    notEmpty: { message: 'Selecciona un departamento' },
                                }
                            },
                            np_drpJobPosition: {
                                validators: {
                                    notEmpty: { message: 'Selecciona un puesto' },
                                }
                            },

                            //STEP 3
                            np_emp_txtNSS: {
                                validators: {
                                    notEmpty: { message: 'Captura el NSS' },
                                }
                            },
                            np_emp_drpContributionBase: {
                                validators: {
                                    notEmpty: { message: 'Selecciona la base de cotización' },
                                }
                            },
                            np_emp_txtSBCFixedPart: {
                                validators: {
                                    notEmpty: { message: 'Captura el SBC parte fija' },
                                }
                            },
                            np_emp_txtSBCVariablePart: {
                                validators: {
                                    notEmpty: { message: 'Captura el SBC parte variable' },
                                }
                            },
                            np_emp_txtSBCMax25UMA: {
                                validators: {
                                    notEmpty: { message: 'Captura SBC (tope 25 UMA)' },
                                }
                            },
                            np_drpRegimeType: {
                                validators: {
                                    notEmpty: { message: 'Debes seleccionar el tipo de régimen' },
                                }
                            },

                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            let saveEmployee = function () {

                                //Save
                                let data = saveModel.get();
                                data.NSS = data.NSS ? data.NSS.replace(/-/g, '') : null;

                                UX.Loaders.SetLoader('#' + modalID);
                                UX.Cotorra.Catalogs.Save(
                                    catName,
                                    data,
                                    (saved) => {

                                        if (externalEdition) {
                                            externalEdition.callback({ Saved: saved, Data: data, JobPositionName: data.JobPositionsOptions.find(item => item.ID == data.JobPositionID).Name });
                                            UX.Modals.CloseModal(modalID);
                                            return;
                                        }

                                        if (isNew) { UX.Modals.CloseModal(modalID); }
                                        UX.Cotorra.Employees.UI.Init();
                                    },
                                    UX.Common.ErrorModal,
                                    () => { UX.Loaders.RemoveLoader('#' + modalID); }
                                );
                            }

                            if (isNew) {
                                //Save employee
                                let step = saveModel.get('Step');
                                let totalSteps = saveModel.get('TotalSteps');
                                let regimeType = saveModel.get('RegimeType');

                                if (step === 2 && regimeType >= 5 && regimeType <= 11) {
                                    step++;
                                }

                                if (step < totalSteps) {
                                    //Next step
                                    saveModel.set('Step', ++step);
                                } else {
                                    //Save
                                    saveEmployee();
                                }
                            } else {
                                //Update employee
                                saveEmployee();
                            }

                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    //Init elements
                    $container.initUIElements();

                    if (!isNew) {
                        UX.Cotorra.Common.SetZipCodeInfo('#' + modalID, saveModel);
                    } else {
                        UX.Cotorra.Common.InitSearchZipCode('#' + modalID, saveModel, $fv);
                    }

                    //Next step
                    $('#btnNextStep').off('click').on('click', function () {
                        $fv.data('formValidation').validate();
                    });

                    //Prev step
                    $('#btnPrevStep').off('click').on('click', function () {
                        let step = saveModel.get('Step');
                        saveModel.set('Step', --step);
                    });

                    let updateEmployee = function (updateType) {
                        saveModel.set('UpdateType', updateType);
                        $fv.data('formValidation').validate();
                    }

                    //Update buttons
                    $('#btnSaveEmployeePersonalData').off('click').on('click', function () { updateEmployee('PersonalData'); });
                    $('#btnSaveEmployeeAddress').off('click').on('click', function () { updateEmployee('Address'); });
                    $('#btnSaveEmployeeEmployment').off('click').on('click', function () { updateEmployee('Employment') });
                    $('#btnSaveEmployeeSocialSecurity').off('click').on('click', function () { updateEmployee('SocialSecurity'); });
                    $('#btnSaveEmployeePayment').off('click').on('click', function () { updateEmployee('Payment'); });
                    $('#btnSaveEmployeeKiosk').off('click').on('click', function () { updateEmployee('Kiosk'); });
                    $('#btnSaveEmployeeVacations').off('click').on('click', function () { updateEmployee('Vacations'); });
                    $('#btnSaveEmployeeHistoric').off('click').on('click', function () { updateEmployee(''); });

                    //Checkbox readonly 
                    //np-identity-vinculated-switch
                    $('#np-identity-vinculated-switch').off('click').on('click', function () {
                        return false;
                    });

                    //Tools buttons
                    $('#btnDeleteEmployee').off('click').on('click', function () {
                        UX.Cotorra.Employees.UI.Delete(saveModel.get('ID'), modalID);
                    });

                    $('#btnSalaryRaise').off('click').on('click', function () {
                        UX.Cotorra.Employees.UI.SetUMA();
                        UX.Cotorra.SalaryUpdate.UI.Init(row, UX.Cotorra.Employees.SaveModel);
                    });

                    $('#btnSettlementEmployee').off('click').on('click', function () {
                        UX.Cotorra.SettlementProcess.UI.InitModal(row);
                    });

                    $('#np_emp_calculateSBC').off('click').on('click', function () {
                        UX.Cotorra.Employees.UI.CalculateSBCorModal(row, UX.Cotorra.Employees.SaveModel);
                    });
                });
        },

        Delete: function (id, modalID) {
            //Show modal

            UX.Modals.Confirm('Eliminar colaborador', '¿Deseas eliminar el colaborador?', 'Sí, eliminar', 'No, espera',
                () => {
                    UX.Loaders.SetLoader('#' + modalID);
                    UX.Cotorra.Catalogs.Delete(this.CatalogName,
                        { id: id },
                        () => {
                            UX.Cotorra.Employees.UI.Init();
                            UX.Modals.CloseModal(modalID);
                        },
                        UX.Common.ErrorModal,
                        () => {
                            UX.Loaders.RemoveLoader('#' + modalID);
                        }
                    );
                },
                () => {

                });
        },

        CalculateSBCorModal: (row, model) => {
            let isNew = UX.Cotorra.Employees.SaveModel.get('IsNew');

            if (isNew === true) {
                let params = {
                    EntryDate: moment(UX.Cotorra.Employees.SaveModel.get('EntryDate'), 'DD/MM/YYYY'),
                    BenefitType: UX.Cotorra.Employees.SaveModel.get('BenefitType'),
                    PeriodTypeID: UX.Cotorra.Employees.SaveModel.get("PeriodTypeID"),
                    DailySalary: UX.Cotorra.Employees.SaveModel.get('DailySalary'),
                }

                let sbc = UX.Cotorra.Employees.UI.CalculateSBC(params);
                UX.Cotorra.Employees.SaveModel.set('SBCFixedPart', sbc);
            }
            else {
                UX.Cotorra.Employees.UI.SetUMA();
                UX.Cotorra.SBCUpdate.UI.Init(row, model);
            }
        },

        CalculateSBC: (params) => {

            let inMoment = params.EntryDate;
            let benefitTypeName = params.BenefitType == 2 ? 'Personalizada' : 'De ley';

            let benefitType = UX.Cotorra.Catalogs.BenefitTypes[0][0].Name == benefitTypeName ? 0 : 1
            let periodTypeID = params.PeriodTypeID;

            let dailySalaryString = params.DailySalary;
            let dailySalary = 0;
            if (isNaN(dailySalaryString)) {
                dailySalary = dailySalaryString ? dailySalaryString.replace(/,/g, '') : 0;
            }
            else {
                dailySalary = dailySalaryString;
            }

            dailySalary = parseFloat(dailySalary);
            let periodType = UX.Cotorra.Catalogs.PeriodTypes.find(element => element.ID == periodTypeID);
            if (!periodType) {
                return 0;
            }
            let initialDate = periodType.DetailInitialDate;
            let periodInitialDate = moment(initialDate, 'YYYY/MM/DD');
            let difference = ((periodInitialDate.diff(inMoment, 'days')) / 365);
            difference = difference < 0 ? difference * -1 : difference;
            let seniority = isNaN(difference) ? difference : Math.floor(difference) + 1
            let integrationFactor = UX.Cotorra.Catalogs.BenefitTypes[benefitType][seniority - 1].IntegrationFactor;
            let sbc = (dailySalary * integrationFactor).toFixed(2);

            return sbc;

        },

        SetUMA: () => {

            let UMA = 0;
            let UMAToFindDate = moment(new Date());
            let periodTypeID = UX.Cotorra.Employees.SaveModel.get('PeriodTypeID');
            let periodType = UX.Cotorra.Catalogs.PeriodTypes.find(el => el.ID == periodTypeID);
            if (periodType) {
                let employeeEntryDate = moment(UX.Cotorra.Employees.SaveModel.get('EntryDate'), 'DD/MM/YYYY');
                let periodTypeFinalDate = moment(periodType.DetailFinalDate, 'YYYY-MM-DD');;

                if (employeeEntryDate > periodTypeFinalDate) {
                    UMAToFindDate = employeeEntryDate;
                }
                else {
                    UMAToFindDate = periodTypeFinalDate;
                }

                UMA = UX.Cotorra.Catalogs.UMA.find(element => UMAToFindDate._d.getFullYear() === moment(element.InitialDate)._d.getFullYear()).Value;
            }
            UX.Cotorra.Employees.SaveModel.set('UMA', UMA);

        },

        SetSBCMax25UMA: () => {
            UX.Cotorra.Employees.UI.CalculateSBCMax25UMA(UX.Cotorra.Employees.SaveModel);
        },

        CalculateSBCMax25UMA: (params) => {
            let model = params;

            let UMATop = UX.Cotorra.Employees.SaveModel.get('UMA') * 25;
            let variableStr = model.get('SBCVariablePart') ? model.get('SBCVariablePart') + '' : '0';
            let variable = variableStr.replace(/,/g, '');
            let fixedStr = model.get('SBCFixedPart') ? model.get('SBCFixedPart') + '' : '0';
            let fixed = fixedStr.replace(/,/g, '');
            let sbc = parseFloat(variable) + parseFloat(fixed);
            if (sbc > UMATop) {
                sbc = UMATop;
            }
            model.set('SBCMax25UMA', sbc);
        }
    }
};