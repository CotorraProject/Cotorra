'use strict';

UX.Cotorra.Concepts = {
    UI: {
        CatalogName: {
            'P': 'Perceptions',
            'D': 'Deductions',
            'L': 'Liabilities'
        },
        ContainerSelector: '#np-catalogs',
        TitleModalsString: {
            'P': 'percepción',
            'D': 'deducción',
            'L': 'obligación'
        },
        CatalogNameType: {
            'P': 1,
            'D': 3,
            'L': 2
        },

        Init: function () {

            let containerSelector = this.ContainerSelector;

            (async function () {

                await UX.Cotorra.Catalogs.Require({
                    loader: {
                        container: containerSelector,
                        removeWhenFinish: false
                    },
                    catalogs: ['AccumulatedTypes'],
                    forceLoad: false
                });

                UX.Cotorra.GenericCatalog.UI.Init('Concepts', containerSelector);
            })();

        },

        LoadGrid: function (data = []) {
            if (data.length > 0) {
                data = data.filter(el => { return el.ConceptType == UX.Cotorra.Concepts.UI.CatalogNameType[UX.Cotorra.Concepts.UI.ConceptType]; });
            }
            let cn = 'concepts';

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Code: { type: 'number' },
                Name: { type: 'string' },
                SATGroupCode: { type: 'string' },
                GlobalAutomatic: { type: 'boolean' },
                AutomaticDismissal: { type: 'boolean' },
                Kind: { type: 'boolean' },
                Print: { type: 'boolean' },
                ConceptPaymentRelationship: { type: 'object' }
            };

            //Set columns
            let columns = [
                { field: 'Code', title: 'No.', width: 50 },
                { field: 'Name', title: 'Nombre', width: 180 },
                {
                    field: 'SATGroupCode', title: 'Código SAT', width: 250,
                    template: UX.Cotorra.Common.GridFormatCatalogProp('UX.Cotorra.SATGroupCodes.Concepts.' + this.CatalogName[this.ConceptType] + '', 'SATGroupCode', 'Code', 'GetFullName()')
                },
                { field: 'GlobalAutomatic', title: 'Default', template: UX.Cotorra.Common.GridFormatBoolean('GlobalAutomatic'), width: 100 },
                { field: 'AutomaticDismissal', title: 'Mostrar en liquidación', template: UX.Cotorra.Common.GridFormatBoolean('AutomaticDismissal'), width: 100 },
                { field: 'Kind', title: 'Especie', template: UX.Cotorra.Common.GridFormatBoolean('Kind'), width: 100 },
                { field: 'Print', title: 'Imprimir', template: UX.Cotorra.Common.GridFormatBoolean('Print'), width: 100 },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#' + cn + 'ActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns,
                pageSize: 20,
            });
        },

        OpenSave: function (obj = null) {
            let catNam = this.CatalogName[this.ConceptType];
            let titStr = this.TitleModalsString[this.ConceptType];
            let ct = this.ConceptType;
            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'record_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nueva ' + titStr + '' : 'Editar ' + titStr + '', 'm', '<div id="' + containerID + '"></div>',
                function () {
                    let $container = $('#' + containerID);
                    let dpoAccTypes = UX.Cotorra.Catalogs.AccumulatedTypes.filter(x => { return x.TypeOfAccumulated === 1; });

                    //Init template
                    var saveModel = new Ractive({
                        el: '#' + containerID,
                        template: '#' + 'concepts' + '_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Code: row ? row.dataItem.Code : '',
                            Name: row ? row.dataItem.Name : '',
                            GlobalAutomatic: row ? row.dataItem.GlobalAutomatic : '',
                            Kind: row ? row.dataItem.Kind : '',
                            AutomaticDismissal: row ? row.dataItem.AutomaticDismissal : '',
                            Print: row ? row.dataItem.Print : '',
                            SATGroupCode: row ? row.dataItem.SATGroupCode : '',
                            ConceptType: ct,

                            //TotalAmount
                            Label: row ? (row.dataItem.Label === '' ? row.dataItem.Name : row.dataItem.Label) : '',
                            Formula: row ? row.dataItem.Formula : '',
                            TotalAmountAccumulatedType: '',
                            TotalAmountFiscalAccumulated: [],
                            TotalAmountOtherAccumulated: [],

                            //Amount1
                            Label1: row ? (row.dataItem.Label1 === '' ? 'ISR gravado' : row.dataItem.Label1) : 'ISR Gravado',
                            Formula1: row ? row.dataItem.Formula1 : '',
                            TotalAmount1AccumulatedType: '',
                            TotalAmount1FiscalAccumulated: [],
                            TotalAmount1OtherAccumulated: [],

                            //Amount2
                            Label2: row ? (row.dataItem.Label2 === '' ? 'ISR exento' : row.dataItem.Label2) : 'ISR Exento',
                            Formula2: row ? row.dataItem.Formula2 : '',
                            TotalAmount2AccumulatedType: '',
                            TotalAmount2FiscalAccumulated: [],
                            TotalAmount2OtherAccumulated: [],

                            //Amount3
                            Label3: row ? (row.dataItem.Label3 === '' ? 'IMSS gravado' : row.dataItem.Label3) : 'IMSS Gravado',
                            Formula3: row ? row.dataItem.Formula3 : '',
                            TotalAmount3AccumulatedType: '',
                            TotalAmount3FiscalAccumulated: [],
                            TotalAmount3OtherAccumulated: [],

                            //Amount4
                            Label4: row ? (row.dataItem.Label4 === '' ? 'IMSS exento' : row.dataItem.Label4) : 'IMSS Exento',
                            Formula4: row ? row.dataItem.Formula4 : '',
                            TotalAmount4AccumulatedType: '',
                            TotalAmount4FiscalAccumulated: [],
                            TotalAmount4OtherAccumulated: [],

                            //Options
                            TotalAmountAccumulatedTypes: dpoAccTypes,
                            TotalAmount1AccumulatedTypes: dpoAccTypes,
                            TotalAmount2AccumulatedTypes: dpoAccTypes,
                            TotalAmount3AccumulatedTypes: dpoAccTypes,
                            TotalAmount4AccumulatedTypes: dpoAccTypes
                        }
                    });

                    //Check relations
                    let cpr = row ? row.dataItem.ConceptPaymentRelationship : [];
                    for (var i = 0; i < cpr.length; i++) {

                        let accType = UX.Cotorra.Catalogs.AccumulatedTypes.find(x => x.ID === cpr[i].ID);

                        let cat = '';
                        switch (cpr[i].ConceptPaymentType) {
                            case 1: cat = 'TotalAmount'; break;
                            case 2: cat = 'TotalAmount1'; break;
                            case 3: cat = 'TotalAmount2'; break;
                            case 4: cat = 'TotalAmount3'; break;
                            case 5: cat = 'TotalAmount4'; break;
                        }

                        switch (cpr[i].RelationType) {
                            case 1: cat += 'FiscalAccumulated'; break;
                            case 2: cat += 'OtherAccumulated'; break;
                        }

                        saveModel.push(cat, accType);
                    }

                    saveModel.on({
                        addTAAccumulated: function (event) {
                            let accID = this.get('TotalAmountAccumulatedType');
                            this.set('TotalAmountAccumulatedType', '');
                            this.push('TotalAmountFiscalAccumulated', UX.Cotorra.Catalogs.AccumulatedTypes.find(x => x.ID === accID));
                            $('#np_ps_drpTotalAmountAccumulatedTypes').focus();
                        },

                        removeTAFiscalAccumulated: function (event) {
                            let selectedAccIDs = this.get('TotalAmountFiscalAccumulatedIDs');
                            for (var i = 0; i < selectedAccIDs.length; i++) {
                                let itemToDelete = this.get('TotalAmountFiscalAccumulated').find(x => x.ID === selectedAccIDs[i]);
                                let index = this.get('TotalAmountFiscalAccumulated').indexOf(itemToDelete);
                                this.splice('TotalAmountFiscalAccumulated', index, 1);
                            }
                            this.set('TotalAmountFiscalAccumulatedIDs', '');
                        },

                        removeTAOtherAccumulated: function (event) {
                            let selectedAccIDs = this.get('TotalAmountOtherAccumulatedIDs');
                            for (var i = 0; i < selectedAccIDs.length; i++) {
                                let itemToDelete = this.get('TotalAmountOtherAccumulated').find(x => x.ID === selectedAccIDs[i]);
                                let index = this.get('TotalAmountOtherAccumulated').indexOf(itemToDelete);
                                this.splice('TotalAmountOtherAccumulated', index, 1);
                            }
                            this.set('TotalAmountOtherAccumulatedIDs', '');
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
                            np_ps_txtCode: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el número' }
                                }
                            },
                            np_ps_txtName: {
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
                                'concepts',
                                data,
                                (id) => {
                                    let grid = row ? row.$kendoGrid : $('.np-' + 'concepts-grid').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = id;

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.Code = data.Code;
                                        dataItem.Name = data.Name;
                                        dataItem.GlobalAutomatic = data.GlobalAutomatic;
                                        dataItem.Kind = data.Kind;
                                        dataItem.AutomaticDismissal = data.AutomaticDismissal;
                                        dataItem.Print = data.Print;
                                        dataItem.SATGroupCode = data.SATGroupCode;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }
                                    UX.Cotorra.Catalogs.Reload({ catalog: 'Concepts', serviceparams: { type: '' } });
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

                    //Masks
                    $('#np_ps_txtCode').mask('0000');

                    let satGroupData = [];
                    if (ct === 'P') {
                        satGroupData = UX.Cotorra.SATGroupCodes.Concepts.Perceptions;
                    } else if (ct === 'D') {
                        satGroupData = UX.Cotorra.SATGroupCodes.Concepts.Deductions;
                    } else if (ct === 'L') {
                        satGroupData = UX.Cotorra.SATGroupCodes.Concepts.Liabilities;
                    }

                    //SAT group code
                    let satGroupCode = $("#np_ps_drpSATCode").genericCatalog(
                        {
                            data: satGroupData,
                            columns: [
                                { Width: "15%", Field: "Code", SearchType: "StartWith" },
                                { Width: "85%", Field: "Name", SearchType: "Contains" },
                            ],
                            fieldID: "Code",
                            fieldText: "Fullname",
                            fieldSort: "Code",
                            selectorName: "np_ps_drpSATCode"
                        });

                    satGroupCode.setValue(saveModel ? saveModel.get("SATGroupCode").toString() : null);
                    satGroupCode.onChange(() => { saveModel.set("SATGroupCode", satGroupCode.getValue()); });

                    //Get data (if applicable)
                });

            $('#' + modalID).addClass('modal-concepts');
        },

        Delete: function (obj) {
            let row = UX.Cotorra.Common.GetRowData(obj);
            row.dataItem.ConceptType = this.ConceptType;
            let containerSelector = this.ContainerSelector;

            //Show modal
            UX.Modals.Confirm("Eliminar concepto", "¿Deseas eliminar el concepto?", 'Sí, eliminar', 'No, espera',
                () => {
                    UX.Loaders.SetLoader(containerSelector);
                    UX.Cotorra.Catalogs.Delete(
                        'concepts',
                        {
                            id: row.dataItem.ID,
                            ConceptType: row.dataItem.ConceptType
                        },
                        (data) => { $(row.el).fadeOut(function () { row.$kendoGrid.dataSource.remove(row.dataItem); }); },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader(containerSelector); }
                    );
                },
                () => {
                });
        }
    }
};

