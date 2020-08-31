'use strict'

UX.Cotorra.Incidents = {

    UI: {

        Init: (params = {}) => {

            let modalID = UX.Modals.OpenModal(
                'Captura de faltas, horas extras y otros',
                'm', '<div id="np-editincidents"></div>',
                function () {

                    let editModel = new Ractive({
                        el: '#np-editincidents',
                        template: '#np-editincidents-template',
                        data: {
                            PeriodDetailID: params.PeriodDetailID,
                            Date: params.Date,
                            Employee: params.Employee,
                            IncidentDate: moment(params.Date).locale('es').format("DD MMMM YYYY"),
                            Incidents: [],
                            IncidentTypesOptions: [],
                            DisableAddIncident: false,
                            DisableApplyChanges: true,
                            DisableDaysOptions: false
                        },
                        modifyArrays: true
                    });

                    //template methods
                    editModel.on({
                        addIncident: function (event) {
                            editModel.push('Incidents', { Value: '', IncidentTypeID: '' });
                            $('#np-incidents').initUIElements();
                            $('#np-incidents input').decimalMask(2);
                            $('#np-incidents input').last().focus().select();
                        },

                        removeIncident: function (event) {
                            $(event.node).closest('.row').remove();
                            editModel.splice('Incidents', parseInt(event.keypath.split('.')[1]), 1);

                            if (editModel.get('Incidents').length === 0) {
                                $('#np-incidents .row.titles').remove();
                            }
                        },

                        applyChanges: function (event) {

                            let incidents = editModel.get('Incidents').map((x) => {
                                return {
                                    IncidentTypeID: x.IncidentTypeID,
                                    Value: x.Value,
                                    EmployeeID: editModel.get('Employee.ID'),
                                    PeriodDetailID: editModel.get('PeriodDetailID'),
                                    Date: editModel.get('Date').format('YYYY-MM-DD')
                                };
                            });

                            //Breakdown days incidents (tipically it should be only one)
                            var brokendownIncidents = [];
                            for (var i = 0; i < incidents.length; i++) {
                                let inc = incidents[i];
                                let it = UX.Cotorra.Catalogs.IncidentTypes.find(x => x.ID == incidents[i].IncidentTypeID);

                                //Check type of incident (days or hours)
                                if (it.TypeOfIncident === 2) {
                                    //Days
                                    let value = parseInt(inc.Value);
                                    let date = inc.Date;
                                    for (var j = 0; j < value; j++) {
                                        //Override value and date
                                        brokendownIncidents.push({
                                            IncidentTypeID: inc.IncidentTypeID,
                                            Value: 1,
                                            EmployeeID: inc.EmployeeID,
                                            PeriodDetailID: inc.PeriodDetailID,
                                            Date: moment(date).add(j, 'days').format("YYYY-MM-DD")
                                        })
                                    }
                                } else {
                                    //Hours
                                    brokendownIncidents.push(inc);
                                }
                            }

                            UX.Loaders.SetLoader('#' + modalID);
                            UX.Common.ClearFocus();

                            UX.Cotorra.PrePayroll.Save(
                                {
                                    incidents: brokendownIncidents,
                                    employeeID: editModel.get("Employee.ID"),
                                    periodDetailID: editModel.get("PeriodDetailID"),
                                    capturedOnDate: moment(editModel.get("Date")).format("YYYY-MM-DD")
                                },
                                'SaveIncidents',
                                (data) => {
                                    UX.Modals.CloseModal(modalID);
                                    if (params.OnSaveSucessCallback) {
                                        params.OnSaveSucessCallback(brokendownIncidents, data);
                                    }
                                },
                                (error) => {
                                    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', () => { });
                                },
                                (complete) => {
                                    UX.Loaders.RemoveLoader('#' + modalID);
                                });
                        },

                        closeModal: function (event) {
                            UX.Modals.CloseModal(modalID);
                        },
                    });

                    //observers
                    editModel.observe('Incidents', function (newValue, oldValue, keypath) {

                        let incidents = newValue;

                        //Check values for disabling 'apply changes'
                        if (incidents.find(x => x.Value === '') !== undefined) {
                            editModel.set('DisableApplyChanges', true);
                            editModel.set('DisableAddIncident', true);
                        } else if (incidents.find(x => x.IncidentTypeID === '') !== undefined) {
                            editModel.set('DisableApplyChanges', true);
                            editModel.set('DisableAddIncident', true);
                        } else {
                            editModel.set('DisableApplyChanges', false);
                            editModel.set('DisableAddIncident', false);
                        }

                        if (UX.Cotorra.Catalogs.IncidentTypes) {
                            //Check all incidents
                            editModel.set('DisableDaysOptions', false);
                            editModel.set('ActualIncidentsIDs', '');

                            for (var i = 0; i < incidents.length; i++) {
                                let it = UX.Cotorra.Catalogs.IncidentTypes.find(x => x.ID == incidents[i].IncidentTypeID);
                                if (it && it.TypeOfIncident === 2) {
                                    editModel.set('DisableAddIncident', true);
                                }
                                if (it && it.TypeOfIncident === 3) {
                                    editModel.set('DisableDaysOptions', true);
                                }

                                let aiids = editModel.get('ActualIncidentsIDs');
                                aiids += incidents[i].IncidentTypeID + '|';
                                editModel.set('ActualIncidentsIDs', aiids);
                            }
                        }


                    }, { defer: true, init: true });

                    UX.Cotorra.Incidents.EditModel = editModel;

                    let getIncidentsReq = UX.Cotorra.PrePayroll.Get(
                        {
                            employeeID: editModel.get('Employee.ID'),
                            periodDetailID: editModel.get('PeriodDetailID'),
                            incidentDate: editModel.get('Date').format('YYYY-MM-DD')
                        },
                        'GetIncidents',
                        (data) => { editModel.set("Incidents", data); },
                        (error) => { },
                        (complete) => { });

                    (async function () {

                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#np-incidents',
                                removeWhenFinish: true
                            },
                            catalogs: ['IncidentTypes'],
                            requests: [getIncidentsReq],
                            forceLoad: false
                        });

                        editModel.set('IncidentTypesOptions', UX.Cotorra.Catalogs.IncidentTypes.filter(x => { return x.ItConsiders === 1 || x.ItConsiders === 2 }));

                        $('#btnAddIncident').focus();

                    })();
                });
        }
    }
};

