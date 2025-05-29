import { usePidgetSettings } from '@workspace/utils-react';
import { useCallback } from 'react';
import {
    IForms,
    ISendSMSRequest,
    IColumnPickerColumn,
    IPlacementFilter,
    IApplicantFilter,
} from '../contexts/rcrm/rcrm';
import { PidgetSettings } from '../contexts/Settings/PidgetSettings';

export const useRCRMForms = (): IForms => {
    const pidgetSettings = usePidgetSettings<PidgetSettings>();

    const showApplicantCardView = (applicantId: number, onClose?: () => void) => {
        pidgetSettings.rcrm?.forms.showApplicantCardView(applicantId, onClose);
    };
    const showApplicantAvailabilitySearchForm = (
        jobId: number,
        showAsChildWindow: boolean,
        callback?: (success: boolean, filterApplicantTempId: string) => void,
    ) => {
        pidgetSettings.rcrm?.forms.showApplicantAvailabilitySearchForm(
            jobId,
            showAsChildWindow,
            callback,
        );
    };
    const showClientCardView = (clientId: number) => {
        pidgetSettings.rcrm?.forms.showClientCardView(clientId);
    };
    const showContactCardView = (contactId: number) => {
        pidgetSettings.rcrm?.forms.showContactCardView(contactId);
    };
    const showPersonCardView = (personId: number) => {
        pidgetSettings.rcrm?.forms.showPersonCardView(personId);
    };
    const showPlacementCardView = (placementId: number) => {
        pidgetSettings.rcrm?.forms.showPlacementCardView(placementId);
    };
    const showContractJobForm = (
        jobId: number,
        showAsChildWindow: boolean,
        callback?: () => void,
    ) => {
        pidgetSettings.rcrm?.forms.showContractJobForm(jobId, showAsChildWindow, callback);
    };
    const showPermanentJobForm = (
        jobId: number,
        showAsChildWindow: boolean,
        callback?: () => void,
    ) => {
        pidgetSettings.rcrm?.forms.showPermanentJobForm(jobId, showAsChildWindow, callback);
    };
    const showJobForm = (
        jobId: number,
        showAsChildWindow: boolean,
        newContractJob: boolean,
        callback?: () => void,
    ) => {
        pidgetSettings.rcrm?.forms.showJobForm(jobId, showAsChildWindow, false, callback);
    };
    const showApplicantForm = (
        applicantId: number,
        showAsChildWindow: boolean,
        onClose?: () => void,
    ) => {
        pidgetSettings.rcrm?.forms.showApplicantForm(applicantId, showAsChildWindow, onClose);
    };
    const showClientForm = (clientId: number, showAsChildWindow: boolean, onClose?: () => void) => {
        pidgetSettings.rcrm?.forms.showClientForm(clientId, showAsChildWindow, onClose);
    };
    const showPlacementForm = (
        placementId: number,
        showAsChildWindow: boolean,
        onClose?: (result: boolean) => void,
    ) => {
        pidgetSettings.rcrm?.forms.showPlacementForm(placementId, showAsChildWindow, onClose);
    };
    const showContractJobShiftForm = (
        jobId: number,
        showAsChildWindow: boolean,
        callback: () => void,
    ) => {
        pidgetSettings.rcrm?.forms.showContractJobShiftForm(jobId, showAsChildWindow, callback);
    };
    const showBroadcastShiftsForm = (jobId: number, startDate: Date, endDate: Date) => {
        pidgetSettings.rcrm?.forms.showBroadcastShiftsForm(jobId, startDate, endDate);
    };
    const showReviewBroadcastForm = (jobId: number, clientId: number) => {
        pidgetSettings.rcrm?.forms.showReviewBroadcastForm(jobId, clientId);
    };
    const closeWindow = (windowId: string) => {
        pidgetSettings.rcrm?.forms.closeWindow(windowId);
    };
    const refreshCurrentForm = () => {
        pidgetSettings.rcrm?.forms.refreshCurrentForm();
    };
    const currentFormName = (): string => {
        return pidgetSettings.rcrm?.forms.currentFormName() || '';
    };
    const showSmsForShift = (smsDetails: ISendSMSRequest, callback?: () => void) => {
        pidgetSettings.rcrm?.forms.showSmsForShift(smsDetails, callback);
    };
    const sendSmsToApplicants = (applicantIds: number[]) => {
        pidgetSettings.rcrm?.forms.sendSmsToApplicants(applicantIds);
    };
    const sendSmsToClients = (clientIds: number[]) => {
        pidgetSettings.rcrm?.forms.sendSmsToClients(clientIds);
    };
    const sendSmsToContacts = (clientContactIds: number[]) => {
        pidgetSettings.rcrm?.forms.sendSmsToContacts(clientContactIds);
    };
    const sendEmailToApplicants = (applicantIds: number[]) => {
        pidgetSettings.rcrm?.forms.sendEmailToApplicants(applicantIds);
    };
    const sendEmailToContacts = (contactIds: number[]) => {
        pidgetSettings.rcrm?.forms.sendEmailToContacts(contactIds);
    };
    const showJobPicklist = (callback: (jobId: number) => void) => {
        pidgetSettings.rcrm?.forms.showJobPicklist(callback);
    };
    const showJobCreateWizardWindow = (
        employmentType: string,
        onJobCreated?: (jobId: number) => void,
    ) => {
        pidgetSettings.rcrm?.forms.showJobCreateWizardWindow(employmentType, onJobCreated);
    };
    const showScheduling20Window = () => {
        pidgetSettings.rcrm?.forms.showScheduling20Window();
    };
    const showEmailFormForApplicant = (applicantId: number) => {
        pidgetSettings.rcrm?.forms.showEmailFormForApplicant(applicantId);
    };
    const showSmsFormForApplicant = (applicantId: number, message?: string) => {
        pidgetSettings.rcrm?.forms.showSmsFormForApplicant(applicantId, message);
    };
    const showExtendPlacementForm = (
        jobId: number,
        jobDescription: string,
        callback: (success: boolean, applicantIds: string) => void,
    ) => {
        pidgetSettings.rcrm?.forms.showExtendPlacementForm(jobId, jobDescription, callback);
    };
    const showExtendPlacementFormFromPlacements = (
        placementIds: number[],
        callback: (success: boolean, applicantIds: string) => void,
    ) => {
        pidgetSettings.rcrm?.forms.showExtendPlacementFormFromPlacements(placementIds, callback);
    };
    const showEditPlacementForm = (
        jobId: number,
        jobDescription: string,
        dateFrom: Date,
        dateTo: Date,
        callback: () => void,
    ) => {
        pidgetSettings.rcrm?.forms.showEditPlacementForm(
            jobId,
            jobDescription,
            dateFrom,
            dateTo,
            callback,
        );
    };
    const showCVWizardForApplicantActions = (applicantActionIds: number[]) => {
        pidgetSettings.rcrm?.forms.showCVWizardForApplicantActions(applicantActionIds);
    };
    const showApplicantDiary = (applicantId: number, onClose?: () => void) => {
        pidgetSettings.rcrm?.forms.showApplicantDiary(applicantId, onClose);
    };
    const showJobCardView = (jobId: number) => {
        pidgetSettings.rcrm?.forms.showJobCardView(jobId);
    };
    const showSavedSearchesAutoMatchPicklist = useCallback(
        (jobId: number, callback: (savedSearchId?: number) => void) => {
            pidgetSettings.rcrm?.forms.showSavedSearchesAutoMatchPicklist(jobId, callback);
        },
        [pidgetSettings.rcrm?.forms],
    );
    const addToSavedTagFile = (recordIds: number[], recordType: string) => {
        pidgetSettings.rcrm?.forms.addToSavedTagFile(recordIds, recordType);
    };
    const showConfigureColumnsWindow = useCallback(
        (
            availableColumns: IColumnPickerColumn[],
            selectedColumns: IColumnPickerColumn[],
            doneFunc: (
                button: string,
                availableColumns: IColumnPickerColumn[],
                selectedColumns: IColumnPickerColumn[],
            ) => void,
        ) => {
            pidgetSettings.rcrm?.forms?.showConfigureColumnsWindow(
                availableColumns,
                selectedColumns,
                doneFunc,
            );
        },
        [pidgetSettings.rcrm?.forms],
    );
    const addToTagFile = (recordType: string, recordIds: number[]) => {
        pidgetSettings.rcrm?.forms.addToTagFile(recordType, recordIds);
    };

    const showContractPlacementFilterWindow = useCallback(
        (filters: IPlacementFilter, callback: (obj: IPlacementFilter) => void) => {
            pidgetSettings.rcrm?.forms.showContractPlacementFilterWindow(filters, callback);
        },
        [pidgetSettings.rcrm?.forms],
    );

    const showApplicantFilterWindow = useCallback(
        (filters: IApplicantFilter, callback: (obj: IApplicantFilter) => void) => {
            pidgetSettings.rcrm?.forms.showApplicantFilterWindow(filters, callback);
        },
        [pidgetSettings.rcrm?.forms],
    );

    return {
        closeWindow,
        currentFormName,
        refreshCurrentForm,
        sendEmailToApplicants,
        sendSmsToContacts,
        sendEmailToContacts,
        sendSmsToApplicants,
        sendSmsToClients,
        showApplicantAvailabilitySearchForm,
        showApplicantCardView,
        showApplicantDiary,
        showApplicantForm,
        showClientForm,
        showPlacementForm,
        showBroadcastShiftsForm,
        showCVWizardForApplicantActions,
        showClientCardView,
        showContactCardView,
        showContractJobForm,
        showContractJobShiftForm,
        showEmailFormForApplicant,
        showExtendPlacementForm,
        showExtendPlacementFormFromPlacements,
        showEditPlacementForm,
        showJobCardView,
        showJobCreateWizardWindow,
        showScheduling20Window,
        showJobForm,
        showJobPicklist,
        showPermanentJobForm,
        showPersonCardView,
        showPlacementCardView,
        showReviewBroadcastForm,
        showSmsForShift,
        showSmsFormForApplicant,
        showSavedSearchesAutoMatchPicklist,
        addToSavedTagFile,
        showConfigureColumnsWindow,
        addToTagFile,
        showContractPlacementFilterWindow,
        showApplicantFilterWindow,
    };
};
