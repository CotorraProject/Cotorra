'use strict';

UX.Cotorra.Departments = {

    UI: {

        CatalogName: 'Departments',
        ContainerSelector: '#np-catalogs',
        TitleModalsString: 'Departamento',

        Init: function () {

            let catalogName = this.CatalogName;
            let containerSelector = this.ContainerSelector;

            (async function () {

                await UX.Cotorra.Catalogs.Require({
                    loader: {
                        container: '#np-catalogs',
                        removeWhenFinish: false
                    },
                    catalogs: ['Areas'],
                    forceLoad: false
                });

                UX.Cotorra.GenericCatalog.UI.Init(catalogName, containerSelector);
            })();
        },

        LoadGrid: function (data) {

            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' }
            };

            //Set columns
            let columns = [
                { field: 'Name', title: 'Nombre' },
                {
                    field: 'AreaID', title: 'Área',
                    template: UX.Cotorra.Common.GridFormatCatalogProp('UX.Cotorra.Catalogs.Areas', 'AreaID', 'ID', 'Name')
                },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#departmentsActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns });
        },

        OpenSave: (obj = null) => {

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'departments_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo departamento' : 'Editar departamento', 's',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var departmentModel = new Ractive({
                        el: '#departments_save_wrapper',
                        template: '#departments_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Name: row ? row.dataItem.Name : '',
                            AreaID: row ? row.dataItem.AreaID : '',

                            AreasOptions: UX.Cotorra.Catalogs.Areas
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Init elements
                    $container.initUIElements();
                    $container.find('#np_dep_txtName').focus();

                    //Set validations
                    $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_dep_txtName: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el nombre del departamento' }
                                }
                            }
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = departmentModel.get();
                            UX.Cotorra.Catalogs.Save('Departments',
                                data,
                                (id) => {

                                    let grid = row ? row.$kendoGrid : $('.np-departments-grid').data('kendoGrid');

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
                                        dataItem.AreaID = data.AreaID;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    
                                    UX.Cotorra.Catalogs.Reload({ catalog: 'Departments' });
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

                    //Get data (if applicable)
                });
        },

        Delete: function (obj) {
            UX.Cotorra.GenericCatalog.UI.Delete(obj, 'Eliminar ' + this.TitleModalsString, '¿Deseas eliminar el ' + this.TitleModalsString + '?',
                this.CatalogName, this.ContainerSelector);
        }
    }
};





