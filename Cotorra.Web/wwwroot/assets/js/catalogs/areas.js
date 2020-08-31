'use strict';

UX.Cotorra.Areas = {

    UI: {
        Init: () => {
            //Show grid
            UX.Cotorra.Areas.UI.LoadGrid([]);
            UX.Loaders.SetLoader('#np-catalogs');

            //Get Areas
            UX.Cotorra.Catalogs.Get("Areas",
                null,
                (data) => {
                    UX.Cotorra.Areas.UI.LoadGrid(data);
                },
                (error) => {
                    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
                },
                (complete) => {
                    UX.Loaders.RemoveLoader('#np-catalogs');
                });

            //Set top action behaviors
            $('#np_btnAddArea').off('click').on('click', function (ev) {
                UX.Cotorra.Areas.UI.OpenSave();
            });
        },

        LoadGrid: (data) => {

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' }
            };

            //Set columns
            let columns = [
                { field: 'Name', title: 'Nombre' },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#areasActionTemplate').html())
                }
            ];

            //Init grid
            let $areasGrid = UX.Common.InitGrid({ selector: '.np-areas-grid', data: data, fields: fields, columns: columns });
        },

        OpenSave: (obj = null) => {

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'areas_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nueva área' : 'Editar área', 's',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var areaModel = new Ractive({
                        el: '#areas_save_wrapper',
                        template: '#areas_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Name: row ? row.dataItem.Name : '',
                        }
                    });

                    //Cancel button
                    $('#np_btnCancelSaveArea').on('click', function () {
                        UX.Modals.CloseModal(modalID);
                    });

                    //Save button
                    $('#np_btnSaveArea').on('click', function () {
                        $('#np_area_savearea').data('formValidation').validate();
                    });

                    //Init elements
                    $container.initUIElements();
                    $container.find('#np_area_txtName').focus();

                    //Set validations
                    $('#np_area_savearea').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_area_txtName: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el nombre del área' }
                                }
                            }
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = areaModel.get();
                            UX.Cotorra.Catalogs.Save('Areas',
                                data,
                                (id) => {

                                    let grid = row ? row.$kendoGrid : $('.np-areas-grid').data('kendoGrid');

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

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    UX.Cotorra.Catalogs.Reload({ catalog: 'Areas' });
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
            //Show modal
            UX.Modals.Confirm('Eliminar área', '¿Deseas eliminar el área seleccionada?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);
                    UX.Loaders.SetLoader('#np-catalogs');
                    UX.Cotorra.Catalogs.Delete('Areas',
                        { id: row.dataItem.ID },
                        () => { $(row.el).fadeOut(function () { row.$kendoGrid.dataSource.remove(row.dataItem); }); },
                        (error) => { UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', function () { }); },
                        () => { UX.Loaders.RemoveLoader('#np-catalogs'); }
                    );
                },
                () => {
                });
        }
    }
};



