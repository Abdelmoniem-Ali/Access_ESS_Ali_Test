import { TaskAssignmentInfo } from './TaskAssignmentInfo';
import { TaskRecurrenceInfo } from './TaskRecurrenceInfo';

export type TaskInfo = {
    taskId: number;
    subject: string;
    dueDate: Date;
    startDate: Date;
    statusDescription: string;
    priorityDescription: string;
    percCompleted?: number;
    completedDate?: Date;
    isPrivate?: string;
    notebookItemId?: number;
    jobId?: number;
    personId?: number;
    clientId?: number;
    placementId?: number;
    applicantActionId?: number;
    invoiceId?: number;
    reviewListId?: number;
    mailshotId?: number;
    linkXML?: string;
    taskAssignment?: TaskAssignmentInfo;
    notes?: string;
    recurrenceInfo?: TaskRecurrenceInfo;
};
