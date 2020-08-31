'use strict';

UX.Cotorra.WorkCenters = {

    UI: {

        CatalogName: 'WorkCenters',
        ContainerSelector: '#np-catalogs',
        TitleModalsString: 'Centro de trabajo',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        },

        LoadGrid: function (data) {

            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
                ZipCode: { type: 'string' },
                FederalEntity: { type: 'string' },
                Municipality: { type: 'string' },
            };

            //Set columns
            let columns = [
                { field: 'Name', title: 'Nombre', width: 200 },
                { field: 'ZipCode', title: 'Código postal', width: 120 },
                { field: 'FederalEntity', title: 'Estado', width: 180 },
                { field: 'Municipality', title: 'Municipio', width: 200 },
                {
                    title: ' ', width: 100,
                    template: UX.Cotorra.Common.GridCommonActionsTemplate(this.CatalogName)
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns });

        },

        OpenSave: function (obj = null) {

            let catNam = this.CatalogName;
            let titStr = this.TitleModalsString;

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'record_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo ' + titStr + '' : 'Editar ' + titStr + '', 'm', '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    let saveModel = new Ractive({
                        el: '#' + containerID,
                        template: '#' + catNam.toLowerCase() + '_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Name: row ? row.dataItem.Name : '',

                            //Address properties
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
                        }
                    });

                    //Set ZipCode
                    UX.Cotorra.Common.SetZipCodeInfo('#' + modalID, saveModel);

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Masks

                    //Set validations
                    let $fv = $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_pt_txtName: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el nombre' }
                                }
                            },

                            //Address form validation options
                            np_er_txtZipCode: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el C.P.' }
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

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = saveModel.get();
                            UX.Cotorra.Catalogs.Save(
                                catNam,
                                data,
                                (id) => {

                                    let grid = row ? row.$kendoGrid : $('.np-' + catNam.toLowerCase() + '-grid').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = id;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.Name = data.Name;

                                        dataItem.ZipCode = data.ZipCode;
                                        dataItem.FederalEntity = data.FederalEntity;
                                        dataItem.Municipality = data.Municipality;
                                        dataItem.Street = data.Street;
                                        dataItem.ExteriorNumber = data.ExteriorNumber;
                                        dataItem.InteriorNumber = data.InteriorNumber;
                                        dataItem.Suburb = data.Suburb;
                                        dataItem.Reference = data.Reference;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    UX.Cotorra.Catalogs.Reload({ catalog: catNam });
                                    UX.Modals.CloseModal(modalID);
                                },
                                (error) => {
                                    UX.Common.ErrorModal(error);
                                },
                                (complete) => {
                                    UX.Loaders.RemoveLoader('#' + modalID);
                                }
                            );
                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    //Init elements
                    $container.initUIElements();

                    //Get data (if applicable)

                    //Init ZipCode search
                    UX.Cotorra.Common.InitSearchZipCode('#' + modalID, saveModel, $fv);
                });
        },

        Delete: function (obj) {
            UX.Cotorra.GenericCatalog.UI.Delete(obj, 'Eliminar ' + this.TitleModalsString, '¿Deseas eliminar el ' + this.TitleModalsString + '?',
                this.CatalogName, this.ContainerSelector);
        }
    }
};