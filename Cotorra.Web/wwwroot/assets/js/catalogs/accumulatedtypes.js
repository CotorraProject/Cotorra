'use strict';

UX.Cotorra.AccumulatedTypes = {

    UI: {

        CatalogName: 'AccumulatedTypes',
        ContainerSelector: '#np-catalogs',
        TitleModalsString: 'Tipo de acumulado',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        },

        LoadGrid: function (data) {

            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
                TypeOfAccumulated: { type: 'number' },
            };

            //Set columns
            let columns = [
                { field: 'Name', title: 'Nombre' },
                {
                    field: 'TypeOfAccumulated', title: 'Tipo',
                    template: UX.Cotorra.Common.GridFormatCatalogProp('UX.Cotorra.TypesOfAccumulated', 'TypeOfAccumulated', 'ID', 'Name')
                }, 
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#' + cn + 'ActionTemplate').html())
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
                !row ? 'Nuevo ' + titStr + '' : 'Editar ' + titStr + '', 's', '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var saveModel = new Ractive({
                        el: '#' + containerID,
                        template: '#' + catNam.toLowerCase() + '_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Name: row ? row.dataItem.Name : '',
                            TypeOfAccumulated: row ? row.dataItem.TypeOfAccumulated : '1',
                            TypeOfAccumulatedOptions: UX.Cotorra.TypesOfAccumulated
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Masks

                    //Set validations
                    $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_txtName: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el nombre' }
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
                                        dataItem.TypeOfAccumulated = data.TypeOfAccumulated;

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
                });
        },

        Delete: function (obj) {
            let ts = this.TitleModalsString;
            UX.Cotorra.GenericCatalog.UI.Delete(obj, 'Eliminar ' + ts, '¿Deseas eliminar el ' + ts + '?',
                this.CatalogName, this.ContainerSelector);
        }
    }

};