'use strict';

UX.Cotorra.Shifts = {

    ShiftWorkingDayOptions: [
        { id: 1, description: 'Diurna' },
        { id: 2, description: 'Nocturna' },
        { id: 3, description: 'Mixta' },
        { id: 4, description: 'Por horas' },
        { id: 5, description: 'Reducida' },
        { id: 6, description: 'Continuada' },
        { id: 7, description: 'Partida' },
        { id: 8, description: 'Por turnos' },
        { id: 99, description: 'Otra jornada' }
    ],

    UI: {
        CatalogName: 'WorkShift',
        ContainerSelector: '#np-catalogs',
        TitleModalsString: 'Turnos',

        Init: () => {
            let catalogName = this.CatalogName;
            let containerSelector = this.ContainerSelector;

            //(async function () {
            //    UX.Cotorra.GenericCatalog.UI.Init(catalogName, containerSelector);
            //})();

            //Show grid
            UX.Cotorra.Shifts.UI.LoadGrid([]);
            UX.Loaders.SetLoader('#np-catalogs');

            //Get shifts
            UX.Cotorra.Catalogs.Get("Shifts",
                null,
                (data) => {
                    UX.Cotorra.Shifts.UI.LoadGrid(data);
                },
                (error) => {
                    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
                },
                (complete) => {
                    UX.Loaders.RemoveLoader('#np-catalogs');
                });

            //Set top action behaviors
            $('#np_btnAddShift').off('click').on('click', function (ev) {
                UX.Cotorra.Shifts.UI.OpenSave();
            });
        },

        LoadGrid: (data) => {

            //Transform data (if applicable)
            for (var i = 0; i < data.length; i++) {
                let swdo = UX.Cotorra.Shifts.ShiftWorkingDayOptions.find((x) => { return x.id === data[i].ShiftWorkingDayType });
                data[i].ShiftWorkingDayString = swdo ? data[i].ShiftWorkingDayType + ' ' + swdo.description : 'No se encontró la jornada turno';
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
                TotalHours: { type: 'number' },
                ShiftWorkingDayType: { type: 'int' }
            };

            //Set columns
            let columns = [
                { field: 'Name', title: 'Nombre' },
                { field: 'TotalHours', title: 'No. de horas' },
                { field: 'ShiftWorkingDayString', title: 'Jornada turno' },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#shiftsActionTemplate').html())
                }
            ];

            //Init grid
            let $shiftsGrid = UX.Common.InitGrid({ selector: '.np-shifts-grid', data: data, fields: fields, columns: columns });

        },

        OpenSave: (obj = null) => {

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'shifts_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo turno' : 'Editar turno', 's',
                '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var shiftModel = new Ractive({
                        el: '#shifts_save_wrapper',
                        template: '#shifts_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Name: row ? row.dataItem.Name : '',
                            TotalHours: row ? row.dataItem.TotalHours : '',
                            ShiftWorkingDayType: row ? row.dataItem.ShiftWorkingDayType : '',
                            ShiftWorkingDayOptions: UX.Cotorra.Shifts.ShiftWorkingDayOptions
                        }
                    });

                    //Cancel button
                    $('#np_btnCancelSaveShift').on('click', function () {
                        UX.Modals.CloseModal(modalID);
                    });

                    //Save button
                    $('#np_btnSaveShift').on('click', function () {
                        $('#np_sh_saveshift').data('formValidation').validate();
                    });

                    //Init elements
                    $container.initUIElements();
                    $container.find('#np_sh_txtName').focus();
                    $container.find('#np_sh_txtTotalHours').inputmask("decimal", {
                        radixPoint: ".", groupSeparator: ",", digits: 2, autoGroup: true, rightAlign: false
                    });

                    //Set validations
                    $('#np_sh_saveshift').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_sh_txtName: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el nombre del turno' }
                                }
                            },
                            np_sh_txtTotalHours: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el número de horas' },
                                    greaterThan: { value: 0, message: 'El número de horas debe ser mayor a 0' },
                                    lessThan: { value: 48, message: 'El número de horas debe ser menor o igual a 48' }
                                }
                            },
                            np_sh_drpShiftWorkingDay: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la jornada turno' }
                                }
                            }
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = shiftModel.get();
                            UX.Cotorra.Catalogs.Save('Shifts',
                                data,
                                (id) => {

                                    let grid = row ? row.$kendoGrid : $('.np-shifts-grid').data('kendoGrid');

                                    //Shift work day string name
                                    let swdo = UX.Cotorra.Shifts.ShiftWorkingDayOptions.find((x) => { return x.id === data.ShiftWorkingDayType; });
                                    swdo = swdo ? data.ShiftWorkingDayType + ' ' + swdo.description : 'No se encontró la jornada turno';

                                    if (!row) {
                                        //Set data
                                        data.ID = id;
                                        data.ShiftWorkingDayString = swdo;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.Name = data.Name;
                                        dataItem.TotalHours = data.TotalHours;
                                        dataItem.ShiftWorkingDayType = data.ShiftWorkingDayType;
                                        dataItem.ShiftWorkingDayString = swdo;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    UX.Cotorra.Catalogs.Reload({ catalog: 'Shifts' });
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
            UX.Modals.Confirm('Eliminar turno', '¿Deseas eliminar el turno seleccionado?', 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);

                    UX.Loaders.SetLoader('#np-catalogs');
                    UX.Cotorra.Catalogs.Delete('Shifts',
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

