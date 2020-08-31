'use strict';

UX.Cotorra.AnualEmploymentSubsidy = {

    UI: {

        CatalogName: 'AnualEmploymentSubsidy',
        ContainerSelector: '#np-farescatalogs',
        TitleModalsString: 'subsidio al empleo',

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
                $drpValidity.off('change').on('change', function () { UX.Cotorra.AnualEmploymentSubsidy.UI.LoadGrid([data[parseInt($(this).val())]], false); });
                $drpValidity.change();
                return;
            } else if (data.length === 1) {
                selectedDropData = data[0];
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                LowerLimit: { type: 'string' },
                AnualSubsidy: { type: 'string' }
            };

            //Set columns
            let columns = [
                { field: 'LowerLimit', title: 'Límite inferior', template: UX.Cotorra.Common.GridFormatCurrency('LowerLimit') },
                { field: 'AnualSubsidy', title: 'Subsidio al empleo', template: UX.Cotorra.Common.GridFormatCurrency('AnualSubsidy') },
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
                            LowerLimit: row ? row.dataItem.LowerLimit : '',
                            AnualSubsidy: row ? row.dataItem.AnualSubsidy : ''
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Set validations
                    $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_LowerLimit: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el límite inferior' }
                                }
                            },
                            np_AnualSubsidy: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el subsidio al empleo' }
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
                                        dataItem.LowerLimit = data.LowerLimit;
                                        dataItem.AnualSubsidy = data.AnualSubsidy;

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


