'use strict';

UX.Cotorra.Positions = {

    PositionRiskOptions: [
        { id: 1, description: 'Class I' },
        { id: 2, description: 'Class II' },
        { id: 3, description: 'Class III' },
        { id: 4, description: 'Class IV' },
        { id: 5, description: 'Class V' },
        { id: 99, description: 'No aplica' },
    ],

    UI: {
        Init: () => {
            //Show grid
            UX.Cotorra.Positions.UI.LoadGrid([]);
            UX.Loaders.SetLoader('#np-catalogs');

            //Get positions            
            UX.Cotorra.Catalogs.Get("Positions", null,
                (data) => {
                    UX.Cotorra.Positions.UI.LoadGrid(data);
                },
                (error) => {
                    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
                },
                (complete) => {
                    UX.Loaders.RemoveLoader('#np-catalogs');
                });

            //Set top action behaviors
            $('#np_btnAddPosition').off('click').on('click', function (ev) {
                UX.Cotorra.Positions.UI.OpenSave();
            });
        },

        LoadGrid: (data) => {

            //Transform data (if applicable)
            for (var i = 0; i < data.length; i++) {
                let swdo = UX.Cotorra.Positions.PositionRiskOptions.find((x) => { return x.id === data[i].JobPositionRiskType });
                data[i].JobPositionRiskTypeString = swdo ? data[i].JobPositionRiskType + ' ' + swdo.description : 'No se encontró el riesgo de trabajo';
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
                JobPositionRiskType: { type: 'int' }
            };

            //Set columns
            let columns = [
                { field: 'Name', title: 'Nombre del puesto' },
                { field: 'JobPositionRiskTypeString', title: 'Riesgo de Trabajo' },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#positionsActionTemplate').html())
                }
            ];

            //Init grid
            let $positionsGrid = UX.Common.InitGrid({ selector: '.np-positions-grid', data: data, fields: fields, columns: columns });

        },

        OpenSave: (obj = null) => {

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'positions_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo puesto' : 'Editar puesto', 's',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var positionModel = new Ractive({
                        el: '#positions_save_wrapper',
                        template: '#positions_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Name: row ? row.dataItem.Name : '',
                            JobPositionRiskType: row ? row.dataItem.JobPositionRiskType : '',
                            PositionRiskOptions: UX.Cotorra.Positions.PositionRiskOptions
                        }
                    });

                    //Init components
                    //Cancel button
                    $('#np_btnCancelSavePosition').on('click', function () {
                        UX.Modals.CloseModal(modalID);
                    });

                    //Save button
                    $('#np_btnSavePosition').on('click', function () {
                        $('#np_ps_saveposition').data('formValidation').validate();
                    });

                    //Init elements
                    $container.initUIElements();

                    //Set validations
                    $('#np_ps_saveposition').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_ps_txtName: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el nombre del puesto' }
                                }
                            },
                            np_sh_drpRiskType: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el riesgo de trabajo' }
                                }
                            }
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = positionModel.get();
                            UX.Cotorra.Catalogs.Save(
                                'Positions',
                                data,
                                (id) => {

                                    let grid = row ? row.$kendoGrid : $('.np-positions-grid').data('kendoGrid');

                                    //Shoft risk type string name
                                    let swdo = UX.Cotorra.Positions.PositionRiskOptions.find((x) => { return x.id === data.JobPositionRiskType; });
                                    swdo = swdo ? data.JobPositionRiskType + ' ' + swdo.description : 'No se encontró el riesgo de trabajo';

                                    if (!row) {
                                        //Set data
                                        data.ID = id;
                                        data.JobPositionRiskTypeString = swdo;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.Name = data.Name;
                                        dataItem.JobPositionRiskType = data.JobPositionRiskType;
                                        dataItem.JobPositionRiskTypeString = swdo;

                                        //Redraw
                                      
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    UX.Cotorra.Catalogs.Reload({ catalog: 'Positions' });
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

        Delete: function(obj) {
            //Show modal
            UX.Modals.Confirm('Eliminar puesto', '¿Deseas eliminar el puesto seleccionado?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);

                    UX.Loaders.SetLoader('#np-catalogs');
                    UX.Cotorra.Catalogs.Delete(
                        'Positions',
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
        }
    }
};