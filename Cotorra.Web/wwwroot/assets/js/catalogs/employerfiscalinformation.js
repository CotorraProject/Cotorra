'use strict';

//Check for namespaces
if (!UX.Cotorra) { UX.Cotorra = {}; }
if (!UX.Cotorra.EmployerFiscalInformation) { UX.Cotorra.EmployerFiscalInformation = {}; }


const toBase64 = file => new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = error => reject(error);
});

UX.Cotorra.EmployerFiscalInformation = {

    cfb: '',

    kfb: '',

    ivService: '',

    psService: '',

    saltService: '',

    rfc: '',

    UI: {
        Init: () => {
            //Show grid
            UX.Cotorra.EmployerFiscalInformation.UI.LoadGrid([]);
            UX.Loaders.SetLoader('#np-catalogs');

            //Get EmployerFiscalInformation
            UX.Cotorra.Catalogs.Get("EmployerFiscalInformation",
                null,
                (data) => {
                    UX.Cotorra.EmployerFiscalInformation.UI.LoadGrid(data);
                },
                (error) => {
                    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
                },
                (complete) => {
                    UX.Loaders.RemoveLoader('#np-catalogs');
                });

            //Set top action behaviors
            $('#np_btnAddEmployerFiscalInformation').off('click').on('click', function (ev) {
                UX.Cotorra.EmployerFiscalInformation.UI.OpenSave();
            });
        },

        LoadGrid: (data) => {

            //Set fields
            let fields = {
                ID: { type: 'string' },
                RFC: { type: 'string' },
                CertificateNumber: { type: 'string' },
                StartDate: { type: 'string' },
                ExpirationDate: { type: 'string' },
            };

            //Set columns
            let columns = [
                { field: 'RFC', title: 'RFC', width: 150 },
                { field: 'CertificateNumber', title: 'No. Certificado', width: 150 },
                { field: 'StartDate', title: 'Fecha creación', width: 150 },
                { field: 'ExpirationDate', title: 'Fecha de expiración', width: 150 },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#employerfiscalinformationActionTemplate').html())
                }
            ];

            //Init grid
            let $employerfiscalinformationGrid = UX.Common.InitGrid({ selector: '.np-employerfiscalinformation-grid', data: data, fields: fields, columns: columns });

        },

        OpenSave: (obj = null) => {
            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'employerfiscalinformation_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo certificado' : 'Editar certificado', 's',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);
                    UX.Cotorra.EmployerFiscalInformation.cfb = '';
                    UX.Cotorra.EmployerFiscalInformation.kfb = '';

                    //Init template
                    var employerfiscalinformationModel = new Ractive({
                        el: '#employerfiscalinformation_save_wrapper',
                        template: '#employerfiscalinformation_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            RFC: UX.Cotorra.EmployerFiscalInformation.rfc,
                            IsCertificateConfigured: row ? row.dataItem.IsCertificateConfigured : false,
                            CertificateNumber: row ? row.dataItem.CertificateNumber : '0000000000',
                            StartDate: row ? row.dataItem.StartDate : '',
                            ExpirationDate: row ? row.dataItem.ExpirationDate : '',
                            cfb: '',
                            kfb: '',
                            pfb: '',
                        }
                    });

                    //Set validations
                    var fv = $('#np_er_saveemployerfiscalinformation').formValidation({
                        framework: 'bootstrap',
                        fields: {
                            np_er_txtPassword: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la contraseña del certificado' },
                                }
                            },
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();

                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });


                    //Cancel button
                    $('#np_btnCancelSaveEmployerFiscalInformation').on('click', function () {
                        UX.Modals.CloseModal(modalID);
                    });

                    //Save button
                    $('#np_btnSaveEmployerFiscalInformation').on('click', function () {
                        //$('#np_er_saveemployerfiscalinformation').data('formValidation').validate();
                        //Set loader
                        UX.Loaders.SetLoader('#' + modalID);

                        //EData
                        let data = employerfiscalinformationModel.get();

                        //Creating the Vector
                        var iv = CryptoJS.enc.Hex.parse(UX.Cotorra.EmployerFiscalInformation.ivService);
                        var ps = CryptoJS.enc.Utf8.parse(UX.Cotorra.EmployerFiscalInformation.psService);
                        var salt = CryptoJS.enc.Utf8.parse(UX.Cotorra.EmployerFiscalInformation.saltService);

                        //Creating the key in PBKDF2 format to be used during the decryption
                        var key128Bits1000Iterations =
                            CryptoJS.PBKDF2(ps.toString(CryptoJS.enc.Utf8), salt, { keySize: 128 / 32, iterations: 1000 });

                        var cfg = { mode: CryptoJS.mode.CBC, iv: iv, padding: CryptoJS.pad.Pkcs7 };
                        data.cfb = CryptoJS.AES.encrypt(UX.Cotorra.EmployerFiscalInformation.cfb, key128Bits1000Iterations, cfg).toString();
                        data.kfb = CryptoJS.AES.encrypt(UX.Cotorra.EmployerFiscalInformation.kfb, key128Bits1000Iterations, cfg).toString();

                        //RFC
                        data.RFC = UX.Cotorra.EmployerFiscalInformation.rfc;

                        //Save data
                        UX.Cotorra.Catalogs.Save('EmployerFiscalInformation',
                            data,
                            (response) => {
                                let grid = row ? row.$kendoGrid : $('.np-employerfiscalinformation-grid').data('kendoGrid');

                                if (!row) {
                                    //Set data
                                    data.ID = response.ID;
                                    data.CertificateNumber = response.CertificateNumber;
                                    data.StartDate = response.StartDate;
                                    data.ExpirationDate = response.ExpirationDate;
                                    UX.Cotorra.EmployerFiscalInformation.UI.SetGridData([data]);

                                    //Insert
                                    let dataItem = grid.dataSource.insert(0, data);
                                    $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                } else {
                                    //Update data
                                    let dataItem = row.dataItem;
                                    row.dataItem.ID = response.ID;
                                    dataItem.ID = response.ID;
                                    dataItem.RFC = response.RFC;
                                    dataItem.CertificateNumber = response.CertificateNumber;
                                    dataItem.StartDate = response.StartDate;
                                    dataItem.ExpirationDate = response.ExpirationDate;

                                    UX.Cotorra.EmployerFiscalInformation.UI.SetGridData([dataItem]);

                                    //Redraw
                                    UX.Common.KendoFastRedrawRow(grid, dataItem);
                                    $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                }
                                UX.Cotorra.Catalogs.Reload({ catalog: 'EmployerFiscalInformation' });
                                UX.Modals.CloseModal(modalID);
                            },
                            (error) => {
                                UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
                            },
                            (complete) => {
                                UX.Loaders.RemoveLoader('#' + modalID);
                            }
                        );
                    });
                    
                    //Get data (if applicable)
                    $('#certificate').on('change', async function () {
                        let cerfiles = $('#certificate').prop('files');
                        UX.Cotorra.EmployerFiscalInformation.cfb = await toBase64(cerfiles[0]);
                    });

                    $('#key').on('change', async function () {
                        let cerfiles = $('#key').prop('files');
                        UX.Cotorra.EmployerFiscalInformation.kfb = await toBase64(cerfiles[0]);
                    });


                });
        },

        Delete: function (obj) {
            //Show modal
            UX.Modals.Confirm('Eliminar certificado', '¿Deseas eliminar el certificado seleccionado?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);
                   
                    UX.Loaders.SetLoader('#np-catalogs');
                    UX.Cotorra.Catalogs.Delete('EmployerFiscalInformation',
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

        SetGridData: (data) => {


        }
    }
};


