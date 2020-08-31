'use strict';

UX.Cotorra.IMSS = {

    UI: {

        CatalogName: 'IMSS',
        ContainerSelector: '#np-farescatalogs',
        TitleModalsString: 'IMSS',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        },

        LoadGrid: function (data) {

            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                IMMSBranch: { type: 'string' },
                EmployerShare: { type: 'number' },
                EmployeeShare: { type: 'number' },
                MaxSMDF: { type: 'number' }
            };

            //Set columns
            let columns = [
                { field: 'IMMSBranch', title: 'Rama IMSS' },
                { field: 'EmployerShare', title: 'Patrón', template: UX.Cotorra.Common.GridFormatPerc('EmployerShare') },
                { field: 'EmployeeShare', title: 'Obrero', template: UX.Cotorra.Common.GridFormatPerc('EmployeeShare') },
                { field: 'MaxSMDF', title: 'Valor Tope SMDF' },
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
                !row ? 'Nueva ' + titStr + '' : 'Editar rama' + titStr + '', 's', '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var saveModel = new Ractive({
                        el: '#' + containerID,
                        template: '#' + catNam.toLowerCase() + '_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            IMMSBranch: row ? row.dataItem.IMMSBranch : '',
                            EmployerShare: row ? row.dataItem.EmployerShare : '',
                            EmployeeShare: row ? row.dataItem.EmployeeShare : '',
                            MaxSMDF: row ? row.dataItem.MaxSMDF : '',
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Masks
                    $("#np_EmployerShare").decimalMask(6);
                    $("#np_EmployeeShare").decimalMask(6);
                    $("#np_MaxSMDF").mask('0000');

                    //Set validations
                    $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_IMMSBranch: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar la rama IMSS' }
                                }
                            },
                            np_EmployerShare: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el valor patrón' }
                                }
                            },
                            np_EmployeeShare: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el valor obrero' }
                                }
                            },
                            np_MaxSMDF: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el tope SMDF' }
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
                                        dataItem.IMMSBranch = data.IMMSBranch;
                                        dataItem.EmployerShare = data.EmployerShare;
                                        dataItem.EmployeeShare = data.EmployeeShare;
                                        dataItem.MaxSMDF = data.MaxSMDF;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

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