'use strict';

//Check for namespaces
if (!UX.Cotorra) { UX.Cotorra = {}; }
if (!UX.Cotorra.EmployerRegistration) { UX.Cotorra.EmployerRegistration = {}; }

UX.Cotorra.EmployerRegistration = {

    RiskClassOptions: [
        { id: '01', description: 'Clase 1' },
        { id: '02', description: 'Clase 2' },
        { id: '03', description: 'Clase 3' },
        { id: '04', description: 'Clase 4' },
        { id: '05', description: 'Clase 5' },
        { id: '99', description: 'No aplica' },
    ],

    UI: {
        Init: () => {
            //Show grid
            UX.Cotorra.EmployerRegistration.UI.LoadGrid([]);
            UX.Loaders.SetLoader('#np-catalogs');

            //Get employerregistration
            UX.Cotorra.Catalogs.Get("EmployerRegistration",
                null,
                (data) => { UX.Cotorra.EmployerRegistration.UI.LoadGrid(data); },
                (error) => { UX.Common.ErrorModal(error); },
                (complete) => { UX.Loaders.RemoveLoader('#np-catalogs'); });

            //Set top action behaviors
            $('#np_btnAddEmployerRegistration').off('click').on('click', function (ev) {
                UX.Cotorra.EmployerRegistration.UI.OpenSave();
            });
        },

        LoadGrid: (data) => {

            //Transform data
            UX.Cotorra.EmployerRegistration.UI.SetGridData(data);

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Code: { type: 'string' },
                RiskClassString: { type: 'string' },
                RiskClassFraction: { type: 'number' },
                ZipCode: { type: 'string' },
                Address: { type: 'string' },
                FederalEntity: { type: 'string' },
                Municipality: { type: 'string' },
                Suburb: { type: 'string' },
                Reference: { type: 'string' }
            };

            //Set columns
            let columns = [
                { field: 'Code', title: 'Registro patronal', width: 150 },
                { field: 'RiskClassString', title: 'Clase riesgo', width: 130 },
                { field: 'RiskClassFraction', title: 'Fracción riesgo', width: 130 },
                { field: 'ZipCode', title: 'C.P.', width: 80 },
                //{ field: 'Address', title: 'Dirección', width: 300 },
                //{ field: 'Suburb', title: 'Colonia', hidden: false, width: 220 },
                //{ field: 'Municipality', title: 'Municipio', hidden: false, width: 220 },
                { field: 'FederalEntity', title: 'Estado', hidden: false, width: 120 },
                //{ field: 'CURP', title: 'CURP', width: 180 },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#employerregistrationActionTemplate').html())
                }
            ];

            //Init grid
            let $employerregistrationGrid = UX.Common.InitGrid({ selector: '.np-employerregistration-grid', data: data, fields: fields, columns: columns });

        },

        OpenSave: (obj = null) => {

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'employerregistration_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo registro patronal' : 'Editar registro patronal', 'm',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var employerregistrationModel = new Ractive({
                        el: '#employerregistration_save_wrapper',
                        template: '#employerregistration_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Code: row ? row.dataItem.Code : '',
                            RiskClass: row ? row.dataItem.RiskClass : '',
                            RiskClassFraction: row ? row.dataItem.RiskClassFraction : '',
                            ZipCode: row ? row.dataItem.ZipCode : '',
                            FederalEntity: row ? row.dataItem.FederalEntity : '',
                            Municipality: row ? row.dataItem.Municipality : '',
                            Street: row ? row.dataItem.Street : '',
                            ExteriorNumber: row ? row.dataItem.ExteriorNumber : '',
                            InteriorNumber: row ? row.dataItem.InteriorNumber : '',
                            Suburb: row ? row.dataItem.Suburb : '',
                            Reference: row ? row.dataItem.Reference : '',
                            RiskClassOptions: UX.Cotorra.EmployerRegistration.RiskClassOptions,
                            FederalEntityOptions: [{ id: '', description: '- - -' }],
                            MunicipalityOptions: [{ id: '', description: '- - -' }],
                            SuburbOptions: [],
                        }
                    });

                    UX.Loaders.SetLoader('#' + modalID);
                    UX.Cotorra.EmployerRegistration.UI.SetZipCodeInfo(
                        {
                            zipCode: employerregistrationModel.get('ZipCode'),
                            State: employerregistrationModel.get('FederalEntity'),
                            Municipality: employerregistrationModel.get('Municipality'),
                            Suburb: employerregistrationModel.get('Suburb')
                        },
                        employerregistrationModel,
                        function () { UX.Loaders.RemoveLoader('#' + modalID); }
                    );

                    //Cancel button
                    $('#np_btnCancelSaveEmployerRegistration').on('click', function () {
                        UX.Modals.CloseModal(modalID);
                    });

                    //Save button
                    $('#np_btnSaveEmployerRegistration').on('click', function () {
                        $('#np_er_saveemployerregistration').data('formValidation').validate();
                    });

                    //Init elements
                    $container.initUIElements();
                    $container.find('#np_er_txtCode').focus();
                    $container.find('#np_er_txtCode').mask('CCC-00000-00-0', {
                        translation: {
                            'C': {
                                pattern: /[0-9a-zA-Z]/
                            }
                        }
                    });
                    $container.find('#np_er_txtRiskClassFraction').mask('0.000000');


                    var lookupZipCode = function (zipCode) {
                        //Set new data
                        UX.Loaders.SetLoader('#' + modalID);
                        UX.Cotorra.EmployerRegistration.UI.SetZipCodeInfo({ zipCode: zipCode }, employerregistrationModel,
                            function () {
                                UX.Loaders.RemoveLoader('#' + modalID);
                                $(fv).data('formValidation').revalidateField('np_er_drpFederalEntity');
                                $(fv).data('formValidation').revalidateField('np_er_drpMunicipality');
                            });
                        return false;
                    };

                    $container.find('#np_er_txtZipCode').off('keypress').on('keypress', function (e) {
                        if (e.which == 13) {
                            e.preventDefault();

                            //Get zip code
                            let zipCode = $(this).val();
                            lookupZipCode(zipCode);
                        }
                    });

                    $container.find('#np_btnSearchZipCode').off('click').on('click', function (e) {
                        e.preventDefault();
                        //Get zip code
                        let zipCode = $("#np_er_txtZipCode").val();
                        lookupZipCode(zipCode);
                    });

                    //Set validations
                    var fv = $('#np_er_saveemployerregistration').formValidation({
                        framework: 'bootstrap',
                        fields: {
                            np_er_txtCode: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el registro' },
                                    stringLength: {
                                        min: 11,
                                        message: 'El registro debe tener 11 caracteres'
                                    }
                                }
                            },
                            np_er_drpRiskClass: {
                                validators: {
                                    notEmpty: { message: 'Debes seleccionar la clase de riesgo' },
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
                            },
                            np_er_txtPassword: {
                                validators: {
                                    required: function (element) {
                                        return $("#certificate").prop("files").length > 0 &&
                                            $("#key").prop("files").length > 0;
                                    },
                                    notEmpty: { message: 'Debes capturar la contraseña del certificado' }
                                }
                            }
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //EData
                            let data = employerregistrationModel.get();


                            //Save data
                            UX.Cotorra.Catalogs.Save('EmployerRegistration',
                                data,
                                (id) => {

                                    let grid = row ? row.$kendoGrid : $('.np-employerregistration-grid').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = id;
                                        UX.Cotorra.EmployerRegistration.UI.SetGridData([data]);

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.Code = data.Code;
                                        dataItem.RiskClass = data.RiskClass;
                                        dataItem.RiskClassFraction = data.RiskClassFraction;
                                        dataItem.ZipCode = data.ZipCode;
                                        dataItem.FederalEntity = data.FederalEntity;
                                        dataItem.Municipality = data.Municipality;
                                        dataItem.Street = data.Street;
                                        dataItem.ExteriorNumber = data.ExteriorNumber;
                                        dataItem.InteriorNumber = data.InteriorNumber;
                                        dataItem.Suburb = data.Suburb;
                                        dataItem.Reference = data.Reference;

                                        UX.Cotorra.EmployerRegistration.UI.SetGridData([dataItem]);

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

                                    UX.Modals.CloseModal(modalID);

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


                });
        },

        Delete: function (obj) {
            //Show modal
            UX.Modals.Confirm('Eliminar registro patronal', '¿Deseas eliminar el registro patronal seleccionado?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);

                    UX.Loaders.SetLoader('#np-catalogs');
                    UX.Cotorra.Catalogs.Delete('EmployerRegistration',
                        {
                            id: row.dataItem.ID
                        },
                        () => {
                            $(row.el).fadeOut(function () { row.$kendoGrid.dataSource.remove(row.dataItem); });
                        },
                        (error) => {
                            UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', function () { });
                        },
                        () => {
                            UX.Loaders.RemoveLoader('#np-catalogs');
                        }
                    );
                },
                () => {
                });
        },

        SetZipCodeInfo: (zipCodeInfo, model, callback) => {

            //Clear all
            model.set('FederalEntityOptions', [{ id: '', description: '- - -' }]);
            model.set('MunicipalityOptions', [{ id: '', description: '- - -' }]);
            model.set('SuburbOptions', []);

            model.set('FederalEntity', '');
            model.set('Municipality', '');
            model.set('Suburb', '');

            UX.Cotorra.Common.GetZipCodeInfo(
                zipCodeInfo.zipCode,
                (data) => {

                    if (data.length > 0) {
                        let states = data
                            .map((x) => { return x.state; })
                            .filter((v, i, s) => { return s.indexOf(v) === i; })
                            .map((x) => { return { id: x, description: x }; });

                        let municips = data
                            .map((x) => { return x.municipality; })
                            .filter((v, i, s) => { return s.indexOf(v) === i; })
                            .map((x) => { return { id: x, description: x }; });

                        let suburbs = data.map((x) => {
                            return { id: x.suburb, description: x.suburb };
                        });

                        model.set('FederalEntityOptions', states);
                        model.set('MunicipalityOptions', municips);
                        model.set('SuburbOptions', suburbs);

                        if (zipCodeInfo.State) { model.set('FederalEntity', zipCodeInfo.State); }
                        if (zipCodeInfo.Municipality) { model.set('Municipality', zipCodeInfo.Municipality); }
                        if (zipCodeInfo.Suburb) { model.set('Suburb', zipCodeInfo.Suburb); }
                    }

                    callback.call();
                },
                (error) => {
                },
                (complete) => {
                }
            );
        },

        SetGridData: (data) => {

            for (var i = 0; i < data.length; i++) {

                let rco = UX.Cotorra.EmployerRegistration.RiskClassOptions.find((x) => { return x.id === data[i].RiskClass });
                data[i].RiskClassString = rco ? rco.id + ' ' + rco.description : 'No se encontró la clase de riesgo';

                data[i].Address =
                    (data[i].Street && data[i].Street != "" ? data[i].Street + ' ' : '') +
                    (data[i].ExteriorNumber && data[i].ExteriorNumber != "" ? data[i].ExteriorNumber + ' ' : '') +
                    (data[i].InteriorNumber && data[i].InteriorNumber != "" ? data[i].InteriorNumber : '');
            }

        }
    }
};


