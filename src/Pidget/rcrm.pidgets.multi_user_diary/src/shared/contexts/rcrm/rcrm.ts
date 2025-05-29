export interface IRCRMContext {
    forms: IForms;
    notifications: IRCRMNotifications;
    events: IRCRMEvents;
    context?: IRCRMPluginContext;
}

interface IRCRMPluginContext {
    recordType: string;
    recordId: string;
}

export interface ISendSMSRequest {
    startDateTime: string;
    endDateTime: string;
    jobId: number;
    jobShiftId: string;
    jobShiftDescription: string;
    type: string; //JOBSHIFT
    applicantsExist: boolean; // TODO: out-of-work prop
    ApplicantIds?: number[];
}

export interface IColumnPickerColumn {
    name: string;
    caption: string;
    selected?: boolean;
}

export interface IPlacementFilter {
    statusFilter?: string;
    employmentTypeFilter?: string;
    ownershipFilter?: string;
    userFilter?: string;
    groupFilter?: string;
    sectorFilter?: string;
}

export interface IApplicantFilter {
    ownershipFilter?: string;
    userFilter?: string;
    groupFilter?: string;
    sectorFilter?: string;
}

export interface IForms {
    showContractPlacementFilterWindow(
        filters: IPlacementFilter,
        callback: (obj: IPlacementFilter) => void,
    ): void;
    showApplicantFilterWindow(
        filters: IApplicantFilter,
        callback: (obj: IApplicantFilter) => void,
    ): void;
    showJobCardView(jobId: number): void;
    showApplicantCardView(applicantId: number, onClose?: () => void): void;
    showApplicantAvailabilitySearchForm(
        jobId: number,
        showAsChildWindow: boolean,
        callback?: (success: boolean, filterApplicantTempId: string) => void,
    ): void;
    showClientCardView(clientId: number): void;
    showContactCardView(contactId: number): void;
    showPersonCardView(personId: number): void;
    showPlacementCardView(placementId: number): void;
    showContractJobForm(jobId: number, showAsChildWindow: boolean, callback?: () => void): void;
    showPermanentJobForm(jobId: number, showAsChildWindow: boolean, callback?: () => void): void;
    showJobForm(
        jobId: number,
        showAsChildWindow: boolean,
        newContractJob: boolean,
        callback?: () => void,
    ): void;
    showApplicantForm(applicantId: number, showAsChildWindow: boolean, onClose?: () => void): void;
    showClientForm(clientId: number, showAsChildWindow: boolean, onClose?: () => void): void;
    showPlacementForm(
        placementId: number,
        showAsChildWindow: boolean,
        onClose?: (result: boolean) => void,
    ): void;
    showContractJobShiftForm(jobId: number, showAsChildWindow: boolean, callback: () => void): void;
    showBroadcastShiftsForm(jobId: number, startDate: Date, endDate: Date): void;
    showReviewBroadcastForm(jobId: number, clientId: number): void;
    closeWindow(windowId: string): void;
    refreshCurrentForm(): void;
    currentFormName(): string;
    showSmsForShift(smsDetails: ISendSMSRequest, callback?: () => void): void;
    sendSmsToApplicants(applicantIds: number[]): void;
    sendSmsToClients(clientIds: number[]): void;
    sendSmsToContacts(clientContactIds: number[]): void;
    sendEmailToApplicants(applicantIds: number[]): void;
    sendEmailToContacts(contactIds: number[]): void;
    showJobPicklist(callback: (jobId: number) => void): void;
    showJobCreateWizardWindow(employmentType: string, onJobCreated?: (jobId: number) => void): void;
    showScheduling20Window(): void;
    showEmailFormForApplicant(applicantId: number): void;
    showSmsFormForApplicant(applicantId: number, message?: string): void;
    showExtendPlacementForm(
        jobId: number,
        jobDescription: string,
        callback: (success: boolean, applicantIds: string) => void,
    ): void;
    showExtendPlacementFormFromPlacements(
        placementIds: number[],
        callback: (success: boolean, applicantIds: string) => void,
    ): void;
    showEditPlacementForm(
        jobId: number,
        jobDescription: string,
        dateFrom: Date,
        dateTo: Date,
        callback: () => void,
    ): void;
    showCVWizardForApplicantActions(applicantActionIds: number[]): void;
    showApplicantDiary(applicantId: number, onClose?: () => void): void;
    showSavedSearchesAutoMatchPicklist(
        jobId: number,
        callback: (savedSearchId?: number) => void,
    ): void;
    addToSavedTagFile(recordIds: number[], recordType: string): void;
    showConfigureColumnsWindow(
        availableColumns: IColumnPickerColumn[],
        selectedColumns: IColumnPickerColumn[],
        doneFunc: (
            button: string,
            availableColumns: IColumnPickerColumn[],
            selectedColumns: IColumnPickerColumn[],
        ) => void,
    ): void;
    addToTagFile(recordType: string, recordIds: number[]): void;
}

interface IRCRMNotifications {
    showErrorToast(message: string, title?: string): void;
    showSuccessToast(message: string, title?: string): void;
    showWarningToast(message: string, title?: string): void;
}

interface IRCRMEvents {
    onJobCreated(jobId: number): void;
}

export class MockRCRMContext implements IRCRMContext {
    forms: IForms;
    notifications: IRCRMNotifications;
    events: IRCRMEvents;
    constructor() {
        this.forms = new MockForms();
        this.notifications = new MockNotifications();
        this.events = new MockEvents();
    }
}
class MockEvents implements IRCRMEvents {
    onJobCreated(jobId: number) {
        console.log(`onJobCreated called with jobId ${jobId}`);
    }
}

class MockNotifications implements IRCRMNotifications {
    showErrorToast(message: string, title?: string | undefined): void {
        alert(`Error Message: ${message},Title:${title}`);
    }
    showSuccessToast(message: string, title?: string | undefined): void {
        alert(`Success Message: ${message},Title:${title}`);
    }
    showWarningToast(message: string, title?: string | undefined): void {
        alert(`Warning Message: ${message},Title:${title}`);
    }
}

class MockForms implements IForms {
    showApplicantFilterWindow(
        filters: IApplicantFilter,
        callback: (obj: IApplicantFilter) => void,
    ): void {
        throw new Error('Method not implemented.');
    }
    showContractPlacementFilterWindow(
        filters: IPlacementFilter,
        callback: (obj: IPlacementFilter) => void,
    ): void {
        throw new Error('Method not implemented.');
    }
    addToSavedTagFile(recordIds: number[], recordType: string): void {
        alert(`addToSavedTagFile`);
    }
    addToTagFile(recordType: string, recordIds: number[]) {
        throw new Error('Method not implemented.');
    }
    showClientForm(clientId: number, showAsChildWindow: boolean): void {
        alert(`showClientForm ${clientId},${showAsChildWindow}`);
    }
    sendSmsToApplicants(applicantIds: number[]): void {
        alert(`sendSmsToApplicants`);
        console.log(`sendSmsToApplicants ${applicantIds}`);
    }
    sendSmsToClients(clientIds: number[]): void {
        alert(`sendSmsToClients`);
        console.log(`sendSmsToClients ${clientIds}`);
    }
    sendSmsToContacts(clientContactIds: number[]): void {
        alert(`sendSmsToContacts`);
        console.log(`sendSmsToContacts ${clientContactIds}`);
    }
    sendEmailToApplicants(applicantIds: number[]): void {
        alert('sendEmailToApplicants');
        console.log(`sendEmailToApplicants ${applicantIds}`);
    }
    sendEmailToContacts(contactIds: number[]): void {
        alert('sendEmailToContacts');
        console.log(`sendEmailToContacts ${contactIds}`);
    }
    showBroadcastShiftsForm(jobId: number): void {
        alert(`showBroadcastShiftsForm jobId:${jobId}`);
    }
    showReviewBroadcastForm(jobId: number, clientId: number): void {
        alert(`showReviewBroadcastForm jobId:${jobId} clientId:${clientId}`);
    }
    showJobPicklist(callback: (jobId: number) => void): void {
        alert('showJobPicklist');
        console.log(callback);
    }
    showJobCreateWizardWindow(
        employmentType: string,
        onJobCreated?: (jobId: number) => void,
    ): void {
        alert(`showJobCreateWizardWindow ${employmentType}`);
        onJobCreated?.(0);
    }
    showScheduling20Window() {
        alert('showScheduling20Window');
    }
    showJobCardView(jobId: number): void {
        alert(`showJobCardView ${jobId}`);
    }
    showApplicantCardView(applicantId: number): void {
        alert(`showApplicantCardView ${applicantId}`);
    }
    showClientCardView(clientId: number): void {
        alert(`showClientCardView ${clientId}`);
    }
    showContactCardView(contactId: number): void {
        alert(`showContactCardView ${contactId}`);
    }
    showPersonCardView(personId: number): void {
        alert(`showPersonCardView ${personId}`);
    }
    showPlacementCardView(placementId: number): void {
        alert(`showPlacementCardView ${placementId}`);
    }
    showContractJobForm(jobId: number, showAsChildWindow: boolean, callback?: () => void): void {
        alert(`showContractJobForm ${jobId},${showAsChildWindow},${callback}`);
        if (callback) {
            callback();
        }
    }
    showPermanentJobForm(jobId: number, showAsChildWindow: boolean, callback?: () => void): void {
        alert(`showPermanentForm ${jobId},${showAsChildWindow},${callback}`);
        if (callback) {
            callback();
        }
    }
    showApplicantForm(applicantId: number, showAsChildWindow: boolean): void {
        alert(`showApplicantForm ${applicantId},${showAsChildWindow}`);
    }
    showPlacementForm(
        placementId: number,
        showAsChildWindow: boolean,
        onClose?: (result: boolean) => void,
    ): void {
        alert(`showPlacementForm ${placementId},${showAsChildWindow}`);
    }
    showContractJobShiftForm(
        jobId: number,
        showAsChildWindow: boolean,
        callback: () => void,
    ): void {
        alert(`showContractJobForm ${jobId},${showAsChildWindow},${callback}`);
        callback();
    }
    closeWindow(windowId: string) {
        alert(`Close Window ${windowId}`);
    }
    refreshCurrentForm() {
        alert(`Refresh Current Form`);
    }
    currentFormName() {
        return ``;
    }
    showJobForm(
        jobId: number,
        showAsChildWindow: boolean,
        newContractJob: boolean,
        callback?: () => void,
    ): void {
        alert(`showContractJobForm ${jobId},${showAsChildWindow},${callback}`);
        if (callback) {
            callback();
        }
    }
    showSmsForShift(smsDetails: ISendSMSRequest) {
        alert(`Send Shift SMS ${smsDetails.jobShiftDescription}`);
    }
    showSmsFormForApplicant(applicantId: number, message?: string): void {
        alert(`showSmsFormForApplicant ${applicantId} ${message}`);
    }
    showEmailFormForApplicant(applicantId: number): void {
        alert(`showEmailFormForApplicant ${applicantId}`);
    }
    showExtendPlacementForm(
        jobId: number,
        jobDescription: string,
        callback: (success: boolean, applicantIds: string) => void,
    ): void {
        alert(`showExtendPlacementForm ${jobId} ${jobId} ${jobDescription} `);
        callback(true, '157,158,159');
    }
    showExtendPlacementFormFromPlacements(
        placementIds: number[],
        callback: (success: boolean, applicantIds: string) => void,
    ): void {
        alert(`showExtendPlacementForm ${placementIds}`);
        callback(true, '157,158,159');
    }
    showEditPlacementForm(
        jobId: number,
        jobDescription: string,
        dateFrom: Date,
        dateTo: Date,
        callback: () => void,
    ): void {
        alert(`showEditPlacementForm ${jobId} ${jobDescription} ${dateFrom} ${dateTo}`);
        callback();
    }
    showApplicantAvailabilitySearchForm(
        jobId: number,
        showAsChildWindow: boolean,
        callback: (success: boolean, filterApplicantTempId: string) => void,
    ): void {
        alert(`showApplicantAvailabilitySearchForm ${jobId},${showAsChildWindow},${callback}`);
        callback(true, '87adeee0-ab79-4b66-99a7-64f38dc07096');
    }

    showCVWizardForApplicantActions(applicantActionIds: number[]): void {
        alert(`showCVWizardForApplicantActions ${applicantActionIds}`);
    }

    showApplicantDiary(applicantId: number, onClose: () => void): void {
        alert(`showApplicantDiary ${applicantId}`);
        onClose?.();
    }

    showSavedSearchesAutoMatchPicklist(
        jobId: number,
        callback: (savedSearchId?: number) => void,
    ): void {
        console.log('showSavedSearchesAutoMatchPicklist');
        callback();
    }

    showConfigureColumnsWindow(
        availableColumns: IColumnPickerColumn[],
        selectedColumns: IColumnPickerColumn[],
        doneFunc: (
            button: string,
            availableColumns: IColumnPickerColumn[],
            selectedColumns: IColumnPickerColumn[],
        ) => void,
    ): void {
        console.log('showConfigureColumnsWindow');
    }
}
