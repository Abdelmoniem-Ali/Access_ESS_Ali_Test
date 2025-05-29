import { IRCRMContext } from '../rcrm/rcrm';

export interface PidgetSettings {
    remoteEntry: string;
    rcrmUrl: string;
    softwareKey: string;
    userId: number;
    rcrm?: IRCRMContext;
    windowHandleId?: string;
    workspaceApiUrl: string;
}
