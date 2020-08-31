'use strict';

UX.Cotorra.Settlement = {

    UI: {

        CatalogName: 'Settlement',
        ContainerSelector: '#np-farescatalogs',
        TitleModalsString: 'finiquito',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        },

        LoadGrid: function (data = [], setValidity = true) {

            let cn = this.CatalogName.toLowerCase();

            //Set validity filter
            let selectedDropData = null;
            if (data.length > 0 && setValidity) {

                //Set options and change behavior
                let $drpValidity = $('#np_fc_drpValidity');
                for (var i = 0; i < data.length; i++) { $drpValidity.append(new Option(moment(data[i][0].Validity).format('DD/MM/YYYY'), i)); }
                $drpValidity.off('change').on('change', function () { UX.Cotorra.Settlement.UI.LoadGrid([data[parseInt($(this).val())]], false); });
                $drpValidity.change();
                return;
            } else if (data.length === 1) {
                selectedDropData = data[0];
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                CASUSMO: { type: 'number' },
                CASISR86: { type: 'number' },
                CalDirecPerc: { type: 'number' },
                Indem90: { type: 'number' },
                Indem20: { type: 'number' },
                PrimaAntig: { type: 'number' },
            };

            //Set columns
            let columns = [
                { field: 'CASUSMO', title: 'CASUSMO', width: 150 },
                { field: 'CASISR86', title: 'CASISR86', width: 150 },
                { field: 'CalDirecPerc', title: 'CalDirecPerc', width: 150 },
                { field: 'Indem90', title: 'Indem90', width: 150 },
                { field: 'Indem20', title: 'Indem20', width: 150 },
                { field: 'PrimaAntig', title: 'PrimaAntig', width: 150 },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#' + cn + 'ActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-' + cn + '-grid', data: selectedDropData ? selectedDropData : data, fields: fields, columns: columns });

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
                            CASUSMO: row ? row.dataItem.CASUSMO : '',
                            CASISR86: row ? row.dataItem.CASISR86 : '',
                            CalDirecPerc: row ? row.dataItem.CalDirecPerc : '',
                            Indem90: row ? row.dataItem.Indem90 : '',
                            Indem20: row ? row.dataItem.Indem20 : '',
                            PrimaAntig: row ? row.dataItem.PrimaAntig : '',
                            Validity: row ? row.dataItem.Validity : '',
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#record_save_wrapper').data('formValidation').validate(); });

                    //Masks
                    $('#np_txtCASUSMO').decimalMask(2);
                    $('#np_txtCASISR86').decimalMask(2);
                    $('#np_txtCalDirecPerc').decimalMask(2);
                    $('#np_txtIndem90').decimalMask(2);
                    $('#np_txtIndem20').decimalMask(2);
                    $('#np_txtPrimaAntig').decimalMask(2);

                    //Set validations
                    $('#record_save_wrapper').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_txtCASUSMO: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el CASUSMO' }
                                }
                            },
                            np_txtCASISR86: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el CASISR86' }
                                }
                            },
                            np_txtCalDirecPerc: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el CalDirecPerc' }
                                }
                            },
                            np_txtIndem90: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el Indem90' }
                                }
                            },
                            np_txtIndem20: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el Indem20' }
                                }
                            },
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
                                        ////Set data
                                        //data.ID = id;

                                        ////Insert
                                        //let dataItem = grid.dataSource.insert(0, data);
                                        //$('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.CASUSMO = data.CASUSMO;
                                        dataItem.CASISR86 = data.CASISR86;
                                        dataItem.CalDirecPerc = data.CalDirecPerc;
                                        dataItem.Indem90 = data.Indem90;
                                        dataItem.Indem20 = data.Indem20;
                                        dataItem.PrimaAntig = data.PrimaAntig;

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
                    $("#np_txtCASUSMO").focus();

                    //Get data (if applicable)
                });
        },
    }

};





